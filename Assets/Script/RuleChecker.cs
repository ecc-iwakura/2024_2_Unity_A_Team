using UnityEngine;
using System;
using System.Collections.Generic;

public class RuleChecker : MonoBehaviour
{
    [System.Flags]
    public enum ButtonFlag
    {
        None = 0,
        Like = 1 << 0,
        Retweet = 1 << 1,
        LikeAndRetweet = Like | Retweet,
        Bookmark = 1 << 2,
        Report = 1 << 3
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
    public class Rule
    {
        public string ruleName;
        public Func<TweetData, ButtonFlag, ButtonFlag> ruleFunction;

        public Rule(string name, Func<TweetData, ButtonFlag, ButtonFlag> function)
        {
            ruleName = name;
            ruleFunction = function;
        }
    }

    [System.Serializable]
    public class RuleReference
    {
        public string ruleName;
    }

    [SerializeField]
    public Rule[] availableRules;

    public KeywordChecker keywordChecker;

    // �C���X�y�N�^�[����I�����邽�߂̃��[�����X�g
    public List<RuleReference> selectedRules = new List<RuleReference>();
    private Dictionary<string, Func<TweetData, ButtonFlag, ButtonFlag>> ruleFunctions = new Dictionary<string, Func<TweetData, ButtonFlag, ButtonFlag>>();

    public void InitializeRules()
    {
        availableRules = new Rule[]
        {
            new Rule("�t�H���[�����Ă���", LikeCheck),
            new Rule("�P�O���ȓ��Ȃ�", RetweetCheck),
            new Rule("�L�[���[�h����������", ReportCheck),
            new Rule("�����Ȃ�������", BookmarkCheck),
        };

        // ���p�\�ȃ��[���������ɓo�^
        foreach (var rule in availableRules)
        {
            // ���[���������łɑ��݂��邩�ǂ������`�F�b�N
            if (!ruleFunctions.ContainsKey(rule.ruleName))
            {
                // �����Ƀ��[����o�^
                ruleFunctions.Add(rule.ruleName, rule.ruleFunction);
            }
            else
            {
                // ���ɓ������O�̃��[�������݂���ꍇ�͌x�����o��
                Debug.LogWarning("���[�������łɑ��݂��܂�: " + rule.ruleName);
            }
        }
    }

    private void Start()
    {
        if (keywordChecker == null) { Debug.LogWarning("�L�[���[�h�`�F�b�J�[������܂���I"); }
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
            if (ruleFunctions.ContainsKey(selectedRule.ruleName))
            {
                // �I�����ꂽ���[���̊֐����擾���A���̌��ʂ� tweetData ��n���ēK�p����
                result = ruleFunctions[selectedRule.ruleName](tweetData, result);
            }
            else
            {
                Debug.LogWarning("���[����������܂���: " + selectedRule.ruleName);
            }
        }

        Debug.Log("���[������: " + result.ToString() + ", Bool:" + tweetData.someBool + ", Int:" + tweetData.someInt);
        return result;
    }

    // ��Ƃ��Ẵ��[���֐�
    private ButtonFlag LikeCheck(TweetData tweetData, ButtonFlag original)
    {
        if (tweetData.someBool)
        {
            return ButtonFlag.Like;
        }
        return original; // ���X�̒l��Ԃ�
    }

    private ButtonFlag RetweetCheck(TweetData tweetData, ButtonFlag original)
    {
        if (tweetData.someInt <= 10)
        {
            return ButtonFlag.Retweet;
        }
        return original; // ���X�̒l��Ԃ�
    }

    private ButtonFlag ReportCheck(TweetData tweetData, ButtonFlag original)
    {
        bool IsKey = keywordChecker.CheckForKeyword(tweetData.someString);

        if (IsKey)
        {
            return ButtonFlag.Like;
        }

        return original; // ���X�̒l��Ԃ�
    }

    private ButtonFlag BookmarkCheck(TweetData tweetData, ButtonFlag original)
    {
        if(original == ButtonFlag.None) {

            return ButtonFlag.Bookmark;
        }
        return original; // ���X�̒l��Ԃ�
    }
}
