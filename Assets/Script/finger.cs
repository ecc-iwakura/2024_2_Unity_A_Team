using System.Collections;
using UnityEngine;

public class finger : MonoBehaviour
{
    // Cursor settings
    [SerializeField] private RectTransform mainCanvasRect; // メインキャンバスのRectTransform
    [SerializeField] private Texture2D cursorImage; // カーソル画像
    [SerializeField] private Vector2 hotSpot = Vector2.zero; // カーソルのホットスポット

    // Movement settings
    [SerializeField] private float offsetX = 1f; // X方向のオフセット
    [SerializeField] private float offsetY = 1f; // Y方向のオフセット
    [SerializeField] private float followSpeed = 5f; // 通常の追従速度
    [SerializeField] private float clickedFollowSpeed = 30f; // クリック時の追従速度
    [SerializeField] private Vector2 minXmaxX = new Vector2(-5f, 5f); // X方向の移動範囲
    [SerializeField] private Vector2 minYmaxY = new Vector2(-5f, 5f); // Y方向の移動範囲

    // Mouse range settings
    [SerializeField] private Vector2 MouseMin = new Vector2(0f, 0f); // マウスのX, Yの最小値
    [SerializeField] private Vector2 MouseMax = new Vector2(Screen.width, Screen.height); // マウスのX, Yの最大値

    private Vector3 originalPosition; // 初期位置
    private Vector2 originalMinXmaxX; // X方向の元の移動範囲
    private Vector2 originalMinYmaxY; // Y方向の元の移動範囲
    private float pressTime = 0f; // クリックが始まった時間
    private bool isMovingCoroutine = false; // 移動コルーチンが実行中かどうか
    private Vector3 targetPosition; // ターゲット位置
    private const float LONG_PRESS_THRESHOLD = 0.2f; // 長押しの時間の閾値

    void Start()
    {
        mainCanvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        Cursor.SetCursor(cursorImage, hotSpot, CursorMode.Auto);
        originalPosition = transform.position;
        originalMinXmaxX = minXmaxX;
        originalMinYmaxY = minYmaxY;
    }

    void Update()
    {
        HandleMouseInput();
        if (!isMovingCoroutine)
        {
            FollowMouse();
        }

        float distance = Vector3.Distance(transform.position, targetPosition);
        float targetRotation = distance * 30f;

        // 現在の回転角度と目標の回転角度の間を線形補間して滑らかな回転を実現
        Quaternion currentRotation = transform.rotation;
        Quaternion targetQuaternion = Quaternion.Euler(0f, 0f, targetRotation);
        transform.rotation = Quaternion.Lerp(currentRotation, targetQuaternion, Time.deltaTime * followSpeed);
    }

    // マウス入力を処理する
    private void HandleMouseInput()
    {
        if (IsMouseInRange())
        {
            UnityEngine.Debug.Log("マウスの範囲内ですよ～");

            if (Input.GetMouseButtonDown(0) && !isMovingCoroutine)
            {
                targetPosition = GetMouseCanvasPosition();
                StartCoroutine(MoveToTarget(targetPosition));
            }
        }
        else { UnityEngine.Debug.Log("マウスの範囲外だよばか！"); }

    }

