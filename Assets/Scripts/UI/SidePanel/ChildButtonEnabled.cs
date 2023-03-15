using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// containerObject に登録されたオブジェクト以下にあるボタンに対して、
/// 一括で有効、無効の切り替えを制御するクラス。
/// </summary>
public class ChildButtonEnabled : MonoBehaviour
{
    [SerializeField] private GameObject containerObject;

    private SidePanelContainer container;
    private bool lastMoving;
    private List<Button> buttons;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        container = containerObject.GetComponent<SidePanelContainer>();

        buttons = new List<Button>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Button b = child.GetComponent<Button>();
            if (b != null)
            {
                buttons.Add(b);
            }
        }

        lastMoving = false;
        Enabled();
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        // コンテナが移動中かどうか確認
        bool isMoving = false;
        if (container != null)
        {
            isMoving = container.IsMoving();
        }

        // 移動中であればボタンを無効に、そうでなければ有効にする
        if (lastMoving != isMoving)
        {
            lastMoving = isMoving;
            if (isMoving)
            {
                Disabled();
            }
            else
            {
                Enabled();
            }
        }
    }

    /// <summary>
    /// ボタンを無効にする
    /// </summary>
    private void Disabled()
    {
        foreach (var item in buttons)
        {
            item.enabled = false;
        }
        // Debug.Log(string.Format("ChildButtonEnabled.Disabled()"));
    }

    /// <summary>
    /// ボタンを有効にする
    /// </summary>
    private void Enabled()
    {
        foreach (var item in buttons)
        {
            item.enabled = true;
        }
        // Debug.Log(string.Format("ChildButtonEnabled.Enabled()"));
    }
}
