using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// サイドパネルの開閉を制御するクラス
/// </summary>
public class SidePanelContainer : MoveContainer
{
    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        BaseStart();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        BaseUpdate();
    }

    /// <summary>
    /// パネルを開く際に呼ばれる。
    /// </summary>
    public void OnEnterTirgger()
    {
        // Debug.Log("OnEnterTirgger");

        if (currentStat == Stat.Close)
        {
            SetMove(Stat.Open);
        }
    }

    /// <summary>
    /// パネルを閉じる際に呼ばれる。
    /// </summary>
    public void OnLeaveTrigger()
    {
        // Debug.Log("OnLeaveTrigger");

        if (currentStat == Stat.Open)
        {
            SetMove(Stat.Close);
        }
    }

    /// <summary>
    /// パネルがクリックした際に呼ばれる。
    /// </summary>
    public void OnClickTrigger()
    {
        // Debug.Log("OnClickTrigger");

        if (currentStat == Stat.Close)
        {
            SetMove(Stat.Open);
        }
        else if (currentStat == Stat.Open)
        {
            SetMove(Stat.Close);
        }
    }

}
