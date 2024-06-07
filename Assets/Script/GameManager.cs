using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public followplus followPlusScript; // followplus�X�N���v�g�ւ̎Q��
    public TimelineManager timelineManager; // TimelineManager�X�N���v�g�ւ̎Q��
    public RuleChecker ruleChecker; //  RuleChecker�X�N���v�g�ւ̎Q��


    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();

    public void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("�t�H���[�v���X������܂���I"); }
        if (timelineManager == null) { Debug.LogWarning("�^�C�����C���}�l�[�W���[������܂���I"); }
        if (ruleChecker == null) { Debug.LogWarning("���[���`�F�b�J�[������܂���I"); }
    }
    // �t�H�����[����followerThreshold���r���A�C�x���g�����s����
    public void CheckFollowerThreshold()
    {
        foreach (var eventInfo in difficultyEvents)
        {
            if (followPlusScript.maxFollowers > eventInfo.followerThreshold)
            {
                // �C�x���g�����s����
                ExecuteEvent(eventInfo);
                // followerThreshold�̏��Ȃ����ɕ��ёւ���
                difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
                return;
            }
        }
    }

    // �C�x���g�����s����֐�
    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        // �����ŃC�x���g�ɔ����������s��
    }
}

// DifficultyEvent�N���X�̒�`
[System.Serializable]
public class DifficultyEvent
{
    public int followerThreshold;           // �t�H�����[�������̐��𒴂�����C�x���g���J�n����
    public float tweetCooldownReduction;    // �c�C�[�g�Ԋu�̌�����
    public float tweetSpeedReduction;       // �c�C�[�g�X�s�[�h�̌�����
    public string tweetIDToAdd;             // �ǉ�����c�C�[�gID
    public string ruleFunctionName;         // �ǉ����郋�[���֐���
    public RuleChecker.ButtonFlag actionFlag; // ButtonFlag

    public DifficultyEvent(int followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.actionFlag = actionFlag;
    }
}
