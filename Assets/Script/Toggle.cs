using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �p�u���b�N�֐� ToggleActive ���`
    public void ToggleActive()
    {
        // ���̃I�u�W�F�N�g�̃A�N�e�B�u��Ԃ��g�O���i���]�j����
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SetActive(bool IsActive)
    {
        // ���̃I�u�W�F�N�g�̃A�N�e�B�u��Ԃ��g�O���i���]�j����
        gameObject.SetActive(IsActive);
    }
}
