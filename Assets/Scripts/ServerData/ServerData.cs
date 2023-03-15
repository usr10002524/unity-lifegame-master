using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 保存用データ
/// json化するために[System.Serializable]をつけている
/// </summary>
[System.Serializable]
public class ServerData
{
    public static readonly int SUCCESS = 0;
    public static readonly int FAIL = 1;

    /// <summary>
    /// 各データのバージョン
    /// </summary>
    public class DataVersion
    {
        public static readonly int SoundSettings = 1;
        public static readonly int WorldData = 1;
    }

    /// <summary>
    /// データ名
    /// </summary>
    public class DataName
    {
        public static readonly string SoundSettings = "SoundSettings";
        public static readonly string WorldData = "WorldData";
    }

    /// <summary>
    /// ロードの際に使用するデータ
    /// key はデータ名、value はjson文字列
    /// </summary>
    [System.Serializable]
    public class ServerDataItem
    {
        public string key;
        public string value;
    }

    /// <summary>
    /// サウンド設定
    /// </summary>
    [System.Serializable]
    public class SoundSettings
    {
        public int dataVersion; // データバージョン
        public float volume;    // サウンドボリューム(0-1)
        public int bgmIndex;   // 選択中のBGM

        public bool IsValid()
        {
            return (dataVersion > 0);
        }
    }

    /// <summary>
    /// プレーデータ
    /// </summary>
    [System.Serializable]
    public class WorldData
    {
        public int dataVersion; // データバージョン
        public int cols;    // ワールドの横サイズ
        public int rows;    // ワールドの縦サイズ
        public string cells;    // 各セルの状態

        public bool IsValid()
        {
            if (dataVersion <= 0) { return false; }
            if ((cols != (int)LifeGameConsts.WorldSize.Small)
                && (cols != (int)LifeGameConsts.WorldSize.Medium)
                && (cols != (int)LifeGameConsts.WorldSize.Large))
            {
                return false;
            }
            if ((rows != (int)LifeGameConsts.WorldSize.Small)
                && (rows != (int)LifeGameConsts.WorldSize.Medium)
                && (rows != (int)LifeGameConsts.WorldSize.Large))
            {
                return false;
            }
            int cellCount = (cols * rows);
            int dataLen = (0 == (cellCount % 4)) ? (cellCount / 4) : (cellCount / 4 + 1);
            if (cells.Length != dataLen)
            {
                return false;
            }

            return true;
        }
    }
}
