using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// カーソルの処理を行うクラス
/// </summary>
public class Cursor : MonoBehaviour
{
    private static readonly string[] raycastTargets = { "Cell" };

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        UpdateCursor();
    }

    /// <summary>
    /// カーソルの更新処理
    /// </summary>
    private void UpdateCursor()
    {
        // レイを投げる原点と方向
        Camera mainCamera = Camera.main;
        Vector2 origin = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = Vector2.zero;
        // レイの距離
        float distance = 20.0f;

        // Debug.Log(string.Format("ray position x:{0} y:{1}", origin.x, origin.y));
        // Debug.Log(string.Format("ray direction x:{0} y:{1}", direction.x, direction.y));


        // レイヤーマスクを作成
        int layerMask = LayerMask.GetMask(raycastTargets);
        // Debug.Log(string.Format("layerMask:{0}", layerMask));

        // レイを投げる
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        if (hit.collider != null)
        {
            // レイが当たれば対象の位置にカーソルを動かす
            transform.position = hit.transform.position;
            // Debug.Log(string.Format("ray hit. name:{0}", hit.collider.name));
        }
        else
        {
            // 対象がいないのでファークリップの向こう移動させて表示されなくする
            transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.farClipPlane * 2);
            // Debug.Log(string.Format("ray not hit. "));
        }
    }
}
