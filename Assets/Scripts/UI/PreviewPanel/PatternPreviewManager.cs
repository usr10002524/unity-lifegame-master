using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPreviewManager : MonoBehaviour
{
    [SerializeField] private GameObject previewPanel;

    private PatternPreview controller;

    public static PatternPreviewManager Instance { get; private set; }

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
        if (previewPanel != null)
        {
            controller = previewPanel.GetComponent<PatternPreview>();
        }
    }

    /// <summary>
    /// プレビューパネルが開いているかどうかを確認する。
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
