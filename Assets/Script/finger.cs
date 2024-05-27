using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finger : MonoBehaviour
{
    public Texture2D cursorImage; // カーソルとして表示する画像
    public Vector2 hotSpot = Vector2.zero; // カーソルのホットスポット
    public float offsetY = 1f; // カーソルと画像の高さのオフセット
    public float offsetX = 1f; // カーソルと画像の高さのオフセット

    public float followSpeed = 5f; // 画像が追従する速度
    public Vector2 minXmaxX = new Vector2(-5f, 5f); // X座標の動かせる範囲
    public Vector2 minYmaxY = new Vector2(-5f, 5f); // Y座標の動かせる範囲

    private Vector3 targetPosition;

    void Start()
    {
        Cursor.SetCursor(cursorImage, hotSpot, CursorMode.Auto); // カーソルを設定
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition; // マウスカーソルのスクリーン座標を取得
        mousePos.z = -Camera.main.transform.position.z; // カメラからの距離を考慮してZ座標を設定
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(mousePos); // スクリーン座標をワールド座標に変換
        float clampedX = Mathf.Clamp(cursorPos.x, minXmaxX.x, minXmaxX.y); // X座標を制限
        float clampedY = Mathf.Clamp(cursorPos.y, minYmaxY.x, minYmaxY.y); // Y座標を制限
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f); // 制限された位置を目標位置として設定
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); // カーソルを滑らかに追従させる
    }
}