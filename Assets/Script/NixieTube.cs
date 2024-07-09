using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DigitObjectArray
{
    public GameObject[] digitObjects; // 0����9�܂ł̃I�u�W�F�N�g���i�[����z��
}

public class NixieTube : MonoBehaviour
{
    public DigitObjectArray[] digitArrays; // �e����1����9�A0�܂ł̃I�u�W�F�N�g���i�[����z��̔z��
    public GameObject[] unitObjects;      // �P�ʁiK�AM�AB�AT�Ȃǁj�̃I�u�W�F�N�g���i�[����z��

    public ulong currentValue = 0; // ���ݕ\�����Ă���l
    private Coroutine currentCoroutine; // ���ݎ��s���̃R���[�`��

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay(1);
    }

    // Update is called once per frame
    void Update()
    {
        // �Ⴆ�΁A�l���ω�����^�C�~���O�ōX�V����ꍇ�͂����ɏ�����ǉ�����
    }

    // ���l�ɉ����ĕ\�����X�V���郁�\�b�h
    [ContextMenu("UpdateDisplay")]
    public void UpdateDisplay(ulong valueToShow)
    {
        // �����̃R���[�`�������s���ł���Β�~����
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // �V�����l�ŃR���[�`�����J�n���āA�l�����ԂɍX�V����
        currentCoroutine = StartCoroutine(AnimateDisplayChange(valueToShow));
    }

    // ���l�̕ύX���A�j���[�V�����ōs���R���[�`��
    private IEnumerator AnimateDisplayChange(ulong targetValue)
    {
        while (currentValue != targetValue)
        {
            // �ڕW�l�ƌ��݂̒l�̍��Ɋ�Â��đ҂����ԂƑ����ʂ𒲐�
            ulong valueDifference = targetValue > currentValue ? targetValue - currentValue : currentValue - targetValue;
            int magnitudeDifference = Mathf.FloorToInt(Mathf.Log10((float)valueDifference));
            int divisionFactor = magnitudeDifference / 3;
            int remainderFactor = magnitudeDifference % 3 + 1;

            ulong increment = (ulong)Mathf.Pow(10, 3 * divisionFactor);
            float waitTime = Mathf.Pow(0.1f, remainderFactor);

            // ���݂̒l��K�؂ȑ����ʂŖڕW�l�ɋ߂Â���
            if (currentValue < targetValue)
            {
                currentValue = (ulong)Mathf.Min(currentValue + increment, targetValue);
            }
            else if (currentValue > targetValue)
            {
                currentValue = (ulong)Mathf.Max(currentValue - increment, targetValue);
            }

            // ���݂̒l��\������
            DisplayValue(currentValue);

            // �����҂��Ă��玟�̍X�V���s��
            yield return new WaitForSeconds(waitTime);
        }
    }


    // ���ۂɕ\�����X�V���郁�\�b�h
    private void DisplayValue(ulong valueToShow)
    {
        // �\������P�ʂ̃C���f�b�N�X��������
        int unitIndex = 0;
        ulong displayValue = valueToShow;

        // ���l��1000�𒴂���ꍇ�A�K�؂ȒP�ʂɐ؂�ւ���
        while (displayValue >= 1000 && unitIndex < unitObjects.Length)
        {
            displayValue /= 1000;
            unitIndex++;
        }

        // �l�𕶎���ɕϊ����āA3���ɂȂ�悤��0�Ńp�f�B���O����
        string valueString = displayValue.ToString().PadLeft(3, '0');

        // �e���̃I�u�W�F�N�g��\������
        for (int i = 0; i < 3; i++)
        {
            int digit = int.Parse(valueString[i].ToString());
            // �Ή����錅�̃I�u�W�F�N�g���A�N�e�B�u�ɂ���
            for (int j = 0; j < digitArrays[i].digitObjects.Length; j++)
            {
                digitArrays[i].digitObjects[j].SetActive(j == digit);
            }
        }

        // �P�ʂ�\������
        DisplayUnits(unitIndex);
    }

    // �P�ʂ�\�����郁�\�b�h
    public void DisplayUnits(int index)
    {
        // �P�ʂ̃I�u�W�F�N�g���A�N�e�B�u�ɂ���
        for (int i = 0; i < unitObjects.Length; i++)
        {
            if (unitObjects[i] != null)
            {
                unitObjects[i].SetActive(i == index);
            }
        }
    }
}