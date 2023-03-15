using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// エイリアス
using PlayMode = LifeGameConsts.PlayMode;
using EditMode = LifeGameConsts.EditMode;

public class PatternPaste : MonoBehaviour
{
    private static readonly string[] raycastTargets = { "Cell" };
    private static readonly string patternLayer = "Pattern";

    [SerializeField] private GameObject patternBase;
    [SerializeField] private GameObject cellPrefab;

    private string patternName;
    private PatternLoader.PatternData patternData;
    private Vector2Int offsLeftTop;

    // パターン表示用セルのリスト
    protected List<GameObject> cells;

    private void Awake()
    {
        patternData = new PatternLoader.PatternData();
        cells = new List<GameObject>();
        offsLeftTop = new Vector2Int();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePatternPaste();
    }

    /// <summary>
    /// パターン貼り付けの定期処理
    /// </summary>
    private void UpdatePatternPaste()
    {
        if (!CheckUpdateBase())
        {
            if (patternBase.activeInHierarchy)
            {
                // ClearCells();
                patternBase.SetActive(false);
            }
            return;
        }
        else
        {
            if (!patternBase.activeInHierarchy)
            {
                patternBase.SetActive(true);
            }
        }

        if (CheckResetPattern())
        {
            patternName = GameController.Instance.GetPatternName();
            patternData = GameController.Instance.GetPatternData();
            SetupPattern(patternData, patternBase);

        }

        if (!IsUpdate())
        {
            return;
        }
        UpdateCursor();
    }

    /// <summary>
    /// ベースオブジェクトを有効にするかどうかをチェックする。
    /// </summary>
    /// <returns>有効にする場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool CheckUpdateBase()
    {
        if (!GameController.Instance.IsEditMode())
        {
            return false;
        }
        if (!GameController.Instance.IsPatternPasteMode())
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// パターンのリセットを行うかチェックする
    /// </summary>
    /// <returns>リセットを行う必要がある場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool CheckResetPattern()
    {
        string name = GameController.Instance.GetPatternName();
        if (name.Length == 0)
        {
            return false;
        }

        PatternLoader.PatternData data = GameController.Instance.GetPatternData();
        if (!data.IsValid())
        {
            return false;
        }

        if ((patternName != name)
            || data.IsDirty())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// パターンの準備を行う。
    /// </summary>
    /// <param name="data">パターンデータ</param>
    /// <param name="parent">パターンを接続する親のオブジェクト</param>
    private void SetupPattern(PatternLoader.PatternData data, GameObject parent)
    {
        ClearCells();

        SpriteRenderer renderer = cellPrefab.GetComponent<SpriteRenderer>();
        // セルのスプライトからPPUを取得し、縦横のサイズを算出する
        float PPU = renderer.sprite.pixelsPerUnit;
        float cellWidth = renderer.sprite.texture.width / PPU;
        float cellHeight = renderer.sprite.texture.height / PPU;
        // Z位置はそのまま使用する
        float cellZ = cellPrefab.transform.position.z;
        // パターンのサイズを取得する
        int colCount = data.GetWidth();
        int rowCount = data.GetHeight();
        // パターンが偶数だと表示の際半マスずれるので奇数になるように調整する
        int colPadding = (0 == (colCount % 2)) ? -1 : 0;
        int rowPadding = (0 == (rowCount % 2)) ? -1 : 0;
        float worldWidth = cellWidth * (colCount + colPadding);
        float worldHeight = cellHeight * (rowCount + rowPadding);
        // パターンは左上を原点としてセルを並べるので、そのための原点を算出する
        float offsLeft = -((worldWidth - cellWidth) / 2.0f);
        float offsTop = ((worldHeight - cellHeight) / 2.0f);

        // カーソル位置からパターン左上へのオフセットを算出する
        offsLeftTop.x = -(colCount + colPadding) / 2;
        offsLeftTop.y = -(rowCount + rowPadding) / 2;

        // セルを指定されたオブジェクトの子オブジェクトとして生成する
        // 親オブジェクトは原点に戻しておく。子オブジェクトをつけた際に、transformが子に適応されるため。
        parent.transform.position = Vector3.zero;
        for (int r = 0; r < rowCount; r++)
        {
            for (int c = 0; c < colCount; c++)
            {
                int stat = 0;
                patternData.GetStat(c, r, ref stat);

                float x = offsLeft + c * cellWidth;
                float y = offsTop - r * cellHeight;
                float z = cellZ;
                Vector3 position = new Vector3(x, y, z);

                // セルの生成
                GameObject child = Instantiate(cellPrefab, position, cellPrefab.transform.rotation, parent.transform);
                // カーソルの位置取得の際、自身も対象となってしまうため、レイヤー名を変更しておく。
                child.layer = LayerMask.NameToLayer(patternLayer);
                // セルは入力を受け付ける。その際セルがどこの位置かわかるように座標を渡しておく。
                CellInput input = child.GetComponent<CellInput>();
                if (input != null)
                {
                    input.SetPosition(new Vector2Int(c, r));
                }

                // セルの状態に応じて色を変更する。
                ImageController image = child.GetComponent<ImageController>();
                if (stat == 0)
                {
                    image.Stop(ImageController.StopPosition.Start);
                }
                else
                {
                    image.Stop(ImageController.StopPosition.End);
                }

                cells.Add(child);
            }
        }
    }

    /// <summary>
    /// 現在のパターンを破棄する
    /// </summary>
    private void ClearCells()
    {
        foreach (var item in cells)
        {
            Destroy(item);
        }

        cells.Clear();
    }

    /// <summary>
    /// カーソルの更新処理
    /// </summary>
    private void UpdateCursor()
    {
        // レイを投げる原点と方向
        Camera mainCamera = Camera.main;
        Vector2 origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        // レイの距離
        float distance = 20.0f;

        // Debug.Log(string.Format("ray position x:{0} y:{1}", origin.x, origin.y));
        // Debug.Log(string.Format("ray direction x:{0} y:{1}", direction.x, direction.y));


        // レイヤーマスクを作成
        int layerMask = LayerMask.GetMask(raycastTargets);
        // Debug.Log(string.Format("layerMask:{0}", layerMask));

        // レイを投げる
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        if (hit.collider != null)
        {
            // レイが当たれば対象の位置にカーソルを動かす
            patternBase.transform.position = hit.transform.position;
            // Debug.Log(string.Format("ray hit. name:{0}", hit.collider.name));

            if (Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }
                CellInput cellInput = hit.transform.GetComponent<CellInput>();
                if (cellInput != null)
                {
                    Vector2Int pos = cellInput.GetPosition();
                    GameController.Instance.PatternPaste(pos, offsLeftTop);
                    // Debug.Log(string.Format("PatternPast MouseButtonUp. cellPos x:{0} y:{0}", pos.x, pos.y));
                }
                else
                {
                    // Debug.Log(string.Format("PatternPast MouseButtonUp."));
                }
            }
        }
        else
        {
            // 対象がいないのでファークリップの向こう移動させて表示されなくする
            patternBase.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.farClipPlane * 2);
            // Debug.Log(string.Format("ray not hit. "));
        }
    }

    private bool IsUpdate()
    {
        if (HelpPanelManager.Instance.IsActive()) { return false; }
        if (PatternPreviewManager.Instance.IsActive()) { return false; }

        return true;
    }
}
