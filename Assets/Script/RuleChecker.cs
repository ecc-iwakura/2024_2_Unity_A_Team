using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class RuleChecker : MonoBehaviour
{
    [System.Flags]
    public enum ButtonFlag
    {
        None = 0,
        Like = 1 << 0,
        Retweet = 1 << 1,
        LikeRT = Like | Retweet,
        Bookmark = 1 << 2,
        Report = 1 << 3,
        Other = 1 << 4
    }

    public enum Terms
    {
        False = 0,
        OR = 1 << 0,
        Equal = 1 << 1,
    }

    [System.Serializable]
    public class TweetData
    {
        public bool someBool;
        public int someInt;
        public string someString;

        public TweetData(bool b, int i, string s)
        {
            someBool = b;
            someInt = i;
            someString = s;
        }
    }

    [System.Serializable]
    public class Condition
    {
        public string conditionName;
        public Func<TweetData, Terms> conditionFunction;

        public Condition(string name, Func<TweetData, Terms> function)
        {
            conditionName = name;
            conditionFunction = function;
        }
    }

    [System.Serializable]
    public class RuleReference
    {
        public string conditionName;
        public ButtonFlag actionFlag;
    }


    public KeywordChecker keywordChecker;
    public TMP_Text Log;
    public TMP_Text ruleDisplayText; // �ǉ�: ���[����\�����邽�߂�TMP�e�L�X�g

    [Tooltip("�V�[������ 'Followplus' �I�u�W�F�N�g�ւ̎Q�ƁB")]
    public followplus Followplus;

    [SerializeField]
    public Condition[] availableConditions;

    // �C���X�y�N�^�[����I�����邽�߂̃��[�����X�g
    public List<RuleReference> selectedRules = new List<RuleReference>();
    private Dictionary<string, Func<TweetData, Terms>> conditionFunctions = new Dictionary<string, Func<TweetData, Terms>>();

    public void InitializeRules()
    {
        availableConditions = new Condition[]
        {
            new Condition("�t�H���[�����Ă�����", FollowCondition),
            new Condition("�P�O���ȓ��Ȃ�", TimeCondition),
            new Condition("�u�L�[���[�h�v���������炻��ȊO����������", KeywordCondition)
        };

        // ���p�\�ȏ����������ɓo�^
        foreach (var condition in availableConditions)
        {
            if (!conditionFunctions.ContainsKey(condition.conditionName))
            {
                conditionFunctions.Add(condition.conditionName, condition.conditionFunction);
            }
            else
            {
                Debug.LogWarning("���������łɑ��݂��܂�: " + condition.conditionName);
            }
        }

        DisplaySelectedRules();
    }

    private void Start()
    {
        if (keywordChecker == null) { Debug.LogWarning("�L�[���[�h�`�F�b�J�[������܂���I"); }

        Followplus = GameObject.Find("FollowPlus").GetComponent<followplus>();
        if (Followplus == null)
        {

            Debug.LogError("Followplus��������܂���ł����I");
        }

        InitializeRules();
    }

    public ButtonFlag ApplyRules(TweetData tweetData)
    {
        if (tweetData == null)
        {
            Debug.LogError("TweetData������܂���I");
            return ButtonFlag.None;
        }

        ButtonFlag result = ButtonFlag.None;

        foreach (var selectedRule in selectedRules)
        {
            if (string.IsNullOrEmpty(selectedRule.conditionName))
            {
                Debug.LogWarning("�I�����ꂽ���[���ɏ�����������܂���I");
                continue; // ���[���ɏ��������Ȃ��ꍇ�A���̃��[���ɐi��
            }

            if (conditionFunctions.ContainsKey(selectedRule.conditionName))
            {

                Terms terms = conditionFunctions[selectedRule.conditionName](tweetData);

                // �I�����ꂽ�����̊֐����擾���A��������������ꍇ�̂݃A�N�V������K�p����
                if (terms == Terms.OR)
                {
                    result |= selectedRule.actionFlag;
                }
                else if(terms == Terms.Equal)
                {
                    result = selectedRule.actionFlag;
                }
            }
            else
            {
                Debug.LogWarning("������������܂���: " + selectedRule.conditionName);
            }
        }

        Debug.Log("���[������: " + result.ToString() + ", Bool:" + tweetData.someBool + ", Int:" + tweetData.someInt);
        return result;
    }

    public void CheckAction(ButtonFlag correctAction, bool shouldLike, bool shouldRetweet, bool shouldBookmark, bool shouldReport)
    {

        ButtonFlag userAction = ButtonFlag.Other;

        // ���[�U�[�̍s����ݒ�
        if (!shouldLike && !shouldRetweet && !shouldBookmark && !shouldReport)
        {
            userAction = ButtonFlag.None;
        }
        else if (shouldLike && !shouldRetweet && !shouldBookmark && !shouldReport)
        {
            userAction = ButtonFlag.Like;
        }
        else if (!shouldLike && shouldRetweet && !shouldBookmark && !shouldReport)
        {
            userAction = ButtonFlag.Retweet;
        }
        else if (shouldLike && shouldRetweet && !shouldBookmark && !shouldReport)
        {
            userAction = ButtonFlag.LikeRT;
        }
        else if (!shouldLike && !shouldRetweet && shouldBookmark && !shouldReport)
        {
            userAction = ButtonFlag.Bookmark;
        }
        else if (!shouldLike && !shouldRetweet && !shouldBookmark && shouldReport)
        {
            userAction = ButtonFlag.Report;
        }


        bool isCorrect = userAction == correctAction;

        string userActionText = userAction.ToString();
        string correctActionText = correctAction.ToString();
        string statusText = isCorrect ? "<color=green><b>TRUE!</b></color>" : "<color=red><b>FALSE!</b></color>";
        string logMessage = $"{userActionText} == {correctActionText} => {statusText}";
        string finalLogMessage = $"{System.DateTime.Now} - {logMessage}";

        Debug.LogWarning(finalLogMessage);

        UpdateLog(logMessage);

        if (isCorrect)
        {
            Followplus.CorrectAction();
        }
        else
        {
            Followplus.IncorrectAction();
        }


    }

    private void UpdateLog(string newLog)
    {
        string[] currentLog = Log.text.Split('\n');
        List<string> updatedLog = new List<string>(currentLog);

        updatedLog.Add(newLog);
        if (updatedLog.Count > 10)
        {
            updatedLog.RemoveAt(0);
        }

        Log.text = string.Join("\n", updatedLog);
    }

    public void AddRule(string conditionName, ButtonFlag actionFlag)
    {
        foreach (var condition in availableConditions)
        {
            if (condition.conditionName == conditionName)
            {
                foreach (var rule in selectedRules)
                {
                    if (rule.conditionName == conditionName)
                    {
                        Debug.LogWarning("�I�����ꂽ�������͊��ɑI������Ă��܂�: " + conditionName);
                        return;
                    }
                }

                // �������ɑΉ����郋�[�������������ꍇ�A�V�������[�����쐬���Ēǉ�
                selectedRules.Add(new RuleReference { conditionName = conditionName, actionFlag = actionFlag });
                DisplaySelectedRules(); // ���[�����ĕ\��
                return;
            }
        }

        Debug.LogWarning("�������ɑΉ�����֐���������܂���ł���: " + conditionName);
    }
    // �I�����ꂽ���[����\�����郁�\�b�h
    [ContextMenu("Rules�\���I")]
    public void DisplaySelectedRules()
    {
        string rulesText = "";
        foreach (var rule in selectedRules)
        {
            rulesText += $" {rule.conditionName}{rule.actionFlag}\n";
        }
        ruleDisplayText.text = rulesText;
    }

    private Terms FollowCondition(TweetData tweetData)
    {
        return tweetData.someBool ? Terms.OR : Terms.False;
    }

    private Terms TimeCondition(TweetData tweetData)
    {
        return tweetData.someInt <= 10 ? Terms.OR : Terms.False;
    }

    private Terms KeywordCondition(TweetData tweetData)
    {
        return keywordChecker.CheckForKeyword(tweetData.someString) ? Terms.Equal : Terms.False;
    }
}
