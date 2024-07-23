using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

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

    [System.Serializable]
    public class ButtonFlagImage
    {
        public ButtonFlag buttonFlag;
        public Sprite image;
        public string flagName; // �{�^���t���O�̖��O���i�[���邽�߂̃t�B�[���h��ǉ�
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
        public bool IsFollow;
        public bool IsImage;
        public int Time;
        public string Text;

        public TweetData(bool b, bool i, int t, string s)
        {
            IsFollow = b;
            IsImage = i;
            Time = t;
            Text = s;
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

    [System.Serializable]
    public class RuleDisplay
    {
        public TMP_Text ruleText;
        public Image ruleImage;
    }



    public KeywordChecker keywordChecker;
    public TMP_Text Log;
    public TMP_Text ruleDisplayText;


    [Tooltip("�V�[������ 'Followplus' �I�u�W�F�N�g�ւ̎Q�ƁB")]
    public followplus Followplus;

    public bool IsQuick = false;
    public GameObject NoneRule; // NoneRule�Ƃ���GameObject�̎Q�Ƃ��C���X�y�N�^�[����ݒ肵�Ă�������

    public UnityEvent CorrectSE;
    public UnityEvent IncorrectSE;
    public UnityEvent ContinueSE;

    public Animator Flick; // Animator�̎Q�Ƃ�ǉ�

    public int Logs = 5;

    [SerializeField]
    public Condition[] availableConditions;

    public List<RuleReference> selectedRules = new List<RuleReference>();
    private Dictionary<string, Func<TweetData, Terms>> conditionFunctions = new Dictionary<string, Func<TweetData, Terms>>();

    public ButtonFlagImage[] buttonFlagImage; // �C���X�y�N�^�[�Őݒ肷��y�A
    private Dictionary<ButtonFlag, ButtonFlagImage> buttonFlagImages = new Dictionary<ButtonFlag, ButtonFlagImage>();

    public RuleDisplay[] ruleDisplays;


    public void InitializeRules()
    {
        availableConditions = new Condition[]
        {
            new Condition("�t�H���[�����Ă�����", FollowCondition),
            new Condition("�P�O���ȓ��Ȃ�", TimeCondition),
            new Condition("NG���[�h����������", KeywordCondition),
            new Condition("�摜����������", ImageCondition)
        };

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

        InitializeButtonFlagImages();
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


    private void InitializeButtonFlagImages()
    {
        foreach (var pair in buttonFlagImage)
        {
            if (!buttonFlagImages.ContainsKey(pair.buttonFlag))
            {
                buttonFlagImages.Add(pair.buttonFlag, pair);
            }
            else
            {
                Debug.LogWarning("����ButtonFlag��������ݒ肳��Ă��܂�: " + pair.buttonFlag);
            }
        }
    }

    private ButtonFlagImage GetButtonFlag(ButtonFlag flag)
    {
        if (buttonFlagImages.TryGetValue(flag, out ButtonFlagImage outimage))
        {
            return outimage;
        }
        else
        {
            Debug.LogWarning("�w�肳�ꂽButtonFlag�ɑΉ�����摜��������܂���: " + flag);
            return null;
        }
    }

    public ButtonFlag ApplyRules(TweetData tweetData, List<RuleReference> selectedRules)
    {
        if (tweetData == null)
        {
            Debug.LogError("TweetData������܂���I");
            return ButtonFlag.None;
        }

        // �I�����ꂽ���[��������Ȃ��ꍇ�A������Other��Ԃ�
        if (selectedRules == null || selectedRules.Count == 0)
        {
            Debug.LogWarning("�I�����ꂽ���[��������܂���I");
            return ButtonFlag.Other;
        }

        ButtonFlag result = ButtonFlag.None;

        foreach (var selectedRule in selectedRules)
        {
            if (string.IsNullOrEmpty(selectedRule.conditionName))
            {
                Debug.LogWarning("�I�����ꂽ���[���ɏ�����������܂���I");
                continue;
            }

            if (conditionFunctions.ContainsKey(selectedRule.conditionName))
            {
                Terms terms = conditionFunctions[selectedRule.conditionName](tweetData);

                if (terms == Terms.OR)
                {
                    result |= selectedRule.actionFlag;
                }
                else if (terms == Terms.Equal)
                {
                    result = selectedRule.actionFlag;
                    break;
                }
            }
            else
            {
                Debug.LogWarning("������������܂���: " + selectedRule.conditionName);
            }
        }

        Debug.Log("���[������: " + result.ToString() + ", Bool:" + tweetData.IsFollow + ", Int:" + tweetData.Time);
        return result;
    }


    public void CheckAction(ButtonFlag correctAction, bool shouldLike, bool shouldRetweet, bool shouldBookmark, bool shouldReport)
    {
        ButtonFlag userAction = ButtonFlag.None;

        if (shouldLike)
        {
            userAction |= ButtonFlag.Like;
        }

        if (shouldRetweet)
        {
            userAction |= ButtonFlag.Retweet;
        }

        if (shouldBookmark)
        {
            userAction |= ButtonFlag.Bookmark;
        }

        if (shouldReport)
        {
            userAction |= ButtonFlag.Report;
        }

        bool isCorrect = correctAction == ButtonFlag.Other || userAction == correctAction;

        string userActionText = userAction.ToString();
        string correctActionText = correctAction.ToString();
        string statusText = isCorrect ? "<color=green><b>TRUE!</b></color>" : "<color=red><b>FALSE!</b></color>";
        string logMessage = $"{userActionText} == {correctActionText} => {statusText}";
        string finalLogMessage = $"{System.DateTime.Now} - {logMessage}";

        Debug.LogWarning(finalLogMessage);

        if (Log != null)
        {
            UpdateLog(logMessage);
        }

        if (isCorrect)
        {
            Followplus.EvaluateAction(true);
            CorrectSE.Invoke();
        }
        else
        {
            Followplus.EvaluateAction(false);
            IncorrectSE.Invoke();
        }
    }

    public void SendReturn(bool value)
    {
        if (value)
        {
            Followplus.EvaluateAction(false);
            Flick.SetTrigger("Flick"); // �g���K�[���N��
            IncorrectSE.Invoke();
        }
    }

    public int QuickCheck(ButtonFlag correctAction, bool shouldLike, bool shouldRetweet, bool shouldBookmark, bool shouldReport , bool Isnot)
    {
        ButtonFlag userAction = ButtonFlag.None;

        // �w�肳�ꂽ�����Ɋ�Â��ăt���O��ǉ�
        if (shouldLike)
        {
            userAction |= ButtonFlag.Like;
        }

        if (shouldRetweet)
        {
            userAction |= ButtonFlag.Retweet;
        }

        if (shouldBookmark)
        {
            userAction |= ButtonFlag.Bookmark;
        }

        if (shouldReport)
        {
            userAction |= ButtonFlag.Report;
        }

        // �������ǂ����̍ŏI����
        bool isCorrect = correctAction == ButtonFlag.Other || userAction == correctAction;
        bool isIncorrect = !isCorrect && ((userAction & ~correctAction) != 0);

        string userActionText = userAction.ToString();
        string correctActionText = correctAction.ToString();
        string statusText = isCorrect ? "<color=green><b>TRUE!</b></color>" : (isIncorrect ? "<color=red><b>FALSE!</b></color>" : "<color=yellow><b>CONTINUE</b></color>");
        string logMessage = $"{userActionText} == {correctActionText} => {statusText}";
        string finalLogMessage = $"{System.DateTime.Now} - {logMessage}";

        Debug.LogWarning(finalLogMessage);

        if (Log != null)
        {
            UpdateLog(logMessage);
        }

        // ���茋�ʂɉ����ď����𕪊�
        if (isCorrect)
        {
            if (!Isnot)
            {
                Followplus.EvaluateAction(true);
                CorrectSE.Invoke();
            }
            return 1;
        }
        else if (isIncorrect)
        {
            if (!Isnot)
            {
                Followplus.EvaluateAction(false);
                IncorrectSE.Invoke();
            }
            return 2;
        }
        else
        {
            if (!Isnot)
            {
                ContinueSE.Invoke();
            }
            return 0;
        }
    }

    public void UpdateLog(string newLog)
    {
        StartCoroutine(UpdateLogCoroutine(newLog));
    }

    private IEnumerator UpdateLogCoroutine(string newLog)
    {
        Log.enabled = false;
        yield return new WaitForSeconds(0.1f);

        string[] currentLog = Log.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> updatedLog = new List<string>(currentLog);

        Debug.Log("Before adding new log:");
        foreach (var log in updatedLog)
        {
            Debug.Log(log);
        }

        updatedLog.Add(newLog);
        if (updatedLog.Count > Logs)
        {
            updatedLog.RemoveAt(0);
        }

        Debug.Log("After adding new log:");
        foreach (var log in updatedLog)
        {
            Debug.Log(log);
        }

        Log.text = string.Join("\n", updatedLog);
        Log.enabled = true;

        Debug.Log("Final log text: " + Log.text);
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

                selectedRules.Add(new RuleReference { conditionName = conditionName, actionFlag = actionFlag });
                Debug.LogWarning("�V�������[����ǉ����܂����I" + conditionName + actionFlag);
                DisplaySelectedRules();
                return;
            }
        }

        Debug.LogWarning("�������ɑΉ�����֐���������܂���ł���: " + conditionName);
    }

    [ContextMenu("Rules�\���I")]
    public void DisplaySelectedRules()
    {
        bool hasRules = selectedRules.Count > 0;

        for (int i = 0; i < ruleDisplays.Length; i++)
        {
            if (i < selectedRules.Count)
            {
                RuleReference rule = selectedRules[i];
                ButtonFlagImage flagdata = GetButtonFlag(rule.actionFlag);
                if (flagdata != null && rule != null)
                {
                    ruleDisplays[i].ruleText.text = $"{rule.conditionName} {flagdata.flagName}";
                    ruleDisplays[i].ruleImage.sprite = flagdata.image;
                }
            }
            else
            {
                // �I�����ꂽ���[�����Ȃ��ꍇ�̏������L�q����ꍇ�͂����ɒǉ����܂�
            }
        }

        // selectedRules�̒�����0�̏ꍇ�̏���
        if (!hasRules && NoneRule != null)
        {
            NoneRule.SetActive(true);
        }
        else if (NoneRule != null)
        {
            NoneRule.SetActive(false);
        }
    }



    private Terms FollowCondition(TweetData tweetData)
    {
        return tweetData.IsFollow ? Terms.OR : Terms.False;
    }

    private Terms TimeCondition(TweetData tweetData)
    {
        return tweetData.Time <= 10 ? Terms.OR : Terms.False;
    }

    private Terms KeywordCondition(TweetData tweetData)
    {
        return keywordChecker.CheckForKeyword(tweetData.Text) ? Terms.Equal : Terms.False;
    }

    private Terms ImageCondition(TweetData tweetData)
    {
        return tweetData.IsImage ? Terms.OR : Terms.False;
    }
}
