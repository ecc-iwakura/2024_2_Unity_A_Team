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
        public Func<TweetData, ButtonFlag> ruleFunction;

        public Rule(string name, Func<TweetData, ButtonFlag> function)
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
    // インスペクターから選択するためのルールリスト
    public List<RuleReference> selectedRules = new List<RuleReference>();

    private void Awake()
    {
        // 利用可能なルールを配列に追加
        availableRules = new Rule[]
        {
            new Rule("ExampleRule", ExampleRuleFunction),
            new Rule("AnotherRule", AnotherRuleFunction),
            new Rule("LikeIfFollowing", LikeIfFollowing)
        };
    }

    private void Start()
    {
        // 例として、ツイートデータを作成
        TweetData tweet = new TweetData(true, 15, "example");

        // 選択されたルールを適用
        ButtonFlag result = ApplyRules(tweet);
        Debug.Log(result);
    }

    public ButtonFlag ApplyRules(TweetData tweetData)
    {
        ButtonFlag result = ButtonFlag.None;

        foreach (var selectedRule in selectedRules)
        {
            var rule = Array.Find(availableRules, r => r.ruleName == selectedRule.ruleName);
            if (rule != null)
            {
                result = rule.ruleFunction(tweetData);
                if (result != ButtonFlag.None)
                {
                    break;
                }
            }
        }

        return result;
    }

    // 例としてのルール関数
    private ButtonFlag ExampleRuleFunction(TweetData tweetData)
    {
        if (tweetData.someBool)
        {
            return ButtonFlag.Like;
        }
        return ButtonFlag.None;
    }

    private ButtonFlag AnotherRuleFunction(TweetData tweetData)
    {
        if (tweetData.someInt > 10)
        {
            return ButtonFlag.Retweet;
        }
        return ButtonFlag.None;
    }

    private ButtonFlag LikeIfFollowing(TweetData tweetData)
    {
        if (tweetData.someBool && tweetData.someString == "example")
        {
            return ButtonFlag.Like;
        }
        return ButtonFlag.None;
    }
}
