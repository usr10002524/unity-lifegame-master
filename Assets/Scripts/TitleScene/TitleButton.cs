using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// タイトル画面の各種ボタンを制御するクラス
/// </summary>
public class TitleButton : MonoBehaviour
{
    private EventTrigger eventTrigger;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        SetupEvent();
    }

    /// <summary>
    /// イベントリスナーの準備を行う
    /// </summary>
    private void SetupEvent()
    {
        // イベントを登録する
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener(PointerEnterCallback);
        eventTrigger.triggers.Add(entry);
    }

    /// <summary>
    /// オブジェクトにマウスが重なった際のコールバック
    /// </summary>
    /// <param name="data">BaseEventData</param>
    public void PointerEnterCallback(BaseEventData data)
    {
        SeManager.Instance.PlaySe(SeType.seSelect);
    }

    /// <summary>
    /// オブジェクトがクリックされた際のコールバック
    /// </summary>
    public void ClickCallback()
    {
        SeManager.Instance.PlaySe(SeType.seStart);
    }
}
