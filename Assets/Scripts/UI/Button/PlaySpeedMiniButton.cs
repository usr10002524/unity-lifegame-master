using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlaySpeed = LifeGameConsts.PlaySpeed;

/// <summary>
/// サマリーパネル プレースピード表示の制御を行うクラス
/// </summary>
public class PlaySpeedMiniButton : MiniButtonControl
{
    private class SpriteName
    {
        public static readonly string on = "s_icon_on";
        public static readonly string off = "s_icon_off";
    }

    [SerializeField] PlaySpeed speed;

    private PlaySpeed lastPlaySpeed;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        BaseStart();
        lastPlaySpeed = GameController.Instance.GetPlaySpeed();

        string spriteName = GetSpriteName(lastPlaySpeed);
        ReplaceSprite(spriteName);
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        PlaySpeed playSpeed = GameController.Instance.GetPlaySpeed();
        if (playSpeed != lastPlaySpeed)
        {
            lastPlaySpeed = playSpeed;
            string spriteName = GetSpriteName(playSpeed);
            ReplaceSprite(spriteName);
        }
    }

    /// <summary>
    /// プレースピードに対応したスプライト名を取得する
    /// </summary>
    /// <param name="playSpeed">プレースピード</param>
    /// <returns>スプライト名</returns>
    private string GetSpriteName(PlaySpeed playSpeed)
    {
        if (speed <= playSpeed)
        {
            return SpriteName.on;
        }
        else
        {
            return SpriteName.off;
        }
    }
}
