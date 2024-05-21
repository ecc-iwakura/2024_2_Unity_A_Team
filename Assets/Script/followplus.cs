using UnityEngine;

public class followplus : MonoBehaviour
{
    public int followers = 0; // �t�H�����[�����Ǘ�����ϐ�
    private bool firstCorrectAction = true; // ���߂Đ������s�����s��ꂽ���ǂ������Ǘ�����t���O

    // �������s�����������ɌĂяo�����֐�
    [ContextMenu("CorrectAction")]
    public void CorrectAction()
    {
        if (firstCorrectAction)
        {
            followers += 1; // ���߂Ă̐������s���̏ꍇ�A�t�H�����[���Œ��1����
            Debug.Log($"���߂Ă̐������s�������s����܂����I�t�H�����[��: {followers}");
            firstCorrectAction = false; // ���߂Ă̐������s�����s��ꂽ�t���O��false�ɐݒ�
        }
        else
        {
            // �t�H�����[��5%�`10%�ő������A�����1�`10�̌Œ�l��ǉ�
            float increaseRate = Random.Range(0.05f, 0.10f); // 5%�`10%�̑�����
            int increaseAmountFromRate = Mathf.CeilToInt(followers * increaseRate); // �������銄�������̃t�H�����[��
            int fixedIncrease = Random.Range(1, 11); // 1�`10�̌Œ�l
            int totalIncrease = increaseAmountFromRate + fixedIncrease; // ���v�����t�H�����[��

            followers += totalIncrease;
            Debug.Log($"�������s�������s����܂����I�t�H�����[��: {followers} (������: {increaseRate:P0}, ������: {increaseAmountFromRate}, �Œ葝��: {fixedIncrease}, ���v����: {totalIncrease})");
        }

        // �������s���ɑ΂���ǉ��̏����������ɋL�q
    }


    // �Ԉ�����s�����������ɌĂяo�����֐�
    [ContextMenu("IncorrectAction")]
    public void IncorrectAction()
    {
        // �t�H�����[����40%�`50%�Ō���
        float decreaseRate = Random.Range(0.40f, 0.50f); // 40%�`50%�̌�����
        int decreaseAmount = Mathf.CeilToInt(followers * decreaseRate); // ��������t�H�����[��
        followers -= decreaseAmount;
        if (followers < 0) followers = 0; // �t�H�����[�������ɂȂ�Ȃ��悤�ɂ���
        Debug.Log($"�Ԉ�����s�������s����܂���...�t�H�����[��: {followers} (������: {decreaseRate:P0}, ������: {decreaseAmount})");

        // �Ԉ�����s���ɑ΂���ǉ��̏����������ɋL�q
    }

    // �s����]������֐�
    public void EvaluateAction(bool isCorrect)
    {
        if (isCorrect)
        {
            CorrectAction();
        }
        else
        {
            IncorrectAction();
        }
    }
}