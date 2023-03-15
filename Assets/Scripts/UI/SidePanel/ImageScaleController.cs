using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Image のスケールを制御するクラス
/// </summary>
public class ImageScaleController : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;
    [SerializeField] private Vector3 panelOpenScale = Vector3.one;
    [SerializeField] private Vector3 panelCloseScale = Vector3.one;
    [SerializeField] protected float duration;
    [SerializeField] protected float animationTimer;
    [SerializeField] protected AnimationCurve curve;

    private SidePanelContainer.Stat lastStat;
    private SidePanelContainer container;

    private Coroutine transitionCoroutine;
    private Image image;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        container = containerObject.GetComponent<SidePanelContainer>();
        lastStat = container.GetStat();
        image = GetComponent<Image>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        SidePanelContainer.Stat stat = container.GetStat();

        if (stat != lastStat)
        {
            lastStat = stat;
            //ステータスが変更されたらスケール変更のコルーチンを起動
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            transitionCoroutine = StartCoroutine(ScaleTransition());
        }
    }

    /// <summary>
    /// スケール変化を行うコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator ScaleTransition()
    {
        // Debug.Log(string.Format("ScaleTransition() being."));
        bool isEnd = false;
        Vector3 startScale = (lastStat == SidePanelContainer.Stat.Close) ? panelCloseScale : panelOpenScale;
        Vector3 endScale = (lastStat == SidePanelContainer.Stat.Close) ? panelOpenScale : panelCloseScale;
        animationTimer = 0.0f;

        while (!isEnd)
        {
            yield return null;

            // 時間経過からアニメーションカーブの値を取得
            animationTimer += Time.deltaTime;
            float time = Mathf.Clamp(animationTimer / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            Vector3 scale = Vector3.Lerp(startScale, endScale, t);
            image.transform.localScale = scale;

            if (animationTimer >= duration)
            {
                isEnd = true;
            }
        }

        // Debug.Log(string.Format("ScaleTransition() end."));

    }
}
