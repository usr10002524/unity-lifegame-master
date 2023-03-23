using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// コンフィグメニューの各項目の設定内容に応じた処理を行うクラス
/// </summary>
public class SettingItemBase : MonoBehaviour
{
    [SerializeField] protected string itemName;

    /// <summary>
    /// 設定内容の名前を取得する
    /// </summary>
    /// <returns>設定内容の名前</returns>
    public string GetItemName()
    {
        return itemName;
    }

    /// <summary>
    /// ローカリゼーションのテキストを設定する。
    /// </summary>
    /// <param name="str">設定するテキスト</param>
    public virtual void SetString(string str)
    {
        itemName = str;
    }

    /// <summary>
    /// アイテムが選択状態となったときの処理。
    /// 要オーバーライド。
    /// </summary>
    public virtual void OnSelected() { }

    /// <summary>
    /// アイテムが非選択状態となったときの処理。
    /// 要オーバーライド。
    /// </summary>
    public virtual void OnDeselected() { }
}
