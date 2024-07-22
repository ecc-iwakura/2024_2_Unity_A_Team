using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using TMPro;
using System.IO;

public class GameManager : MonoBehaviour
{
    [Header("ゲームオブジェクト参照")]
    public followplus followPlusScript;      // フォロープラスのスクリプト
    public TimelineManager timelineManager;  // タイムラインマネージャー
    public RuleChecker ruleChecker;          // ルールチェッカー
    public KeywordChecker keywordChecker;    // キーワードチェッカー

    [Header("ゲーム設定")]
    public List<DifficultyEvent> difficultyEvents = new List<DifficultyEvent>();  // 難易度イベントのリスト
    private bool IsGameover = false;       // ゲームオーバーのフラグ
    private int currentEventIndex = 0;     // 現在のイベントインデックス
    public TMP_Text GameOverText;          // ゲームオーバー時に表示するテキスト
    public UnityEvent GameOverEvent;       // ゲームオーバー時に発行するイベント
    public GameObject maskObject;
    public GameObject maskObject2;

    [Header("次レベル設定")]
    public float animationDuration = 0.2f; // アニメーションの時間（秒）
    private float elapsedTime = 0f; // 現在の経過時間
    private Vector3 initialPosition; // 初期のマスクオブジェクトのローカル座標
    private Vector3 targetPosition; // 目標のマスクオブジェクトのローカル座標

    [Header("その他")]
    public TMP_Text NextLevelFollowerText; // 次のレベルのフォロワー数を表示するテキスト
    private int oldcurrentEventIndex = 0;
    public bool over = false;

    void Start()
    {
        initialPosition = maskObject.transform.localPosition;
        targetPosition = initialPosition;

        NextLevelFollower();
        if (followPlusScript == null) { Debug.LogWarning("フォロープラスがありません！"); }
        if (timelineManager == null) { Debug.LogWarning("タイムラインマネージャーがありません！"); }
        if (ruleChecker == null) { Debug.LogWarning("ルールチェッカーがありません！"); }
        if (keywordChecker == null) { Debug.LogWarning("キーワードチェッカーがありません！"); }

        difficultyEvents.Sort((x, y) => x.followerThreshold.CompareTo(y.followerThreshold));
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

            if (followPlusScript.maxFollowers >= (ulong)eventInfo.followerThreshold)
            {
                if (!eventInfo.IsExecuted)
                {
                    eventInfo.IsExecuted = true;
                    ExecuteEvent(eventInfo);
                    Debug.Log($"イベント実行");
                }

                currentEventIndex++;
                NextLevelFollower();
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

        // AddTweet インスタンスの作成
        AddTweet addTweet = new AddTweet(
            eventInfo.tweetIDToAdd,
            eventInfo.tweetCooldownReduction, // tweetCooldownReduction をパーセンテージとして扱う
            eventInfo.tweetSpeedReduction,    // tweetSpeedReduction をパーセンテージとして扱う
            eventInfo.ruleFunctionName,
            eventInfo.actionFlag,
            eventInfo.keyWord
        );

        // スタックに追加
        timelineManager.stackTweetIDs.Add(addTweet);


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
    public void IsLevUp()
    {
        over = false;
        NextLevelFollower();
    }
    public void NextLevelFollower()
    {
        if(oldcurrentEventIndex != currentEventIndex)
        {
            oldcurrentEventIndex = currentEventIndex;
            over = true;
        }

        if (currentEventIndex < difficultyEvents.Count - 1)
        {
            ulong currentFollowers = followPlusScript.followers;
            ulong threshold = difficultyEvents[currentEventIndex].followerThreshold;

            // Calculate ratio between current followers and threshold, clamped between 0 and 1
            float ratio = Mathf.Clamp01((float)currentFollowers / (float)threshold);

            if(over) { ratio = 1; }
            // Scale the ratio to fit between 0 and 25
            float scaledValue = ratio * 50f;

            // Update the Y position of the maskObject in local space
            targetPosition.y = scaledValue;

            // Start animation coroutine
            StartCoroutine(MoveMaskObject());

            // Update text
            ulong value = threshold - currentFollowers;
            (int intValue, string unit) = followPlusScript.FormatNumber(value);
            NextLevelFollowerText.text = $"{intValue}{unit}";
        }
        else
        {
            NextLevelFollowerText.text = $"これでもうおしまい！";
        }

        if( over ) { NextLevelFollowerText.text = $"UP!"; }
    }

    private IEnumerator MoveMaskObject()
    {
        elapsedTime = 0f;
        Vector3 startPosition = maskObject.transform.localPosition;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationDuration);
            maskObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            maskObject2.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        // Ensure final position is exactly the target position
        maskObject.transform.localPosition = targetPosition;
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
