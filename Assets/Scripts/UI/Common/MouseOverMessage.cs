using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// オブジェクトにマウスポインタが重なった際に
/// メッセージエリアに説明を表示するクラス
/// </summary>
public class MouseOverMessage : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private string message;

    private RectTransform rectTransform;
    private Vector2 rectMin = new Vector2();
    private Vector2 rectMax = new Vector2();
    private bool isEnter;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        isEnter = false;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        UpdatePanelRect();
        if (IsPointerInsidePanel())
        {
            if (!isEnter)
            {
                OnEnterRect();
            }
            isEnter = true;
        }
        else
        {
            isEnter = false;
        }
    }

    /// <summary>
    /// 説明テキストをMessageManagerにセットする。
    /// 矩形にマウスポインタが入った際に呼ばれる。
    /// </summary>
    private void OnEnterRect()
    {
        MessageManager.Instance.InsertMessage(message);
    }

    /// <summary>
    /// オブジェクトのワールド空間での矩形を再計算する
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
    /// マウスポインタが矩形に入っているかどうかを半手する。
    /// </summary>
    /// <returns>マウスポインタが矩形に入っている場合はtrue、素でない場合はfalseを返す。</returns>
    private bool IsPointerInsidePanel()
    {
        Vector3 pos = Input.mousePosition;

        if (pos.x < rectMin.x) { return false; }
        if (pos.x > rectMax.x) { return false; }
        if (pos.y < rectMin.y) { return false; }
        if (pos.y > rectMax.y) { return false; }

        return true;
    }
}
