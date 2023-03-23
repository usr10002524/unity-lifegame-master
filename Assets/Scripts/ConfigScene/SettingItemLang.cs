using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM設定の各項目内容の処理を行うクラス
/// </summary>
public class SettingItemLang : SettingItemBase
{
    [SerializeField] float fadeoutDuration;
    [SerializeField] LangType langType;

    /// <summary>
    /// アイテムが選択状態となったときの処理。
    /// </summary>
    public override void OnSelected()
    {
        //対応する言語に切り替える
        AppLocale.SetLocale(langType);
    }
}
