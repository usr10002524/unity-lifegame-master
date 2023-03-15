using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// セルの色変化を行うクラス
/// </summary>
public class ImageController : MonoBehaviour
{
    /// <summary>
    /// 停止時のアニメーション位置
    /// </summary>
    public enum StopPosition
    {
        None,   //変更しない
        Start,  //最初のフレームで停止
        End,    //最後のフレームで停止
    }

    protected SpriteRenderer spriteRenderer;

    [SerializeField] protected Color startColor;
    [SerializeField] protected Color endColor;
    [SerializeField] protected bool loop;
    [SerializeField] protected bool reverse;
    [SerializeField] protected int loopTimes = 0;
    [SerializeField] protected int loopCount = 0;
    [SerializeField] protected float duration;
    [SerializeField] protected float timeRatio = 1.0f;
    [SerializeField] protected float animationTimer;
    [SerializeField] protected AnimationCurve curve;

    protected bool isPlaying = false;
    protected bool started = false;
    protected bool finished = false;

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        UpdateImage();
    }

    /// <summary>
    /// 色変化を開始する
    /// </summary>
    /// <param name="ratio">再生速度にかける係数</param>
    /// <param name="_reverse">色変化の方向</param>
    public void Play(float ratio, bool _reverse = false)
    {
        isPlaying = true;
        started = false;
        finished = false;
        reverse = _reverse;
        animationTimer = 0.0f;
        timeRatio = ratio;
    }

    /// <summary>
    /// 色変化を停止する
    /// </summary>
    /// <param name="position">停止させる位置</param>
    /// <param name="_reverse">色変化の方向</param>
    public void Stop(StopPosition position, bool _reverse = false)
    {
        isPlaying = false;
        reverse = _reverse;

        Color color = SeekColor(position);
        SetColorImmediate(color);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Stop(StopPosition.Start);
    }

    /// <summary>
    /// 色変化の更新処理
    /// </summary>
    private void UpdateImage()
    {
        if (isPlaying)
        {
            spriteRenderer.color = UpdateColor();
        }
    }

    /// <summary>
    /// 指定した色を設定する
    /// </summary>
    /// <param name="color">色</param>
    private void SetColorImmediate(Color color)
    {
        spriteRenderer.color = color;
    }

    /// <summary>
    /// カラーアニメーションの更新
    /// </summary>
    /// <returns>Color</returns>
    private Color UpdateColor()
    {
        if (!started)
        {
            started = true;
        }
        else
        {
            UpdateAnimationTime();
        }

        float rate = GetFramerate();
        Color color = GetColorFromCurve(rate);
        return color;
    }

    /// <summary>
    /// アニメーションタイムの更新
    /// </summary>
    private void UpdateAnimationTime()
    {
        if (finished)
        {
            return; //終了済み
        }

        animationTimer += (Time.deltaTime * timeRatio);
        if (animationTimer >= duration)
        {
            if (loop)
            {
                if (loopTimes > 0)
                {
                    loopCount++;
                    if (loopCount < loopTimes)
                    {
                        animationTimer %= duration;
                    }
                    else
                    {
                        //ループ終了
                        animationTimer = duration;
                        finished = true;
                    }
                }
                else
                {
                    //無限ループ
                }
            }
            else
            {
                //アニメーション終了
                animationTimer = duration;
                finished = true;
            }
        }
    }

    /// <summary>
    /// 全体のアニメーション時間に対する現在位置を取得する
    /// </summary>
    /// <returns>全体のアニメーション時間に対する現在位置</returns>
    private float GetFramerate()
    {
        float rate = animationTimer / duration;
        return rate;
    }

    /// <summary>
    /// 時間割合に対する色を取得する
    /// </summary>
    /// <param name="rate">時間割合</param>
    /// <returns>色</returns>
    private Color GetColorFromCurve(float rate)
    {
        float t = curve.Evaluate(rate);

        Color color = Color.Lerp(GetStartColor(), GetEndColor(), t);
        return color;
    }

    /// <summary>
    /// 色変化の開始の色を取得する
    /// </summary>
    /// <returns>開始の色</returns>
    private Color GetStartColor()
    {
        return (reverse) ? endColor : startColor;
    }

    /// <summary>
    /// 色変化の終了の色を取得する
    /// </summary>
    /// <returns>終了の色</returns>
    private Color GetEndColor()
    {
        return (reverse) ? startColor : endColor;
    }

    /// <summary>
    /// 現在の色を取得する
    /// </summary>
    /// <param name="position">停止位置</param>
    /// <returns>色</returns>
    private Color SeekColor(StopPosition position)
    {
        float rate = 0.0f;

        switch (position)
        {
            case StopPosition.None: rate = GetFramerate(); break;
            case StopPosition.Start: rate = 0.0f; break;
            case StopPosition.End: rate = 1.0f; break;
        }

        Color color = GetColorFromCurve(rate);
        return color;
    }
}