    private bool IsMouseInRange()
    {
        // マウスの位置をローカル座標に変換
        Vector2 localMousePosition = GetMouseCanvasPosition();

        // キャンバスのローカル座標に変換したマウスの範囲を取得
        Vector2 canvasMouseMin = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMin.x, MouseMin.y, Camera.main.WorldToScreenPoint(transform.position).z)));
        Vector2 canvasMouseMax = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMax.x, MouseMax.y, Camera.main.WorldToScreenPoint(transform.position).z)));

        // キャンバスのスケールを取得してギズモを拡大縮小
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // キャンバスのローカル座標を基準にして線を描画
        Vector2 bottomLeft = mainCanvasRect.TransformPoint(canvasMouseMin);
        Vector2 topRight = mainCanvasRect.TransformPoint(canvasMouseMax);

        // ローカル座標系での範囲チェック
        bool isInRange = localMousePosition.x >= bottomLeft.x && localMousePosition.x <= topRight.x &&
                         localMousePosition.y >= bottomLeft.y && localMousePosition.y <= topRight.y;

        // 範囲の位置とマウスの位置をログ出力
        Debug.Log("範囲左下: " + bottomLeft + ", マウスの位置: " + localMousePosition + ", 範囲右上: " + topRight);

        return isInRange;
    }


    // マウスを追従する
    private void FollowMouse()
    {
        // マウスのキャンバス内の2D位置を取得
        Vector2 mousePos = GetMouseCanvasPosition();

        // キャンバスのローカル座標に変換した範囲を取得
        Vector2 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(new Vector2(minXmaxX.x, minXmaxX.y));
        Vector2 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(new Vector2(minYmaxY.x, minYmaxY.y));

        // キャンバスのスケールを適用
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // キャンバスのローカル座標を基準にして範囲をクランプ
        Vector2 bottomLeftGizmo = mainCanvasRect.TransformPoint(canvasMouseMinGizmo);
        Vector2 topRightGizmo = mainCanvasRect.TransformPoint(canvasMouseMaxGizmo);

        float clampedX = Mathf.Clamp(mousePos.x, topRightGizmo.x, bottomLeftGizmo.x);
        float clampedY = Mathf.Clamp(mousePos.y, topRightGizmo.y, bottomLeftGizmo.y);

        // 目標位置を設定して、オブジェクトを追従
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // マウスのキャンバス座標を取得するメソッド
    private Vector2 GetMouseCanvasPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        // マウスのスクリーン座標をワールド座標に変換、オブジェクトのZ軸位置を考慮
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        // ワールド座標をキャンバスのローカル座標に変換
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return mainCanvasRect.InverseTransformPoint(worldPos);
    }

    // 指定されたターゲット位置に移動するコルーチン
    IEnumerator MoveToTarget(Vector3 target)
    {
        isMovingCoroutine = true;
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, followSpeed * Time.deltaTime);
            yield return null;
        }
        isMovingCoroutine = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // ローカルZ軸を取得
        float localZ = transform.localPosition.z;

        // キャンバスのローカル座標に変換
        Vector2 canvasMouseMin = mainCanvasRect.InverseTransformPoint(new Vector3(MouseMin.x, MouseMin.y, localZ));
        Vector2 canvasMouseMax = mainCanvasRect.InverseTransformPoint(new Vector3(MouseMax.x, MouseMax.y, localZ));

        // キャンバスのスケールを取得してギズモを拡大縮小
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // キャンバスのローカル座標を基準にして線を描画
        Vector3 bottomLeft = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMin.x, canvasMouseMin.y, localZ));
        Vector3 topRight = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMax.x, canvasMouseMax.y, localZ));
        Vector3 topLeft = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMin.x, canvasMouseMax.y, localZ));
        Vector3 bottomRight = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMax.x, canvasMouseMin.y, localZ));

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        Gizmos.color = Color.blue;

        // キャンバスのローカル座標に変換したマウスの範囲を取得
        Vector2 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(new Vector3(minXmaxX.x, minXmaxX.y, localZ));
        Vector2 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(new Vector3(minYmaxY.x, minYmaxY.y, localZ));

        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // キャンバスのローカル座標を基準にして線を描画
        Vector3 bottomLeftGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMinGizmo.x, canvasMouseMinGizmo.y, localZ));
        Vector3 topRightGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMaxGizmo.x, canvasMouseMaxGizmo.y, localZ));
        Vector3 topLeftGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMinGizmo.x, canvasMouseMaxGizmo.y, localZ));
        Vector3 bottomRightGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMaxGizmo.x, canvasMouseMinGizmo.y, localZ));

        Gizmos.DrawLine(bottomLeftGizmo, topLeftGizmo);
        Gizmos.DrawLine(topLeftGizmo, topRightGizmo);
        Gizmos.DrawLine(topRightGizmo, bottomRightGizmo);
        Gizmos.DrawLine(bottomRightGizmo, bottomLeftGizmo);
    }




}
