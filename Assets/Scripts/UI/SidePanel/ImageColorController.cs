using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PlayMode = LifeGameConsts.PlayMode;

/// <summary>
/// Imageオブジェクトの色を制御するクラス
/// </summary>
public class ImageColorController : MonoBehaviour
{
    [SerializeField] private Color editModeColor;
    [SerializeField] private Color viewModeColor;
    [SerializeField] protected float duration;
    [SerializeField] protected float animationTimer;
    [SerializeField] protected AnimationCurve curve;

    private PlayMode lastPlayMode;
    private Coroutine transitionCoroutine;
    private Image image;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        lastPlayMode = GameController.Instance.GetPlayMode();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        PlayMode playMode = GameController.Instance.GetPlayMode();
        if (playMode != lastPlayMode)
        {
            lastPlayMode = playMode;
            //プレーモードが変更されたら色変更のコルーチンを起動
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(ColorTransition());
        }
    }

    /// <summary>
    /// 色変更を行うコルーチン
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator ColorTransition()
    {
        // Debug.Log(string.Format("ColorTransition() being."));
        bool isEnd = false;
        Color startColor = (lastPlayMode == PlayMode.Edit) ? viewModeColor : editModeColor;
        Color endColor = (lastPlayMode == PlayMode.Edit) ? editModeColor : viewModeColor;
        animationTimer = 0.0f;

        while (!isEnd)
        {
            yield return null;

            // 時間経過からアニメーションカーブの値を取得
            animationTimer += Time.deltaTime;
            float time = Mathf.Clamp(animationTimer / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            Color color = Color.Lerp(startColor, endColor, t);
            image.color = color;

            if (animationTimer >= duration)
            {
                isEnd = true;
            }
        }

        // Debug.Log(string.Format("ColorTransition() end."));

    }
}
