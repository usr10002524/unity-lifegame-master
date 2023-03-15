using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サウンド管理クラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    [SerializeField] protected List<GameObject> audioList;
    [SerializeField] private float originalVolume;

    protected AudioSource audioSource;
    protected Dictionary<string, AudioClip> audioMap;

    private Coroutine fadeCoroutine;


    /// <summary>
    /// エディタで登録された、NamedAudioClipがアタッチされたゲームオブジェクトから
    /// マップデータを作成する
    /// </summary>
    protected void CraeteAudioMap()
    {
        audioMap = new Dictionary<string, AudioClip>();

        foreach (var item in audioList)
        {
            NamedAudioClip nac = item.GetComponent<NamedAudioClip>();
            if (nac != null)
            {
                if (!audioMap.ContainsKey(nac.name))
                {
                    audioMap.Add(nac.name, nac.audioClip);
                }
                else
                {
                    Debug.Log(string.Format("SoundManager.CraeteAudioMap() name={0} is already entried.", nac.name));
                }
            }
        }
    }

    /// <summary>
    /// AudioClipをPlayOneShotで再生する。
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    protected void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null)
        {
            if (clip != null)
            {
                audioSource.volume = originalVolume;
                audioSource.PlayOneShot(clip);
                // Debug.Log(string.Format("CraeteAudioMap.playAudioClip() play name={0} volume={1}", clip.name, originalVolume));
            }
            else
            {
                Debug.Log("CraeteAudioMap.playAudioClip() clip=null");
            }
        }
        else
        {
            Debug.Log("CraeteAudioMap.playAudioClip() audioSource=null");
        }
    }

    /// <summary>
    /// AudioClipを再生する。
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    protected void Play(AudioClip clip)
    {
        if (fadeCoroutine != null)
        {
            // フェードが動いていれば止める
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource != null)
        {
            if (clip != null)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.volume = originalVolume;
                audioSource.Play();
                // Debug.Log(string.Format("CraeteAudioMap.playAudioClip() play name={0} volume={1}", clip.name, originalVolume));

            }
            else
            {
                Debug.Log("CraeteAudioMap.playAudioClip() clip=null");
            }
        }
        else
        {
            Debug.Log("CraeteAudioMap.playAudioClip() audioSource=null");
        }
    }

    /// <summary>
    /// 再生中のAudioClipを停止する
    /// </summary>
    protected void stopAudioClip()
    {
        if (fadeCoroutine != null)
        {
            // フェードが動いていれば止める
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }

        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// 再生中のAudioClipをフェードアウト後、停止させる
    /// </summary>
    /// <param name="duration">フェードアウト時間（秒）</param>
    protected void fadeStopAudioClip(float duration)
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeoutStop(duration));
        }
    }

    /// <summary>
    /// フェードアウト＋停止用コルーチン
    /// </summary>
    /// <param name="duration">フェードアウト時間（秒）</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator FadeoutStop(float duration)
    {
        if (audioSource == null)
        {
            yield break;
        }

        bool isEnd = false;
        float fadeoutDuration = duration;
        float startVolume = audioSource.volume;
        float endVolume = 0.0f;
        float fadeTimer = 0.0f;

        if (fadeoutDuration <= 0.0f)
        {
            fadeoutDuration = 0.25f;
        }

        while (!isEnd)
        {
            yield return null;
            if (!audioSource.isPlaying)
            {
                break;  //もう止まっていれば何もしない
            }

            fadeTimer += Time.deltaTime;
            float time = Mathf.Clamp(fadeTimer / fadeoutDuration, 0.0f, 1.0f);
            float t = Mathf.Lerp(startVolume, endVolume, time);

            audioSource.volume = t;
            if (fadeTimer >= fadeoutDuration)
            {
                isEnd = true;
            }
        }

        audioSource.Stop();
        fadeCoroutine = null;
    }
}
