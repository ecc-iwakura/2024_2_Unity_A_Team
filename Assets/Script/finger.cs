using System.Collections;
using UnityEngine;

public class finger : MonoBehaviour
{
    public Texture2D cursorImage;
    public Vector2 hotSpot = Vector2.zero;
    public float offsetY = 1f;
    public float offsetX = 1f;
    private Vector3 originalPosition;
    private bool isMoving = false;
    public float followSpeed = 5f;
    public Vector2 minXmaxX = new Vector2(-5f, 5f);
    public Vector2 minYmaxY = new Vector2(-5f, 5f);
    private float speed = 5f;
    private Vector2 originalMinXmaxX;
    private Vector2 originalMinYmaxY;
    private bool isClicked = false;
    public float clickedFollowSpeed = 10f;
    private Vector3 targetPosition;
    private bool isMovingCoroutine = false;
    private bool isMovingToClick = false;
    private Vector3 originalClickPosition;

    private const float LONG_PRESS_THRESHOLD = 0.2f; // 長押しの時間の閾値
    private float pressTime = 0f; // クリックが始まった時間
    private bool isLongPress = false; // 長押し状態

    void Start()
    {
        Cursor.SetCursor(cursorImage, hotSpot, CursorMode.Auto);
        originalPosition = transform.position;
        originalMinXmaxX = new Vector2(4f, 5f);
        originalMinYmaxY = new Vector2(-3f, 3f);
    }

    void Update()
    {
        // マウスの左クリックを検出して押し始めの時間を記録
        if (Input.GetMouseButtonDown(0))
        {
            pressTime = Time.time;
            isLongPress = false;
        }

        // マウスの左ボタンが押され続けている場合、長押しを検出
        if (Input.GetMouseButton(0))
        {
            if (Time.time - pressTime >= LONG_PRESS_THRESHOLD)
            {
                isLongPress = true;
            }
        }

        // マウスボタンが離されたら、長押し状態をリセット
        if (Input.GetMouseButtonUp(0))
        {
            isLongPress = false;
            isClicked = false;
            minXmaxX = originalMinXmaxX;
            minYmaxY = originalMinYmaxY;
            followSpeed = speed;
            isMovingToClick = false;
        }



        if (!isMovingCoroutine)
        {
            // 移動用コルーチンが実行中でない場合、移動を開始
            if (Input.GetMouseButtonDown(0) && !isMoving)
            {
                isMovingCoroutine = true;
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0f; // 2Dなのでz軸は固定

                StartCoroutine(MoveToTarget(targetPosition));
            }
        }



        // 通常の動きを行う
        if (!isMovingCoroutine)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -Camera.main.transform.position.z;
            Vector2 cursorPos = Camera.main.ScreenToWorldPoint(mousePos);
            float clampedX = Mathf.Clamp(cursorPos.x, minXmaxX.x, minXmaxX.y);
            float clampedY = Mathf.Clamp(cursorPos.y, minYmaxY.x, minYmaxY.y);
            targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f);
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }

    IEnumerator MoveToTarget(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        if (isMovingToClick)
        {
            while (Vector3.Distance(transform.position, originalClickPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalClickPosition, speed * Time.deltaTime);
                yield return null;
            }

            isMovingCoroutine = false;
        }
        else
        {
            while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, speed * Time.deltaTime);
                yield return null;
            }

            // 移動コルーチンが終了したことを示す
            isMovingCoroutine = false;
            // 元の位置に戻った後に通常の動作を再開
            isMoving = false;
        }
    }
}
