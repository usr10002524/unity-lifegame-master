using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpPanelController : MonoBehaviour
{
    /// <summary>
    /// ヘルプパネルの開閉状態
    /// </summary>
    public enum Stat
    {
        Close,
        Open,
    }

    [SerializeField] private List<GameObject> pageObjects;
    [SerializeField] private GameObject imageGameObject;
    [SerializeField] private GameObject contentsObject;
    [SerializeField] private GameObject titleGameObject;
    [SerializeField] private GameObject pageGameObject;
    [SerializeField] private GameObject shadeGameObject;

    [SerializeField] private Vector2 openPosition;
    [SerializeField] private Vector2 closePosition;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;

    /// <summary>
    /// 1ページあたりの情報
    /// </summary>
    private class PageInfo
    {
        // public string title;
        // public Sprite sprite;

        public HelpPage helpPage;
    }

    private int currentPange;
    private List<PageInfo> pageInfos;

    private RectTransform rectTransform;
    private float animationTimer;
    private TextMeshProUGUI title;
    private TextMeshProUGUI pageNumber;
    private Image image;

    private Stat currentStat;
    private Stat setStat;
    private Coroutine moveCroutine;

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
            Close();
        }
        else if (IsClose())
        {
            Open();
        }
        else
        {
            ;
        }
    }

    /// <summary>
    /// ヘルプパネルを開く
    /// </summary>
    public void Open()
    {
        if (IsMoving())
        {
            return; // 移動中なので無視
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
        moveCroutine = StartCoroutine(MoveCoroutine());
    }

    /// <summary>
    /// ヘルプパネルを閉じる
    /// </summary>
    public void Close()
    {
        if (IsMoving())
        {
            return; // 移動中なので無視
        }
        if (IsClose())
        {
            return; // すでに閉じている
        }
        setStat = Stat.Close;
        moveCroutine = StartCoroutine(MoveCoroutine());
    }

    /// <summary>
    /// 次のページへ切り替える。
    /// </summary>
    public void NextPage()
    {
        if (IsMoving())
        {
            return; // 移動中なので無視
        }
        if (IsClose())
        {
            return; // メニューは閉じているので無視
        }

        //次のページがあれば表示する
        if (currentPange + 1 < pageInfos.Count)
        {
            currentPange++;
            SetPage(currentPange);
        }
    }

    /// <summary>
    /// 前のページへ切り替える。
    /// </summary>
    public void PrevPage()
    {
        if (IsMoving())
        {
            return; // 移動中なので無視
        }
        if (IsClose())
        {
            return; // メニューは閉じているので無視
        }

        //前のページがあれば表示する
        if (currentPange > 0)
        {
            currentPange--;
            SetPage(currentPange);
        }
    }

    /// <summary>
    /// 開閉動作中か確認する。
    /// </summary>
    /// <returns>開閉動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsMoving()
    {
        return (moveCroutine != null);
    }

    /// <summary>
    /// 開く動作中か確認する。
    /// </summary>
    /// <returns>開く動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsOpen()
    {
        return (currentStat == Stat.Open);
    }

    /// <summary>
    /// 他のメニューが開いているか確認する。
    /// </summary>
    /// <returns>他のメニューが開いている場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsOtherMenuOpen()
    {
        if (PatternPreviewManager.Instance.IsActive())
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 閉じる動作中か確認する。
    /// </summary>
    /// <returns>閉じる動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsClose()
    {
        return (currentStat == Stat.Close);
    }


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        Setup();
    }

    /// <summary>
    /// ヘルプページの準備を行う。
    /// </summary>
    private void Setup()
    {
        // 各ページのタイトルとテクスチャを取得
        SetupPageInfo();

        // 制御を行う各コンポーネントを取得
        image = imageGameObject.GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        title = titleGameObject.GetComponent<TextMeshProUGUI>();
        pageNumber = pageGameObject.GetComponent<TextMeshProUGUI>();

        // フラグリセット
        currentStat = Stat.Close;

        // 現在のページを設定
        SetPage(currentPange);

        // 非表示にしておく
        SetContentsActive(false);
    }

    /// <summary>
    /// GameObject からページの情報を取得し準備を行う
    /// </summary>
    private void SetupPageInfo()
    {
        if (pageInfos != null)
        {
            pageInfos.Clear();
        }
        else
        {
            pageInfos = new List<PageInfo>();
        }
        currentPange = 0;

        foreach (var item in pageObjects)
        {
            HelpPage helpPage = item.GetComponent<HelpPage>();
            if (helpPage != null)
            {
                PageInfo info = new PageInfo();
                info.helpPage = helpPage;
                pageInfos.Add(info);
            }
        }
    }

    /// <summary>
    /// 現在のページを再描画する。
    /// </summary>
    public void Redraw()
    {
        // 現在のページを設定
        SetPage(currentPange);
    }

    /// <summary>
    /// 現在のページ数と総ページ数を表示する。
    /// </summary>
    /// <param name="page">ページ数</param>
    private void SetPage(int page)
    {
        if (pageInfos == null)
        {
            return;
        }
        if (page < 0 || page >= pageInfos.Count)
        {
            return;
        }

        if (pageInfos[page].helpPage != null)
        {
            title.SetText(pageInfos[page].helpPage.title);
            image.sprite = pageInfos[page].helpPage.sprite;
        }
        pageNumber.SetText(string.Format("{0} / {1}", page + 1, pageInfos.Count));
    }

    // 各オブジェクトのアクティブ化を切り替える。
    private void SetContentsActive(bool active)
    {
        contentsObject.SetActive(active);
        shadeGameObject.SetActive(active);
    }

    /// <summary>
    /// パネルの開閉を行うコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator MoveCoroutine()
    {
        bool isEnd = false;
        Vector2 startPos = (setStat == Stat.Open) ? closePosition : openPosition;
        Vector2 endPos = (setStat == Stat.Open) ? openPosition : closePosition;
        Vector3 startScale = (setStat == Stat.Open) ? Vector3.zero : Vector3.one;
        Vector3 endScale = (setStat == Stat.Open) ? Vector3.one : Vector3.zero;

        animationTimer = 0.0f;

        if (setStat == Stat.Open)
        {
            // Active化する前に最初のパラメータをセットする。
            Vector2 positon = Vector2.Lerp(startPos, endPos, 0);
            rectTransform.anchoredPosition = positon;

            Vector3 scale = Vector3.Lerp(startScale, endScale, 0);
            rectTransform.localScale = scale;

            SetContentsActive(true);
        }

        while (!isEnd)
        {
            yield return null;

            animationTimer += Time.deltaTime;
            float time = Mathf.Clamp(animationTimer / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            Vector2 positon = Vector2.Lerp(startPos, endPos, t);
            rectTransform.anchoredPosition = positon;

            Vector3 scale = Vector3.Lerp(startScale, endScale, t);
            rectTransform.localScale = scale;

            if (animationTimer >= duration)
            {
                isEnd = true;
            }
        }

        if (setStat == Stat.Close)
        {
            SetContentsActive(false);
        }

        currentStat = setStat;
        moveCroutine = null;
    }
}
