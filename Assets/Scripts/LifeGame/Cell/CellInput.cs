using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// セルに対する入力を処理するクラス
/// </summary>
public class CellInput : MonoBehaviour
{
    private bool mouseDown;
    private Vector2Int position;

    private static bool anyCellMouseDown;

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        mouseDown = false;
        position = new Vector2Int(-1, -1);
    }

    /// <summary>
    /// セルの位置を設定する
    /// </summary>
    /// <param name="pos">セルの位置</param>
    public void SetPosition(Vector2Int pos)
    {
        position = pos;
    }

    /// <summary>
    /// セルの位置を取得する
    /// </summary>
    /// <returns>セルの位置</returns>
    public Vector2Int GetPosition()
    {
        return position;
    }

    /// <summary>
    /// セルに入力があったか確認する
    /// </summary>
    /// <returns>セルに入力があった場合はtrue、そうでない場合はfalseを返す</returns>
    public bool IsClicked()
    {
        bool clicked = mouseDown;
        mouseDown = false;
        return clicked;
    }

    /// <summary>
    /// GameObject に対してマウスが押下された場合の処理
    /// </summary>
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!GameController.Instance.IsEditMode())
        {
            return;
        }

        if (GameController.Instance.IsPatternPasteMode())
        {
            return;
        }

        // Debug.Log(string.Format("Cell Clicked. x:{0} y:{1}", position.x, position.y));
        mouseDown = true;
        anyCellMouseDown = true;
    }

    /// <summary>
    /// GameObject内でマウスが離された場合の処理
    /// </summary>
    private void OnMouseUp()
    {
        anyCellMouseDown = false;
    }

    /// <summary>
    /// GameObject内にマウスが入った場合の処理
    /// </summary>
    private void OnMouseEnter()
    {
        if (anyCellMouseDown)
        {
            mouseDown = true;
        }
    }

    /// <summary>
    /// GameObjectからマウスが出た場合の処理
    /// </summary>
    public void SetMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!GameController.Instance.IsEditMode())
        {
            return;
        }

        if (anyCellMouseDown)
        {
            mouseDown = true;
        }
    }
}
