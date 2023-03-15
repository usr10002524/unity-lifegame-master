using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfigMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> itemList;
    [SerializeField] private GameObject titleObject;
    [SerializeField] private GameObject instObject;

    private static readonly string defaultInstText = "コンフィグでは各種設定を行うことができます";

    // Update is called once per frame
    void Update()
    {
        UpdateInstText();
    }

    /// <summary>
    /// 説明テキストの更新処理
    /// </summary>
    private void UpdateInstText()
    {
        if (instObject == null)
        {
            return;
        }

        TextMeshProUGUI textMeshPro = instObject.GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            return;
        }

        // 説明テキストに表示を行う
        string instText = "";
        // マウスオーバー中のアイテムがあれば、そのアイテムに対する説明を表示する。
        if (IsItemMouseOver(ref instText))
        {
            textMeshPro.text = instText;
        }
        // マウスオーバー中のアイテムがなければ、最後に選択したアイテムに対する説明を表示する。
        else if (IsItemSelected(ref instText))
        {
            textMeshPro.text = instText;
        }
        // マウスオーバー、選択中どちらのあいてむもなければデフォルトのテキストを表示する。
        else
        {
            textMeshPro.text = defaultInstText;
        }
    }

    /// <summary>
    /// マウスオーバー中のアイテムの説明テキストを取得する。
    /// </summary>
    /// <param name="instText">マウスオーバー中の説明テキスト</param>
    /// <returns>マウスオーバー中のテキストがあるか</returns>
    private bool IsItemMouseOver(ref string instText)
    {
        foreach (var item in itemList)
        {
            if (item == null)
            {
                continue;
            }

            ConfigItemBase configItem = item.GetComponent<ConfigItemBase>();
            if (configItem != null)
            {
                if (configItem.IsOver())
                {
                    instText = configItem.GetInstractionText();
                    return true;
                }
            }
        }

        instText = "";
        return false;
    }

    /// <summary>
    /// 選択中のアイテムの説明テキストを取得する。
    /// </summary>
    /// <param name="instText">選択中の説明テキスト</param>
    /// <returns>選択中のテキストがあるか</returns>
    private bool IsItemSelected(ref string instText)
    {
        foreach (var item in itemList)
        {
            if (item == null)
            {
                continue;
            }

            ConfigItemBase configItem = item.GetComponent<ConfigItemBase>();
            if (configItem != null)
            {
                if (configItem.IsSelected())
                {
                    instText = configItem.GetInstractionText();
                    return true;
                }
            }
        }

        instText = "";
        return false;
    }

    /// <summary>
    /// コンフィグメニューを抜ける際の処理を行う
    /// </summary>
    public void OnExitMenu()
    {
        //各メニューに対し、メニューを抜ける際の処理を呼ぶ
        foreach (var item in itemList)
        {
            if (item == null)
            {
                continue;
            }

            ConfigItemBase configItem = item.GetComponent<ConfigItemBase>();
            if (configItem != null)
            {
                configItem.OnExitMenu();
            }
        }
    }
}
