//using UnityEngine;
//using TMPro; // TextMeshPro���g�����߂ɕK�v

//public class followplus : MonoBehaviour
//{
//    public int followers = 0; // �t�H�����[�����Ǘ�����ϐ�
//    private int maxFollowers = 0; // �ō����B�_�̃t�H�����[��
//    private bool firstCorrectAction = true; // ���߂Đ������s�����s��ꂽ���ǂ������Ǘ�����t���O

//    // UI�e�L�X�g�R���|�[�l���g�ւ̎Q��
//    public TMP_Text totalFollowersText;
//    public TMP_Text changeInFollowersText;

//    void Start()
//    {
//        UpdateUI(0); // ������Ԃ��X�V
//    }

//    // �������s�����������ɌĂяo�����֐�
//    [ContextMenu("CorrectAction")]
//    public void CorrectAction()
//    {
//        int increaseAmount = 0;

//        if (firstCorrectAction)
//        {
//            followers += 1; // ���߂Ă̐������s���̏ꍇ�A�t�H�����[���Œ��1����
//            firstCorrectAction = false; // ���߂Ă̐������s�����s��ꂽ�t���O��false�ɐݒ�
//            increaseAmount = 1;
//        }
//        else
//        {
//            // �t�H�����[��5%�`10%�ő������A�����1�`10�̌Œ�l��ǉ�
//            float increaseRate = Random.Range(0.05f, 0.10f); // 5%�`10%�̑�����
//            int increaseAmountFromRate = Mathf.CeilToInt(followers * increaseRate); // �������銄�������̃t�H�����[��
//            int fixedIncrease = Random.Range(1, 11); // 1�`10�̌Œ�l
//            increaseAmount = increaseAmountFromRate + fixedIncrease; // ���v�����t�H�����[��

//            followers += increaseAmount;
//        }

//        // �ō����B�_�̍X�V
//        if (followers > maxFollowers)
//        {
//            maxFollowers = followers;
//        }

//        Debug.Log($"�������s�������s����܂����I�t�H�����[��: {followers} (������: {increaseAmount})");
//        UpdateUI(increaseAmount);
//    }

//    // �Ԉ�����s�����������ɌĂяo�����֐�
//    [ContextMenu("IncorrectAction")]
//    public void IncorrectAction()
//    {
//        // �ō����B�_�̃t�H�����[����40%�`50%�Ō���
//        float decreaseRate = Random.Range(0.40f, 0.50f); // 40%�`50%�̌�����
//        int decreaseAmount = Mathf.CeilToInt(maxFollowers * decreaseRate); // ��������t�H�����[��
//        followers -= decreaseAmount;
//        if (followers < 0) followers = 0; // �t�H�����[�������ɂȂ�Ȃ��悤�ɂ���

//        Debug.Log($"�Ԉ�����s�������s����܂���...�t�H�����[��: {followers} (������: {decreaseAmount})");
//        UpdateUI(-decreaseAmount);
//    }

//    // �s����]������֐�
//    public void EvaluateAction(bool isCorrect)
//    {
//        if (isCorrect)
//        {
//            CorrectAction();
//        }
//        else
//        {
//            IncorrectAction();
//        }
//    }

//    // UI���X�V����֐�
//    private void UpdateUI(int changeAmount)
//    {
//        totalFollowersText.text = $"followers: {followers}";
//        changeInFollowersText.text = changeAmount >= 0 ? $": +{changeAmount}" : $": -{changeAmount}";
//    }
//}

using UnityEngine;
using TMPro; // TextMeshPro���g�����߂ɕK�v

public class followplus : MonoBehaviour
{
    public int followers = 0; // �t�H�����[�����Ǘ�����ϐ�
    public int maxFollowers = 0; // �ō����B�_�̃t�H�����[��
    private bool firstCorrectAction = true; // ���߂Đ������s�����s��ꂽ���ǂ������Ǘ�����t���O

    [Header("�t�H�����[�����ݒ�")]
    [Tooltip("�t�H�����[������������ŏ�����")]
    public float minIncreaseRate = 0.05f; // �ŏ�������
    [Tooltip("�t�H�����[������������ő劄��")]
    public float maxIncreaseRate = 0.10f; // �ő呝����
    [Tooltip("�t�H�����[������������ŏ��Œ�l")]
    public int minFixedIncrease = 1; // �ŏ��Œ葝���l
    [Tooltip("�t�H�����[������������ő�Œ�l")]
    public int maxFixedIncrease = 10; // �ő�Œ葝���l

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
        int increaseAmount = 0;

        if (firstCorrectAction)
        {
            followers += 1; // ���߂Ă̐������s���̏ꍇ�A�t�H�����[���Œ��1����
            firstCorrectAction = false; // ���߂Ă̐������s�����s��ꂽ�t���O��false�ɐݒ�
            increaseAmount = 1;
        }
        else
        {
            // �t�H�����[��5%�`10%�ő������A�����1�`10�̌Œ�l��ǉ�
            float increaseRate = Random.Range(0.15f, 0.25f); // 5%�`10%�̑�����
            int increaseAmountFromRate = Mathf.CeilToInt(followers * increaseRate); // �������銄�������̃t�H�����[��
            int fixedIncrease = Random.Range(1, 11); // 1�`10�̌Œ�l
            increaseAmount = increaseAmountFromRate + fixedIncrease; // ���v�����t�H�����[��

            followers += increaseAmount;
        }

        // �ō����B�_�̍X�V
        if (followers > maxFollowers)
        {
            maxFollowers = followers;
        }

        Debug.Log($"�������s�������s����܂����I�t�H�����[��: {followers} (������: {increaseAmount})");
        UpdateUI(increaseAmount);
    }

    // �Ԉ�����s�����������ɌĂяo�����֐�
    [ContextMenu("IncorrectAction")]
    public void IncorrectAction()
    {
        // �ō����B�_�̃t�H�����[����40%�`50%�Ō���
        float decreaseRate = Random.Range(0.40f, 0.50f); // 40%�`50%�̌�����
        int decreaseAmount = Mathf.CeilToInt(maxFollowers * decreaseRate); // ��������t�H�����[��
        followers -= decreaseAmount;
        if (followers < 0) followers = 0; // �t�H�����[�������ɂȂ�Ȃ��悤�ɂ���

        Debug.Log($"�Ԉ�����s�������s����܂���...�t�H�����[��: {followers} (������: {decreaseAmount})");
        UpdateUI(-decreaseAmount);
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