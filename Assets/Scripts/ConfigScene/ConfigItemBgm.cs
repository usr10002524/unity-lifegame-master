using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// BGM設定の処理を行うクラス
/// </summary>
public class ConfigItemBgm : ConfigItemBase
{
    [SerializeField] private GameObject panelObject;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color overColor;
    [SerializeField] private Color selectColor;
    [SerializeField] private Color disableColor;
    [SerializeField] private List<GameObject> itemList;
    [SerializeField] private GameObject currentItem;
    [SerializeField] private GameObject prevItem;
    [SerializeField] private GameObject nextItem;

    //--- アイテム選択関連 ---
    private enum MoveDirection
    {
        Prev,
        Next,
    }

    [SerializeField] float moveDureation;
    [SerializeField] AnimationCurve moveCurve;
    private int currentIndex;
    private Coroutine moveCoroutine;
    private Vector2 origCurrentPos;
    private Vector2 origPrevPos;
    private Vector2 origNextPos;

    //--- ServerData 関連 ---
    private ServerData.SoundSettings soundSettings;
    private bool isDirty;


    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        GetServerData();

        rectTransform = GetComponent<RectTransform>();
        UpdateWorldRect();

        InitItemPosition();

        // Debug.Log(string.Format("itemList.Count: {0}", itemList.Count));
        // Debug.Log(string.Format("current index: {0}", currentIndex));
        SetItems();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        UpdateInput();
    }

    //--- メニュー関連処理 ---
    /// <summary>
    /// コンフィグメニューから抜ける際にしておくことがあれば、ここで行う
    /// </summary>
    public override void OnExitMenu()
    {
        // Debug.Log("ConfigItemBgm.OnExitMenu() called.");
        SaveServerData();
    }

    //--- ServerData 関連処理 ---
    /// <summary>
    /// ロード済みサーバデータを取得する
    /// </summary>
    private void GetServerData()
    {
        bool needReset = false;
        if (AtsumaruAPI.Instance.IsValid())
        {
            soundSettings = AtsumaruAPI.Instance.GetSoundSettings();
        }
        else
        {
            soundSettings = new ServerData.SoundSettings();
        }

        if (soundSettings.IsValid())
        {
            // 選択項目を復元する。範囲外だった場合は0にして置く
            currentIndex = soundSettings.bgmIndex;
            if (currentIndex < 0 || currentIndex >= itemList.Count)
            {
                needReset = true;
            }
        }
        else
        {
            needReset = true;
        }

        if (needReset)
        {
            // Debug.Log(string.Format("GetServerData() sound setting in invalid."));

            soundSettings.dataVersion = ServerData.DataVersion.SoundSettings;
            soundSettings.bgmIndex = 0;
            soundSettings.volume = 0.5f;
            currentIndex = soundSettings.bgmIndex;
        }
    }

    /// <summary>
    /// 項目の設定内容の更新を行う
    /// </summary>
    private void UpdateServerData()
    {
        if (soundSettings.bgmIndex != currentIndex)
        {
            soundSettings.bgmIndex = currentIndex;
            isDirty = true;
        }
    }

    /// <summary>
    /// 項目の設定が更新されていれば保存を行う
    /// </summary>
    private void SaveServerData()
    {
        if (isDirty)
        {
            // Debug.Log(string.Format("SaveServerData() bgmIndex:{0}", soundSettings.bgmIndex));

            if (AtsumaruAPI.Instance.IsValid())
            {
                AtsumaruAPI.Instance.SaveSoundSettings(soundSettings);
            }
        }
    }


    //--- パネル関連処理 ---
    /// <summary>
    /// 項目にマウスオーバーされた際の処理を行う
    /// </summary>
    public override void TriggerMouseEnter()
    {
        // 選択状態に応じてパネルの色を変更する
        if (isSelected)
        {
            SetPanelColor(selectColor);
        }
        else
        {
            SetPanelColor(overColor);
        }
    }

    /// <summary>
    /// 項目からマウスが出た際の処理を行う
    /// </summary>
    public override void TriggerMouseExit()
    {
        // 選択状態に応じてパネルの色を変更する
        if (isSelected)
        {
            SetPanelColor(selectColor);
        }
        else
        {
            SetPanelColor(normalColor);
        }
    }

    /// <summary>
    /// 項目が選択された際の処理を行う
    /// </summary>
    public override void TriggerSelect()
    {
        // 選択状態に応じてパネルの色を変更する
        SetPanelColor(selectColor);
        OnSelectCurrentItem();
    }

    /// <summary>
    /// 項目が非選択状態になった際の処理を行う
    /// </summary>
    public override void TriggerDeselect()
    {
        SetPanelColor(normalColor);
        OnDeselectCurrentItem();
    }


    /// <summary>
    /// パネルの色を指定した色に変更する。
    /// </summary>
    /// <param name="color">変更する色</param>
    private void SetPanelColor(Color color)
    {
        if (panelObject == null)
        {
            return;
        }

        Image image = panelObject.GetComponent<Image>();
        if (image == null)
        {
            return;
        }

        image.color = color;
    }



    //--- アイテム選択関連処理 ---
    /// <summary>
    /// 選択項目の各初期座標を保持しておく
    /// </summary>
    private void InitItemPosition()
    {
        origCurrentPos = GetAnchoredPosion(currentItem);
        origPrevPos = GetAnchoredPosion(prevItem);
        origNextPos = GetAnchoredPosion(nextItem);
    }

    /// <summary>
    /// アイテム数を考慮し、指定したindexの次のindexを取得する。
    /// </summary>
    /// <param name="index">基準となるindex</param>
    /// <returns>基準の次のindex</returns>
    private int GetPrevIndex(int index)
    {
        int count = itemList.Count;
        if (count > 0)
        {
            int prevIndex = (index + count - 1) % count;
            return prevIndex;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// アイテム数を考慮し、指定したindexの前のindexを取得する。
    /// </summary>
    /// <param name="index">基準となるindex</param>
    /// <returns>基準の前のindex</returns>
    private int GetNextIndex(int index)
    {
        int count = itemList.Count;
        if (count > 0)
        {
            int nextIndex = (index + 1) % count;
            return nextIndex;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 選択中のindexに応じて、設定項目の内容を更新する
    /// </summary>
    private void SetItems()
    {
        // Debug.Log(string.Format("itemList.Count: {0}", itemList.Count));
        // Debug.Log(string.Format("current index: {0}", currentIndex));

        SetCurrentItem(itemList[currentIndex]);

        int nextIndex = GetNextIndex(currentIndex);
        SetNextItem(itemList[nextIndex]);

        int prevIndex = GetPrevIndex(currentIndex);
        SetPrevItem(itemList[prevIndex]);
    }

    /// <summary>
    /// 現在表示中のアイテムを設定する
    /// </summary>
    /// <param name="obj">currentIndexの位置のアイテム</param>
    private void SetCurrentItem(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        SettingItemBase item = obj.GetComponent<SettingItemBase>();
        if (item == null)
        {
            return;
        }

        string itemName = item.GetItemName();
        SetItemName(currentItem, itemName);
        SetAnchordPosition(currentItem, origCurrentPos);
    }

    /// <summary>
    /// 現在表示中の次のアイテムを設定する
    /// </summary>
    /// <param name="obj">currentIndexの次の位置のアイテム</param>
    private void SetNextItem(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        SettingItemBase item = obj.GetComponent<SettingItemBase>();
        if (item == null)
        {
            return;
        }

        string itemName = item.GetItemName();
        SetItemName(nextItem, itemName);
        SetAnchordPosition(nextItem, origNextPos);
    }

    /// <summary>
    /// 現在表示中の前のアイテムを設定する
    /// </summary>
    /// <param name="obj">currentIndexの前の位置のアイテム</param>
    private void SetPrevItem(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        SettingItemBase item = obj.GetComponent<SettingItemBase>();
        if (item == null)
        {
            return;
        }

        string itemName = item.GetItemName();
        SetItemName(prevItem, itemName);
        SetAnchordPosition(prevItem, origPrevPos);
    }

    /// <summary>
    /// 選択アイテムの名前を設定する
    /// </summary>
    /// <param name="obj">選択アイテムのGameObject</param>
    /// <param name="itemName">設定する名前の文字列</param>
    private void SetItemName(GameObject obj, string itemName)
    {
        if (obj == null)
        {
            return;
        }
        TextMeshProUGUI textMeshPro = obj.GetComponent<TextMeshProUGUI>();
        if (textMeshPro == null)
        {
            return;
        }

        textMeshPro.text = itemName;
    }


    /// <summary>
    /// 選択項目を前にスクロールさせる
    /// </summary>
    public void ItemMovePrev()
    {
        if (moveCoroutine != null)
        {
            return;
        }
        OnDeselectCurrentItem();
        moveCoroutine = StartCoroutine(MoveCoroutine(MoveDirection.Prev));
    }

    /// <summary>
    /// 選択項目を次にスクロール差sる
    /// </summary>
    public void ItemMoveNext()
    {
        if (moveCoroutine != null)
        {
            return;
        }
        OnDeselectCurrentItem();
        moveCoroutine = StartCoroutine(MoveCoroutine(MoveDirection.Next));
    }

    /// <summary>
    /// スクロールを行うコルーチン
    /// </summary>
    /// <param name="dir">スクロール方向</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator MoveCoroutine(MoveDirection dir)
    {
        bool isEnd = false;
        Vector2 startOffs = Vector2.zero;
        Vector2 endOffs = GetMoveVector(dir);
        float moveTimer = 0.0f;

        while (!isEnd)
        {
            yield return null;

            moveTimer += Time.deltaTime;
            float time = Mathf.Clamp(moveTimer / moveDureation, 0.0f, 1.0f);
            float t = moveCurve.Evaluate(time);

            Vector2 offs = Vector2.Lerp(startOffs, endOffs, t);

            SetAnchordPosition(currentItem, origCurrentPos + offs);
            SetAnchordPosition(prevItem, origPrevPos + offs);
            SetAnchordPosition(nextItem, origNextPos + offs);

            if (moveTimer >= moveDureation)
            {
                isEnd = true;
            }
        }

        // 実際にインデックスを変更
        if (dir == MoveDirection.Prev)
        {
            currentIndex = GetPrevIndex(currentIndex);
        }
        else if (dir == MoveDirection.Next)
        {
            currentIndex = GetNextIndex(currentIndex);
        }
        // アイテムの表示を入れ替える
        SetItems();
        OnSelectCurrentItem();
        UpdateServerData();

        moveCoroutine = null;
    }

    /// <summary>
    /// 指定した方向に対する移動ベクトルを取得する
    /// </summary>
    /// <param name="dir">移動方向</param>
    /// <returns>移動ベクトル</returns>
    private Vector2 GetMoveVector(MoveDirection dir)
    {
        if (dir == MoveDirection.Prev)
        {
            Vector2 v = origNextPos - origCurrentPos;
            return v;
        }
        else if (dir == MoveDirection.Next)
        {
            Vector2 v = origPrevPos - origCurrentPos;
            return v;
        }
        else
        {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// currentIndexの位置のアイテムが選択状態になったときの処理を行う
    /// </summary>
    private void OnSelectCurrentItem()
    {
        if (currentItem == null)
        {
            return;
        }

        GameObject obj = itemList[currentIndex];
        SettingItemBase item = obj.GetComponent<SettingItemBase>();
        item.OnSelected();
    }

    /// <summary>
    /// currentIndexの位置のアイテムが非選択状態になったときの処理を行う
    /// </summary>
    private void OnDeselectCurrentItem()
    {
        if (currentItem == null)
        {
            return;
        }

        GameObject obj = itemList[currentIndex];
        SettingItemBase item = obj.GetComponent<SettingItemBase>();
        item.OnDeselected();
    }
}
