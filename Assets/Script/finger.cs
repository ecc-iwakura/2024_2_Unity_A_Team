using System.Collections;
using UnityEngine;

public class finger : MonoBehaviour
{
    // Cursor settings
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
    private bool isLongPress = false; // 長押し状態かどうか
    private bool isMovingCoroutine = false; // 移動コルーチンが実行中かどうか
    private Vector3 targetPosition; // ターゲット位置
    private const float LONG_PRESS_THRESHOLD = 0.2f; // 長押しの時間の閾値

    void Start()
    {
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
    }

    // マウス入力を処理する
    private void HandleMouseInput()
    {
        if (IsMouseInRange())
        {
            if (Input.GetMouseButtonDown(0))
            {
                pressTime = Time.time;
                isLongPress = false;
            }

            if (Input.GetMouseButton(0) && Time.time - pressTime >= LONG_PRESS_THRESHOLD)
            {
                isLongPress = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                ResetLongPressState();
            }

            if (Input.GetMouseButtonDown(0) && !isMovingCoroutine)
            {
                targetPosition = GetMouseWorldPosition();
                StartCoroutine(MoveToTarget(targetPosition));
            }
        }

    }

    private bool IsMouseInRange()
    {
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x >= MouseMin.x && mousePos.x <= MouseMax.x &&
               mousePos.y >= MouseMin.y && mousePos.y <= MouseMax.y;
    }
    // 長押し状態をリセットする
    private void ResetLongPressState()
    {
        isLongPress = false;
        minXmaxX = originalMinXmaxX;
        minYmaxY = originalMinYmaxY;
        followSpeed = clickedFollowSpeed;
    }

    // マウスを追従する
    private void FollowMouse()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        float clampedX = Mathf.Clamp(mousePos.x, minXmaxX.x, minXmaxX.y);
        float clampedY = Mathf.Clamp(mousePos.y, minYmaxY.x, minYmaxY.y);
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // マウスのワールド座標を取得する
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
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
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(MouseMin.x, MouseMin.y, -Camera.main.transform.position.z));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(MouseMax.x, MouseMax.y, -Camera.main.transform.position.z));
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, topRight.z);

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
    }
}
