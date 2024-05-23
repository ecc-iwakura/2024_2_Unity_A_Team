using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


public class TimelineManager : MonoBehaviour
{
    public GameObject tweetPrefab;         // ツイートプレハブ
    public RectTransform timeline;         // タイムラインのRectTransform
    public Transform spawnPoint;           // スポーン地点
    public int maxTweets = 10;             // 最大ツイート数

    [SerializeField]
    private List<TweetObjectData> tweetObjectList = new List<TweetObjectData>(); // ツイートオブジェクトとTweetScriptのセットのリスト

    private float currentYPosition = 0f;   // 現在のY位置
    public float tweetCooldown = 3f;       // ツイートの間隔（秒）

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
            if (!isTweetMoving)
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
        // ランダムなツイートデータを生成
        (string text, Sprite image, string accountName, string accountID) = GenerateRandomTweetData();

        GameObject newTweet = null;
        RectTransform tweetRect = null;
        if (tweetObjectList.Count < maxTweets)
        {
            UnityEngine.Debug.Log("新品生成！");
            // スポーン地点にツイートプレハブを生成
            newTweet = Instantiate(tweetPrefab, spawnPoint.position, Quaternion.identity, timeline);

            // 新しいツイートのTweetScriptコンポーネントを取得
            TweetScript tweetScript = newTweet.GetComponent<TweetScript>();

            tweetRect = newTweet.GetComponent<RectTransform>();
            // ツイートの内容を設定
            tweetScript.UpdateTweet(text, image, accountName, accountID);

            // ツイートオブジェクトとTweetScriptのセットをリストに追加
            tweetObjectList.Add(new TweetObjectData(newTweet, tweetScript, tweetRect));
        }
        else
        {
            UnityEngine.Debug.Log("再利用！");
            // リストで１番古いツイートを取得して再利用
            var oldTweetObjectData = tweetObjectList[0];
            tweetObjectList.RemoveAt(0);

            // ツイートの内容を更新
            oldTweetObjectData.tweetScript.UpdateTweet(text, image, accountName, accountID);

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

        // タイムラインをツイートの高さ分だけ下に移動
        StartCoroutine(MoveTimeline(tweetHeight));
    }

    // コルーチンでタイムラインをゆっくり下に移動
    private IEnumerator MoveTimeline(float tweetHeight)
    {
        isTweetMoving = true; // ツイートが移動中であることを示すフラグを設定

        float duration = tweetCooldown; // 移動にかける基本時間

        // 移動にかける時間を距離に応じて調整
        float distanceFactor = Mathf.Clamp(tweetHeight / 100f, 0.5f, 2f);
        duration *= distanceFactor;

        float distance = tweetHeight; // 移動距離

        Vector2 startPos = timeline.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, tweetHeight); // 下に移動

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            timeline.anchoredPosition = Vector2.Lerp(startPos, endPos, t * t); // 移動をより滑らかにするために t * t を使用

            // 距離に応じて時間を調整
            elapsed += Time.deltaTime;
            yield return null;
        }

        timeline.anchoredPosition = endPos;

        // 現在のY位置を更新
        currentYPosition += tweetHeight;
        timeline.sizeDelta = new Vector2(timeline.sizeDelta.x, currentYPosition);

        yield return new WaitForSeconds(tweetCooldown); // クールダウン

        isTweetMoving = false; // ツイートが移動中でないことを示すフラグをリセット
    }

    // ランダムなツイートデータを生成するメソッド
    private (string, Sprite, string, string) GenerateRandomTweetData()
    {
        string[] randomTexts = { "これはテストツイートです", "これはテストツイートです。これはテストツイートです。これはテストツイートです。", "これはテストツイートです。これはテストツイートです。これはテストツイートです。これはテストツイートです。これはテストツイートです。" };
        string[] randomAccountNames = { "TestTweet", "TestTweetです！", "TestTweetだってば" };
        string[] randomAccountIDs = { "test_account", "random_account1", "another_account" };

        string text = randomTexts[Random.Range(0, randomTexts.Length)];
        string accountName = randomAccountNames[Random.Range(0, randomAccountNames.Length)];
        string accountID = randomAccountIDs[Random.Range(0, randomAccountIDs.Length)];

        // ランダムな画像を生成（今回はnullのままにしておきますが、画像を追加する場合は適切に設定）
        Sprite image = null; // 画像が必要な場合はランダムに画像を選択する処理を追加

        return (text, image, accountName, accountID);
    }
}