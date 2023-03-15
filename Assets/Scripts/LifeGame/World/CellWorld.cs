using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using LifeGameConsts;
using System;

// エイリアス
using CellStat = LifeGameConsts.Cell.Stat;
using PlayMode = LifeGameConsts.PlayMode;
using PlaySpeed = LifeGameConsts.PlaySpeed;
using WorldSize = LifeGameConsts.WorldSize;


/// <summary>
/// ワールドの処理を行うクラス
/// </summary>
public class CellWorld : MonoBehaviour
{
    // ベースとなるゲームオブジェクト
    [SerializeField] protected GameObject worldBase;
    // セル1つあたりのゲームオブジェクト
    [SerializeField] protected GameObject cellPrefab;
    // ワールドの横に並べるセル数
    protected int colCount;
    // ワールドの縦に並べるセル数
    protected int rowCount;
    // プレーモードごとの更新間隔
    [SerializeField] protected float[] playModeInterval = new float[(int)PlaySpeed.Max] { 1.0f, 1.0f, 1.0f, 1.0f };

    // ライフゲームのコア
    protected LifeGame core;
    // ワールドに存在するセルのリスト
    protected List<GameObject> cells;
    // カーソルのゲームオブジェクト
    protected GameObject cursor;
    // ワールドの横幅
    protected float worldWidth;
    // ワールドの縦幅
    protected float worldHeight;
    // Viewモードのコルーチン
    protected Coroutine viewModeCoroutine;

    // 前フレームのプレーモード
    protected PlayMode lastPlayMode;
    // 前フレームのプレースピード
    protected PlaySpeed lastPlaySpeed;


    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        rowCount = (int)WorldSize.Default;
        colCount = (int)WorldSize.Default;
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        cells = new List<GameObject>();

