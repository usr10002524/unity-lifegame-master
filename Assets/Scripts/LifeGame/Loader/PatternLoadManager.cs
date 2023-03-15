using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLoadManager : MonoBehaviour
{
    private class PatternInfo
    {
        public string filename;
        public string patternName;

        public PatternInfo(string filename, string patternName)
        {
            this.filename = filename;
            this.patternName = patternName;
        }
    }

    private PatternLoader patternLoader;
    private bool isLoadCompleted;

    private static readonly string addressableGroup = "Patterns";
    private static readonly string addressableExt = "txt";

    private static readonly PatternInfo[] patternInfos = {
        new PatternInfo("blinker","ブリンカー"),
        new PatternInfo("trafficlight","信号灯"),
        new PatternInfo("toad","ヒキガエル"),
        new PatternInfo("beacon","ビーコン"),
        new PatternInfo("clock","時計"),
        new PatternInfo("clock2","時計II"),
        new PatternInfo("pulsar","パルサー"),
        new PatternInfo("pinwheel","回転花火"),
        new PatternInfo("koksgalaxy","銀河"),
        new PatternInfo("pentadecathlon","ペンタデカスロン"),
        new PatternInfo("glider","グライダー"),
        new PatternInfo("lwss","宇宙船"),
        new PatternInfo("spider","クモ"),
        new PatternInfo("hammerhead","ハンマーヘッド"),
        new PatternInfo("robster","ロブスター"),
        new PatternInfo("acorn","どんぐり"),
        new PatternInfo("diehard","ダイハード"),
    };

    // private static readonly string[] patterns =
    // {
    //     "aircraftcarrier",
    //     "barge",
    //     "beacon",
    //     "beehive",
    //     "blinker",
    //     "block",
    //     "boat",
    //     "canoe",
    //     "clock",
    //     "clock2",
    //     "eater1",
    //     "glider",
    //     "gosperglidergun",
    //     "hertzoscillator",
    //     "honeyfarm",
    //     "hookwithtail",
    //     "hwss",
    //     "loaf",
    //     "longboat",
    //     "lwss",
    //     "mango",
    //     "mwss",
    //     "pentadecathlon",
    //     "pinwheel",
    //     "pond",
    //     "pulsar",
    //     "snake",
    //     "toad",
    //     "trafficlight",
    //     "tub",
    // };

    public static PatternLoadManager Instance { get; private set; }

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

    // Start is called before the first frame update
    void Start()
    {
        patternLoader = new PatternLoader(addressableGroup, addressableExt);
        StartCoroutine(LoadPattern());
    }

    private IEnumerator LoadPattern()
    {
        int index = 0;
        bool isEnd = false;

        while (!isEnd)
        {
            if (index < patternInfos.Length)
            {
                string name = patternInfos[index].filename;
                // Debug.Log(string.Format("LoadPattern() Load Start. index:{0} name:{1}", index, name));

                patternLoader.Load(name);
                yield return null;

                while (!patternLoader.IsLoadComplete(name))
                {
                    yield return null;
                }
                // Debug.Log(string.Format("LoadPattern() Load Finish. index:{0} name:{1}", index, name));

                index++;
            }
            else
            {
                isEnd = true;
            }
        }

        isLoadCompleted = true;
        Debug.Log("LoadPattern() Load Finished.");
    }

    private string FilenamneToPatternName(string filename)
    {
        foreach (var item in patternInfos)
        {
            if (item.filename == filename)
            {
                return item.patternName;
            }
        }

        return "";
    }



    public bool IsLoadCompleted()
    {
        return isLoadCompleted;
    }

    public List<string> GetFilenameList()
    {
        return patternLoader.GetPatternList();
    }

    public List<string> GetPatternList()
    {
        List<string> patternList = new List<string>();
        List<string> fileList = patternLoader.GetPatternList();

        foreach (var item in fileList)
        {
            string patternName = FilenamneToPatternName(item);
            patternList.Add(patternName);
        }

        return patternList;
    }

    public PatternLoader.PatternData GetData(string name)
    {
        return patternLoader.GetData(name);
    }



}
