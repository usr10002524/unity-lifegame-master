using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メッセージ領域に表示する文字列を管理するクラス
/// </summary>
public class WorldMessage : MonoBehaviour
{


    [SerializeField] private GameObject worldObject;

    private CellWorld cellWorld;
    private List<WorldMessagePredicate.Predicate> predicates;


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        predicates = new List<WorldMessagePredicate.Predicate>();
        cellWorld = worldObject.GetComponent<CellWorld>();

        predicates.Add(new WorldMessagePredicate.CellCount(cellWorld));
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        bool published = false;
        string message = "";

        // 各条件をチェック
        foreach (var item in predicates)
        {
            if (item.Check(Time.deltaTime))
            {
                // メッセージ発行条件を満たした
                published = true;
                message = item.GetMessage();
                break;
            }
        }

        if (published)
        {
            MessageManager.Instance.PushMessage(message);
            // メッセージ発行条件を満たした場合は各条件をリセット
            foreach (var item in predicates)
            {
                item.Reset();
            }
        }
    }
}
