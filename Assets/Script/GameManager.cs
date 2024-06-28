using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    public followplus followPlusScript;
    public TimelineManager timelineManager;
    public RuleChecker ruleChecker;
    public KeywordChecker keywordChecker;

    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();
    private bool IsGameover = false;
    private int currentEventIndex = 0;
    public TMP_Text GameOverText;
    public UnityEvent GameOverEvent;
    public AudioSource audioSource; // オーディオソースを追加
    private float initialTweetSpeedTime; // 初期のtweetSpeedTimeを保持する変数

    void Start()
    {
        if (followPlusScript == null) { Debug.LogWarning("フォロープラスがありません！"); }
        if (timelineManager == null) { Debug.LogWarning("タイムラインマネージャーがありません！"); }
        if (ruleChecker == null) { Debug.LogWarning("ルールチェッカーがありません！"); }
        if (keywordChecker == null) { Debug.LogWarning("キーワードチェッカーがありません！"); }

        difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
        initialTweetSpeedTime = timelineManager.tweetSpeedTime; // 初期値を記録
    }

    void Update()
    {
        CheckFollowerThreshold();
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

            if (followPlusScript.maxFollowers > (ulong)eventInfo.followerThreshold)
            {
                if (!eventInfo.IsExecuted)
                {
                    eventInfo.IsExecuted = true;
                    ExecuteEvent(eventInfo);
                    Debug.Log($"イベント実行");
                }

                currentEventIndex++;
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

        // Adjust tweet cooldown reduction
        timelineManager.tweetCooldown *= (1.0f + eventInfo.tweetCooldownReduction);

        // Adjust tweet speed reduction
        timelineManager.tweetSpeedTime *= (1.0f + eventInfo.tweetSpeedReduction);

        float speedRatio = initialTweetSpeedTime / timelineManager.tweetSpeedTime;
        audioSource.pitch = speedRatio;

        if (!string.IsNullOrEmpty(eventInfo.ruleFunctionName) && eventInfo.actionFlag != null)
        {
            AddRuleTweet addRuleTweet = new AddRuleTweet(eventInfo.tweetIDToAdd, eventInfo.ruleFunctionName, eventInfo.actionFlag);
            timelineManager.stackTweetIDs.Add(addRuleTweet);
        }

        if (!string.IsNullOrEmpty(eventInfo.keyWord))
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
        IsGameover = true;
    }

    public void SaveGameData(string filePath)
    {
        // シリアライズするオブジェクトを配列に変換する
        var eventArray = difficultyEvents.ToArray();

        // JSONにシリアライズする
        string jsonData = JsonHelper.ToJson(eventArray, true);

        // ファイルに書き込む
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Game data saved to: " + filePath);
    }

    public void LoadGameData(string filePath)
    {
        if (File.Exists(filePath))
        {
            // ファイルからJSONデータを読み込む
            string jsonData = File.ReadAllText(filePath);

            // JSONをデシリアライズしてオブジェクトに戻す
            var eventArray = JsonHelper.FromJson<DifficultyEvent>(jsonData);

            // リストに戻す
            difficultyEvents = new List<DifficultyEvent>(eventArray);

            Debug.Log("Game data loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("No saved game data found at: " + filePath);
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

// JsonUtilityを使用してジェネリックなJsonHelperクラスを作成する
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
