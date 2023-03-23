using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// WebGLでローカルストレージを呼び出すクラス
/// 内部で、ローカルストレージ用プラグインとやり取りを行っている
/// </summary>
public class LocalStorageAPI : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    
    [DllImport("__Internal")]
    private static extern void loadLocalData(string gameObject, string methodName);

    [DllImport("__Internal")]
    private static extern void saveLocalData(string gameObject, string methodName, string dataJson);

    [DllImport("__Internal")]
    private static extern void deleteLocalData(string gameObject, string methodName, string dataJson);
#else
    //エディタ用のダミー関数
    private static void loadLocalData(string gameObject, string methodName) { }
    private static void saveLocalData(string gameObject, string methodName, string dataJson) { }
    private static void deleteLocalData(string gameObject, string methodName, string dataJson) { }
#endif
    private bool localDataLoaded;
    private ServerData.SoundSettings soundSetting = new ServerData.SoundSettings();
    private ServerData.WorldData worldData = new ServerData.WorldData();
    private ServerData.LangSettings langSetting = new ServerData.LangSettings();

    public static LocalStorageAPI Instance { get; private set; }

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
    /// サーバデータのロードを行う
    /// </summary>
    /// <returns>ロード開始したかどうか</returns>
    public bool LoadLocalrData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        localDataLoaded = false;

        loadLocalData("LocalStorage", "ReceiveLoadLocalData");
        // Debug.Log(string.Format("LoadServerData() called."));
        return true;
#else
        // ロード済みとする
        localDataLoaded = true;
        // Debug.Log(string.Format("LoadServerData() called.(dummy loaded)"));
        return true;
