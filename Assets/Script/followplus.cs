using UnityEngine;
using TMPro;

public class followplus : MonoBehaviour
{
    public ulong followers = 0; // �t�H�����[�����Ǘ�����ϐ�
    public ulong maxFollowers = 0; // �ō����B�_�̃t�H�����[��
    private bool firstCorrectAction = true; // ���߂Đ������s�����s��ꂽ���ǂ������Ǘ�����t���O

    [Header("�t�H�����[�����ݒ�")]
    [Tooltip("�t�H�����[������������ŏ�����")]
    public float minIncreaseRate = 0.05f; // �ŏ�������
    [Tooltip("�t�H�����[������������ő劄��")]
    public float maxIncreaseRate = 0.10f; // �ő呝����
    [Tooltip("�t�H�����[������������ŏ��Œ�l")]
    public ulong minFixedIncrease = 1; // �ŏ��Œ葝���l
    [Tooltip("�t�H�����[������������ő�Œ�l")]
    public ulong maxFixedIncrease = 10; // �ő�Œ葝���l

    [Header("�t�H�����[�����ݒ�")]
    [Tooltip("�ō����B�_�̃t�H�����[�����猸������ŏ�����")]
    public float minDecreaseRate = 0.40f; // �ŏ�������
    [Tooltip("�ō����B�_�̃t�H�����[�����猸������ő劄��")]
    public float maxDecreaseRate = 0.50f; // �ő匸����

    // UI�e�L�X�g�R���|�[�l���g�ւ̎Q��
    public TMP_Text totalFollowersText;
    public TMP_Text changeInFollowersText;

    void Start()
    {
        UpdateUI(0); // ������Ԃ��X�V
    }

    // �������s�����������ɌĂяo�����֐�
    [ContextMenu("CorrectAction")]
    public void CorrectAction()
    {
        ulong increaseAmount = 0;

        if (firstCorrectAction)
        {
            followers += 1; // ���߂Ă̐������s���̏ꍇ�A�t�H�����[���Œ��1����
            firstCorrectAction = false; // ���߂Ă̐������s�����s��ꂽ�t���O��false�ɐݒ�
            increaseAmount = 1;
        }
        else
        {
            // �t�H�����[��5%�`10%�ő������A�����1�`10�̌Œ�l��ǉ�
            float increaseRate = Random.Range(minIncreaseRate, maxIncreaseRate); // 5%�`10%�̑�����
            ulong increaseAmountFromRate = (ulong)Mathf.CeilToInt(followers * increaseRate); // �������銄�������̃t�H�����[��
            ulong fixedIncrease = (ulong)Random.Range((int)minFixedIncrease, (int)(maxFixedIncrease + 1)); // 1�`10�̌Œ�l
            increaseAmount = (ulong)increaseAmountFromRate + (ulong)fixedIncrease; // ���v�����t�H�����[��

            followers += (ulong)increaseAmount;
        }

        // �ō����B�_�̍X�V
        if (followers > maxFollowers)
        {
            maxFollowers = followers;
        }

        Debug.Log($"�������s�������s����܂����I�t�H�����[��: {followers} (������: {increaseAmount})");
        UpdateUI((int)increaseAmount);
    }

    // �Ԉ�����s�����������ɌĂяo�����֐�
    [ContextMenu("IncorrectAction")]
    public void IncorrectAction()
    {
        // �ō����B�_�̃t�H�����[����40%�`50%�Ō���
        float decreaseRate = Random.Range(minDecreaseRate, maxDecreaseRate); // 40%�`50%�̌�����
        ulong decreaseAmount = (ulong)Mathf.CeilToInt(maxFollowers * decreaseRate); // ��������t�H�����[��
        if (decreaseAmount > followers)
        {
            decreaseAmount = followers; // �t�H�����[����茸���ʂ������ꍇ�͑S�Č��炷
        }
        followers -= (ulong)decreaseAmount;

        Debug.Log($"�Ԉ�����s�������s����܂���...�t�H�����[��: {followers} (������: {decreaseAmount})");
        UpdateUI(-(int)decreaseAmount);
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

    // UI���X�V����֐�
    private void UpdateUI(int changeAmount)
    {
        totalFollowersText.text = $"{followers}";
        if (changeAmount >= 0)
        {
            changeInFollowersText.text = $"UP: +{changeAmount}";
            changeInFollowersText.color = Color.blue; // �F�ɐݒ�
        }
        else
        {
            changeInFollowersText.text = $"DOWN: {changeAmount}";
            changeInFollowersText.color = Color.red; // �ԐF�ɐݒ�
        }
    }
}
