using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayMode = LifeGameConsts.PlayMode;
using EditMode = LifeGameConsts.EditMode;

/// <summary>
/// サマリーパネル 編集モードミニボタンの制御を行うクラス
/// </summary>
public class EditModeMiniButton : MiniButtonControl
{

    private class SpriteName
    {
        public static readonly string write = "write_w";
        public static readonly string write_g = "write_g";
        public static readonly string erase = "erase_w";
        public static readonly string pattern = "pattern_w";
    }

    private PlayMode lastPlayMode;
    private EditMode lastEditMode;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        BaseStart();
        lastPlayMode = GameController.Instance.GetPlayMode();
        lastEditMode = GameController.Instance.GetEditMode();

        string spriteName = GetSpriteName(lastPlayMode, lastEditMode);
        ReplaceSprite(spriteName);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        PlayMode playMode = GameController.Instance.GetPlayMode();
        EditMode editMode = GameController.Instance.GetEditMode();
        if ((playMode != lastPlayMode) || (editMode != lastEditMode))
        {
            lastPlayMode = playMode;
            lastEditMode = editMode;
            string spriteName = GetSpriteName(playMode, editMode);
            ReplaceSprite(spriteName);
        }
    }

    /// <summary>
    /// 指定した、プレーモード、編集モードに対応したスプライト名を取得する
    /// </summary>
    /// <param name="playMode">プレーモード</param>
    /// <param name="editMode">編集モード</param>
    /// <returns>スプライト名</returns>
    private string GetSpriteName(PlayMode playMode, EditMode editMode)
    {
        if (playMode == PlayMode.View)
        {
            return SpriteName.write_g;
        }
        else
        {
            switch (editMode)
            {
                case EditMode.Write: return SpriteName.write;
                case EditMode.Erase: return SpriteName.erase;
                case EditMode.PatternPaste: return SpriteName.pattern;
                default: return "";
            }
        }
    }
}
