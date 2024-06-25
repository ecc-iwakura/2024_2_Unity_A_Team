using UnityEngine;

public class Finger_neo : MonoBehaviour
{
    public Transform targetObject; // �}�E�X���Ǐ]����^�[�Q�b�g�I�u�W�F�N�g
    public RectTransform canvasRect; // �L�����o�X��RectTransform
    public Vector3 rangeMin; // 3�����͈͂̍ŏ��l
    public Vector3 rangeMax; // 3�����͈͂̍ő�l
    public float followSpeed = 5f; // �Ǐ]���x

    public Animator animator; // �A�j���[�^�[
    public Camera mainCamera;
    private Vector3 lastTargetPosition; // �Ō�Ƀ^�[�Q�b�g�ɓ����������W

    void Start()
    {
        // ���C���J�������擾
        mainCamera = Camera.main;
    }

    void Update()
    {
        // ���C�L���X�g�Ń}�E�X���^�[�Q�b�g�I�u�W�F�N�g�ɓ������Ă��邩�`�F�b�N
        IsMouseInRange();


    }

    private void IsMouseInRange()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �^�[�Q�b�g�I�u�W�F�N�g�Ƃ̃��C�L���X�g���`�F�b�N
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("�������Ă���I�u�W�F�N�g: " + hit.transform.name); // ���C�L���X�g�����������I�u�W�F�N�g�����O�ɏo��
            if (hit.transform.CompareTag("Target"))
            {
                Vector3 hitPosition = hit.point;
                Vector3 targetPosition = hit.transform.position;
                FollowMouse(hitPosition);
                lastTargetPosition = targetPosition;

                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetTrigger("Touch");
                    Debug.Log("2aaaa"); // ���C�L���X�g�����������I�u�W�F�N�g�����O�ɏo��
                }
            }
            else if (hit.transform.CompareTag("BackGround"))
            {
                Vector3 hitPosition = hit.point;
                // ���������o�b�N�O���E���h�I�u�W�F�N�g�̈ʒu�ƒ��߂œ��������^�[�Q�b�g�I�u�W�F�N�g�̈ʒu�̕��ς����
                Vector3 averagePosition = Vector3.Lerp(Vector3.Lerp(lastTargetPosition, hitPosition, 0.3f), hit.point, 0.3f);

                FollowMouse(averagePosition); 
            }
        }
    }


    private void FollowMouse(Vector3 mousePosition)
    {
        // �L�����o�X�̃X�P�[�����擾
        Vector3 canvasScale = canvasRect.localScale;

        // �L�����o�X�̃��[�J�����W�ɕϊ�
        Vector3 localMousePosition = canvasRect.InverseTransformPoint(mousePosition);

        // 3�����͈͓��Ƀ}�E�X�ʒu�𐧌�
        float clampedX = Mathf.Clamp(localMousePosition.x, rangeMin.x * canvasScale.x, rangeMax.x * canvasScale.x);
        float clampedY = Mathf.Clamp(localMousePosition.y, rangeMin.y * canvasScale.y, rangeMax.y * canvasScale.y);
        float clampedZ = Mathf.Clamp(localMousePosition.z, rangeMin.z * canvasScale.z, rangeMax.z * canvasScale.z);

        // �������ꂽ�ʒu�����[���h���W�ɕϊ�
        Vector3 targetPosition = canvasRect.TransformPoint(new Vector3(clampedX, clampedY, clampedZ));

        // �I�u�W�F�N�g�̈ʒu���X�V
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

        // �L�����o�X�̃��[�J�����W�ɕϊ������͈͂̍ŏ��l�ƍő�l���v�Z
        Vector3 canvasRangeMin = canvasRect.InverseTransformPoint(rangeMin);
        Vector3 canvasRangeMax = canvasRect.InverseTransformPoint(rangeMax);

        // �L�����o�X�̃X�P�[�����擾
        Vector3 canvasScale = canvasRect.localScale;

        // �L�����o�X�̃��[�J�����W�ɕϊ������͈͂̒��S���W���v�Z
        Vector3 canvasRangeCenter = (canvasRangeMin + canvasRangeMax) / 2f;

        // �L�����o�X�̃��[�J�����W�ɕϊ������͈͂̃T�C�Y���v�Z
        Vector3 canvasRangeSize = canvasRangeMax - canvasRangeMin;

        // �M�Y����`��
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(canvasRangeCenter, Vector3.Scale(canvasRangeSize, canvasScale));
    }
}
