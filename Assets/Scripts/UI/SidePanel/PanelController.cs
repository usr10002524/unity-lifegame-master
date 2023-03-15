using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サイドパネルの開閉のトリガーを制御するクラス
/// </summary>
public class PanelController : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;
    [SerializeField] private float enterTriggerDuration;
    [SerializeField] private float leaveTriggerDuration;

    private SidePanelContainer container;
    private RectTransform rectTransform;
    [SerializeField] private Vector2 rectMin = new Vector2();
    [SerializeField] private Vector2 rectMax = new Vector2();
    [SerializeField] private float timeInside;
    [SerializeField] private float timeOutside;
    [SerializeField] private bool isInside;
    [SerializeField] private bool isOutside;
    [SerializeField] private bool lastSide;  //true:inside, false:outside


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        container = containerObject.GetComponent<SidePanelContainer>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (!IsValid())
        {
            return; //パラメータが揃っていない場合は動かさない
        }

        CheckMouseEnter();

        if (isInside)
        {
            container.OnEnterTirgger();
        }
        else if (isOutside)
        {
            container.OnLeaveTrigger();
        }
    }

    /// <summary>
    /// 必要なオブジェクトがセットされているか確認する。
    /// </summary>
    /// <returns>必要なオブジェクトがセットされている場合はtrue、そうでない場合はfalse</returns>
    private bool IsValid()
    {
        if (rectTransform == null) { return false; }
        if (container == null) { return false; }
        return true;
    }

    /// <summary>
    /// オブジェクト内にマウスポインターがあるかチェックし、パネルの開閉を制御する。
    /// </summary>
    private void CheckMouseEnter()
    {
        //範囲矩形を更新する
        UpdatePanelRect();

        //判定をスキップするかチェック
        if (!CheckMouseEnterEnable())
        {
            isInside = false;
            isOutside = false;
            timeInside = 0.0f;
            timeOutside = 0.0f;
            return;
        }

        //マウスがパネルの内側にいるかどうか判定する
        bool inside = IsPointerInsidePanel();
        //前フレームと違っていたらタイマーをリセット
        if (inside != lastSide)
        {
            lastSide = inside;
            timeInside = 0.0f;
            timeOutside = 0.0f;
        }

        if (inside)
        {
            //内側にいる場合、所定経過時間が過ぎたらフラグを立てる
            if (!isInside)
            {
                timeInside += Time.deltaTime;
                if (timeInside >= enterTriggerDuration)
                {
                    isInside = true;
                    isOutside = false;
                }
            }
        }
        else
        {
            //内側にいる場合、所定経過時間が過ぎたらフラグを立てる
            if (!isOutside)
            {
                timeOutside += Time.deltaTime;
                if (timeOutside >= leaveTriggerDuration)
                {
                    isOutside = true;
                    isInside = false;
                }
            }
        }
    }

    /// <summary>
    /// オブジェクト内判定を行うか確認する。
    /// </summary>
    /// <returns>オブジェクト内判定を行う場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool CheckMouseEnterEnable()
    {
        // コンテナ移動中は判定を動かさない
        if (container.IsMoving()) { return false; }

        return true;
    }

    /// <summary>
    /// パネル矩形のワールド座標を更新する。
    /// </summary>
    private void UpdatePanelRect()
    {
        if (rectTransform != null)
        {
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
            rectMin.x = Mathf.Max(rectMin.x, 0);
            rectMin.y = Mathf.Max(rectMin.y, 0);
            rectMax.x = Mathf.Min(rectMax.x, Screen.width);
            rectMax.y = Mathf.Min(rectMax.y, Screen.height);
        }
    }

    /// <summary>
    /// マウスポインタが矩形内にあるか判定する。
    /// </summary>
    /// <returns>マウスポインタが矩形内にある場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool IsPointerInsidePanel()
    {
        if (ForcePointerOutSide())
        {
            return false;
        }

        Vector3 pos = Input.mousePosition;

        if (pos.x < rectMin.x) { return false; }
        if (pos.x > rectMax.x) { return false; }
        if (pos.y < rectMin.y) { return false; }
        if (pos.y > rectMax.y) { return false; }

        return true;
    }

    /// <summary>
    /// IsPointerInsidePanel のチェックを行う際、強制的に外側にあると判定するかどうかを確認する。
    /// </summary>
    /// <returns>強制外側判定する場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool ForcePointerOutSide()
    {
        // ヘルプウィンドウ表示中は強制的に外側判定とする。
        if (HelpPanelManager.Instance.IsActive()) { return true; }
        if (PatternPreviewManager.Instance.IsActive()) { return true; }

        return false;
    }
}
