using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static RuleChecker;
using System;
using UnityEngine.Events;

[System.Serializable]
public class TweetObjectData
{
    public GameObject tweetObject;
    public TweetScript tweetScript;
    public RectTransform tweetRect; // RectTransformを追加

    public TweetObjectData(GameObject tweetObject, TweetScript tweetScript, RectTransform tweetRect)
    {
        this.tweetObject = tweetObject;
        this.tweetScript = tweetScript;
        this.tweetRect = tweetRect; // レクトを追加
    }

    public void Deconstruct(out GameObject tweetObject, out TweetScript tweetScript, out RectTransform tweetRect)
    {
        tweetObject = this.tweetObject;
        tweetScript = this.tweetScript;
        tweetRect = this.tweetRect; // レクトを追加
    }
}

[System.Serializable]
public class AddRuleTweet
{
    public string tweetID;
    public string ruleFunctionName;
    public ButtonFlag ruleFlag;

    public AddRuleTweet(string tweetID, string ruleFunctionName, ButtonFlag ruleFlag)
    {
        this.tweetID = tweetID;
        this.ruleFunctionName = ruleFunctionName;
        this.ruleFlag = ruleFlag;
    }
}


public class TimelineManager : MonoBehaviour
{
    public GameObject tweetPrefab;         // ツイートプレハブ
    public RectTransform timeline;         // タイムラインのRectTransform
    public Transform spawnPoint;           // スポーン地点
    public TweetDatabase tweetDatabase;         // タイムラインのRectTransform
    public KeywordChecker keywordChecker;
    public RuleChecker ruleChecker;
    public int maxTweets = 10;             // 最大ツイート数
    public bool IsStop = false;

    public UnityEvent AddTweetSE;

    [SerializeField]
    private List<TweetObjectData> tweetObjectList = new List<TweetObjectData>(); // ツイートオブジェクトとTweetScriptのセットのリスト


    public List<AddRuleTweet> stackTweetIDs = new List<AddRuleTweet>(); // スタックツイートIDリスト

    private float currentYPosition = 0f;   // 現在のY位置
    public float tweetCooldown = 3f;       // ツイートの間隔（秒）
    public float tweetSpeedTime = 3f;       // ツイートの間隔（秒）

    private bool isTweetMoving = false;    // ツイートが移動中かどうかを示すフラグ

    // 初期化時に開始
    void Start()
    {
        StartCoroutine(GenerateTweets());
    }

    // ツイートを生成するコルーチン
    private IEnumerator GenerateTweets()
    {
        while (true)
        {
            if (!isTweetMoving && !IsStop)
            {
                AddTweet(); // ツイートを追加
            }

            yield return new WaitForSeconds(0.1f); // クールダウン
        }
    }

    // Method to add a new tweet to the timeline
    [ContextMenu("Add Test Tweet")] // インスペクターから呼び出すためのコンテキストメニュー
    public void AddTweet()
    {
        string text = "";
        Sprite image = null;
        Sprite accountImage = null;
        string accountName = "";
        string accountID = "";

        if (stackTweetIDs.Count > 0)
        {
            // スタックツイートIDリストにツイートIDがある場合はそのツイートIDを使ってツイートを生成
            AddRuleTweet stack = stackTweetIDs[0];
            string tweetID = stack.tweetID; // リストの先頭からツイートIDを取得
            stackTweetIDs.RemoveAt(0); // リストから削除
            (text, image, accountImage, accountName, accountID) = GenerateTweetData(tweetID);

            if (!string.IsNullOrEmpty(stack.ruleFunctionName) && stack.ruleFlag != null)
            {
                UnityEngine.Debug.LogError($"到達！{stack.ruleFunctionName} {stack.ruleFlag}");
                ruleChecker.AddRule(stack.ruleFunctionName, stack.ruleFlag);
            }


        }
        else
        {
            (text, image, accountImage, accountName, accountID) = GenerateRandomTweetData();
        }


        GameObject newTweet = null;
        RectTransform tweetRect = null;
        bool isKeyword = false;
        if (tweetObjectList.Count < maxTweets)
        {
            UnityEngine.Debug.Log("新品生成！");
            // スポーン地点にツイートプレハブを生成
            newTweet = Instantiate(tweetPrefab, spawnPoint.position, Quaternion.identity, timeline);

            newTweet.transform.rotation = timeline.rotation;

            // 新しいツイートのTweetScriptコンポーネントを取得
            TweetScript tweetScript = newTweet.GetComponent<TweetScript>();

            tweetRect = newTweet.GetComponent<RectTransform>();

            isKeyword = keywordChecker.CheckForKeyword(text);

            
            // ツイートの内容を設定
            tweetScript.UpdateTweet(text, image, accountImage, accountName, accountID, isKeyword, ruleChecker.selectedRules);

            // ツイートオブジェクトとTweetScriptのセットをリストに追加
            tweetObjectList.Add(new TweetObjectData(newTweet, tweetScript, tweetRect));
        }
        else
        {
            UnityEngine.Debug.Log("再利用！");
            // リストで１番古いツイートを取得して再利用
            var oldTweetObjectData = tweetObjectList[0];

            oldTweetObjectData.tweetScript.TweetCheck();

            tweetObjectList.RemoveAt(0);

            isKeyword = keywordChecker.CheckForKeyword(text);
            // ツイートの内容を更新
            oldTweetObjectData.tweetScript.UpdateTweet(text, image, accountImage, accountName, accountID, isKeyword, ruleChecker.selectedRules);

            oldTweetObjectData.tweetObject.transform.rotation = timeline.rotation;
            // 再利用するツイートオブジェクトの位置を設定
            oldTweetObjectData.tweetObject.transform.position = spawnPoint.position;

            // ツイートオブジェクトとTweetScriptのセットをリストの末尾に追加（再利用）
            tweetObjectList.Add(oldTweetObjectData);

            // newTweet変数を再利用するために初期化
            newTweet = oldTweetObjectData.tweetObject;
            tweetRect = oldTweetObjectData.tweetRect;
        }

        // ツイートの高さを取得

        float tweetHeight = tweetRect.sizeDelta.y * 1.1f;

        AddTweetSE.Invoke();
        // タイムラインをツイートの高さ分だけ下に移動
        StartCoroutine(MoveTimeline(tweetHeight));

    }

