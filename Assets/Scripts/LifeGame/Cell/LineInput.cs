using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各セルの入力を OnMouseDown, OnMouseEnter でみているが、
/// マウスの動きが早いとイベントが取れないことがある。
/// そのため、フレーム間でのマウス座標に対してレイを飛ばし、
/// ヒットしたセルに対して、OnMouseEnter 相当の処理を行うようにする。
/// </summary>
public class LineInput : MonoBehaviour
{
    [SerializeField] List<Vector2> inputList;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        inputList = new List<Vector2>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Camera mainCamera = Camera.main;
            Vector2 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            inputList.Add(position);
            if (inputList.Count >= 3)
            {
                inputList.RemoveAt(0);
            }

            if (inputList.Count == 2)
            {
                // 2点間の距離、方向を算出する
                Vector2 start = inputList[0];
                Vector2 end = inputList[1];
                Vector2 dir = end - start;
                float distance = dir.magnitude;
                dir.Normalize();

                string[] raycastTargets = { "Cell" };
                int layerMask = LayerMask.GetMask(raycastTargets);

                RaycastHit2D[] hits = Physics2D.RaycastAll(start, dir, distance, layerMask);
                foreach (var item in hits)
                {
                    GameObject gameObject = item.collider.gameObject;
                    CellInput cellInput = gameObject.GetComponent<CellInput>();
                    if (cellInput != null)
                    {
                        cellInput.SetMouseDown();
                    }
                }
            }
        }
        else
        {
            // マウスが離されたらリセット
            inputList.Clear();
        }
    }
}
