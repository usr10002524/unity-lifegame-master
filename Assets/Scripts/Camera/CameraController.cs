using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// マウスによるカメラのコントロールを行うクラス
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)]
    private float wheelSpeed = 1f;

    [SerializeField, Range(0.1f, 10f)]
    private float moveSpeed = 0.3f;

    // [SerializeField, Range(0.1f, 10f)]
    // private float rotateSpeed = 0.3f;

    [SerializeField]
    private float orthoMin = 10;

    [SerializeField]
    private float orthoMax = 40;

    [SerializeField]
    private GameObject cameraConfiner;

    private CinemachineVirtualCamera vitualCamera;
    private CinemachineConfiner2D confiner;
    private Vector3 preMousePos;
    private Vector3 origin;

    /// <summary>
    /// 正射投影で表示するサイズ応じてカメラのパラメータを設定する
    /// </summary>
    /// <param name="orthoSize">正射投影で表示するサイズ</param>
    public void Setup(float orthoSize)
    {
        //カメラの移動範囲を設定する
        PolygonCollider2D collider = cameraConfiner.GetComponent<PolygonCollider2D>();
        if (collider != null)
        {
            float h = orthoSize;
            float w = h * (float)Screen.width / (float)Screen.height;

            Vector2[] newPoints = new Vector2[4]{
                new Vector2(-w, h),  //Left - Top
                new Vector2(w, h),  //Right - Top
                new Vector2(w, -h),   //Right - Bottom
                new Vector2(-w, -h),   //Left - Bottom
            };

            collider.SetPath(0, newPoints);
        }

        //カメラの表示範囲を設定する
        CinemachineVirtualCamera camera = vitualCamera.GetComponent<CinemachineVirtualCamera>();
        if (camera != null)
        {
            camera.m_Lens.OrthographicSize = orthoSize;
            orthoMax = orthoSize;

            //カメラのパラメータを変更したら InvalidateCache を呼ぶ必要がある。
            if (confiner != null)
            {
                confiner.InvalidateCache();
            }
        }
    }


    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        origin = transform.position;

        vitualCamera = GetComponent<CinemachineVirtualCamera>();
        if (vitualCamera != null)
        {
            confiner = vitualCamera.GetComponent<CinemachineConfiner2D>();
        }
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        MouseUpdate();
    }

    /// <summary>
    /// マウス入力に応じた更新処理
    /// </summary>
    private void MouseUpdate()
    {
        if (!IsUpdate())
        {
            return;
        }

        // マウスホイール
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0.0f)
        {
            MouseWheel(scrollWheel);
        }

        // マウス右ボタンドラッグ処理を行うために、
        // ボタンが謳歌されたときのマウスの座標を保持しておく
        if (Input.GetMouseButtonDown(1))
        {
            preMousePos = Input.mousePosition;
        }

        // マウスドラッグ処理
        MouseDrag(Input.mousePosition);

        // ボタン入力処理
        if (Input.GetMouseButtonUp(1))
        {
            AdjustPosition();
        }
        if (Input.GetMouseButtonUp(2))
        {
            ResetCamera();
        }
    }

    /// <summary>
    /// マウスホイール入力に対する処理
    /// </summary>
    /// <param name="delta">加速度(0.0-1.0)</param>
    private void MouseWheel(float delta)
    {
        if (vitualCamera == null)
        {
            return;
        }
        if (confiner == null)
        {
            return;
        }
        if (!IsMousePointerInScreen())
        {
            return;
        }

        // 入力に応じて表示範囲を変更する（カメラの拡大縮小）
        float ortho = vitualCamera.m_Lens.OrthographicSize;
        ortho += (-delta * wheelSpeed);
        ortho = Mathf.Clamp(ortho, orthoMin, orthoMax);

        if (ortho != vitualCamera.m_Lens.OrthographicSize)
        {
            vitualCamera.m_Lens.OrthographicSize = ortho;
            confiner.InvalidateCache();
        }

        return;
    }

    /// <summary>
    /// マウスドラッグに対する処理
    /// </summary>
    /// <param name="mousePos">ボタン押下位置</param>
    private void MouseDrag(Vector3 mousePos)
    {
        Vector3 diff = mousePos - preMousePos;

        if (diff.magnitude < Vector3.kEpsilon)
        {
            return; //ほぼ0のときは無視
        }

        if (Input.GetMouseButton(2))
        {
        }
        else if (Input.GetMouseButton(1))
        {
            //マウス右ボタンドラッグでカメラを平行移動させる
            transform.Translate(-diff * Time.deltaTime * moveSpeed);
        }

        preMousePos = mousePos;
    }

    /// <summary>
    /// カメラを各軸に沿って回転させる
    /// </summary>
    /// <param name="angle">軸ごとの回転角度</param>
    public void CameraRotate(Vector2 angle)
    {
        transform.RotateAround(transform.position, transform.right, angle.x);
        transform.RotateAround(transform.position, Vector3.up, angle.y);
    }

    /// <summary>
    /// メインカメラの位置を自身に反映させる。
    /// CameraConfinerの設定で範囲外には出ないが、座標は範囲外に値を取ることがあるので、
    /// メインカメラの位置に合わせておく。
    /// </summary>
    private void AdjustPosition()
    {
        transform.position = new Vector3(Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z);
    }

    /// <summary>
    /// カメラの位置を初期位置に戻す
    /// </summary>
    private void ResetCamera()
    {
        transform.position = origin;
        if (vitualCamera != null && confiner != null)
        {
            vitualCamera.m_Lens.OrthographicSize = orthoMax;
            confiner.InvalidateCache();
        }
    }

    /// <summary>
    /// マウスポインタがスクリーン内にあるか確認する。
    /// </summary>
    /// <returns>trueの場合はスクリーン内、faseの場合はスクリーン外</returns>
    private bool IsMousePointerInScreen()
    {
        Vector3 pos = Input.mousePosition;
        if (pos.x < 0) { return false; }
        if (pos.x > Screen.width) { return false; }
        if (pos.y < 0) { return false; }
        if (pos.y > Screen.height) { return false; }

        return true;
    }

    /// <summary>
    /// アップデート処理を行うかどうかをチェックする。
    /// </summary>
    /// <returns>アップデートを行う場合はtrue、そうでない場合はfalseを返す</returns>
    private bool IsUpdate()
    {
        // 画面上にウィンドウが表示されているときはカメラの変更を行わない
        if (HelpPanelManager.Instance.IsActive()) { return false; }
        if (PatternPreviewManager.Instance.IsActive()) { return false; }

        return true;
    }
}
