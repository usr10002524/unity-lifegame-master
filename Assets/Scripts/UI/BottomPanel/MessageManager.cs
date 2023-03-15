using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メッセージエリアのメッセージ管理クラス
/// このクラスで受け付けてコントロールクラスへ渡す
/// </summary>
public class MessageManager : MonoBehaviour
{
    [SerializeField] private GameObject messageController;

    private ScrollMessage scrollMessage;

    public static MessageManager Instance { get; private set; }


    /// <summary>
    /// メッセージの登録を行う。
    /// キューに積むタイプ
    /// </summary>
    /// <param name="message">メッセージのテキスト</param>
    public void PushMessage(string message)
    {
        if (scrollMessage != null)
        {
            scrollMessage.PushMessage(message);
        }
    }

    /// <summary>
    /// メッセージの登録を行う。
    /// 現在のメッセージに優先して上書きするタイプ。
    /// </summary>
    /// <param name="message">メッセージのテキスト</param>
    public void InsertMessage(string message)
    {
        if (scrollMessage != null)
        {
            scrollMessage.InsertMessage(message);
        }
    }

    /// <summary>
    /// メッセージ表示中か確認する。
    /// </summary>
    /// <returns>メッセージ表示中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsShowMessage()
    {
        if (scrollMessage == null)
        {
            return false;
        }
        else
        {
            return scrollMessage.IsShowMesssage();
        }
    }

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        if (messageController != null)
        {
            scrollMessage = messageController.GetComponent<ScrollMessage>();
        }
    }
}
