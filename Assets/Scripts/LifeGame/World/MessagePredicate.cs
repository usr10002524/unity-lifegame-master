using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

/// <summary>
/// 各種メッセージを表示する際の条件を判定するクラス
/// </summary>
namespace WorldMessagePredicate
{
    /// <summary>
    /// 条件判定の基底クラス
    /// </summary>
    abstract class Predicate
    {
        protected CellWorld cellWorld;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="world">ワールド</param>
        protected Predicate(CellWorld world)
        {
            cellWorld = world;
        }

        /// <summary>
        /// チェックを行う
        /// </summary>
        /// <param name="dt">前フレームからの経過時間（秒）</param>
        /// <returns>条件を満たした場合はtrue、そうでない場合はfalseを返す。</returns>
        abstract public bool Check(float dt);

        /// <summary>
        /// 条件をリセットする。
        /// </summary>
        abstract public void Reset();

        /// <summary>
        /// 条件を満たした際のメッセージの文字列を取得する。
        /// </summary>
        /// <returns>メッセージの文字列</returns>
        abstract public string GetMessage();
    }


    /// <summary>
    /// セル数を条件としたクラス
    /// </summary>
    class CellCount : Predicate
    {
        private static readonly float durationOnce = 10.0f;
        private static readonly float durationLater = 30.0f;
        private static readonly int cond = 10;

        private bool once;
        private float timer;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="world">ワールド</param>
        public CellCount(CellWorld world)
        : base(world)
        {
            once = false;
            timer = 0.0f;
        }

        /// <summary>
        /// チェックを行う
        /// </summary>
        /// <param name="dt">前フレームからの経過時間（秒）</param>
        /// <returns>条件を満たした場合はtrue、そうでない場合はfalseを返す。</returns>
        public override bool Check(float dt)
        {
            if (CheckCondition())
            {
                float duration = GetDuration();
                if (timer < duration)
                {
                    timer += dt;
                }

                if (timer >= duration)
                {
                    once = true;    // 次回からは間隔を変更する
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                timer = 0.0f;
                return false;
            }
        }

        /// <summary>
        /// 条件を満たした際のメッセージの文字列を取得する。
        /// </summary>
        /// <returns>メッセージの文字列</returns>
        public override string GetMessage()
        {
            if (GameController.Instance.IsEditMode())
            {
                int cells = cellWorld.GetAliveCellCount();
                if (cells < cond)
                {
                    return GetEntry("main.message.letsput");
                }
                else
                {
                    return GetEntry("main.message.letsplay");
                }
            }
            else if (GameController.Instance.IsViewMode())
            {
                return GetEntry("main.message.letswatch");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 条件をリセットする。
        /// </summary>
        public override void Reset()
        {
            timer = 0.0f;
        }

        /// <summary>
        /// 判定を行う条件を満たしているか確認する
        /// </summary>
        /// <returns>条件を満たしている場合はtrue、そうでない場合はfalseを返す。</returns>
        private bool CheckCondition()
        {
            // int cells = cellWorld.GetAliveCellCount();
            // if (cells >= cond)
            // {
            //     once = true;    // 配置セル数が一度 cond を超えた場合は表示間隔を変更する
            //     return false;
            // }
            // if (GameController.Instance.IsViewMode())
            // {
            //     return false;   // Viewモードのときは表示しない
            // }
            if (MessageManager.Instance.IsShowMessage())
            {
                return false;   // 他のメッセージ表示中は表示しない
            }

            return true;
        }

        /// <summary>
        /// 条件判定に用いる秒数を取得する。
        /// 初回とそれ以降で条件が異なる。
        /// </summary>
        /// <returns>条件判定に用いる秒数</returns>
        private float GetDuration()
        {
            if (once)
            {
                return durationLater;
            }
            else
            {
                return durationOnce;
            }
        }

        private string GetEntry(string target)
        {
            string targetTableName = "StringTable";
            LocalizedStringDatabase stringDatabase = LocalizationSettings.StringDatabase;
            var entry = LocalizationSettings.StringDatabase.GetTableEntry(targetTableName, target).Entry;
            if (entry == null)
            {
                return "";
            }
            else
            {
                return entry.Value;
            }
        }
    }


}