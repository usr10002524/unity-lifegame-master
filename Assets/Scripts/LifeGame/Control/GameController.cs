using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// エイリアス
using PlayMode = LifeGameConsts.PlayMode;
using EditMode = LifeGameConsts.EditMode;
using PlaySpeed = LifeGameConsts.PlaySpeed;

/// <summary>
/// UIとゲーム制御をつなぐクラス
/// </summary>
public class GameController : MonoBehaviour
{
    [SerializeField] private PlayMode playMode = PlayMode.Edit;
    [SerializeField] private EditMode editMode = EditMode.Write;
    [SerializeField] private PlaySpeed playSpeed = PlaySpeed.Medium;
    [SerializeField] private bool requestClear = false;
    [SerializeField] private bool requestSave = false;

    [SerializeField] private float sceneTransitDuration = 0.5f;
    [SerializeField] private Color sceneTransitColor = Color.black;
    [SerializeField] private int sceneTransitSortOrder = 1;

    private float fadeDump = 1.0f;
    private string patternName;
    private PatternLoader.PatternData patternData;
    [SerializeField] private bool requestPaste = false;
    [SerializeField] private Vector2Int pastePos = new Vector2Int();
    [SerializeField] private Vector2Int pasteOffs = new Vector2Int();


    public static GameController Instance { get; private set; }

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        if (sceneTransitDuration > 0)
        {
            fadeDump = 1.0f / sceneTransitDuration;
        }
        else
        {
            fadeDump = 1.0f;
        }
    }

    /// <summary>
    /// プレーモードを取得する。
    /// </summary>
    /// <returns>プレーモード</returns>
    public PlayMode GetPlayMode()
    {
        return playMode;
    }

    /// <summary>
    /// 現在のプレーモードが編集モードかどうか確認する。
    /// </summary>
    /// <returns>プレーモードが編集モードのときはtrue、そうでないときはfalseを返す。</returns>
    public bool IsEditMode()
    {
        return (playMode == PlayMode.Edit);
    }

    /// <summary>
    /// 現在のプレーモードが鑑賞モードかどうか確認する。
    /// </summary>
    /// <returns>プレーモードが鑑賞モードのときはtrue、そうでないときはfalseを返す。</returns>
    public bool IsViewMode()
    {
        return (playMode == PlayMode.View);
    }

    /// <summary>
    /// 現在の編集モードを取得する。
    /// </summary>
    /// <returns></returns>
    public EditMode GetEditMode()
    {
        return editMode;
    }

    /// <summary>
    /// 現在の編集モードが書き込みかどうか確認する。
    /// </summary>
    /// <returns>編集モードが書き込みのときはtrue、そうでないときはfalseを返す。</returns>
    public bool IsWriteMode()
    {
        return (editMode == EditMode.Write);
    }

    /// <summary>
    /// 現在の編集モードが消去かどうか確認する。
    /// </summary>
    /// <returns>編集モードが消去のときはtrue、そうでないときはfalseを返す。</returns>
    public bool IsEraseMode()
    {
        return (editMode == EditMode.Erase);
    }

    /// <summary>
    /// 現在の編集モードがパターン貼り付けかどうか確認する。
    /// </summary>
    /// <returns>編集モードがパターン貼り付けのときはtrue、そうでないときはfalseを返す。</returns>
    public bool IsPatternPasteMode()
    {
        return (editMode == EditMode.PatternPaste);
    }

    /// <summary>
    /// クリア要求が来ているか確認する。
    /// </summary>
    /// <returns>クリア要求が来ているときはtrue、そうでないときはfalseを返す</returns>
    public bool IsRequestClear()
    {
        return requestClear;
    }

    /// <summary>
    /// クリア要求フラグをリセットする。
    /// </summary>
    public void ResetRequestClear()
    {
        requestClear = false;
    }

    /// <summary>
    /// タイトル遷移要求が来ているか確認する。
    /// </summary>
    /// <returns>タイトル遷移要求が来ているときはtrue、そうでないときはfalseを返す</returns>
    public bool IsRequestSave()
    {
        return requestSave;
    }

    /// <summary>
    /// タイトル遷移要求フラグをリセットする。
    /// </summary>
    public void ResetRequestSave()
    {
        requestSave = false;
    }

    /// <summary>
    /// 現在のプレースピードを取得する。
    /// </summary>
    /// <returns>現在のプレースピード</returns>
    public PlaySpeed GetPlaySpeed()
    {
        return playSpeed;
    }


    /// <summary>
    /// 編集モードボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickEditButton()
    {
        playMode = PlayMode.Edit;
        editMode = EditMode.Write;
    }

    /// <summary>
    /// 鑑賞モードボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickViewButton()
    {
        playMode = PlayMode.View;
        editMode = EditMode.Write;
    }

    /// <summary>
    /// 書き込みボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickWriteButton()
    {
        if (playMode == PlayMode.Edit)
        {
            editMode = EditMode.Write;
        }
    }

    /// <summary>
    /// 消去ボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickEraseButton()
    {
        if (playMode == PlayMode.Edit)
        {
            editMode = EditMode.Erase;
        }
    }

    /// <summary>
    /// クリアボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickClearButton()
    {
        requestClear = true;
    }

    /// <summary>
    /// スピードアップボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickSpeedUpButton()
    {
        playSpeed = SpeedUp(playSpeed);
        // Debug.Log(string.Format("OnClickSpeedUpButton() speed:{0}", playSpeed));
    }

    /// <summary>
    /// スピードダウンボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickSpeedDownButton()
    {
        playSpeed = SpeedDown(playSpeed);
        // Debug.Log(string.Format("OnClickSpeedDownButton() speed:{0}", playSpeed));
    }

    /// <summary>
    /// 保存ボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickSaveButton()
    {
        if (AtsumaruAPI.Instance.IsValid())
        {
            requestSave = true;
        }
    }

    /// <summary>
    /// タイトル遷移ボタンが押されたときの処理を行う。
    /// </summary>
    public void OnClickReturnButton()
    {
        // Debug.Log(string.Format("OnClickReturnButton()"));
        Initiate.Fade("TitleScene", sceneTransitColor, fadeDump, sceneTransitSortOrder);
    }

    /// <summary>
    /// パターン選択が行われたときの処理を行う。
    /// </summary>
    /// <param name="name">パターン名</param>
    public void OnClickPatternSelect(string name, PatternLoader.PatternData data)
    {
        if (playMode == PlayMode.Edit)
        {
            editMode = EditMode.PatternPaste;
            patternName = name;
            patternData = data;
        }
    }

    /// <summary>
    /// 現在選択中のパターン名を取得する。
    /// 編集モードかつ、パターン貼り付けモードのときのみ有効な値が取得できます。
    /// </summary>
    /// <returns></returns>
    public string GetPatternName()
    {
        if (playMode == PlayMode.Edit)
        {
            if (editMode == EditMode.PatternPaste)
            {
                return patternName; // 編集モードかつ、パターン貼り付けモードのときのみ返す。
            }
        }

        return "";
    }

    /// <summary>
    /// 現在選択中のパターンデータを取得する。
    /// 編集モードかつ、パターン貼り付けモードのときのみ有効な値が取得できます。
    /// </summary>
    /// <returns></returns>
    public PatternLoader.PatternData GetPatternData()
    {
        if (playMode == PlayMode.Edit)
        {
            if (editMode == EditMode.PatternPaste)
            {
                return patternData; // 編集モードかつ、パターン貼り付けモードのときのみ返す。
            }
        }

        return null;
    }

    public void PatternPaste(Vector2Int mousePos, Vector2Int offsLeftTop)
    {
        if (playMode == PlayMode.Edit)
        {
            if (editMode == EditMode.PatternPaste)
            {
                requestPaste = true;
                pastePos = mousePos;
                pasteOffs = offsLeftTop;
            }
        }
    }

    public bool IsRequestPaste(out Vector2Int pastePos, out Vector2Int pasteOffs, out PatternLoader.PatternData patternData)
    {
        pastePos = this.pastePos;
        pasteOffs = this.pasteOffs;
        patternData = this.patternData;

        return requestPaste;
    }

    public void ClearRequestPaste()
    {
        requestPaste = false;
    }


    /// <summary>
    /// 指定したプレースピードのひとつ上のプレースピードを取得する。
    /// </summary>
    /// <param name="speed">プレースピード</param>
    /// <returns>ひとつ上のプレースピード</returns>
    private PlaySpeed SpeedUp(PlaySpeed speed)
    {
        switch (speed)
        {
            case PlaySpeed.Slow: return PlaySpeed.Medium;
            case PlaySpeed.Medium: return PlaySpeed.Fast;
            case PlaySpeed.Fast: return PlaySpeed.Fastest;
            default: return speed;  //変更しない
        }
    }

    /// <summary>
    /// 指定したプレースピードのひとつ下のプレースピードを取得する。
    /// </summary>
    /// <param name="speed">プレースピード</param>
    /// <returns>ひとつ下のプレースピード</returns>
    private PlaySpeed SpeedDown(PlaySpeed speed)
    {
        switch (speed)
        {
            case PlaySpeed.Medium: return PlaySpeed.Slow;
            case PlaySpeed.Fast: return PlaySpeed.Medium;
            case PlaySpeed.Fastest: return PlaySpeed.Fast;
            default: return speed;  //変更しない
        }
    }
}
