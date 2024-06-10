using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using static RuleChecker;

public class GameManager : MonoBehaviour
{
    public followplus followPlusScript; // followplusスクリプトへの参照
    public TimelineManager timelineManager; // TimelineManagerスクリプトへの参照
    public RuleChecker ruleChecker; // RuleCheckerスクリプトへの参照

    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();
    private bool IsGameover = false;
    private int currentEventIndex = 0; // 現在のイベントインデックス

    void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("フォロープラスがありません！"); }
        if (timelineManager == null) { Debug.LogWarning("タイムラインマネージャーがありません！"); }
        if (ruleChecker == null) { Debug.LogWarning("ルールチェッカーがありません！"); }

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
        if(followPlusScript.followers < 0)
        {
            if(!IsGameover)
            {
                GameOver();
                IsGameover = false;
            }

        }
        else if (currentEventIndex < difficultyEvents.Count)
        {
            var eventInfo = difficultyEvents[currentEventIndex];

            if (followPlusScript.maxFollowers > eventInfo.followerThreshold)
            {
                if(!eventInfo.IsExecuted)
                {
                    // イベントを実行する
                    ExecuteEvent(eventInfo);

                    eventInfo.IsExecuted = true;
                }
                    // 次のイベントに進む
                    currentEventIndex++;
            }
        }
    }

    // イベントを実行する関数
    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        // ツイート間隔とスピードの減少
        timelineManager.tweetCooldown -= eventInfo.tweetCooldownReduction;
        timelineManager.tweetSpeedTime -= eventInfo.tweetSpeedReduction;

        // スタックツイートIDの追加
        timelineManager.stackTweetIDs.Add(eventInfo.tweetIDToAdd);

        StartCoroutine(AddRuleWithDelay(eventInfo.ruleFunctionName, eventInfo.actionFlag, 10f));

        Debug.Log($"Event executed: {eventInfo.ruleFunctionName} added with action {eventInfo.actionFlag}");
    }

    public void GameOver()
    {

    }

    private IEnumerator AddRuleWithDelay(string ruleFunctionName, ButtonFlag actionFlag, float delay)
    {
        yield return new WaitForSeconds(delay); // 遅延時間待機

        // ルールの追加
        ruleChecker.AddRule(ruleFunctionName, actionFlag);

        Debug.Log($"Event executed with delay: {ruleFunctionName} added with action {actionFlag}");
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
    public RuleChecker.ButtonFlag actionFlag; // ButtonFlag
    public bool IsExecuted;                 // イベントが実行されたかどうか

    public DifficultyEvent(int followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.actionFlag = actionFlag;
        this.IsExecuted = false; // 初期状態では実行されていない
    }
}
