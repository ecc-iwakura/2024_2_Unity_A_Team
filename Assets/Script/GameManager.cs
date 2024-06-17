using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static RuleChecker;
using static TimelineManager;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public followplus followPlusScript; // followplus�X�N���v�g�ւ̎Q��
    public TimelineManager timelineManager; // TimelineManager�X�N���v�g�ւ̎Q��
    public RuleChecker ruleChecker; // RuleChecker�X�N���v�g�ւ̎Q��
    public KeywordChecker keywordChecker; // �L�[���[�h�`�F�b�J�[�ւ̎Q��

    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();
    private bool IsGameover = false;
    private int currentEventIndex = 0; // ���݂̃C�x���g�C���f�b�N�X
    public UnityEvent GameOverEvent;


    void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("�t�H���[�v���X������܂���I"); }

        if (timelineManager == null) { Debug.LogWarning("�^�C�����C���}�l�[�W���[������܂���I"); }

        if (ruleChecker == null) { Debug.LogWarning("���[���`�F�b�J�[������܂���I"); }

        if (keywordChecker == null) { Debug.LogWarning("�L�[���[�h�`�F�b�J�[������܂���I"); }

        // followerThreshold�̏��Ȃ����ɕ��ёւ���
        difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
    }

    void Update()
    {
        CheckFollowerThreshold();
    }

    // �t�H�����[����followerThreshold���r���A�C�x���g�����s����
    public void CheckFollowerThreshold()
    {
        if(followPlusScript.followers <= 0)
        {
            if(!IsGameover)
            {
                GameOver();
                IsGameover = true;
            }

        }
        else if (currentEventIndex < difficultyEvents.Count)
        {
            var eventInfo = difficultyEvents[currentEventIndex];

            if (followPlusScript.maxFollowers > eventInfo.followerThreshold)
            {
                if(!eventInfo.IsExecuted)
                {
                    // �C�x���g�����s����
                    ExecuteEvent(eventInfo);

                    eventInfo.IsExecuted = true;
                }
                    // ���̃C�x���g�ɐi��
                    currentEventIndex++;
            }
        }
    }

    // �C�x���g�����s����֐�
    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        // �c�C�[�g�Ԋu�ƃX�s�[�h�̌���
        if (eventInfo.tweetCooldownReduction != 0)
        {
            timelineManager.tweetCooldown += eventInfo.tweetCooldownReduction;
        }

        if (eventInfo.tweetSpeedReduction != 0)
        {
            timelineManager.tweetSpeedTime += eventInfo.tweetSpeedReduction;
        }

        // ���[���̒ǉ����K�v�ȏꍇ
        if (!string.IsNullOrEmpty(eventInfo.ruleFunctionName) && eventInfo.actionFlag != null)
        {
            // AddRuleTweet �I�u�W�F�N�g���쐬���Ēǉ�
            AddRuleTweet addRuleTweet = new AddRuleTweet(eventInfo.tweetIDToAdd, eventInfo.ruleFunctionName, eventInfo.actionFlag);
            timelineManager.stackTweetIDs.Add(addRuleTweet); // AddRuleTweet ��K�؂ɒǉ�
        }

        // �L�[���[�h�̒ǉ�
        if (!string.IsNullOrEmpty(eventInfo.keyWord))
        {
            keywordChecker.keywords.Add(eventInfo.keyWord);
        }

        Debug.Log($"Event executed: {eventInfo.ruleFunctionName} added with action {eventInfo.actionFlag}");
    }

    public void GameOver()
    {
        timelineManager.IsStop = true;
        GameOverEvent.Invoke();
    }


}


[System.Serializable]
public class DifficultyEvent
{
    public int followerThreshold;           // �t�H�����[�������̐��𒴂�����C�x���g���J�n����
    public float tweetCooldownReduction;    // �c�C�[�g�Ԋu�̌�����
    public float tweetSpeedReduction;       // �c�C�[�g�X�s�[�h�̌�����
    public string tweetIDToAdd;             // �ǉ�����c�C�[�gID
    public string ruleFunctionName;         // �ǉ����郋�[���֐���
    public string keyWord;         // �ǉ����郋�[���֐���
    public RuleChecker.ButtonFlag actionFlag; // ButtonFlag
    public bool IsExecuted;                 // �C�x���g�����s���ꂽ���ǂ���

    public DifficultyEvent(int followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, string keyWord, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.keyWord = keyWord;
        this.actionFlag = actionFlag;
        this.IsExecuted = false; // ������Ԃł͎��s����Ă��Ȃ�
    }
}
