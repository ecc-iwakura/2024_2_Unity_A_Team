using System.Collections;
using UnityEngine;

public class finger : MonoBehaviour
{
    // Cursor settings
    [SerializeField] private RectTransform mainCanvasRect; // ���C���L�����o�X��RectTransform
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
    private bool isMovingCoroutine = false; // �ړ��R���[�`�������s�����ǂ���
    private Vector3 targetPosition; // �^�[�Q�b�g�ʒu
    private const float LONG_PRESS_THRESHOLD = 0.2f; // �������̎��Ԃ�臒l

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

        // ���݂̉�]�p�x�ƖڕW�̉�]�p�x�̊Ԃ���`��Ԃ��Ċ��炩�ȉ�]������
        Quaternion currentRotation = transform.rotation;
        Quaternion targetQuaternion = Quaternion.Euler(0f, 0f, targetRotation);
        transform.rotation = Quaternion.Lerp(currentRotation, targetQuaternion, Time.deltaTime * followSpeed);
    }

    // �}�E�X���͂���������
    private void HandleMouseInput()
    {
        if (IsMouseInRange())
        {
            UnityEngine.Debug.Log("�}�E�X�͈͓̔��ł���`");

            if (Input.GetMouseButtonDown(0) && !isMovingCoroutine)
            {
                targetPosition = GetMouseCanvasPosition();
                StartCoroutine(MoveToTarget(targetPosition));
            }
        }
        else { UnityEngine.Debug.Log("�}�E�X�͈̔͊O����΂��I"); }

    }

    private bool IsMouseInRange()
    {
        // �}�E�X�̈ʒu�����[�J�����W�ɕϊ�
        Vector2 localMousePosition = GetMouseCanvasPosition();

        // �L�����o�X�̃��[�J�����W�ɕϊ������}�E�X�͈̔͂��擾
        Vector3 canvasMouseMin = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMin.x, MouseMin.y, -Camera.main.transform.position.z)));
        Vector3 canvasMouseMax = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMax.x, MouseMax.y, -Camera.main.transform.position.z)));

        // �L�����o�X�̃X�P�[�����擾���ăM�Y�����g��k��
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector3 bottomLeft = mainCanvasRect.TransformPoint(canvasMouseMin);
        Vector3 topRight = mainCanvasRect.TransformPoint(canvasMouseMax);

        // ���[�J�����W�n�ł͈̔̓`�F�b�N
        bool isInRange = localMousePosition.x >= topRight.x && localMousePosition.x <= bottomLeft.x &&
                         localMousePosition.y >= topRight.y && localMousePosition.y <= bottomLeft.y;

        // �͈͂̈ʒu�ƃ}�E�X�̈ʒu�����O�o��
        Debug.Log("�͈͍���: " + bottomLeft + ", �}�E�X�̈ʒu: " + localMousePosition + ", �͈͉E��: " + topRight);

        return isInRange;
    }


    // �}�E�X��Ǐ]����
    private void FollowMouse()
    {
        Vector3 mousePos = GetMouseCanvasPosition();

        Vector3 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(minXmaxX.x, minXmaxX.y, -Camera.main.transform.position.z)));
        Vector3 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(minYmaxY.x, minYmaxY.y, -Camera.main.transform.position.z)));

        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector3 bottomLeftGizmo = mainCanvasRect.TransformPoint(canvasMouseMinGizmo);
        Vector3 topRightGizmo = mainCanvasRect.TransformPoint(canvasMouseMaxGizmo);

        float clampedX = Mathf.Clamp(mousePos.x, topRightGizmo.x, topRightGizmo.y);
        float clampedY = Mathf.Clamp(mousePos.y, bottomLeftGizmo.x, bottomLeftGizmo.y);
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // �}�E�X�̃L�����o�X���W���擾����
    private Vector3 GetMouseCanvasPosition()
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

        // �L�����o�X�̃��[�J�����W�ɕϊ�
        Vector3 canvasMouseMin = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMin.x, MouseMin.y, -Camera.main.transform.position.z)));
        Vector3 canvasMouseMax = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMax.x, MouseMax.y, -Camera.main.transform.position.z)));

        // �L�����o�X�̃X�P�[�����擾���ăM�Y�����g��k��
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector3 bottomLeft = mainCanvasRect.TransformPoint(canvasMouseMin);
        Vector3 topRight = mainCanvasRect.TransformPoint(canvasMouseMax);
        Vector3 topLeft = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMin.x, canvasMouseMax.y, 0f));
        Vector3 bottomRight = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMax.x, canvasMouseMin.y, 0f));

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        Gizmos.color = Color.blue;

        // �L�����o�X�̃��[�J�����W�ɕϊ������}�E�X�͈̔͂��擾
        Vector3 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(minXmaxX.x, minXmaxX.y, -Camera.main.transform.position.z)));
        Vector3 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(minYmaxY.x, minYmaxY.y, -Camera.main.transform.position.z)));


        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector3 bottomLeftGizmo = mainCanvasRect.TransformPoint(canvasMouseMinGizmo);
        Vector3 topRightGizmo = mainCanvasRect.TransformPoint(canvasMouseMaxGizmo);
        Vector3 topLeftGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMinGizmo.x, canvasMouseMaxGizmo.y, 0f));
        Vector3 bottomRightGizmo = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMaxGizmo.x, canvasMouseMinGizmo.y, 0f));

        Gizmos.DrawLine(bottomLeftGizmo, topLeftGizmo);
        Gizmos.DrawLine(topLeftGizmo, topRightGizmo);
        Gizmos.DrawLine(topRightGizmo, bottomRightGizmo);
        Gizmos.DrawLine(bottomRightGizmo, bottomLeftGizmo);
    }



}
