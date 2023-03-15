using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayMode = LifeGameConsts.PlayMode;


/// <summary>
/// サマリーパネル プレーモードミニボタンの制御を行うクラス
/// </summary>
public class PlayModeMiniButton : MiniButtonControl
{
    private class SpriteName
    {
        public static readonly string edit = "edit_w";
        public static readonly string view = "view_w";
    }

    private PlayMode lastPlayMode;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        BaseStart();
        lastPlayMode = GameController.Instance.GetPlayMode();

        string spriteName = GetSpriteName(lastPlayMode);
        ReplaceSprite(spriteName);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        PlayMode playMode = GameController.Instance.GetPlayMode();
        if (playMode != lastPlayMode)
        {
            lastPlayMode = playMode;
            string spriteName = GetSpriteName(playMode);
            ReplaceSprite(spriteName);
        }
    }

    /// <summary>
    /// 指定したプレーモードに対応したスプライト名を取得する
    /// </summary>
    /// <param name="playMode">プレーモード</param>
    /// <returns>スプライト名</returns>
    private string GetSpriteName(PlayMode playMode)
    {
        switch (playMode)
        {
            case PlayMode.Edit: return SpriteName.edit;
            case PlayMode.View: return SpriteName.view;
            default: return "";
        }
    }
}
