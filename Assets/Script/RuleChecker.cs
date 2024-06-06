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

    // インスペクターから選択するためのルールリスト
    public List<RuleReference> selectedRules = new List<RuleReference>();
    private Dictionary<string, Func<TweetData, ButtonFlag, ButtonFlag>> ruleFunctions = new Dictionary<string, Func<TweetData, ButtonFlag, ButtonFlag>>();

    public void InitializeRules()
    {
        availableRules = new Rule[]
        {
            new Rule("フォローをしてたら", LikeCheck),
            new Rule("１０分以内なら", RetweetCheck),
            new Rule("キーワードがあったら", ReportCheck),
            new Rule("何もなかったら", BookmarkCheck),
        };

        // 利用可能なルールを辞書に登録
        foreach (var rule in availableRules)
        {
            // ルール名がすでに存在するかどうかをチェック
            if (!ruleFunctions.ContainsKey(rule.ruleName))
            {
                // 辞書にルールを登録
                ruleFunctions.Add(rule.ruleName, rule.ruleFunction);
            }
            else
            {
                // 既に同じ名前のルールが存在する場合は警告を出力
                Debug.LogWarning("ルールがすでに存在します: " + rule.ruleName);
            }
        }
    }

    private void Start()
    {
        if (keywordChecker == null) { Debug.LogWarning("キーワードチェッカーがありません！"); }
    }

    public ButtonFlag ApplyRules(TweetData tweetData)
    {
        if (tweetData == null)
        {
            Debug.LogError("TweetDataがありません！");
            return ButtonFlag.None;
        }

        ButtonFlag result = ButtonFlag.None;

        foreach (var selectedRule in selectedRules)
        {
            if (ruleFunctions.ContainsKey(selectedRule.ruleName))
            {
                // 選択されたルールの関数を取得し、元の結果と tweetData を渡して適用する
                result = ruleFunctions[selectedRule.ruleName](tweetData, result);
            }
            else
            {
                Debug.LogWarning("ルールが見つかりません: " + selectedRule.ruleName);
            }
        }

        Debug.Log("ルール結果: " + result.ToString() + ", Bool:" + tweetData.someBool + ", Int:" + tweetData.someInt);
        return result;
    }

    // 例としてのルール関数
    private ButtonFlag LikeCheck(TweetData tweetData, ButtonFlag original)
    {
        if (tweetData.someBool)
        {
            return ButtonFlag.Like;
        }
        return original; // 元々の値を返す
    }

    private ButtonFlag RetweetCheck(TweetData tweetData, ButtonFlag original)
    {
        if (tweetData.someInt <= 10)
        {
            return ButtonFlag.Retweet;
        }
        return original; // 元々の値を返す
    }

    private ButtonFlag ReportCheck(TweetData tweetData, ButtonFlag original)
    {
        bool IsKey = keywordChecker.CheckForKeyword(tweetData.someString);

        if (IsKey)
        {
            return ButtonFlag.Like;
        }

        return original; // 元々の値を返す
    }

    private ButtonFlag BookmarkCheck(TweetData tweetData, ButtonFlag original)
    {
        if(original == ButtonFlag.None) {

            return ButtonFlag.Bookmark;
        }
        return original; // 元々の値を返す
    }
}
