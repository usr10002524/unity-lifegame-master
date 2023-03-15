using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// エイリアス
using CellStat = LifeGameConsts.Cell.Stat;
using PlayMode = LifeGameConsts.PlayMode;
using PlaySpeed = LifeGameConsts.PlaySpeed;
using WorldSize = LifeGameConsts.WorldSize;

/// <summary>
/// タイトル画面でのワールドの処理を行うクラス
/// </summary>
public class CellWorldTitle : CellWorld
{
    private bool initialized = false;

    /// <summary>
    /// ワールドの横方向のセル数を取得する。
    /// </summary>
    /// <returns>ワールドの横方向のセル数</returns>
    public int GetColCount()
    {
        return colCount;
    }

    /// <summary>
    /// ワールドの縦方向のセル数を取得する。
    /// </summary>
    /// <returns>ワールドの縦方向のセル数</returns>
    public int GetRowCount()
    {
        return rowCount;
    }

    /// <summary>
    /// 指定した位置のセルの状態を取得する。
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <returns>セルの状態</returns>
    public CellStat GetCellStat(int col, int row)
    {
        return core.GetCellStat(col, row);
    }

    /// <summary>
    /// 指定した位置のセルの状態を設定する。
    /// </summary>
    /// <param name="col">セルの位置</param>
    /// <param name="row">セルの位置</param>
    /// <param name="stat">セルの状態</param>
    public void SetCellStat(int col, int row, CellStat stat)
    {
        core.SetCellStat(col, row, stat);
    }

    /// <summary>
    /// セルの状態を即時反映させる
    /// </summary>
    public void ApplyCellStat()
    {
        ForceRedraw();
    }

    /// <summary>
    /// ワールドをクリアする
    /// </summary>
    public void AllCellClear()
    {
        core.Reset();
        ForceRedraw();
    }

    /// <summary>
    /// プレーモードを設定する。
    /// </summary>
    /// <param name="playMode">プレーモード</param>
    public void SetPlayMode(PlayMode playMode)
    {
        ChangeMode(playMode);
    }

    /// <summary>
    /// 初期化済みフラグを取得する。
    /// </summary>
    /// <returns>初期化済みフラグ</returns>
    public bool IsInitialized()
    {
        return initialized;
    }

    /// <summary>
    /// Start時呼ばれる関数
    /// </summary>
    private void Start()
    {
        cells = new List<GameObject>();
        InitializeCore();
        CreateWorld(worldBase);
        // SeupCamera();
        lastPlayMode = GameController.Instance.GetPlayMode();
        initialized = true;
    }

    /// <summary>
    /// 定期処理
    /// </summary>
    private void Update()
    {
        // ワールドのクリア要求が来ていれば、クリアを行う
        if (GameController.Instance.IsRequestClear())
        {
            core.Reset();
            ForceRedraw();
            GameController.Instance.ResetRequestClear();
            return;
        }

        // プレースピードが変わっていたら変更する
        PlaySpeed playSpeed = GameController.Instance.GetPlaySpeed();
        if (playSpeed != lastPlaySpeed)
        {
            lastPlaySpeed = playSpeed;
        }

        //プレーモードが変わっていたら切り替える
        PlayMode playMode = GameController.Instance.GetPlayMode();
        if (playMode != lastPlayMode)
        {
            ChangeMode(playMode);
            lastPlayMode = playMode;
        }

        if (playMode == PlayMode.Edit)
        {
            UpdateWorldInEditMode();
        }
    }

    /// <summary>
    /// ライフゲームコアの初期化
    /// </summary>
    private void InitializeCore()
    {
        colCount = (int)WorldSize.Default;
        rowCount = (int)WorldSize.Default;

        core = new LifeGame();

        LifeGame.Initializer initializer = new LifeGame.Initializer();
        initializer.cols = colCount;
        initializer.rows = rowCount;
        initializer.loopWorld = true;

        core.Initialize(initializer);
    }
}
