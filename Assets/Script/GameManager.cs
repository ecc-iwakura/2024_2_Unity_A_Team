using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("�Q�[���I�u�W�F�N�g�Q��")]
    public followplus followPlusScript;      // �t�H���[�v���X�̃X�N���v�g
    public TimelineManager timelineManager;  // �^�C�����C���}�l�[�W���[
    public RuleChecker ruleChecker;          // ���[���`�F�b�J�[
    public KeywordChecker keywordChecker;    // �L�[���[�h�`�F�b�J�[

    [Header("�Q�[���ݒ�")]
    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();  // ��Փx�C�x���g�̃��X�g
    private bool IsGameover = false;       // �Q�[���I�[�o�[�̃t���O
    private int currentEventIndex = 0;     // ���݂̃C�x���g�C���f�b�N�X
    public TMP_Text GameOverText;          // �Q�[���I�[�o�[���ɕ\������e�L�X�g
    public UnityEvent GameOverEvent;       // �Q�[���I�[�o�[���ɔ��s����C�x���g
    public GameObject maskObject;
    public GameObject maskObject2;

    [Header("�����x���ݒ�")]
    public float animationDuration = 0.2f; // �A�j���[�V�����̎��ԁi�b�j
    private float elapsedTime = 0f; // ���݂̌o�ߎ���
    private Vector3 initialPosition; // �����̃}�X�N�I�u�W�F�N�g�̃��[�J�����W
    private Vector3 targetPosition; // �ڕW�̃}�X�N�I�u�W�F�N�g�̃��[�J�����W

    [Header("���̑�")]
    public TMP_Text NextLevelFollowerText; // ���̃��x���̃t�H�����[����\������e�L�X�g
    private int oldcurrentEventIndex = 0;
    public bool over = false;

    private float reductionInterval = 1f; // ������K�p����Ԋu�i�b�j
    private float timeSinceLastReduction = 0f; // �Ō�Ɍ�����K�p���Ă���̌o�ߎ���

    void Start()
    {
        initialPosition = maskObject.transform.localPosition;
        targetPosition = initialPosition;

        NextLevelFollower();
        if (followPlusScript == null) { Debug.LogWarning("�t�H���[�v���X������܂���I"); }
        if (timelineManager == null) { Debug.LogWarning("�^�C�����C���}�l�[�W���[������܂���I"); }
        if (ruleChecker == null) { Debug.LogWarning("���[���`�F�b�J�[������܂���I"); }
        if (keywordChecker == null) { Debug.LogWarning("�L�[���[�h�`�F�b�J�[������܂���I"); }

        difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
    }

    void Update()
    {
        UpdateReductions();
        CheckFollowerThreshold();
    }


    void UpdateReductions()
    {
        // �o�ߎ��Ԃ��X�V
        timeSinceLastReduction += Time.deltaTime;

        // �ݒ肵���Ԋu�ireductionInterval�j�𒴂����ꍇ�Ɍ�����K�p
        if (timeSinceLastReduction >= reductionInterval)
        {
            float reductionFactor = 0.999f; // 1�b���Ƃ�0.1%����

            timelineManager.tweetCooldown *= reductionFactor;
            timelineManager.tweetSpeedTime *= reductionFactor;

            // �o�ߎ��Ԃ����Z�b�g
            timeSinceLastReduction = 0f;
        }
    }

    public void CheckFollowerThreshold()
    {
        if (followPlusScript == null)
        {
            Debug.LogError("followPlusScript is null in CheckFollowerThreshold!");
            return;
        }

        if (followPlusScript.followers <= 0)
        {
            if (!IsGameover)
            {
                GameOver();
            }
        }
        else if (currentEventIndex < difficultyEvents.Count)
        {
            if (difficultyEvents == null)
            {
                Debug.LogError("difficultyEvents is null in CheckFollowerThreshold!");
                return;
            }

            var eventInfo = difficultyEvents[currentEventIndex];

            if (eventInfo == null)
            {
                Debug.LogError($"eventInfo at index {currentEventIndex} is null in CheckFollowerThreshold!");
                return;
            }

            if (followPlusScript.maxFollowers >= (ulong)eventInfo.followerThreshold)
            {
                if (!eventInfo.IsExecuted)
                {
                    eventInfo.IsExecuted = true;
                    ExecuteEvent(eventInfo);
                    Debug.Log($"�C�x���g���s");
                }

                currentEventIndex++;
                NextLevelFollower();
            }
        }
    }

    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        if (eventInfo == null)
        {
            Debug.LogError("EventInfo is null!");
            return;
        }

        // AddTweet �C���X�^���X�̍쐬
        AddTweet addTweet = new AddTweet(
            eventInfo.tweetIDToAdd,
            eventInfo.tweetCooldownReduction, // tweetCooldownReduction ���p�[�Z���e�[�W�Ƃ��Ĉ���
            eventInfo.tweetSpeedReduction,    // tweetSpeedReduction ���p�[�Z���e�[�W�Ƃ��Ĉ���
            eventInfo.ruleFunctionName,
            eventInfo.actionFlag,
            eventInfo.keyWord
        );

        // �X�^�b�N�ɒǉ�
        timelineManager.stackTweetIDs.Add(addTweet);


        Debug.Log($"Event executed: {eventInfo.ruleFunctionName} added with action {eventInfo.actionFlag}");
    }

    public void GameOver()
    {
        GameOverText.text = FormatFollowers(followPlusScript.maxFollowers) + "�t�H�����[";
        timelineManager.IsStop = true;
        GameOverEvent.Invoke();
        IsGameover = true;
    }

    public void SaveGameData(string filePath)
    {
        // �V���A���C�Y����I�u�W�F�N�g��z��ɕϊ�����
        var eventArray = difficultyEvents.ToArray();

        // JSON�ɃV���A���C�Y����
        string jsonData = JsonHelper.ToJson(eventArray, true);

        // �t�@�C���ɏ�������
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Game data saved to: " + filePath);
    }

    public void LoadGameData(string filePath)
    {
        if (File.Exists(filePath))
        {
            // �t�@�C������JSON�f�[�^��ǂݍ���
            string jsonData = File.ReadAllText(filePath);

            // JSON���f�V���A���C�Y���ăI�u�W�F�N�g�ɖ߂�
            var eventArray = JsonHelper.FromJson<DifficultyEvent>(jsonData);

            // ���X�g�ɖ߂�
            difficultyEvents = new List<DifficultyEvent>(eventArray);

            Debug.Log("Game data loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("No saved game data found at: " + filePath);
        }
    }
    public void IsLevUp()
    {
        over = false;
        NextLevelFollower();
    }
    public void NextLevelFollower()
    {
        if(oldcurrentEventIndex != currentEventIndex)
        {
            oldcurrentEventIndex = currentEventIndex;
            over = true;
        }

        if (currentEventIndex < difficultyEvents.Count - 1)
        {
            ulong currentFollowers = followPlusScript.followers;
            ulong threshold = difficultyEvents[currentEventIndex].followerThreshold;

            // Calculate ratio between current followers and threshold, clamped between 0 and 1
            float ratio = Mathf.Clamp01((float)currentFollowers / (float)threshold);

            if(over) { ratio = 1; }
            // Scale the ratio to fit between 0 and 25
            float scaledValue = ratio * 50f;

            // Update the Y position of the maskObject in local space
            targetPosition.y = scaledValue;

            // Start animation coroutine
            StartCoroutine(MoveMaskObject());

            // Update text
            ulong value = threshold - currentFollowers;
            (int intValue, string unit) = followPlusScript.FormatNumber(value);
            NextLevelFollowerText.text = $"{intValue}{unit}";
        }
        else
        {
            NextLevelFollowerText.text = $"����ł��������܂��I";
        }

        if( over ) { NextLevelFollowerText.text = $"UP!"; }
    }

    private IEnumerator MoveMaskObject()
    {
        elapsedTime = 0f;
        Vector3 startPosition = maskObject.transform.localPosition;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            maskObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            maskObject2.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Ensure final position is exactly the target position
        maskObject.transform.localPosition = targetPosition;
    }

    // �t�H�����[���ɓK�؂ȒP�ʂ�t����֐�
    private string FormatFollowers(ulong followers)
    {
        if (followers >= 10000000000000000) // 1���ȏ�
        {
            return (followers / 10000000000000000).ToString() + "��" + ((followers % 10000000000000000) / 1000000000000).ToString() + "��";
        }
        else if (followers >= 1000000000000) // 1���ȏ�
        {
            return (followers / 1000000000000).ToString() + "��" + ((followers % 1000000000000) / 100000000).ToString() + "��";
        }
        else if (followers >= 100000000) // 1���ȏ�
        {
            return (followers / 100000000).ToString() + "��" + ((followers % 100000000) / 10000).ToString() + "��";
        }
        else if (followers >= 10000) // 1���ȏ�
        {
            return (followers / 10000).ToString() + "��" + (followers % 10000).ToString();
        }
        else if (followers >= 1000) // 1��ȏ�
        {
            return (followers / 1000).ToString() + "��" + (followers % 1000).ToString();
        }
        else // 1�疢��
        {
            return followers.ToString();
        }
    }
}



[System.Serializable]
public class DifficultyEvent
{
    public ulong followerThreshold;
    public float tweetCooldownReduction;
    public float tweetSpeedReduction;
    public string tweetIDToAdd;
    public string ruleFunctionName;
    public string keyWord;
    public RuleChecker.ButtonFlag actionFlag;
    public bool IsExecuted;

    public DifficultyEvent(ulong followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, string keyWord, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.keyWord = keyWord;
        this.actionFlag = actionFlag;
        this.IsExecuted = false;
    }
}

// JsonUtility���g�p���ăW�F�l���b�N��JsonHelper�N���X���쐬����
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
