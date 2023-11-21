using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// サウンドボリュームパネルコントローラ
/// </summary>
public class SoundPanelController : MonoBehaviour
{
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite muteSprite;

    private bool isMute;
    private Image buttonImage;


    /// <summary>
    /// ミュートの設定、解除を行う
    /// </summary>
    /// <param name="flag">ミュートフラグ</param>
    public void SetMute(bool flag)
    {
        isMute = flag;
        UpdateIcon();
    }


    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (buttonObject != null)
        {
            buttonImage = buttonObject.GetComponent<Image>();
        }
    }

    /// <summary>
    /// アイコン表示を更新する
    /// </summary>
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
