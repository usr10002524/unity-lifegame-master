using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BGM設定の各項目内容の処理を行うクラス
/// </summary>
public class SettingItemBgm : SettingItemBase
{
    [SerializeField] float fadeoutDuration;
    [SerializeField] BgmType bgmType;

    /// <summary>
    /// アイテムが選択状態となったときの処理。
    /// </summary>
    public override void OnSelected()
    {
        //対応するBGMのを再生する
        BgmManager.Instance.PlayBgm(bgmType);
    }

    /// <summary>
    /// アイテムが非選択状態となったときの処理。
    /// </summary>
    public override void OnDeselected()
    {
        //対応するBGMをフェードアウト後停止する
        BgmManager.Instance.FadeStopBgm(fadeoutDuration);
    }
}
