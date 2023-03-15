using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルに戻るボタンの処理
/// </summary>
public class ReturnTitle : MonoBehaviour
{
    [SerializeField] private float sceneTransitDuration = 0.5f;
    [SerializeField] private Color sceneTransitColor = Color.black;
    [SerializeField] private int sceneTransitSortOrder = 1;

    /// <summary>
    /// タイトルに戻るボタンが押されたときの処理
    /// </summary>
    public void OnClickButton()
    {
        float fadeDump = 1.0f;
        if (sceneTransitDuration > 0)
        {
            fadeDump = 1.0f / sceneTransitDuration;
        }
        else
        {
            fadeDump = 1.0f;
        }

        Initiate.Fade("TitleScene", sceneTransitColor, fadeDump, sceneTransitSortOrder);
    }
}