#endif
    }

    /// <summary>
    /// サーバデータのロード結果を受け取るクラス
    /// </summary>
    [System.Serializable]
    private class ServerItems
    {
        public int stat;
        public ServerData.ServerDataItem[] data;
    }

    /// <summary>
    /// プラグインからのサーバデータロードの結果を受けるコールバック
    /// </summary>
    public void ReceiveLoadLocalData(string json)
    {
        // Debug.Log(string.Format("ReceiveLoadLocalData() called."));
        // Debug.Log(string.Format("ReceiveLoadLocalData() json={0}.", json));

        ServerItems data = JsonUtility.FromJson<ServerItems>(json);
        // Debug.Log(string.Format("ReceiveLoadLocalData() load stat={0} datalen={1}.", data.stat, data.data.Length));
        localDataLoaded = true;

        if (data.stat != ServerData.SUCCESS)
        {
            return; // ロード失敗
        }

        for (int i = 0; i < data.data.Length; i++)
        {
            string key = data.data[i].key;
            string value = data.data[i].value;

            if (key == ServerData.DataName.SoundSettings)
            {
                ParseSoundSettings(value);
            }
            else if (key == ServerData.DataName.WorldData)
            {
                ParseWorldData(value);
            }
            else if (key == ServerData.DataName.LangSettings)
            {
                ParseLangSettings(value);
            }
        }
    }

    /// <summary>
    /// ローカルストレージのロードが完了したか確認する
    /// </summary>
    /// <returns>取得中はfalse、完了した場合はtrueを返す。</returns>
    public bool IsLocalDataLoaded()
    {
        return localDataLoaded;
    }

    /// <summary>
    /// サウンド設定のJSONをパースする。
    /// </summary>
    /// <param name="json">サウンド設定のJSON</param>
    private void ParseSoundSettings(string json)
    {
        soundSetting = JsonUtility.FromJson<ServerData.SoundSettings>(json);
    }

    /// <summary>
    /// ワールドデータのJSONをパースする
    /// </summary>
    /// <param name="json"></param>
    private void ParseWorldData(string json)
    {
        worldData = JsonUtility.FromJson<ServerData.WorldData>(json);
    }

    /// <summary>
    /// 言語設定のJSONをパースする
    /// </summary>
    /// <param name="json"></param>
    private void ParseLangSettings(string json)
    {
        langSetting = JsonUtility.FromJson<ServerData.LangSettings>(json);
    }

    /// <summary>
    /// ローカルストレージのサウンド設定を取得
    /// </summary>
    /// <returns>サウンド設定</returns>
    public ServerData.SoundSettings GetSoundSettings()
    {
        return soundSetting;
    }

    /// <summary>
    /// ローカルストレージのワールドデータを取得
    /// </summary>
    /// <returns>ワールドデータ</returns>
    public ServerData.WorldData GetWorldData()
    {
        return worldData;
    }

    /// <summary>
    /// ローカルストレージの言語設定を取得
    /// </summary>
    /// <returns>言語設定</returns>
    public ServerData.LangSettings GetLangSettings()
    {
        return langSetting;
    }

    /// <summary>
    /// サウンド設定を保存する
    /// </summary>
    /// <param name="data">保存するデータ</param>
    /// <returns>保存開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool SaveSoundSettings(ServerData.SoundSettings data)
    {
        soundSetting = data;


        ServerItems items = new ServerItems();
        items.data = new ServerData.ServerDataItem[1];
        items.data[0] = new ServerData.ServerDataItem();
        items.data[0].key = ServerData.DataName.SoundSettings;
        items.data[0].value = JsonUtility.ToJson(data);
        string json = JsonUtility.ToJson(items);

        // Debug.Log(string.Format("SaveSoundSettings() called."));
        // Debug.Log(string.Format("SaveSoundSettings(). key:{0} value:{1}", items.data[0].key, items.data[0].value));
        // Debug.Log(string.Format("SaveSoundSettings(). json:{0}", json));

        saveLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// ワールドデータを保存する
    /// </summary>
    /// <param name="data">保存するワールドデータ</param>
    /// <returns>保存開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool SaveWorldData(ServerData.WorldData data)
    {
        worldData = data;

        ServerItems items = new ServerItems();
        items.data = new ServerData.ServerDataItem[1];
        items.data[0] = new ServerData.ServerDataItem();
        items.data[0].key = ServerData.DataName.WorldData;
        items.data[0].value = JsonUtility.ToJson(data);
        string json = JsonUtility.ToJson(items);

        // Debug.Log(string.Format("SaveWorldData() called."));
        // Debug.Log(string.Format("SaveWorldData(). key:{0} value:{1}", items.data[0].key, items.data[0].value));
        // Debug.Log(string.Format("SaveWorldData(). json:{0}", json));

        saveLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// 言語設定を保存する
    /// </summary>
    /// <param name="data">保存するデータ</param>
    /// <returns>保存開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool SaveLangSettings(ServerData.LangSettings data)
    {
        langSetting = data;


        ServerItems items = new ServerItems();
        items.data = new ServerData.ServerDataItem[1];
        items.data[0] = new ServerData.ServerDataItem();
        items.data[0].key = ServerData.DataName.LangSettings;
        items.data[0].value = JsonUtility.ToJson(data);
        string json = JsonUtility.ToJson(items);

        // Debug.Log(string.Format("SaveLangSettings() called."));
        // Debug.Log(string.Format("SaveLangSettings(). key:{0} value:{1}", items.data[0].key, items.data[0].value));
        // Debug.Log(string.Format("SaveLangSettings(). json:{0}", json));

        saveLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }


    /// <summary>
    /// サウンド設定を削除する
    /// </summary>
    /// <returns>削除開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool DeleteSoundSettings()
    {
        ServerData.ServerDataItem item = new ServerData.ServerDataItem();
        item.key = ServerData.DataName.SoundSettings;
        item.value = "";
        string json = JsonUtility.ToJson(item);

        deleteLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// ワールドデータを削除する
    /// </summary>
    /// <returns>削除開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool DeleteWorldData()
    {
        ServerData.ServerDataItem item = new ServerData.ServerDataItem();
        item.key = ServerData.DataName.WorldData;
        item.value = "";
        string json = JsonUtility.ToJson(item);

        deleteLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// 言語設定を削除する
    /// </summary>
    /// <returns>削除開始した場合はtrue, API使用不可の場合はfalseを返す。</returns>
    public bool DeleteLangSettings()
    {
        ServerData.ServerDataItem item = new ServerData.ServerDataItem();
        item.key = ServerData.DataName.LangSettings;
        item.value = "";
        string json = JsonUtility.ToJson(item);

        deleteLocalData("LocalStorage", "ReceiveCommonStat", json);
        return true;
    }

    /// <summary>
    /// プラグインからの共通の汎用返答を受け取るクラス
    /// </summary>
    [System.Serializable]
    private class ReceiveStat
    {
        public int stat;
    }

    /// <summary>
    /// プラグインからの汎用返答を受けるコールバック
    /// </summary>
    /// <param name="json"></param>
    public void ReceiveCommonStat(string json)
    {
        // Debug.Log(string.Format("ReceiveCommonStat() called."));
        // Debug.Log(string.Format("ReceiveCommonStat() json={0}.", json));

        ReceiveStat data = JsonUtility.FromJson<ReceiveStat>(json);
        // Debug.Log(string.Format("ReceiveCommonStat() load stat={0}.", data.stat));
    }
}