    // コルーチンでタイムラインをゆっくり下に移動
    private IEnumerator MoveTimeline(float tweetHeight)
    {
        isTweetMoving = true; // ツイートが移動中であることを示すフラグを設定

        float duration = tweetSpeedTime; // 移動にかける基本時間

        // 移動にかける時間を距離に応じて調整
        float distanceFactor = Mathf.Clamp(tweetHeight / 100f, 0.5f, 2f);
        duration *= distanceFactor;

        Vector2 startPos = timeline.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, tweetHeight); // 下に移動

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // アニメーションを直接計算
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            timeline.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);

            // 経過時間を加算
            elapsed += Time.deltaTime;
            yield return null; // 次のフレームまで待機
        }

        timeline.anchoredPosition = endPos;

        // 現在のY位置を更新
        currentYPosition += tweetHeight;
        timeline.sizeDelta = new Vector2(0, 0);

        yield return new WaitForSeconds(tweetCooldown); // クールダウン

        Vector3[] childWorldPositions = new Vector3[timeline.childCount];
        for (int i = 0; i < timeline.childCount; i++)
        {
            childWorldPositions[i] = timeline.GetChild(i).position;
        }

        // 親オブジェクトの位置を変更
        timeline.anchoredPosition = new Vector2(0.0f, 0.0f);

        // 子オブジェクトの位置を元のワールド座標に戻す
        for (int i = 0; i < timeline.childCount; i++)
        {
            RectTransform childRectTransform = timeline.GetChild(i).GetComponent<RectTransform>();
            childRectTransform.position = childWorldPositions[i];
        }

        isTweetMoving = false; // ツイートが移動中でないことを示すフラグをリセット
    }

    // GenerateRandomTweetDataメソッドの修正
    private (string, Sprite, Sprite, string, string) GenerateRandomTweetData()
    {
        string randomID = tweetDatabase.GetRandomTweetID();

        if (randomID == null)
        {
            Debug.LogWarning("ランダムなツイートIDが取得できませんでした。");
            return ("null", null, null, "null", "null"); // 要素数を5つに修正
        }

        TweetInfo tweetInfo = tweetDatabase.GetTweetInfo(randomID);

        string accountID = tweetDatabase.GetParentAccountID(randomID);
        AccountInfo accountInfo = tweetDatabase.GetAccountInfo(accountID);

        if (tweetInfo == null)
        {
            Debug.LogWarning("ツイート情報が取得できませんでした。ID: " + randomID);
            return ("null", null, null, "null", "null"); // 要素数を5つに修正
        }

        string text = tweetInfo.tweetContent;
        Sprite image = tweetInfo.tweetImageContent;
        Sprite accountImage = accountInfo.accountImage; // フィールドを修正
        string accountName = accountInfo.accountName;

        return (text, image, accountImage, accountName, accountID);
    }

    private (string, Sprite, Sprite, string, string) GenerateTweetData(string ID)
    {
        TweetInfo tweetInfo = tweetDatabase.GetTweetInfo(ID);

        string accountID = tweetDatabase.GetParentAccountID(ID); // randomIDからIDに変更

        if(accountID == null) {
            Debug.LogWarning("アカウントIDがNullでした！");
        }

        AccountInfo accountInfo = tweetDatabase.GetAccountInfo(accountID);

        if (tweetInfo == null)
        {
            Debug.LogWarning("ツイート情報が取得できませんでした。ID: " + ID); // randomIDからIDに変更
            return ("null", null, null, "null", "null");
        }

        string text = tweetInfo.tweetContent;
        Sprite image = tweetInfo.tweetImageContent;
        Sprite accountImage = accountInfo.accountImage;
        string accountName = accountInfo.accountName;

        return (text, image, accountImage, accountName, accountID);
    }
}