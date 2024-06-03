using System.Collections;
using UnityEngine;

public class finger : MonoBehaviour
{
    // Cursor settings
    [SerializeField] private Texture2D cursorImage; // �J�[�\���摜
    [SerializeField] private Vector2 hotSpot = Vector2.zero; // �J�[�\���̃z�b�g�X�|�b�g

    // Movement settings
    [SerializeField] private float offsetX = 1f; // X�����̃I�t�Z�b�g
    [SerializeField] private float offsetY = 1f; // Y�����̃I�t�Z�b�g
    [SerializeField] private float followSpeed = 5f; // �ʏ�̒Ǐ]���x
    [SerializeField] private float clickedFollowSpeed = 30f; // �N���b�N���̒Ǐ]���x
    [SerializeField] private Vector2 minXmaxX = new Vector2(-5f, 5f); // X�����̈ړ��͈�
    [SerializeField] private Vector2 minYmaxY = new Vector2(-5f, 5f); // Y�����̈ړ��͈�

    // Mouse range settings
    [SerializeField] private Vector2 MouseMin = new Vector2(0f, 0f); // �}�E�X��X, Y�̍ŏ��l
    [SerializeField] private Vector2 MouseMax = new Vector2(Screen.width, Screen.height); // �}�E�X��X, Y�̍ő�l

    private Vector3 originalPosition; // �����ʒu
    private Vector2 originalMinXmaxX; // X�����̌��̈ړ��͈�
    private Vector2 originalMinYmaxY; // Y�����̌��̈ړ��͈�
    private float pressTime = 0f; // �N���b�N���n�܂�������
    private bool isLongPress = false; // ��������Ԃ��ǂ���
    private bool isMovingCoroutine = false; // �ړ��R���[�`�������s�����ǂ���
    private Vector3 targetPosition; // �^�[�Q�b�g�ʒu
    private const float LONG_PRESS_THRESHOLD = 0.2f; // �������̎��Ԃ�臒l

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

    // �}�E�X���͂���������
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
    // ��������Ԃ����Z�b�g����
    private void ResetLongPressState()
    {
        isLongPress = false;
        minXmaxX = originalMinXmaxX;
        minYmaxY = originalMinYmaxY;
        followSpeed = clickedFollowSpeed;
    }

    // �}�E�X��Ǐ]����
    private void FollowMouse()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        float clampedX = Mathf.Clamp(mousePos.x, minXmaxX.x, minXmaxX.y);
        float clampedY = Mathf.Clamp(mousePos.y, minYmaxY.x, minYmaxY.y);
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // �}�E�X�̃��[���h���W���擾����
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // �w�肳�ꂽ�^�[�Q�b�g�ʒu�Ɉړ�����R���[�`��
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
