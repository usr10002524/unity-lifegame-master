using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サウンドボリュームコントローラ
/// </summary>
public class SoundVolumeController : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameObject sliderObject;


    private SoundPanelController panelController;
    private SoundSliderController sliderController;

    private bool toggleButton;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (buttonObject != null)
        {
            panelController = buttonObject.GetComponent<SoundPanelController>();
        }
        if (sliderObject != null)
        {
            sliderController = sliderObject.GetComponent<SoundSliderController>();
        }

        toggleButton = false;
        CloseSlider();
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        OnChangeValue(AudioListener.volume);
    }

    /// <summary>
    /// サウンドボリュームのスライダーを表示する
    /// </summary>
    private void OpenSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(true);
        }

    }

    /// <summary>
    /// サウンドボリュームのスライダーを非表示にする
    /// </summary>
    private void CloseSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(false);
        }
        SaveSetting();
    }

    /// <summary>
    /// 設定を保存する
    /// </summary>
    private void SaveSetting()
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
            soundSettings.volume = AudioListener.volume;
            LocalStorageAPI.Instance.SaveSoundSettings(soundSettings);
        }
    }

    /// <summary>
    /// スライダーの開閉をトグルする
    /// </summary>
    public void OnTggleButton()
    {
        if (toggleButton)
        {
            CloseSlider();
        }
        else
        {
            OpenSlider();
        }
        toggleButton = !toggleButton;
    }

    /// <summary>
    /// スライダーを閉じる
    /// </summary>
    public void OnClose()
    {
        if (toggleButton)
        {
            CloseSlider();
            toggleButton = !toggleButton;
        }
    }

    /// <summary>
    /// スライダーの値が変更されたときに呼ばれる
    /// </summary>
    /// <param name="value">変更された値</param>
    public void OnChangeValue(float value)
    {
        if (sliderController != null)
        {
            sliderController.SetVolume(value);
        }
        if (panelController != null)
        {
            panelController.SetMute(!(value > 0));
        }
        AudioListener.volume = value;
    }

}
