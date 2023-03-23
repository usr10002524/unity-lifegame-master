using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// コンフィグメニューの各項目の処理を行うクラス
/// </summary>
public class ConfigItemBase : MonoBehaviour
{
    [SerializeField] protected string instText;

    //--- パネル関連 ---
    protected RectTransform rectTransform;
    protected Vector3 rectMin = new Vector3();
    protected Vector3 rectMax = new Vector3();
    protected bool lastButtonStat;
    protected bool lastSideStat;
    protected bool isSelected;


    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateWorldRect();
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
    public virtual void OnExitMenu()
    {

    }

    //--- パネル関連処理 ---
    /// <summary>
    /// 項目にマウスオーバーされた際の処理を行う
    /// </summary>
    public virtual void TriggerMouseEnter()
    {

    }

    /// <summary>
    /// 項目からマウスが出た際の処理を行う
    /// </summary>
    public virtual void TriggerMouseExit()
    {

    }

    /// <summary>
    /// 項目が選択された際の処理を行う
    /// </summary>
    public virtual void TriggerSelect()
    {

    }

    /// <summary>
    /// 項目が非選択状態になった際の処理を行う
    /// </summary>
    public virtual void TriggerDeselect()
    {

    }

    /// <summary>
    /// 項目が選択中かどうかを確認する
    /// </summary>
    /// <returns>選択状態の場合はtrue、非選択状態のと場合はfalseを返す</returns>
    public virtual bool IsSelected()
    {
        return isSelected;
    }

    /// <summary>
    /// 項目がマウスオーバー中かどうかを確認する
    /// </summary>
    /// <returns>マウスオーバー状態の場合はtrue、非マウスオーバー状態のと場合はfalseを返す</returns>
    public virtual bool IsOver()
    {
        return lastSideStat;
    }

    /// <summary>
    /// 項目の説明テキストを取得する
    /// </summary>
    /// <returns>項目の説明テキスト</returns>
    public virtual string GetInstractionText()
    {
        return instText;
    }

    /// <summary>
    /// ローカリゼーションのテキストを設定する。
    /// </summary>
    /// <param name="str">設定するテキスト</param>
    public virtual void SetString(string str)
    {
        instText = str;
    }

    /// <summary>
    /// 再描画を行う。
    /// </summary>
    public virtual void Redraw()
    {

    }

    /// <summary>
    /// 項目内にマウスポインタがあるかどうか確認する
    /// </summary>
    /// <returns>項目内にマウスポインタがある場合はtrue、そうでない場合はfalseを返す</returns>
    protected virtual bool IsPointerInside()
    {
        if (null == rectTransform)
        {
            return false;
        }

        // マウスポインターが矩形範囲内かチェック
        Vector3 pos = Input.mousePosition;

        if (pos.x < rectMin.x) { return false; }
        if (pos.x > rectMax.x) { return false; }
        if (pos.y < rectMin.y) { return false; }
        if (pos.y > rectMax.y) { return false; }

        return true;
    }

    /// <summary>
    /// ワールド空間内でのゲームオブジェクトの矩形を更新する
    /// </summary>
    /// <param name="trimScreen"></param>
    protected virtual void UpdateWorldRect(bool trimScreen = true)
    {
        if (null == rectTransform)
        {
            return;
        }

        Vector3[] v = new Vector3[4];
        rectTransform.GetWorldCorners(v);

        rectMin.x = v[0].x;
        rectMin.y = v[0].y;
        rectMax = rectMin;

        for (int i = 0; i < v.Length; i++)
        {
            if (v[i].x < rectMin.x) { rectMin.x = v[i].x; }
            if (v[i].y < rectMin.y) { rectMin.y = v[i].y; }
            if (v[i].x > rectMax.x) { rectMax.x = v[i].x; }
            if (v[i].y > rectMax.y) { rectMax.y = v[i].y; }
        }

        //スクリーンによるトリミングをおこなう
        if (trimScreen)
        {
            rectMin.x = Mathf.Max(rectMin.x, 0);
            rectMin.y = Mathf.Max(rectMin.y, 0);
            rectMax.x = Mathf.Min(rectMax.x, Screen.width);
            rectMax.y = Mathf.Min(rectMax.y, Screen.height);
        }

        return;
    }

    /// <summary>
    /// 入力に対する更新を行う。
    /// </summary>
    protected virtual void UpdateInput()
    {
        bool buttonStat = Input.GetMouseButton(0);
        bool sideStat = IsPointerInside();

        // 選択、非選択判定
        if (lastButtonStat != buttonStat)
        {
            if (buttonStat)
            {
                if (sideStat)
                {
                    if (isSelected)
                    {
                        // 選択中
                    }
                    else
                    {
                        // 非選択中
                        isSelected = true;
                        TriggerSelect();
                    }
                }
                else
                {
                    if (isSelected)
                    {
                        // 選択中
                        isSelected = false;
                        TriggerDeselect();
                    }
                    else
                    {
                        // 非選択中
                    }
                }
            }
        }
        // Enter, Exit 判定
        if (lastSideStat != sideStat)
        {
            if (sideStat)
            {
                TriggerMouseEnter();
            }
            else
            {
                TriggerMouseExit();
            }
        }

        lastButtonStat = buttonStat;
        lastSideStat = sideStat;
    }




    //--- 共通 ---
    /// <summary>
    /// 指定したオブジェクトのアンカーを考慮した位置を取得する
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <returns></returns>
    protected Vector2 GetAnchoredPosion(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return Vector2.zero;
        }

        return rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 指定したオブジェクトにアンカーを考慮した位置を設定する
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <param name="pos">アンカーを考慮した座標</param>
    protected virtual void SetAnchordPosition(GameObject obj, Vector2 pos)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            return;
        }
        rectTransform.anchoredPosition = pos;
    }
}
