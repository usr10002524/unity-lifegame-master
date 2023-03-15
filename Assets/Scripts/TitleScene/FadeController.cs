using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェードイン、フェードアウトを行うクラス
/// </summary>
public class FadeController : MonoBehaviour
{
    public enum FadeType
    {
        In,
        Out,
    }

    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float duration;
    [SerializeField] private AnimationCurve curve;

    private Image image;
    private FadeType setType;
    private FadeType currentType;
    private float animationTimer;
    private Coroutine fadeCoroutine;
    private bool isFading;

    /// <summary>
    /// フェードを開始する。
    /// すでに動作中の場合は何もしない。
    /// forceStartがtrueの場合は上書きする。
    /// </summary>
    /// <param name="type">FadeType</param>
    /// <param name="forceStart">フェード動作中でも上書きするか</param>
    public void StartFade(FadeType type, bool forceStart = false)
    {
        if (fadeCoroutine != null)
        {
            if (forceStart)
            {
                // forceStart が有効の場合、動作中でも止める
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            else
            {
                return; // フェード中
            }
        }

        setType = type;
        fadeCoroutine = StartCoroutine(FadeCoroutine());
    }

    /// <summary>
    /// フェードが動作中かどうか確認する
    /// </summary>
    /// <returns>フェード動作中の場合はtrue、そうでない場合はfalseを返す</returns>
    public bool IsFading()
    {
        return (fadeCoroutine != null);
    }

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        isFading = IsFading();
    }

    /// <summary>
    /// フェードを行うコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator FadeCoroutine()
    {
        bool isEnd = false;
        Color fromColor = (setType == FadeType.In) ? startColor : endColor;
        Color toColor = (setType == FadeType.In) ? endColor : startColor;
        animationTimer = 0.0f;

        while (!isEnd)
        {
            yield return null;

            animationTimer += Time.deltaTime;
            float time = Mathf.Clamp(animationTimer / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            Color color = Color.Lerp(fromColor, toColor, t);
            image.color = color;

            if (animationTimer >= duration)
            {
                isEnd = true;
            }
        }

        currentType = setType;
        fadeCoroutine = null;
    }
}
