using UnityEngine;

public class Finger_neo : MonoBehaviour
{
    public Transform targetObject; // マウスが追従するターゲットオブジェクト
    public RectTransform canvasRect; // キャンバスのRectTransform
    public Vector3 rangeMin; // 3次元範囲の最小値
    public Vector3 rangeMax; // 3次元範囲の最大値
    public float followSpeed = 5f; // 追従速度

    public Animator animator; // アニメーター
    public Camera mainCamera;
    private Vector3 lastTargetPosition; // 最後にターゲットに当たった座標

    void Start()
    {
        // メインカメラを取得
        mainCamera = Camera.main;
    }

    void Update()
    {
        // レイキャストでマウスがターゲットオブジェクトに当たっているかチェック
        IsMouseInRange();


    }

    private void IsMouseInRange()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // ターゲットオブジェクトとのレイキャストをチェック
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("当たっているオブジェクト: " + hit.transform.name); // レイキャストが当たったオブジェクトをログに出力
            if (hit.transform.CompareTag("Target"))
            {
                Vector3 hitPosition = hit.point;
                Vector3 targetPosition = hit.transform.position;
                FollowMouse(hitPosition);
                lastTargetPosition = targetPosition;

                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetTrigger("Touch");
                    Debug.Log("2aaaa"); // レイキャストが当たったオブジェクトをログに出力
                }
            }
            else if (hit.transform.CompareTag("BackGround"))
            {
                Vector3 hitPosition = hit.point;
                // 当たったバックグラウンドオブジェクトの位置と直近で当たったターゲットオブジェクトの位置の平均を取る
                Vector3 averagePosition = Vector3.Lerp(Vector3.Lerp(lastTargetPosition, hitPosition, 0.3f), hit.point, 0.3f);

                FollowMouse(averagePosition); 
            }
        }
    }


    private void FollowMouse(Vector3 mousePosition)
    {
        // キャンバスのスケールを取得
        Vector3 canvasScale = canvasRect.localScale;

        // キャンバスのローカル座標に変換
        Vector3 localMousePosition = canvasRect.InverseTransformPoint(mousePosition);

        // 3次元範囲内にマウス位置を制限
        float clampedX = Mathf.Clamp(localMousePosition.x, rangeMin.x * canvasScale.x, rangeMax.x * canvasScale.x);
        float clampedY = Mathf.Clamp(localMousePosition.y, rangeMin.y * canvasScale.y, rangeMax.y * canvasScale.y);
        float clampedZ = Mathf.Clamp(localMousePosition.z, rangeMin.z * canvasScale.z, rangeMax.z * canvasScale.z);

        // 制限された位置をワールド座標に変換
        Vector3 targetPosition = canvasRect.TransformPoint(new Vector3(clampedX, clampedY, clampedZ));

        // オブジェクトの位置を更新
        targetObject.position = Vector3.Lerp(targetObject.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100f);
        }

        // キャンバスのローカル座標に変換した範囲の最小値と最大値を計算
        Vector3 canvasRangeMin = canvasRect.InverseTransformPoint(rangeMin);
        Vector3 canvasRangeMax = canvasRect.InverseTransformPoint(rangeMax);

        // キャンバスのスケールを取得
        Vector3 canvasScale = canvasRect.localScale;

        // キャンバスのローカル座標に変換した範囲の中心座標を計算
        Vector3 canvasRangeCenter = (canvasRangeMin + canvasRangeMax) / 2f;

        // キャンバスのローカル座標に変換した範囲のサイズを計算
        Vector3 canvasRangeSize = canvasRangeMax - canvasRangeMin;

        // ギズモを描画
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(canvasRangeCenter, Vector3.Scale(canvasRangeSize, canvasScale));
    }
}
