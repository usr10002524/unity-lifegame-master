using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サマリーパネルがマウスポインタを避けて移動する処理を制御するクラス
/// </summary>
public class AvoidCursor : MonoBehaviour
{
    [SerializeField] private Vector2Int origPosition;
    [SerializeField] private Vector2Int avoidPosition;

    private RectTransform rectTransform;
    [SerializeField] private Vector2 rectMin = new Vector2();
    [SerializeField] private Vector2 rectMax = new Vector2();

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = origPosition;

        SetupRect();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (IsPointerInsidePanel())
        {
            rectTransform.anchoredPosition = avoidPosition;

        }
        else
        {
            rectTransform.anchoredPosition = origPosition;
        }
    }

    /// <summary>
    /// サマリーパネルの矩形を取得する
    /// </summary>
    private void SetupRect()
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
    /// サマリーパネル内にマウスポインタが入ったかどうか確認する。
    /// </summary>
    /// <returns>マウスポインタが入った場合はtrue、そうでない場合はfalseを返す。</returns>
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
    /// 強制的に外側判定するかどうかを確認する。
    /// </summary>
    /// <returns>強制的に外側判定にする場合はtrue、そうでない場合はfalseを返す。</returns>
    private bool ForcePointerOutSide()
    {
        // ヘルプウィンドウ表示中は強制的に外側判定とする。
        if (HelpPanelManager.Instance.IsActive()) { return true; }
        if (PatternPreviewManager.Instance.IsActive()) { return true; }

        return false;
    }
}
