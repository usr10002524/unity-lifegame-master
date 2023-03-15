using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトルシーンのUIと制御部分をつなぐクラス
/// </summary>
public class TitleSceneController : MonoBehaviour
{
    [SerializeField] private GameObject worldObject;
    [SerializeField] private GameObject fadeObject;
    [SerializeField] private float demoReplaceTime;
    [SerializeField] private float sceneTransitDuration = 0.5f;
    [SerializeField] private Color sceneTransitColor = Color.black;
    [SerializeField] private int sceneTransitSortOrder = 1;


    private CellWorldTitle cellWorld;
    private FadeController fadeController;
    private float fadeDump = 1.0f;


    /// <summary>
    /// Startボタンが押されたときの処理
    /// </summary>
    public void OnStartButton()
    {
        GameManager.Instance.SetStartStat(GameManager.StartStat.Start);
        Initiate.Fade("MainScene", sceneTransitColor, fadeDump, sceneTransitSortOrder);
    }

    /// <summary>
    /// Continueボタンが押されたときの処理
    /// </summary>
    public void OnContinueButton()
    {
        GameManager.Instance.SetStartStat(GameManager.StartStat.Continue);
        Initiate.Fade("MainScene", sceneTransitColor, fadeDump, sceneTransitSortOrder);
    }

    /// <summary>
    /// Configボタンが押されたときの処理
    /// </summary>
    public void OnConfigButton()
    {
        GameManager.Instance.SetStartStat(GameManager.StartStat.Continue);
        Initiate.Fade("ConfigScene", sceneTransitColor, fadeDump, sceneTransitSortOrder);
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        cellWorld = worldObject.GetComponent<CellWorldTitle>();
        fadeController = fadeObject.GetComponent<FadeController>();
        // SetupDemo();
        StartCoroutine(UpdateDemo());

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
    /// デモの準備を行う
    /// </summary>
    private void SetupDemo()
    {
        int cols = cellWorld.GetColCount();
        int rows = cellWorld.GetRowCount();

        GameController.Instance.OnClickEditButton();
        cellWorld.SetPlayMode(LifeGameConsts.PlayMode.Edit);
        // いったんすべてをリセット
        cellWorld.AllCellClear();

        // デモ用にセルを配置する
        {
            // 5*5 のエリアに10個ランダムにセルを配置
            // を20回行う
            const int areaSelect = 20;
            const int area_w = 5;
            const int area_h = 5;
            const int putCount = 10;
            for (int i = 0; i < areaSelect; i++)
            {
                int col = Random.Range(0, cols - area_w + 1);
                int row = Random.Range(0, rows - area_h + 1);

                for (int n = 0; n < putCount; n++)
                {
                    int put_col = col + Random.Range(0, area_w);
                    int put_row = row + Random.Range(0, area_h);

                    //念のためチェック
                    if (put_col < 0 || put_col >= cols) { continue; }
                    if (put_row < 0 || put_row >= rows) { continue; }

                    cellWorld.SetCellStat(put_col, put_row, LifeGameConsts.Cell.Stat.Alive);
                }
            }
            cellWorld.ApplyCellStat();
        }

        GameController.Instance.OnClickViewButton();
        cellWorld.SetPlayMode(LifeGameConsts.PlayMode.View);
    }

    /// <summary>
    /// 一定時間でデモを再生するコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator UpdateDemo()
    {
        bool isEnd = false;

        // Start() だと cellWorld の初期化が終わっていないことがあるので、
        // ここで初期化待ちをする。
        // イケてないが…TT
        while (!cellWorld.IsInitialized())
        {
            yield return null;
        }
        SetupDemo();

        while (!isEnd)
        {
            yield return new WaitForSeconds(demoReplaceTime);

            // フェードイン開始
            fadeController.StartFade(FadeController.FadeType.In);
            // フェード中
            while (fadeController.IsFading())
            {
                yield return null;
            }

            // デモ再配置
            SetupDemo();

            // フェードアウト開始
            fadeController.StartFade(FadeController.FadeType.Out);
            // フェード中
            while (fadeController.IsFading())
            {
                yield return null;
            }
        }
    }
}
