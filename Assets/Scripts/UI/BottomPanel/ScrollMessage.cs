using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// メッセージのスクロール処理を行うクラス
/// </summary>
public class ScrollMessage : MonoBehaviour
{
    [SerializeField] private Vector2 startPostion;
    [SerializeField] private Vector2 endPostion;
    [SerializeField] private float showDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private AnimationCurve curve;

    private List<string> messages = new List<string>();
    private string messageInsert;
    private bool isInsertMessage;
    private List<string> currentMessage = new List<string>();
    private string showMessage;
    private int messageIndex;
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;
    private float animationTimer;
    private Coroutine moveMessageCoroutine;

    private enum Step
    {
        Start,
        Show,
        ShowWait,
        Move,
        MoveWait,
        Next,
        End,
    }


    /// <summary>
    /// メッセージを登録する。
    /// </summary>
    /// <param name="message">表示するメッセージ</param>
    public void PushMessage(string message)
    {
        messages.Add(message);
    }

    /// <summary>
    /// メッセージを挿入する。
    /// 表示中メッセージがあっても上書きして表示する。
    /// 複数回登録されば場合は、最後に登録されたものが有効となる。
    /// </summary>
    /// <param name="message"></param>
    public void InsertMessage(string message)
    {
        messageInsert = message;
        isInsertMessage = true;
    }

    /// <summary>
    /// メッセージ表示中かどうか確認する。
    /// </summary>
    /// <returns>メッセージ表示中の場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsShowMesssage()
    {
        return (moveMessageCoroutine != null);
    }


    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        // 挿入されたメッセージがあった場合は、上書きして表示する
        if (isInsertMessage)
        {
            isInsertMessage = false;
            // 動作中のメッセージがあれば停止
            if (moveMessageCoroutine != null)
            {
                StopCoroutine(moveMessageCoroutine);
            }

            // メッセージをセットしてスクロールを開始する
            SetupInsertMessage();
            moveMessageCoroutine = StartCoroutine(MoveMessage());
        }

        if (moveMessageCoroutine == null)
        {
            // 登録されたメッセージがあれば表示する
            if (messages.Count > 0)
            {
                SetupMessage();
                moveMessageCoroutine = StartCoroutine(MoveMessage());
            }
        }
        else
        {
            ;
        }
    }

    /// <summary>
    /// メッセージのスクロールを行う。
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator MoveMessage()
    {
        bool isEnd = false;
        Step step = Step.Start;
        Vector2 beginPos = startPostion;
        Vector2 endPos = endPostion;

        messageIndex = 0;
        showMessage = "";
        animationTimer = 0.0f;

        while (!isEnd)
        {
            yield return null;

            Step oldStep = step;

            switch (step)
            {
                case Step.Start: StepStart(ref step); break;
                case Step.Show: StepShow(ref step); break;
                case Step.ShowWait: StepShowWait(ref step); break;
                case Step.Move: StepMove(ref step); break;
                case Step.MoveWait: StepMoveWait(ref step); break;
                case Step.Next: StepNext(ref step); break;
                case Step.End:
                default:
                    StepEnd();
                    isEnd = true;
                    break;
            }

            if (oldStep != step)
            {
                // Debug.Log(string.Format("MoveMessage step {0} -> {1}", oldStep, step));
            }
        }

        moveMessageCoroutine = null;
    }

    /// <summary>
    /// メッセージ表示の初期化ステップ
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepStart(ref Step step)
    {
        // パラメータ初期化
        messageIndex = 0;
        showMessage = "";
        animationTimer = 0.0f;

        step = Step.Show;
    }

    /// <summary>
    /// メッセージを表示する
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepShow(ref Step step)
    {
        // 表示するメッセージをセット
        showMessage = "";
        if (messageIndex < currentMessage.Count)
        {
            showMessage = currentMessage[messageIndex];
        }
        textMesh.text = showMessage;
        // 表示位置をリセット
        rectTransform.anchoredPosition = startPostion;
        // タイマーをリセット
        animationTimer = 0.0f;

        step = Step.ShowWait;
    }

    /// <summary>
    /// メッセージ表示中ステップ
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepShowWait(ref Step step)
    {
        animationTimer += Time.deltaTime;
        if (animationTimer < showDuration)
        {
            return;
        }

        step = Step.Move;
    }

    /// <summary>
    /// メッセージをスクロールさせる
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepMove(ref Step step)
    {
        // 次の行があればメッセージに追加する
        int nextIndex = messageIndex + 1;
        if (nextIndex < currentMessage.Count)
        {
            showMessage += "\n";    //改行文字をまず追加
            showMessage += currentMessage[nextIndex];
        }
        textMesh.text = showMessage;

        // 表示位置をリセット
        rectTransform.anchoredPosition = startPostion;
        // タイマーをリセット
        animationTimer = 0.0f;

        step = Step.MoveWait;
    }

    /// <summary>
    /// メッセージスクロール中ステップ
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepMoveWait(ref Step step)
    {
        //curveを適応させ、移動処理を行う
        animationTimer += Time.deltaTime;
        float time = Mathf.Clamp(animationTimer / moveDuration, 0.0f, 1.0f);
        float t = curve.Evaluate(time);

        Vector2 position = Vector3.Lerp(startPostion, endPostion, t);
        rectTransform.anchoredPosition = position;

        if (animationTimer < moveDuration)
        {
            return;
        }

        step = Step.Next;
    }

    /// <summary>
    /// 次のメッセージに切り替える
    /// </summary>
    /// <param name="step">ステップ</param>
    private void StepNext(ref Step step)
    {
        messageIndex++;
        if (messageIndex < currentMessage.Count)
        {
            //まだ次があるので、表示を行う
            step = Step.Show;
        }
        else
        {
            //全部表示したので終了
            step = Step.End;
        }
    }

    /// <summary>
    /// 終了ステップ
    /// </summary>
    private void StepEnd()
    {
        // 後片付け
        textMesh.text = "";
        rectTransform.anchoredPosition = startPostion;
    }

    /// <summary>
    /// 挿入メッセージの表示準備
    /// </summary>
    public void SetupInsertMessage()
    {
        SetupMessage(messageInsert);
        messageInsert = "";
    }

    /// <summary>
    /// 通常メッセージの表示準備
    /// </summary>
    private void SetupMessage()
    {
        // リストから最初の文字列を取り出す
        string message = messages[0];
        messages.RemoveAt(0);

        SetupMessage(message);
    }

    /// <summary>
    /// メッセージを表示する準備を行う
    /// </summary>
    /// <param name="message">メッセージ文字列</param>
    private void SetupMessage(string message)
    {
        currentMessage.Clear();

        // 表示範囲の文字列を切り出す
        TMP_TextInfo info = textMesh.GetTextInfo(message);
        for (int i = 0; i < info.lineCount; i++)
        {
            TMP_LineInfo lineInfo = info.lineInfo[i];
            string line = message.Substring(lineInfo.firstCharacterIndex, lineInfo.characterCount);
            currentMessage.Add(line);
        }
    }
}
