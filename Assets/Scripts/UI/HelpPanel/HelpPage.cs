using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ヘルプ1ページあたりのデータを保持するクラス
/// </summary>
public class HelpPage : MonoBehaviour
{
    [SerializeField] public Sprite sprite;
    [SerializeField] public string title;
    [SerializeField] private UnityEvent redrawEvent;

    /// <summary>
    /// 文字列をセットする
    /// </summary>
    /// <param name="str">文字列</param>
    public void SetString(string str)
    {
        title = str;
        if (redrawEvent != null)
        {
            redrawEvent.Invoke();
        }
    }

    /// <summary>
    /// スプライトをセットする
    /// </summary>
    /// <param name="sprt">スプライト</param>
    public void SetSprite(Sprite sprt)
    {
        sprite = sprt;
        if (redrawEvent != null)
        {
            redrawEvent.Invoke();
        }
    }
}
