using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PatternLoader
{
    public enum LoadStat
    {
        During,
        Succeeded,
        Failed,
    }

    public class PatternData
    {
        public enum Rotate
        {
            Rot0,
            Rot90,
            Rot180,
            Rot270,
        }

        public int height;
        public int width;
        public List<int> stats;

        private bool vFlip;
        private bool hFlip;
        private Rotate rotate;
        private bool isDirty;

        public bool IsValid()
        {
            if (stats == null)
            {
                return false;
            }
            if (stats.Count == 0)
            {
                return false;
            }
            return true;
        }

        public void SetVFlip(bool flip)
        {
            isDirty |= (vFlip == flip);
            vFlip = flip;
        }
        public bool GetVFlip(bool flip)
        {
            return vFlip;
        }

        public void SetHFlip(bool flip)
        {
            isDirty |= (hFlip == flip);
            hFlip = flip;
        }
        public bool GetHFlip(bool flip)
        {
            return hFlip;
        }

        public void SetRotate(Rotate rot)
        {
            isDirty |= (rotate == rot);
            rotate = rot;
        }
        public Rotate SetRotate()
        {
            return rotate;
        }

        public void ResetFlipAndRotate()
        {
            isDirty |= (vFlip == false);
            isDirty |= (hFlip == false);
            isDirty |= (rotate == Rotate.Rot0);

            vFlip = false;
            hFlip = false;
            rotate = Rotate.Rot0;
        }

        public int GetWidth()
        {
            if (rotate == Rotate.Rot0 || rotate == Rotate.Rot180)
            {
                return width;
            }
            else
            {
                return height;
            }
        }

        public int GetHeight()
        {
            if (rotate == Rotate.Rot0 || rotate == Rotate.Rot180)
            {
                return height;
            }
            else
            {
                return width;
            }
        }

        public bool GetStat(int col, int row, ref int stat)
        {
            if (col < 0) { return false; }
            if (col >= GetWidth()) { return false; }
            if (row < 0) { return false; }
            if (row >= GetHeight()) { return false; }

            // 反転を考慮して参照する座標を変換
            if (hFlip)
            {
                col = GetWidth() - 1 - col;
            }
            if (vFlip)
            {
                row = GetHeight() - 1 - row;
            }

            // 回転を考慮して、参照する座標に変換する
            int x = col;
            int y = row;
            switch (rotate)
            {
                case Rotate.Rot0:
                    x = col;
                    y = row;
                    break;

                case Rotate.Rot90:
                    x = row;
                    y = height - 1 - col;
                    break;

                case Rotate.Rot180:
                    x = width - 1 - col;
                    y = height - 1 - row;
                    break;

                case Rotate.Rot270:
                    x = width - 1 - row;
                    y = col;
                    break;
            }

            int index = y * width + x;
            if (index >= 0 && index < stats.Count)
            {
                stat = stats[index];
                return true;
            }
            else
            {
                Debug.Log(string.Format("illegal index:{0} Count:{1} col:{2} row:{3} vFlip:{4} hFlip:{5} Rotate:{6}, x:{7} y:{8}",
                index, stats.Count, col, row, vFlip, hFlip, rotate, x, y));
                return false;
            }
        }

        public bool IsDirty()
        {
            return isDirty;
        }

        public void ResetDirty()
        {
            isDirty = false;
        }
    }

    // ロード結果
    private class Result
    {
        public LoadStat stat;
        public PatternData patternData;
    }

    private string groupName;
    private string groupExt;
    private Dictionary<string, Result> loadMap;
    private bool isLoading;
    private string loadingName;

    public PatternLoader(string group, string ext)
    {
        groupName = group;
        groupExt = ext;
        loadMap = new Dictionary<string, Result>();
        isLoading = false;
        loadingName = "";
    }

    public void Load(string name)
    {
        if (isLoading)
        {
            return; //ロード中
        }

        if (loadMap.ContainsKey(name))
        {
            return; // ロード済み
        }

        isLoading = true;
        loadingName = name;

        Result result = new Result();
        result.stat = LoadStat.During;
        result.patternData = null;
        loadMap.Add(name, result);

        LoadPattern(name);
    }

    public bool IsLoadComplete(string name)
    {
        if (loadMap.ContainsKey(name))
        {
            if (LoadStat.During == loadMap[name].stat)
            {
                return false;   //ロード中
            }
            else
            {
                return true;
            }
        }
        else
        {
            return true; // 未登録の場合は、完了済みで返す
        }
    }

    public List<string> GetPatternList()
    {
        List<string> patternList = new List<string>();

        foreach (var item in loadMap.Keys)
        {
            patternList.Add(item);
        }

        return patternList;
    }

    public PatternData GetData(string name)
    {
        if (loadMap.ContainsKey(name))
        {
            return loadMap[name].patternData;
        }
        else
        {
            return null;
        }
    }


    // パターンの読み込み
    private void LoadPattern(string name)
    {
        string path = groupName + "/" + name + "." + groupExt;
        // Debug.Log(string.Format("LoadPattern() path:{0}", path));

        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(path);
        handle.Completed += OnCompleteHandler;
    }

    // 読み込み終了時のコールバック
    private void OnCompleteHandler(AsyncOperationHandle<TextAsset> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            string name = loadingName;
            // Debug.Log(string.Format("Addressables.LoadAssetAsync Succeeded. name:{0}", name));

            if (loadMap.ContainsKey(name))
            {
                loadMap[name].stat = LoadStat.Succeeded;
                loadMap[name].patternData = ParseTextAsset(obj.Result);
            }
            else
            {
                // ロード開始時に登録されていない？
                // とりあえず完了として登録する
                Result result = new Result();
                result.stat = LoadStat.Succeeded;
                result.patternData = ParseTextAsset(obj.Result);
                loadMap.Add(name, result);
            }
        }
        else
        {
            string name = loadingName;
            // Debug.Log(string.Format("Addressables.LoadAssetAsync Failed. name:{0}", name));

            if (loadMap.ContainsKey(name))
            {
                loadMap[name].stat = LoadStat.Failed;
                loadMap[name].patternData = null;
            }
            else
            {
                // ロード開始時に登録されていない？
                // とりあえず完了として登録する
                Result result = new Result();
                result.stat = LoadStat.Failed;
                result.patternData = null;
                loadMap.Add(name, result);
            }
        }

        Addressables.Release(obj);

        isLoading = false;
        loadingName = "";
    }

    private PatternData ParseTextAsset(TextAsset textAsset)
    {
        int maxWidth = 0;
        List<string> lines = new List<string>();

        StringReader reader = new StringReader(textAsset.text);
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            // Debug.Log(line);

            int commentPos = line.LastIndexOf('!');
            if (0 == commentPos)
            {
                continue;   //この行はコメントなので無視
            }

            lines.Add(line);
            maxWidth = (line.Length > maxWidth) ? line.Length : maxWidth;
        }

        List<int> cells = new List<int>();
        foreach (var line in lines)
        {
            for (int i = 0; i < maxWidth; i++)
            {
                if (i < line.Length)
                {
                    if (line[i] == 'O')
                    {
                        cells.Add(1);
                    }
                    else
                    {
                        cells.Add(0);
                    }
                }
                else
                {
                    cells.Add(0);
                }
            }
        }

        PatternData patternData = new PatternData();
        patternData.height = lines.Count;
        patternData.width = maxWidth;
        patternData.stats = cells;

        //@@@ debug
        // {
        //     Debug.Log(string.Format("lines: {0}", lines.Count));
        //     Debug.Log(string.Format("width: {0}", maxWidth));
        //     Debug.Log(string.Format("size : {0}", cells.Count));

        //     string cellStat = "";
        //     for (int i = 0; i < cells.Count; i++)
        //     {
        //         cellStat += cells[i];
        //     }
        //     Debug.Log(string.Format("stat : {0}", cellStat));
        // }

        return patternData;
    }
}
