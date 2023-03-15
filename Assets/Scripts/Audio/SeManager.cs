using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SE 管理クラス
/// </summary>
public class SeManager : SoundManager
{
    public static SeManager Instance { get; private set; }

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
    /// 指定したSEを再生する。
    /// </summary>
    /// <param name="type">再生するSEのタイプ</param>
    public void PlaySe(SeType type)
    {
        // Debug.Log(string.Format("PlaySe() called. type={0}", type));

        string strType = type.ToString();
        PlaySe(strType);
    }

    /// <summary>
    /// 指定したSEを再生する
    /// </summary>
    /// <param name="strType">再生するSEのタイプ名</param>
    public void PlaySe(string strType)
    {
        // Debug.Log(string.Format("PlaySe() called. strType={0}", strType));
        if (audioMap.ContainsKey(strType))
        {
            AudioClip clip = audioMap[strType];
            if (clip != null)
            {
                PlayOneShot(clip);
            }
            else
            {
                Debug.Log(string.Format("PlaySe() '{0}' audioClip is null.", strType));
            }
        }
        else
        {
            Debug.Log(string.Format("PlaySe() '{0}' not found.", strType));
        }
    }
}
