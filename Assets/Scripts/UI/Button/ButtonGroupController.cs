using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayMode = LifeGameConsts.PlayMode;


/// <summary>
/// ボタンをグループ化し、制御を行うクラス
/// </summary>
public class ButtonGroupController : MonoBehaviour
{
    [SerializeField] private List<GameObject> buttonGroups;

    private PlayMode lastPlayMode;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        lastPlayMode = GameController.Instance.GetPlayMode();
        ChangeActive(lastPlayMode);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        PlayMode playMode = GameController.Instance.GetPlayMode();
        if (lastPlayMode != playMode)
        {
            ChangeActive(playMode);
            lastPlayMode = playMode;
        }
    }

    /// <summary>
    /// 指定したプレーモードに応じて、グループ単位で有効、無効を切り替える
    /// </summary>
    /// <param name="playMode">プレーモード</param>
    private void ChangeActive(PlayMode playMode)
    {
        // プレーモードに対して操作するタグを取得
        string checkTag = GetCheckTag(playMode);
        foreach (var item in buttonGroups)
        {
            if (item.CompareTag(checkTag))
            {
                // タグに一致するオブジェクトを有効化
                item.SetActive(true);
            }
            else
            {
                // タグに一致しないオブジェクトを無効化
                item.SetActive(false);
            }
        }
    }

    /// <summary>
    /// プレーモードに対応するタグを取得する。
    /// </summary>
    /// <param name="playMode">プレーモード</param>
    /// <returns></returns>
    private string GetCheckTag(PlayMode playMode)
    {
        switch (playMode)
        {
            case PlayMode.Edit: return "EditModeButtons";
            case PlayMode.View: return "ViewModeButtons";
            default: return "";
        }
    }
}
