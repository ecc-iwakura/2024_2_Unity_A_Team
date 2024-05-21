using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finger : MonoBehaviour
{
    public Texture2D cursorImage; // �J�[�\���Ƃ��ĕ\������摜
    public Vector2 hotSpot = Vector2.zero; // �J�[�\���̃z�b�g�X�|�b�g
    public float offsetY = 1f; // �J�[�\���Ɖ摜�̍����̃I�t�Z�b�g
    public float offsetX = 1f; // �J�[�\���Ɖ摜�̍����̃I�t�Z�b�g

    public float followSpeed = 5f; // �摜���Ǐ]���鑬�x
    public Vector2 minXmaxX = new Vector2(-5f, 5f); // X���W�̓�������͈�
    public Vector2 minYmaxY = new Vector2(-5f, 5f); // Y���W�̓�������͈�

    private Vector3 targetPosition;

    void Start()
    {
        Cursor.SetCursor(cursorImage, hotSpot, CursorMode.Auto); // �J�[�\����ݒ�
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition; // �}�E�X�J�[�\���̃X�N���[�����W���擾
        mousePos.z = -Camera.main.transform.position.z; // �J��������̋������l������Z���W��ݒ�
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(mousePos); // �X�N���[�����W�����[���h���W�ɕϊ�
        float clampedX = Mathf.Clamp(cursorPos.x, minXmaxX.x, minXmaxX.y); // X���W�𐧌�
        float clampedY = Mathf.Clamp(cursorPos.y, minYmaxY.x, minYmaxY.y); // Y���W�𐧌�
        targetPosition = new Vector3(clampedX * offsetX, clampedY * offsetY, 0f); // �������ꂽ�ʒu��ڕW�ʒu�Ƃ��Đݒ�
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime); // �J�[�\�������炩�ɒǏ]������
    }
}