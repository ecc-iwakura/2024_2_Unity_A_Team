using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public followplus followPlusScript; // followplusスクリプトへの参照
    public TimelineManager timelineManager; // TimelineManagerスクリプトへの参照
    public RuleChecker ruleChecker; //  RuleCheckerスクリプトへの参照


    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();

    public void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("フォロープラスがありません！"); }
        if (timelineManager == null) { Debug.LogWarning("タイムラインマネージャーがありません！"); }
        if (ruleChecker == null) { Debug.LogWarning("ルールチェッカーがありません！"); }
    }
    // フォロワー数とfollowerThresholdを比較し、イベントを実行する
    public void CheckFollowerThreshold()
    {
        foreach (var eventInfo in difficultyEvents)
        {
            if (followPlusScript.maxFollowers > eventInfo.followerThreshold)
            {
                // イベントを実行する
                ExecuteEvent(eventInfo);
                // followerThresholdの少ない順に並び替える
                difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
                return;
            }
        }
    }

    // イベントを実行する関数
    public void ExecuteEvent(DifficultyEvent eventInfo)
    {
        // ここでイベントに伴う処理を行う
    }
}

// DifficultyEventクラスの定義
[System.Serializable]
public class DifficultyEvent
{
    public int followerThreshold;           // フォロワー数がこの数を超えたらイベントを開始する
    public float tweetCooldownReduction;    // ツイート間隔の減少量
    public float tweetSpeedReduction;       // ツイートスピードの減少量
    public string tweetIDToAdd;             // 追加するツイートID
    public string ruleFunctionName;         // 追加するルール関数名
    public RuleChecker.ButtonFlag actionFlag; // ButtonFlag

    public DifficultyEvent(int followerThreshold, float tweetCooldownReduction, float tweetSpeedReduction, string tweetIDToAdd, string ruleFunctionName, RuleChecker.ButtonFlag actionFlag)
    {
        this.followerThreshold = followerThreshold;
        this.tweetCooldownReduction = tweetCooldownReduction;
        this.tweetSpeedReduction = tweetSpeedReduction;
        this.tweetIDToAdd = tweetIDToAdd;
        this.ruleFunctionName = ruleFunctionName;
        this.actionFlag = actionFlag;
    }
}
