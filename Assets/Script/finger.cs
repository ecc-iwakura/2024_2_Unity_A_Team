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
        Vector2 canvasMouseMin = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMin.x, MouseMin.y, Camera.main.WorldToScreenPoint(transform.position).z)));
        Vector2 canvasMouseMax = mainCanvasRect.InverseTransformPoint(Camera.main.ScreenToWorldPoint(new Vector3(MouseMax.x, MouseMax.y, Camera.main.WorldToScreenPoint(transform.position).z)));

        // �L�����o�X�̃X�P�[�����擾���ăM�Y�����g��k��
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector2 bottomLeft = mainCanvasRect.TransformPoint(canvasMouseMin);
        Vector2 topRight = mainCanvasRect.TransformPoint(canvasMouseMax);

        // ���[�J�����W�n�ł͈̔̓`�F�b�N
        bool isInRange = localMousePosition.x >= bottomLeft.x && localMousePosition.x <= topRight.x &&
                         localMousePosition.y >= bottomLeft.y && localMousePosition.y <= topRight.y;

        // �͈͂̈ʒu�ƃ}�E�X�̈ʒu�����O�o��
        Debug.Log("�͈͍���: " + bottomLeft + ", �}�E�X�̈ʒu: " + localMousePosition + ", �͈͉E��: " + topRight);

        return isInRange;
    }


    // �}�E�X��Ǐ]����
    private void FollowMouse()
    {
        // �}�E�X�̃L�����o�X����2D�ʒu���擾
        Vector2 mousePos = GetMouseCanvasPosition();

        // �L�����o�X�̃��[�J�����W�ɕϊ������͈͂��擾
        Vector2 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(new Vector2(minXmaxX.x, minXmaxX.y));
        Vector2 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(new Vector2(minYmaxY.x, minYmaxY.y));

        // �L�����o�X�̃X�P�[����K�p
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Ĕ͈͂��N�����v
        Vector2 bottomLeftGizmo = mainCanvasRect.TransformPoint(canvasMouseMinGizmo);
        Vector2 topRightGizmo = mainCanvasRect.TransformPoint(canvasMouseMaxGizmo);

        float clampedX = Mathf.Clamp(mousePos.x, topRightGizmo.x, bottomLeftGizmo.x);
        float clampedY = Mathf.Clamp(mousePos.y, topRightGizmo.y, bottomLeftGizmo.y);

        // �ڕW�ʒu��ݒ肵�āA�I�u�W�F�N�g��Ǐ]
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    // �}�E�X�̃L�����o�X���W���擾���郁�\�b�h
    private Vector2 GetMouseCanvasPosition()
    {
        Vector3 mousePos = Input.mousePosition;

        // �}�E�X�̃X�N���[�����W�����[���h���W�ɕϊ��A�I�u�W�F�N�g��Z���ʒu���l��
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        // ���[���h���W���L�����o�X�̃��[�J�����W�ɕϊ�
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        return mainCanvasRect.InverseTransformPoint(worldPos);
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

        // ���[�J��Z�����擾
        float localZ = transform.localPosition.z;

        // �L�����o�X�̃��[�J�����W�ɕϊ�
        Vector2 canvasMouseMin = mainCanvasRect.InverseTransformPoint(new Vector3(MouseMin.x, MouseMin.y, localZ));
        Vector2 canvasMouseMax = mainCanvasRect.InverseTransformPoint(new Vector3(MouseMax.x, MouseMax.y, localZ));

        // �L�����o�X�̃X�P�[�����擾���ăM�Y�����g��k��
        Vector3 canvasScale = mainCanvasRect.localScale;
        canvasMouseMin.x *= canvasScale.x;
        canvasMouseMin.y *= canvasScale.y;
        canvasMouseMax.x *= canvasScale.x;
        canvasMouseMax.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
        Vector3 bottomLeft = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMin.x, canvasMouseMin.y, localZ));
        Vector3 topRight = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMax.x, canvasMouseMax.y, localZ));
        Vector3 topLeft = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMin.x, canvasMouseMax.y, localZ));
        Vector3 bottomRight = mainCanvasRect.TransformPoint(new Vector3(canvasMouseMax.x, canvasMouseMin.y, localZ));

        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        Gizmos.color = Color.blue;

        // �L�����o�X�̃��[�J�����W�ɕϊ������}�E�X�͈̔͂��擾
        Vector2 canvasMouseMinGizmo = mainCanvasRect.InverseTransformPoint(new Vector3(minXmaxX.x, minXmaxX.y, localZ));
        Vector2 canvasMouseMaxGizmo = mainCanvasRect.InverseTransformPoint(new Vector3(minYmaxY.x, minYmaxY.y, localZ));

        canvasMouseMinGizmo.x *= canvasScale.x;
        canvasMouseMinGizmo.y *= canvasScale.y;
        canvasMouseMaxGizmo.x *= canvasScale.x;
        canvasMouseMaxGizmo.y *= canvasScale.y;

        // �L�����o�X�̃��[�J�����W����ɂ��Đ���`��
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