        InitializeCore();
        CreateWorld(worldBase);
        SeupCamera();
        PlayBgm();
        lastPlayMode = GameController.Instance.GetPlayMode();
    }

    /// <summary>
    /// Update
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

        // ワールドのセーブ要求が来ていればセーブを行う
        if (GameController.Instance.IsRequestSave())
        {
            ServerData.WorldData worldData = SetupWorldData();
            AtsumaruAPI.Instance.SaveWorldData(worldData);
            GameController.Instance.ResetRequestSave();
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
        bool useContinue = false;
        string cellData = "";

        GameManager.StartStat startStat = GameManager.Instance.GetStartStat();
        ServerData.WorldData worldData = AtsumaruAPI.Instance.GetWorldData();

        if (startStat == GameManager.StartStat.Continue)
        {
            if (worldData.IsValid())
            {
                useContinue = true;
            }
        }

        if (useContinue)
        {
            colCount = worldData.cols;
            rowCount = worldData.rows;
            cellData = worldData.cells;
        }
        else
        {
            colCount = (int)WorldSize.Default;
            rowCount = (int)WorldSize.Default;
            cellData = "";
        }


        core = new LifeGame();

        LifeGame.Initializer initializer = new LifeGame.Initializer();
        initializer.cols = colCount;
        initializer.rows = rowCount;
        initializer.loopWorld = true;

        core.Initialize(initializer);
        if (useContinue)
        {
            core.RestoreFromString(cellData);
        }
    }

    /// <summary>
    /// ワールドにセルを配置する。
    /// </summary>
    /// <param name="parent">ワールドの親となるゲームオブジェクト</param>
    protected void CreateWorld(GameObject parent)
    {
        cells.Clear();

        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        // セルのスプライトからPPUを取得し、縦横のサイズを算出する
        float PPU = renderer.sprite.pixelsPerUnit;
        float cellWidth = renderer.sprite.texture.width / PPU;
        float cellHeight = renderer.sprite.texture.height / PPU;
        // Z位置はそのまま使用する
        float cellZ = cellPrefab.transform.position.z;
        // ワールドのサイズを算出する
        worldWidth = cellWidth * colCount;
        worldHeight = cellHeight * rowCount;
        // ワールドは左上を原点としてセルを並べるので、そのための原点を算出する
        float offsLeft = -((worldWidth - cellWidth) / 2.0f);
        float offsTop = ((worldHeight - cellHeight) / 2.0f);

        // セルを指定されたオブジェクトの子オブジェクトとして生成する
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                float x = offsLeft + c * cellWidth;
                float y = offsTop - r * cellHeight;
                float z = cellZ;
                Vector3 position = new Vector3(x, y, z);

                // セルの生成
                GameObject child = Instantiate(cellPrefab, position, cellPrefab.transform.rotation, parent.transform);
                // セルは入力を受け付ける。その際セルがどこの位置かわかるように座標を渡しておく。
                CellInput input = child.GetComponent<CellInput>();
                if (input != null)
                {
                    input.SetPosition(new Vector2Int(c, r));
                }

                cells.Add(child);
            }
        }
        ForceRedraw();
    }

    /// <summary>
    /// カメラの初期化を行う。
    /// ワールドのサイズに応じて、カメラの位置、移動制限のパラメータを変更している。
    /// </summary>
    private void SeupCamera()
    {
        float orthoSize = worldHeight * 10.0f / 16.0f;

        // カメラコンポーネントを探して、コントローラにワールドのサイズを渡す
        GameObject virtualCamera = GameObject.Find("Virtual Camera");
        if (virtualCamera != null)
        {
            CameraController controller = virtualCamera.GetComponent<CameraController>();
            if (controller != null)
            {
                controller.Setup(orthoSize);
            }
        }
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    private void PlayBgm()
    {
        BgmType bgmType = BgmType.bgm01;
        ServerData.SoundSettings settings = AtsumaruAPI.Instance.GetSoundSettings();
        if (settings.IsValid())
        {
            BgmType type = (BgmType)Enum.ToObject(typeof(BgmType), settings.bgmIndex);
            if (Enum.IsDefined(typeof(BgmType), type))
            {
                bgmType = type;
            }
        }

        // Debug.Log(string.Format("PlayBgm() bgm:{0}", bgmType));
        BgmManager.Instance.PlayBgm(bgmType);
    }

    /// <summary>
    /// 編集モード中の更新処理
    /// </summary>
    protected void UpdateWorldInEditMode()
    {
        EditMode editMode = GameController.Instance.GetEditMode();
        switch (editMode)
        {
            case EditMode.Write:
            case EditMode.Erase:
                UpdateWorldInEditMode_WriteErase();
                break;

            case EditMode.PatternPaste:
                UpdateWorldInEditMode_PatternPaste();
                break;
        }
    }

    public void UpdateWorldInEditMode_WriteErase()
    {
        List<LifeGame.UpdateInfo> updateInfos = new List<LifeGame.UpdateInfo>();
        EditMode editMode = GameController.Instance.GetEditMode();

        foreach (var cell in cells)
        {
            CellInput input = cell.GetComponent<CellInput>();
            if (input == null)
            {
                continue;
            }

            // 入力があれば更新情報を作成
            if (input.IsClicked())
            {
                Vector2Int position = input.GetPosition();
                CellStat lastStat = core.GetCellStat(position.x, position.y);

                //選択中のブラシによって生存or死亡を決める
                CellStat stat = CellStat.None;
                if (editMode == EditMode.Write)
                {
                    stat = CellStat.Alive;
                }
                else if (editMode == EditMode.Erase)
                {
                    stat = CellStat.Dead;
                }
                else
                {
                    continue;
                }

                // 変更があれば更新リストに追加
                if (lastStat != stat)
                {

                    updateInfos.Add(new LifeGame.UpdateInfo(position.x, position.y, stat, lastStat));
                }
            }
        }

        ApplyUpdateInfo(updateInfos, PlayMode.Edit);
    }

    public void UpdateWorldInEditMode_PatternPaste()
    {
        Vector2Int pos;
        Vector2Int offset;
        PatternLoader.PatternData patternData;

        bool request = GameController.Instance.IsRequestPaste(out pos, out offset, out patternData);
        if (!request)
        {
            return; //貼り付け要求なし
        }
        // 要求をクリアしておく
        GameController.Instance.ClearRequestPaste();

        // パターン貼り付けの始点とサイズ
        Vector2Int leftTop = pos + offset;
        int patternWidth = patternData.GetWidth();
        int patternHeight = patternData.GetHeight();

        // 更新情報リスト
        List<LifeGame.UpdateInfo> updateInfos = new List<LifeGame.UpdateInfo>();

        for (int r = 0; r < patternHeight; r++)
        {
            for (int c = 0; c < patternWidth; c++)
            {
                // ワールドの外にはみ出る場合はラップする
                int col = ((leftTop.x + c) + colCount) % colCount;
                int row = ((leftTop.y + r) + rowCount) % rowCount;

                // パターンの状態を取得する
                int pattern = 0;
                patternData.GetStat(c, r, ref pattern);

                CellStat lastStat = core.GetCellStat(col, row);
                CellStat stat = (pattern == 0) ? CellStat.Dead : CellStat.Alive;

                // 変更があれば更新リストに追加
                if (lastStat != stat)
                {
                    updateInfos.Add(new LifeGame.UpdateInfo(col, row, stat, lastStat));
                }
            }
        }

        ApplyUpdateInfo(updateInfos, PlayMode.Edit);
    }

    /// <summary>
    /// 鑑賞モード中の更新処理
    /// </summary>
    protected void UpdateWorldInViewMode()
    {
        List<LifeGame.UpdateInfo> updateInfos = core.Proceed();

        ApplyUpdateInfo(updateInfos, PlayMode.View);
    }

    /// <summary>
    /// 更新情報をワールドに適応する
    /// </summary>
    /// <param name="updateInfos">更新情報リスト</param>
    /// <param name="playMode">現在のプレーモード</param>
    protected void ApplyUpdateInfo(List<LifeGame.UpdateInfo> updateInfos, PlayMode playMode)
    {
        foreach (var info in updateInfos)
        {
            // コアを更新
            core.SetCellStat(info.col, info.row, info.stat);
            // 表示を更新
            int index = core.Coords2index(info.col, info.row);
            if (index >= 0 && index < cells.Count)
            {
                GameObject cell = cells[index];
                ImageController image = cell.GetComponent<ImageController>();

                SetCellImage(image, info.stat, playMode);
            }
        }
    }

    /// <summary>
    /// ImageController に対して、アニメーションの設定を行う
    /// </summary>
    /// <param name="image">ImageController</param>
    /// <param name="stat">セルの状態</param>
    /// <param name="playMode">現在のプレーモード</param>
    protected void SetCellImage(ImageController image, CellStat stat, PlayMode playMode)
    {
        if (image == null)
        {
            return;
        }

        float speedRatio = GetAnimationSpeedRatio();

        if (stat == CellStat.Alive)
        {
            if (playMode == PlayMode.Edit)
            {
                //編集モードのときは即変化
                image.Stop(ImageController.StopPosition.End);
            }
            else if (playMode == PlayMode.View)
            {
                //鑑賞モードのときは色変化アニメーションを再生
                image.Play(speedRatio);
            }
        }
        else if (stat == CellStat.Dead)
        {
            if (playMode == PlayMode.Edit)
            {
                //編集モードのときは即変化
                image.Stop(ImageController.StopPosition.Start);
            }
            else if (playMode == PlayMode.View)
            {
                //鑑賞モードのときは色変化アニメーションを再生
                bool reverse = true;
                image.Play(speedRatio, reverse);
            }
        }
    }

    /// <summary>
    /// 全セルを現在の状態に合わせて再描画を行う。
    /// </summary>
    protected void ForceRedraw()
    {
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                CellStat stat = core.GetCellStat(c, r);
                int index = core.Coords2index(c, r);
                if (index > 0 && index < cells.Count)
                {
                    GameObject cell = cells[index];
                    //表示を更新
                    ImageController image = cell.GetComponent<ImageController>();
                    SetCellImage(image, stat, PlayMode.Edit);
                }
            }
        }
    }

    /// <summary>
    /// プレイモードの変更時の切替処理を行う。
    /// </summary>
    /// <param name="playMode">変更後のプレイモード</param>
    protected void ChangeMode(PlayMode playMode)
    {
        if (playMode == PlayMode.Edit)
        {
            //鑑賞モードのコルーチンが起動していれば止める
            if (viewModeCoroutine != null)
            {
                StopCoroutine(viewModeCoroutine);
                viewModeCoroutine = null;
            }
            //全セルを再描画
            ForceRedraw();
        }
        else if (playMode == PlayMode.View)
        {
            //鑑賞モードのコルーチンが起動していれば止める
            if (viewModeCoroutine != null)
            {
                StopCoroutine(viewModeCoroutine);
                viewModeCoroutine = null;
            }
            viewModeCoroutine = StartCoroutine(UpdateViewMode());
        }
    }

    /// <summary>
    /// Viewモード中に起動するコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    protected IEnumerator UpdateViewMode()
    {
        while (true)
        {
            float interval = GetPlayModeInterval();
            yield return new WaitForSeconds(interval);

            UpdateWorldInViewMode();
        }
    }

    /// <summary>
    /// 現在の鑑賞モードの更新間隔を取得する
    /// </summary>
    /// <returns>現在の鑑賞モードの更新間隔</returns>
    private float GetPlayModeInterval()
    {
        return playModeInterval[(int)lastPlaySpeed];
    }

    /// <summary>
    /// 現在の鑑賞モードのアニメーションスピード比率を取得する
    /// </summary>
    /// <returns>スピード比率</returns>
    private float GetAnimationSpeedRatio()
    {
        float baseSpeed = playModeInterval[(int)PlaySpeed.Medium];
        float current = GetPlayModeInterval();
        float ratio = Mathf.Max(baseSpeed / current, 0.1f);    //遅くなりすぎないようにする

        return ratio;
    }

    /// <summary>
    /// ワールドデータを取得する
    /// </summary>
    /// <returns>ワールドデータ</returns>
    private ServerData.WorldData SetupWorldData()
    {
        ServerData.WorldData worldData = new ServerData.WorldData();
        worldData.dataVersion = ServerData.DataVersion.WorldData;
        worldData.rows = rowCount;
        worldData.cols = colCount;
        worldData.cells = core.GetData();

        return worldData;
    }

    /// <summary>
    /// 現在の生存セル数を取得する
    /// </summary>
    /// <returns>現在の生存セル数</returns>
    public int GetAliveCellCount()
    {
        return core.GetAliveCount();
    }

    /// <summary>
    /// 現在の世代数を取得する
    /// </summary>
    /// <returns>世代数</returns>
    public int GetGeneration()
    {
        return core.GetGeneration();
    }
}
