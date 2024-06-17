using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static RuleChecker;
using static TimelineManager;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    public followplus followPlusScript; // followplusスクリプトへの参照
    public TimelineManager timelineManager; // TimelineManagerスクリプトへの参照
    public RuleChecker ruleChecker; // RuleCheckerスクリプトへの参照
    public KeywordChecker keywordChecker; // キーワードチェッカーへの参照

    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();
    private bool IsGameover = false;
    private int currentEventIndex = 0; // 現在のイベントインデックス
    public TMP_Text GameOverText;
    public UnityEvent GameOverEvent;


    void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("フォロープラスがありません！"); }

        if (timelineManager == null) { Debug.LogWarning("タイムラインマネージャーがありません！"); }

        if (ruleChecker == null) { Debug.LogWarning("ルールチェッカーがありません！"); }

        if (keywordChecker == null) { Debug.LogWarning("キーワードチェッカーがありません！"); }

        // followerThresholdの少ない順に並び替える
        difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
    }

    void Update()
    {
        CheckFollowerThreshold();
    }

    // フォロワー数とfollowerThresholdを比較し、イベントを実行する
    public void CheckFollowerThreshold()
    {
        // followPlusScript が Null の場合は処理を中断する
        if (followPlusScript == null)
        {
            Debug.LogError("followPlusScript is null in CheckFollowerThreshold!");
            return;
        }

        // followers が 0以下の場合、ゲームオーバー処理を実行する
        if (followPlusScript.followers <= 0)
        {
            if (!IsGameover)
            {
                GameOver();
                IsGameover = true;
            }
        }
        else if (currentEventIndex < difficultyEvents.Count)
        {
            // difficultyEvents が Null の場合は処理を中断する
            if (difficultyEvents == null)
            {
                Debug.LogError("difficultyEvents is null in CheckFollowerThreshold!");
                return;
            }

            var eventInfo = difficultyEvents[currentEventIndex];

            // eventInfo が Null の場合は処理を中断する
            if (eventInfo == null)
            {
                Debug.LogError($"eventInfo at index {currentEventIndex} is null in CheckFollowerThreshold!");
                return;
            }

            if (followPlusScript.maxFollowers > eventInfo.followerThreshold)
            {
                if (!eventInfo.IsExecuted)
                {
                    eventInfo.IsExecuted = true;
                    // イベントを実行する
                    ExecuteEvent(eventInfo);

                    Debug.LogError($"イベント実行");
                }

                // 次のイベントに進む
                currentEventIndex++;
            }
        }
    }


    // イベントを実行する関数
    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        if (eventInfo == null)
        {
            Debug.LogError("EventInfo is null!");
            return;
        }
        // ツイート間隔とスピードの減少
        if (eventInfo.tweetCooldownReduction != 0)
        {
            timelineManager.tweetCooldown += eventInfo.tweetCooldownReduction;
        }

        if (eventInfo.tweetSpeedReduction != 0)
        {
            timelineManager.tweetSpeedTime += eventInfo.tweetSpeedReduction;
        }

        // ルールの追加が必要な場合
        if (!string.IsNullOrEmpty(eventInfo.ruleFunctionName) && eventInfo.actionFlag != null)
        {
            // AddRuleTweet オブジェクトを作成して追加
            AddRuleTweet addRuleTweet = new AddRuleTweet(eventInfo.tweetIDToAdd, eventInfo.ruleFunctionName, eventInfo.actionFlag);
            timelineManager.stackTweetIDs.Add(addRuleTweet); // AddRuleTweet を適切に追加
        }

        if (eventInfo.keyWord != null && !string.IsNullOrEmpty(eventInfo.keyWord))
        {
            keywordChecker.AddKeyword(eventInfo.keyWord);
        }

        Debug.Log($"Event executed: {eventInfo.ruleFunctionName} added with action {eventInfo.actionFlag}");
    }

    public void GameOver()
    {
        GameOverText.text = followPlusScript.maxFollowers.ToString() + "フォロワー";
        timelineManager.IsStop = true;
        GameOverEvent.Invoke();
    }


}


[System.Serializable]
public class DifficultyEvent
{
    public int followerThreshold;           // フォロワー数がこの数を超えたらイベントを開始する
    public float tweetCooldownReduction;    // ツイート間隔の減少量
    public float tweetSpeedReduction;       // ツイートスピードの減少量
    public string tweetIDToAdd;             // 追加するツイートID
    public string ruleFunctionName;         // 追加するルール関数名
    public string keyWord;         // 追加するルール関数名
    public RuleChecker.ButtonFlag actionFlag; // ButtonFlag
    public bool IsExecuted;                 // イベントが実行されたかどうか

    public DifficultyEvent(int followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, string keyWord, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.keyWord = keyWord;
        this.actionFlag = actionFlag;
        this.IsExecuted = false; // 初期状態では実行されていない
    }
}
