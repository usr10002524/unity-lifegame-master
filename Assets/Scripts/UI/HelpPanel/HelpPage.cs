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

    public void SetString(string str)
    {
        title = str;
        if (redrawEvent != null)
        {
            redrawEvent.Invoke();
        }
    }

    public void SetSprite(Sprite sprt)
    {
        sprite = sprt;
        if (redrawEvent != null)
        {
            redrawEvent.Invoke();
        }
    }
}
