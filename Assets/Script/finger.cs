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

    private const float LONG_PRESS_THRESHOLD = 0.2f; // �������̎��Ԃ�臒l
    private float pressTime = 0f; // �N���b�N���n�܂�������
    private bool isLongPress = false; // ���������

    void Start()
    {
        Cursor.SetCursor(cursorImage, hotSpot, CursorMode.Auto);
        originalPosition = transform.position;
        originalMinXmaxX = new Vector2(4f, 5f);
        originalMinYmaxY = new Vector2(-3f, 3f);
    }

    void Update()
    {
        // �}�E�X�̍��N���b�N�����o���ĉ����n�߂̎��Ԃ��L�^
        if (Input.GetMouseButtonDown(0))
        {
            pressTime = Time.time;
            isLongPress = false;
        }

        // �}�E�X�̍��{�^���������ꑱ���Ă���ꍇ�A�����������o
        if (Input.GetMouseButton(0))
        {
            if (Time.time - pressTime >= LONG_PRESS_THRESHOLD)
            {
                isLongPress = true;
            }
        }

        // �}�E�X�{�^���������ꂽ��A��������Ԃ����Z�b�g
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
            // �ړ��p�R���[�`�������s���łȂ��ꍇ�A�ړ����J�n
            if (Input.GetMouseButtonDown(0) && !isMoving)
            {
                isMovingCoroutine = true;
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition.z = 0f; // 2D�Ȃ̂�z���͌Œ�

                StartCoroutine(MoveToTarget(targetPosition));
            }
        }



        // �ʏ�̓������s��
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

            // �ړ��R���[�`�����I���������Ƃ�����
            isMovingCoroutine = false;
            // ���̈ʒu�ɖ߂�����ɒʏ�̓�����ĊJ
            isMoving = false;
        }
    }
}
