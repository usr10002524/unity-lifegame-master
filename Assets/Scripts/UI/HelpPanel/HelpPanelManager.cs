using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ヘルプパネルの状態を外部から参照するためのクラス
/// </summary>
public class HelpPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject helpPanel;

    private HelpPanelController controller;

    public static HelpPanelManager Instance { get; private set; }

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
        if (helpPanel != null)
        {
            controller = helpPanel.GetComponent<HelpPanelController>();
        }
    }

    /// <summary>
    /// ヘルプパネルが開いているかどうかを確認する。
    /// </summary>
    /// <returns>ヘルプパネルが開いている場合はtrue、そうでない場合はfalseを返す。</returns>
    public bool IsActive()
    {
        if (controller == null)
        {
            return false;
        }

        return (controller.IsMoving() || controller.IsOpen());
    }
}
