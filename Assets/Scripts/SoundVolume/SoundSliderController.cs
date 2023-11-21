using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// サウンドボリュームスライダーコントローラ
/// </summary>
public class SoundSliderController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject sliderPanel;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private UnityEvent outsideClickTrigger;

    private RectTransform sliderRectTransform;
    private RectTransform panelRectTransform;


    /// <summary>
    /// サウンドボリュームをセットする
    /// </summary>
    /// <param name="volume">サウンドボリュームの値(0-1)</param>
    public void SetVolume(float volume)
    {
        // 0.0~1.0の範囲にクランプする
        volume = Mathf.Clamp01(volume);
        slider.value = volume;
    }

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        slider.minValue = 0.0f;
        slider.maxValue = 1.0f;

        if (sliderPanel != null)
        {
            sliderRectTransform = sliderPanel.GetComponent<RectTransform>();
        }
        if (buttonPanel != null)
        {
            panelRectTransform = buttonPanel.GetComponent<RectTransform>();
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        if (IsMouseClickOutside())
        {
            if (outsideClickTrigger != null)
            {
                outsideClickTrigger.Invoke();
            }
        }
    }

    /// <summary>
    /// スライダーの外でクリックされたかチェックする
    /// </summary>
    /// <returns></returns>
    private bool IsMouseClickOutside()
    {
        if (sliderPanel == null)
        {
            return false;
        }
        if (!sliderPanel.activeInHierarchy)
        {
            return false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (IsInsideObject(panelRectTransform, Input.mousePosition.x, Input.mousePosition.y))
            {
                return false;   // ボタンパネル内のクリックは無視する
            }
            if (!IsInsideObject(sliderRectTransform, Input.mousePosition.x, Input.mousePosition.y))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定された座標がオブジェクトの範囲内かチェックする
    /// </summary>
    /// <param name="rectTransform">オブジェクトのRectTransform</param>
    /// <param name="x">X座標</param>
    /// <param name="y">Y座標</param>
    /// <returns>オブジェクト内のときはtrue、そうでないときはfalseを返す</returns>
    private bool IsInsideObject(RectTransform rectTransform, float x, float y)
    {
        if (rectTransform == null)
        {
            return false;
        }
        Vector2 rectMin = new Vector2();
        Vector2 rectMax = new Vector2();

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

        if ((rectMin.x <= x)
            && (x <= rectMax.x)
            && (rectMin.y <= y)
            && (y <= rectMax.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
