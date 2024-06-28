using UnityEngine;
using TMPro;

public class followplus : MonoBehaviour
{
    public NixieTube nixieTube;
    public ulong followers = 0; // �t�H�����[�����Ǘ�����ϐ�
    public ulong maxFollowers = 0; // �ō����B�_�̃t�H�����[��
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


    public TMP_Text changeInFollowersText;

    void Start()
    {
        if(nixieTube == null)
        {
            UnityEngine.Debug.LogError("nixieTube������܂���I");
        }
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
            ulong increaseAmountFromRate = (ulong)(followers * increaseRate); // �������銄�������̃t�H�����[��
            ulong fixedIncrease = (ulong)Random.Range((int)minFixedIncrease, (int)(maxFixedIncrease + 1)); // 1�`10�̌Œ�l
            increaseAmount = (ulong)increaseAmountFromRate + (ulong)fixedIncrease; // ���v�����t�H�����[��
            followers += (ulong)increaseAmount;
            Debug.LogWarning($"increaseRate {increaseAmountFromRate} (�Œ�l: {fixedIncrease}) ���v���Z�l{increaseAmount}");
        }

        // �ō����B�_�̍X�V
        if (followers > maxFollowers)
        {
            maxFollowers = followers;
        }


        Debug.Log($"�������s�������s����܂����I�t�H�����[��: {followers} (������: {increaseAmount})");
        UpdateUI((ulong)increaseAmount,false);
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
        UpdateUI((ulong)decreaseAmount, true);
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

    private void UpdateUI(ulong changeAmount, bool isDecrease)
    {

        string formattedFollowers = FormatNumber(followers);
        nixieTube.UpdateDisplay(followers);

        if (!isDecrease)
        {
            string changeText = FormatNumber(changeAmount);
            changeInFollowersText.text = $"UP: +{changeText}";
            changeInFollowersText.color = Color.green; // �F�ɐݒ�
        }
        else
        {
            string changeText = FormatNumber(changeAmount);
            changeInFollowersText.text = $"DOWN: -{changeText}";
            changeInFollowersText.color = new Color(243f / 255f, 152f / 255f, 0f / 255f, 1f); // �I�����W�F�ɐݒ�
        }
    }


    // ������K, M, B, T�Ȃǂ̕\�L�Ƀt�H�[�}�b�g����֐�
    private string FormatNumber(ulong number)
    {
        // 1000�ȏ�̐��ɑ΂��āA�K�؂Ȑڔ�����t���ăt�H�[�}�b�g����
        if (number >= 1000000000000) // �� (T)
        {
            return $"{number / 1000000000000f:F1}T";
        }
        else if (number >= 1000000000) // �\�� (B)
        {
            return $"{number / 1000000000f:F1}B";
        }
        else if (number >= 1000000) // �S�� (M)
        {
            return $"{number / 1000000f:F1}M";
        }
        else if (number >= 1000) // �� (K)
        {
            return $"{number / 1000f:F1}K";
        }
        else // 1000�����͂��̂܂ܕ\��
        {
            return number.ToString();
        }
    }
}
