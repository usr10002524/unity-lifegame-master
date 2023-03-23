using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;

public class LocalDataManager : MonoBehaviour
{
    private class Result
    {
        public int stat;
        public string json;

        public Result(int stat, string json)
        {
            this.stat = stat;
            this.json = json;
        }
    }

    private string persistentDataPath;
    private Dictionary<string, Result> dataDict;

    public static LocalDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        dataDict = new Dictionary<string, Result>();
        persistentDataPath = Application.persistentDataPath;
        // Debug.Log(string.Format("LocalDataManager.Awake() persistentDataPath:{0}", persistentDataPath));
    }

    public void LoadData(string key)
    {
        if (IsLoaded(key))
        {
            Debug.Log(string.Format("LocalDataManager.LoadData() already loaded. key:{0}", key));
            return;
        }

        InternalLoad(key);
    }

    public bool IsLoaded(string key)
    {
        return dataDict.ContainsKey(key);
    }


    public int GetResult(string key)
    {
        if (IsLoaded(key))
        {
            return dataDict[key].stat;
        }
        else
        {
            return ServerData.SUCCESS;
        }
    }

    public string GetData(string key)
    {
        if (IsLoaded(key))
        {
            return dataDict[key].json;
        }
        else
        {
            return "";
        }
    }

    public void SaveData(string key, string json)
    {
        InternalSave(key, json);
    }


    private async void InternalLoad(string key)
    {
        // Debug.Log(string.Format("LocalDataManager.InternalLoad() enter. key:{0}", key));

        Result result = null;
        await Task.Run(() =>
        {
            result = LoadOneData(key);
        });

        // Dictionary はスレッドセーフではないので、スレッド外でアクセスする
        if (!IsLoaded(key) && result != null)
        {
            dataDict.Add(key, result);
        }

        // Debug.Log(string.Format("LocalDataManager.InternalLoad() leave. key:{0}", key));
    }


    private Result LoadOneData(string key)
    {
        // Debug.Log(string.Format("LocalDataManager.LoadOneData() enter. key:{0}", key));

        string path = persistentDataPath + "/" + key + ".json";
        string json = "";
        int stat = ServerData.FAIL;
        if (File.Exists(path))
        {
            json = File.ReadAllText(path);
            stat = ServerData.SUCCESS;

            // Debug.Log("LoadOneData : path" + path);
            // Debug.Log("LoadOneData : json" + json);
        }
        else
        {
            Debug.Log("LoadOneData : path not found.");
            json = "";
            stat = ServerData.FAIL;
        }

        // Debug.Log(string.Format("LocalDataManager.LoadOneData() leave. key:{0}", key));
        return new Result(stat, json);
    }

    private async void InternalSave(string key, string json)
    {
        // Dictionary はスレッドセーフではないので、スレッド外でアクセスする
        if (dataDict.ContainsKey(key))
        {
            dataDict.Remove(key);
        }
        dataDict.Add(key, new Result(ServerData.SUCCESS, json));

        await Task.Run(() =>
        {
            SaveOneData(key, json);
        });
    }

    private void SaveOneData(string key, string json)
    {
        string path = persistentDataPath + "/" + key + ".json";
        File.WriteAllText(path, json);
    }
}
