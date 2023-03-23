using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundPanelController : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite muteSprite;

    private bool isMute;
    private Image buttonImage;


    public void SetMute(bool flag)
    {
        isMute = flag;
        UpdateIcon();
    }



    private void Awake()
    {
        if (buttonObject != null)
        {
            buttonImage = buttonObject.GetComponent<Image>();
        }
    }

    private void UpdateIcon()
    {
        if (isMute)
        {
            if (buttonImage != null && muteSprite != null)
            {
                buttonImage.sprite = muteSprite;
            }
        }
        else
        {
            if (buttonImage != null && normalSprite != null)
            {
                buttonImage.sprite = normalSprite;
            }
        }
    }
}
