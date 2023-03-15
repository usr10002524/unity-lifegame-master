using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム管理クラス
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum StartStat
    {
        Start,
        Continue,
    }

    private StartStat startStat;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// タイトル画面でStart,Continueどちらが選択されたかを保持する。
    /// </summary>
    /// <param name="stat">Start,Continueどちらが選択されたか</param>
    public void SetStartStat(StartStat stat)
    {
        startStat = stat;
    }

    /// <summary>
    /// タイトル画面でStart,Continueどちらが選択されたかを取得する。
    /// </summary>
    /// <returns>Start,Continueどちらが選択されたか</returns>
    public StartStat GetStartStat()
    {
        return startStat;
    }



}
