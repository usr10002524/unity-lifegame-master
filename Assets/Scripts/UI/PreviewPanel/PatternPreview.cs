using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class PatternPreview : MonoBehaviour
{
    [SerializeField] private GameObject previewBaseObject;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject dropDownObject;
    [SerializeField] private GameObject contentObject;
    [SerializeField] private GameObject shadeObject;
    [SerializeField] private Color aliveColor;
    [SerializeField] private Color deadColor;

    [SerializeField] private bool vFlip;
    [SerializeField] private bool hFlip;
    [SerializeField] private PatternLoader.PatternData.Rotate rotate;

    [SerializeField] private float startScale = 0.0f;
    [SerializeField] private float endScale = 1.0f;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;

    public enum Stat
    {
        Close,
        Open,
    }

    private TMP_Dropdown dropdown;
    private bool doropdownInitialized;
    private List<string> filenameList;
    private List<string> dropdownItems;
    private static readonly string initialItem = "パターンを選択";

    private string patternName;
    private PatternLoader.PatternData patternData;
    private List<GameObject> cells;

    private Stat setStat = Stat.Close;
    private Stat currentStat = Stat.Close;
    private bool isMoving = false;
    private bool closeByOkButton = false;

    private static readonly float CELL_WIDTH = 16;
    private static readonly float CELL_HEIGHT = 16;
    private static readonly float PREVIEW_PANEL_WIDTH = 320;
    private static readonly float PREVIEW_PANEL_HEIGHT = 240;


    private void Awake()
    {
        cells = new List<GameObject>();
        patternName = "";
        patternData = new PatternLoader.PatternData();
        dropdown = dropDownObject.GetComponent<TMP_Dropdown>();

        SetContentsActive(false);
        RectTransform rectTransform = contentObject.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector2.zero;
        }
    }

    private void Start()
    {
        CheckAndSetupDropdown();
    }

    private void Update()
    {
        CheckAndSetupDropdown();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenWindow();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CloseWindow();
        }
    }
    public void OnDropdownValueChanged(int value)
    {
        if (value < filenameList.Count)
        {
            string name = filenameList[value];
            // Debug.Log(string.Format("DropdownValueChanged() value:{0} name:{1}", value, name));

            if (value > 0)
            {
                ChangePreviewItem(name);
            }
        }
    }

    private void ChangePreviewItem(string name)
    {
        // Debug.Log(string.Format("PatternPreview.OnDropdownValueChanged() name:{0}", name));
        patternName = name;
        patternData = PatternLoadManager.Instance.GetData(patternName);
        patternData.ResetFlipAndRotate();

        Preview();
    }

    public void OnVFlip()
    {
        vFlip = !vFlip;
        patternData.SetVFlip(vFlip);
        Preview();
    }

    public void OnHFlip()
    {
        hFlip = !hFlip;
        patternData.SetHFlip(hFlip);
        Preview();
    }

    public void OnRotationR()
    {
        switch (rotate)
        {
            case PatternLoader.PatternData.Rotate.Rot0: rotate = PatternLoader.PatternData.Rotate.Rot90; break;
            case PatternLoader.PatternData.Rotate.Rot90: rotate = PatternLoader.PatternData.Rotate.Rot180; break;
            case PatternLoader.PatternData.Rotate.Rot180: rotate = PatternLoader.PatternData.Rotate.Rot270; break;
            case PatternLoader.PatternData.Rotate.Rot270: rotate = PatternLoader.PatternData.Rotate.Rot0; break;
        }
        patternData.SetRotate(rotate);
        Preview();
    }

    public void OnRotationL()
    {
        switch (rotate)
        {
            case PatternLoader.PatternData.Rotate.Rot0: rotate = PatternLoader.PatternData.Rotate.Rot270; break;
            case PatternLoader.PatternData.Rotate.Rot90: rotate = PatternLoader.PatternData.Rotate.Rot0; break;
            case PatternLoader.PatternData.Rotate.Rot180: rotate = PatternLoader.PatternData.Rotate.Rot90; break;
            case PatternLoader.PatternData.Rotate.Rot270: rotate = PatternLoader.PatternData.Rotate.Rot180; break;
        }
        patternData.SetRotate(rotate);
        Preview();
    }

    /// <summary>
    /// パネルの開閉状態をトグルで切り替える
    /// </summary>
    public void Toggle()
    {
        if (IsMoving())
        {
            return; // 移動中なので無視
        }

        if (IsOpen())
        {
            CloseWindow();
        }
        else if (IsClose())
        {
            OpenWindow();
        }
        else
        {
            ;
        }
    }

    public void OpenWindow()
    {
        if (IsMoving())
        {
            return; // アニメーション中なので無視
        }
        if (IsOpen())
        {
            return; // すでに開いている
        }
        if (IsOtherMenuOpen())
        {
            return; // 他のメニューが開いている
        }

        setStat = Stat.Open;
        isMoving = true;
        StartCoroutine(MoveWindow());
    }

    public void CloseWindow()
    {
        if (IsMoving())
        {
            return; // アニメーション中なので無視
        }
        if (IsClose())
        {
            return; // すでに開いている
        }

        setStat = Stat.Close;
        isMoving = true;
        closeByOkButton = false;
        StartCoroutine(MoveWindow());
    }

    public void OnOKButton()
    {
        if (IsMoving())
        {
            return; // アニメーション中なので無視
        }
        if (IsClose())
        {
            return; // すでに開いている
        }

        setStat = Stat.Close;
        isMoving = true;
        // パターン未選択でOKが押された場合は、閉じるボタンを押されたときと同じ挙動にする
        if (patternName.Length > 0)
        {
            closeByOkButton = true;
        }
        else
        {
            closeByOkButton = false;
        }

        StartCoroutine(MoveWindow());
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool IsOpen()
    {
        return (currentStat == Stat.Open);
    }

    public bool IsOtherMenuOpen()
    {
        if (HelpPanelManager.Instance.IsActive())
        {
            return true;
        }

        return false;
    }

    public bool IsClose()
    {
        return (currentStat == Stat.Close);
    }

    private void CheckAndSetupDropdown()
    {
        if (!doropdownInitialized)
        {
            if (PatternLoadManager.Instance.IsLoadCompleted())
            {
                InitDropdown();
            }
        }
    }

    private void InitDropdown()
    {
        if (doropdownInitialized)
        {
            return;
        }

        if (dropdown != null)
        {
            dropdown.ClearOptions();

            filenameList = PatternLoadManager.Instance.GetFilenameList();
            filenameList.Insert(0, "");
            dropdownItems = PatternLoadManager.Instance.GetPatternList();
            dropdownItems.Insert(0, initialItem);
            dropdown.AddOptions(dropdownItems);
        }

        doropdownInitialized = true;
    }

    private void Preview()
    {
        if (!patternData.IsValid())
        {
            return;
        }

        ClearCells();

        float cellWidth = CELL_WIDTH;
        float cellHeight = CELL_HEIGHT;
        int patternWidth = patternData.GetWidth();
        int patternHeight = patternData.GetHeight();

        //外側のプラス1マスを考慮したサイズ
        int patternWidthWithExtra = patternWidth + 2;
        int patternHeightWithExtra = patternHeight + 2;

        //プレビューパネルを超えるサイズになる場合は、収まるようにスケールをかける
        int ouerCellCount = 1;
        float ratio = CalcRatio(patternData, ouerCellCount);
        if (ratio != 1.0f)
        {
            cellWidth = CELL_WIDTH * ratio;
            cellHeight = CELL_HEIGHT * ratio;
        }

        // ワールドのサイズを算出する
        float worldWidth = cellWidth * patternWidth;
        float worldHeight = cellHeight * patternHeight;
        // ワールドは左上を原点としてセルを並べるので、そのための原点を算出する
        float offsLeft = -((worldWidth - cellWidth) / 2.0f);
        float offsTop = ((worldHeight - cellHeight) / 2.0f);

        int index = 0;

        // パターンを配置
        for (int r = 0; r < patternHeight; r++)
        {
            for (int c = 0; c < patternWidth; c++)
            {
                if (index < patternData.stats.Count)
                {
                    int s = 0;
                    if (!patternData.GetStat(c, r, ref s))
                    {
                        continue;
                    }

                    float x = offsLeft + c * cellWidth;
                    float y = offsTop - r * cellHeight;
                    Vector2 position = new Vector2(x, y);
                    Vector2 size = new Vector2(cellWidth, cellHeight);

                    AddCell(position, size, s);
                    index++;
                }
            }
        }

        // 外側に1マス追加
        worldWidth = cellWidth * patternWidthWithExtra;
        worldHeight = cellHeight * patternHeightWithExtra;
        offsLeft = -((worldWidth - cellWidth) / 2.0f);
        offsTop = ((worldHeight - cellHeight) / 2.0f);

        for (int r = 0; r < patternHeightWithExtra; r++)
        {
            for (int c = 0; c < patternWidthWithExtra; c++)
            {
                if ((r > 0)
                    && (r < (1 + patternHeight))
                    && (c > 0)
                    && (c < (1 + patternWidth)))
                {
                    continue;
                }

                float x = offsLeft + c * cellWidth;
                float y = offsTop - r * cellHeight;
                Vector2 position = new Vector3(x, y);
                Vector2 size = new Vector2(cellWidth, cellHeight);

                AddCell(position, size, 0);
            }
        }
    }

    private float CalcRatio(PatternLoader.PatternData patternData, int outerCellCount = 0)
    {
        //外側の追加マスを考慮したサイズ
        int patternWidthWithExtra = patternData.GetWidth() + (outerCellCount * 2);
        int patternHeightWithExtra = patternData.GetHeight() + (outerCellCount * 2);

        float hRatio = 1.0f;
        float vRatio = 1.0f;
        float ratio = 1.0f;

        // 縦横の比率を算出（外側に1セル追加するのでその分を考慮する）
        if (CELL_WIDTH * patternWidthWithExtra > PREVIEW_PANEL_WIDTH)
        {
            hRatio = PREVIEW_PANEL_WIDTH / (CELL_WIDTH * patternWidthWithExtra);
        }
        if (CELL_HEIGHT * patternHeightWithExtra > PREVIEW_PANEL_HEIGHT)
        {
            vRatio = PREVIEW_PANEL_HEIGHT / (CELL_HEIGHT * patternHeightWithExtra);
        }

        // 比率が小さい方に合わせる
        ratio = Mathf.Min(hRatio, vRatio);
        return ratio;
    }

    private void AddCell(Vector2 anchoredPosition, Vector2 anchoredSize, int colorStat)
    {
        // セルの生成
        GameObject child = Instantiate(cellPrefab, previewBaseObject.transform, false);

        RectTransform rectTransform = child.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, anchoredSize.x);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, anchoredSize.y);

        Image image = child.GetComponent<Image>();
        if (colorStat == 1)
        {
            image.color = aliveColor;
        }
        else if (colorStat == 0)
        {
            image.color = deadColor;
        }

        cells.Add(child);
    }

    private void ClearCells()
    {
        foreach (var item in cells)
        {
            Destroy(item);
        }

        cells.Clear();
    }

    private IEnumerator MoveWindow()
    {
        bool isEnd = false;
        float fromScale = (setStat == Stat.Open) ? startScale : endScale;
        float toScale = (setStat == Stat.Open) ? endScale : startScale;
        float animationTimer = 0.0f;

        if (setStat == Stat.Open)
        {
            SetContentsActive(true);
        }

        while (!isEnd)
        {
            yield return null;

            animationTimer += Time.deltaTime;
            float time = Mathf.Clamp(animationTimer / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            float scale = Mathf.Lerp(fromScale, toScale, t);
            RectTransform rectTransform = contentObject.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = new Vector2(scale, scale);
            }

            if (animationTimer >= duration)
            {
                isEnd = true;
            }
        }

        if (setStat == Stat.Close)
        {
            SetContentsActive(false);
            if (closeByOkButton)
            {
                GameController.Instance.OnClickPatternSelect(patternName, patternData);
                closeByOkButton = false;
            }
        }
        isMoving = false;
        currentStat = setStat;
    }

    // 各オブジェクトのアクティブ化を切り替える。
    private void SetContentsActive(bool active)
    {
        contentObject.SetActive(active);
        shadeObject.SetActive(active);
    }
}
