using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM管理クラス
/// </summary>
public class BgmManager : SoundManager
{
    public static BgmManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CraeteAudioMap();
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="type">再生するBGMのタイプ</param>
    public void PlayBgm(BgmType type)
    {
        // Debug.Log(string.Format("PlayBgm() called. type={0}", type));

        string strType = type.ToString();
        PlayBgm(strType);
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    /// <param name="strType">再生するBGMのタイプ名</param>
    public void PlayBgm(string strType)
    {
        // Debug.Log(string.Format("PlayBgm() called. strType={0}", strType));
        if (audioMap.ContainsKey(strType))
        {
            AudioClip clip = audioMap[strType];
            if (clip != null)
            {
                Play(clip);
            }
            else
            {
                Debug.Log(string.Format("PlayBgm() '{0}' audioClip is null.", strType));
            }
        }
        else
        {
            Debug.Log(string.Format("PlayBgm() '{0}' not found.", strType));
        }
    }

    /// <summary>
    /// 再生中のBGMを停止する
    /// </summary>
    public void StopBgm()
    {
        stopAudioClip();
    }

    /// <summary>
    /// 生成中のBGMをフェードアウトさせ、その後停止する。
    /// </summary>
    /// <param name="duration">フェード時間（秒）</param>
    public void FadeStopBgm(float duration)
    {
        fadeStopAudioClip(duration);
    }
}
