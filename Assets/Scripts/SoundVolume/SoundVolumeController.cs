using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundVolumeController : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private GameObject sliderObject;


    private SoundPanelController panelController;
    private SoundSliderController sliderController;

    private bool toggleButton;

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

    private void Start()
    {
        OnChangeValue(AudioListener.volume);
    }


    private void OpenSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(true);
        }

    }

    private void CloseSlider()
    {
        if (sliderObject != null)
        {
            sliderObject.SetActive(false);
        }
        SaveSetting();
    }

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

    public void OnClose()
    {
        if (toggleButton)
        {
            CloseSlider();
            toggleButton = !toggleButton;
        }
    }

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
