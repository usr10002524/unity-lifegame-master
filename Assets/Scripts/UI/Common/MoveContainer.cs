using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// パネルの開閉を制御するクラス
/// </summary>
public class MoveContainer : MonoBehaviour
{
    public enum Stat
    {
        Close,
        Open,
    }

    [SerializeField] protected Vector2 startPostion;
    [SerializeField] protected Vector2 endPostion;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected AnimationCurve curve;

    [SerializeField] protected Stat currentStat = Stat.Close;
    [SerializeField] protected Stat setStat = Stat.Close;

    protected RectTransform rectTransform;
    protected Coroutine moveCoroutine;
    protected float moveTime;
    protected bool isMoving;


    /// <summary>
    /// Start相当の処理を行う。
    /// 派生クラスから呼ばれる。
    /// </summary>
    protected void BaseStart()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Update相当の処理を行う。
    /// 派生クラスから呼ばれる。
    /// </summary>
    protected void BaseUpdate()
    {
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     SetMove(Stat.Open);
        // }
        // else if (Input.GetKeyDown(KeyCode.C))
        // {
        //     SetMove(Stat.Close);
        // }
    }

    /// <summary>
    /// パネルの開閉を行う。
    /// </summary>
    /// <param name="stat">MoveContainer.Stat</param>
    public void SetMove(Stat stat)
    {
        if (isMoving)
        {
            return;
        }

        if (currentStat != stat)
        {
            setStat = stat;
            isMoving = true;
            moveCoroutine = StartCoroutine(MoveCroutine());
        }
    }

    /// <summary>
    /// パネルが開閉動作中か確認する
    /// </summary>
    /// <returns>開閉動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsMoving()
    {
        return isMoving;
    }

    /// <summary>
    /// パネルが開く動作中か確認する
    /// </summary>
    /// <returns>開く動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsMovingOpen()
    {
        return (isMoving && setStat == Stat.Open);
    }

    /// <summary>
    /// パネルが閉じる動作中か確認する
    /// </summary>
    /// <returns>閉じる動作中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsMovingClose()
    {
        return (isMoving && setStat == Stat.Close);
    }

    /// <summary>
    /// パネルの現在の状態を取得する
    /// </summary>
    /// <returns>MoveContainer.Stat</returns>
    public Stat GetStat()
    {
        return setStat;
    }

    /// <summary>
    /// パネル開閉動作を行うコルーチン。
    /// </summary>
    /// <returns>IEnumerator</returns>
    protected IEnumerator MoveCroutine()
    {
        bool isEnd = false;
        Vector2 beginPos = (setStat == Stat.Open) ? startPostion : endPostion;
        Vector2 endPos = (setStat == Stat.Open) ? endPostion : startPostion;

        moveTime = 0.0f;

        while (!isEnd)
        {
            yield return null;

            // 時間経過からアニメーションカーブの値を取得
            moveTime += Time.deltaTime;
            float time = Mathf.Clamp(moveTime / duration, 0.0f, 1.0f);
            float t = curve.Evaluate(time);

            Vector2 position = Vector2.Lerp(beginPos, endPos, t);
            rectTransform.anchoredPosition = position;

            if (moveTime >= duration)
            {
                isEnd = true;
            }
        }

        currentStat = setStat;
        isMoving = false;
    }
}
