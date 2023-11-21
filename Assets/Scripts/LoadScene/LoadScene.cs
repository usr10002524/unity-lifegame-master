using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


/// <summary>
/// ロードシーン
/// Atsumaru APIが有効な場合は、ServerDataをロードする
/// Atsumaru APIが無効な場合は、localStorageからロードする
/// </summary>
public class LoadScene : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loadingText;

    private static readonly string baseText = "Now Loading";
    private static readonly string completeText = "COMPLETE!";

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {
            AtsumaruAPI.Instance.LoadServerData();
        }
        else
        {
            LocalStorageAPI.Instance.LoadLocalrData();
        }
        loadingText.gameObject.SetActive(true);
        StartCoroutine(LoadingCoroutine());
    }

    /// <summary>
    /// ServerData をロードする
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator LoadingCoroutine()
    {
        bool isEnd = false;
        float timer = 0.0f;
        float interval = 0.25f;
        float loadTimeoutTimer = 0.0f;
        float loadTimeout = 60.0f;
        int dotCount = 0;
        int maxDotCount = 3;
        float displayCompleteTime = 0.5f;
        float displayMinimumTimer = 0.0f;
        float minimumTime = 0.5f;

        while (!isEnd)
        {
            yield return null;

            displayMinimumTimer += Time.deltaTime;
            if (CheckLoadComplete() && displayMinimumTimer >= minimumTime)
            {
                isEnd = true;
            }

            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0.0f;
                dotCount++;
                dotCount %= (maxDotCount + 1);
                SetText(dotCount);
            }

            loadTimeoutTimer += Time.deltaTime;
            if (loadTimeoutTimer >= loadTimeout)
            {
                Debug.Log("LoadData timeout.");
                isEnd = true;
            }
        }

        if (CheckLoadComplete())
        {
            Restore();
            SetCompleteText();
            yield return new WaitForSeconds(displayCompleteTime);
        }

        SceneManager.LoadScene("TitleScene");
    }

    /// <summary>
    /// ロード中かどうかを判定する。
    /// </summary>
    /// <returns>ロード中の場合はtrue、そうでない場合はfalseを返す</returns>
    private bool CheckLoadComplete()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {
            if (!AtsumaruAPI.Instance.IsServerDataLoaded())
            {
                return false;
            }
        }
        else
        {
            if (!LocalStorageAPI.Instance.IsLocalDataLoaded())
            {
                return false;
            }
        }
        if (!PatternLoadManager.Instance.IsLoadCompleted())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Loding + . を表示する
    /// </summary>
    /// <param name="dotCount"></param>
    private void SetText(int dotCount)
    {
        string text = baseText;
        for (int i = 0; i < dotCount; i++)
        {
            text += ".";
        }
        loadingText.SetText(text);
    }

    /// <summary>
    /// Complete を表示する
    /// </summary>
    private void SetCompleteText()
    {
        loadingText.SetText(completeText);
    }

    /// <summary>
    /// 各種設定の復帰を行う
    /// </summary>
    private void Restore()
    {
        ResotreLocale();
        RestoreSound();
    }

    private void RestoreSound()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {

        }
        else
        {
            ServerData.SoundSettings soundSettings = LocalStorageAPI.Instance.GetSoundSettings();
            if (soundSettings == null)
            {
                soundSettings = new ServerData.SoundSettings();
            }
            // Debug.LogFormat("LoadScene.RestoreSound() volume:{0}", soundSettings.volume);
            AudioListener.volume = soundSettings.volume;
        }
    }

    /// <summary>
    /// 言語設定を反映させる
    /// </summary>
    private void ResotreLocale()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {
            AppLocale.SetLocale(LangType.LangJp);
        }
        else
        {
            ServerData.LangSettings langSettings = LocalStorageAPI.Instance.GetLangSettings();
            if (langSettings == null)
            {
                langSettings = new ServerData.LangSettings();
            }
            AppLocale.SetLocale((LangType)langSettings.langIndex);
        }
    }
}
