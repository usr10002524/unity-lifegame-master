using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LifeGameConsts
{

    namespace Cell
    {
        /// <summary>
        /// セルの状態定義
        /// </summary>
        public enum Stat
        {
            None,   // 不定値
            Alive,  // 生存状態
            Dead,   // 死亡状態
        }
    }

    /// <summary>
    /// ライフゲームのプレイモード。
    /// セルの配置ができる Edit と、ライフゲームを鑑賞する View がある。
    /// </summary>
    public enum PlayMode
    {
        Edit,   // 編集モード
        View,   // 鑑賞モード
    };

    /// <summary>
    /// 編集モード中の書き込みモード
    /// </summary>
    public enum EditMode
    {
        Write,  // 書き込み
        PatternPaste,   // パターン貼り付け
        Erase,  // 消去
    }

    /// <summary>
    /// 鑑賞モードの更新スピード
    /// </summary>
    public enum PlaySpeed
    {
        Slow,
        Medium,
        Fast,
        Fastest,

        Max,
    }

    /// <summary>
    /// ワールドのサイズ
    /// </summary>
    public enum WorldSize
    {
        Small = 32,
        Medium = 64,
        Large = 128,

        Default = Medium,
    }

}
