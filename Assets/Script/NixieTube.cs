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