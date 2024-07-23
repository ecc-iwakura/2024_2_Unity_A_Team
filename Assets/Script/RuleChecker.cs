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
        public string flagName; // ボタンフラグの名前を格納するためのフィールドを追加
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


    [Tooltip("シーン内の 'Followplus' オブジェクトへの参照。")]
    public followplus Followplus;

    public bool IsQuick = false;
    public GameObject NoneRule; // NoneRuleというGameObjectの参照をインスペクターから設定してください

    public UnityEvent CorrectSE;
    public UnityEvent IncorrectSE;
    public UnityEvent ContinueSE;

    public Animator Flick; // Animatorの参照を追加

    public int Logs = 5;

    [SerializeField]
    public Condition[] availableConditions;

    public List<RuleReference> selectedRules = new List<RuleReference>();
    private Dictionary<string, Func<TweetData, Terms>> conditionFunctions = new Dictionary<string, Func<TweetData, Terms>>();

    public ButtonFlagImage[] buttonFlagImage; // インスペクターで設定するペア
    private Dictionary<ButtonFlag, ButtonFlagImage> buttonFlagImages = new Dictionary<ButtonFlag, ButtonFlagImage>();

    public RuleDisplay[] ruleDisplays;


    public void InitializeRules()
    {
        availableConditions = new Condition[]
        {
            new Condition("フォローをしていたら", FollowCondition),
            new Condition("１０分以内なら", TimeCondition),
            new Condition("NGワードがあったら", KeywordCondition),
            new Condition("画像があったら", ImageCondition)
        };

        foreach (var condition in availableConditions)
        {
            if (!conditionFunctions.ContainsKey(condition.conditionName))
            {
                conditionFunctions.Add(condition.conditionName, condition.conditionFunction);
            }
            else
            {
                Debug.LogWarning("条件がすでに存在します: " + condition.conditionName);
            }
        }

        InitializeButtonFlagImages();
        DisplaySelectedRules();
    }

    private void Start()
    {
        if (keywordChecker == null) { Debug.LogWarning("キーワードチェッカーがありません！"); }

        Followplus = GameObject.Find("FollowPlus").GetComponent<followplus>();
        if (Followplus == null)
        {
            Debug.LogError("Followplusが見つかりませんでした！");
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
                Debug.LogWarning("同じButtonFlagが複数回設定されています: " + pair.buttonFlag);
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
            Debug.LogWarning("指定されたButtonFlagに対応する画像が見つかりません: " + flag);
            return null;
        }
    }

    public ButtonFlag ApplyRules(TweetData tweetData, List<RuleReference> selectedRules)
    {
        if (tweetData == null)
        {
            Debug.LogError("TweetDataがありません！");
            return ButtonFlag.None;
        }

        // 選択されたルールが一つもない場合、直ちにOtherを返す
        if (selectedRules == null || selectedRules.Count == 0)
        {
            Debug.LogWarning("選択されたルールがありません！");
            return ButtonFlag.Other;
        }

        ButtonFlag result = ButtonFlag.None;

        foreach (var selectedRule in selectedRules)
        {
            if (string.IsNullOrEmpty(selectedRule.conditionName))
            {
                Debug.LogWarning("選択されたルールに条件名がありません！");
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
                Debug.LogWarning("条件が見つかりません: " + selectedRule.conditionName);
            }
        }

        Debug.Log("ルール結果: " + result.ToString() + ", Bool:" + tweetData.IsFollow + ", Int:" + tweetData.Time);
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
            Flick.SetTrigger("Flick"); // トリガーを起動
            IncorrectSE.Invoke();
        }
    }

    public int QuickCheck(ButtonFlag correctAction, bool shouldLike, bool shouldRetweet, bool shouldBookmark, bool shouldReport , bool Isnot)
    {
        ButtonFlag userAction = ButtonFlag.None;

        // 指定された条件に基づいてフラグを追加
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

        // 正解かどうかの最終判定
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

        // 判定結果に応じて処理を分岐
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
                        Debug.LogWarning("選択された条件名は既に選択されています: " + conditionName);
                        return;
                    }
                }

                selectedRules.Add(new RuleReference { conditionName = conditionName, actionFlag = actionFlag });
                Debug.LogWarning("新しくルールを追加しました！" + conditionName + actionFlag);
                DisplaySelectedRules();
                return;
            }
        }

        Debug.LogWarning("条件名に対応する関数が見つかりませんでした: " + conditionName);
    }

    [ContextMenu("Rules表示！")]
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
                // 選択されたルールがない場合の処理を記述する場合はここに追加します
            }
        }

        // selectedRulesの長さが0の場合の処理
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
