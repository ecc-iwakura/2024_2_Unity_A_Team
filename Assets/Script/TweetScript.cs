using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using static RuleChecker;

public class TweetScript : MonoBehaviour
{
    // Public variables to be set in the Unity Editor
    public TMP_Text tweetText;          // ツイートのテキストを表示するTextMeshProコンポーネント
    public Image tweetImage;            // ツイートの画像を表示するUI Imageコンポーネント
    public TMP_Text accountName;        // アカウント名を表示するTextMeshProコンポーネント
    public TMP_Text accountID;          // アカウントIDを表示するTextMeshProコンポーネント
    public Image accountImage;            // ツイートの画像を表示するUI Imageコンポーネント
    public RectTransform tweetContainer; // ツイート全体を囲むUIのRectTransformコンポーネント

    // Fields for tweet data
    [TextArea(3, 10)] 

    public string tweetContent;        // ツイートの文面
    public Sprite tweetImageContent;   // ツイートの画像
    public Sprite tweetAccountImage;    // アカウント名
    public string tweetAccountName;    // アカウント名
    public string tweetAccountID;      // アカウントID

    [Space(10)] // 10のスペースを追加


    [Tooltip("シーン内の 'RuleChecker' オブジェクトへの参照。")]
    public RuleChecker ruleChecker;
    public bool isFollowing;            // フォローしているかどうか
    public float minutesSincePosted;    // 投稿されてからの時間（分）
    public bool IsKeyword; //キーワードが含まれているかどうか

    public TMP_Text ElapsedTime;        // アカウント名を表示するTextMeshProコンポーネント
    public GameObject IsFollow;        // アカウント名を表示するTextMeshProコンポーネント

    [Space(10)] // 10のスペースを追加

    public UnityEvent onReset;
    private bool shouldLike;
    private bool shouldRetweet;
    private bool shouldBookmark;
    private bool shouldReport;

    [SerializeField]
    private ButtonFlag buttonFlag = ButtonFlag.None;


    // Method to update the tweet content
    public void UpdateTweet(string newText, Sprite newImage, Sprite newAccountImage, string newAccountName, string newAccountID ,bool isKeyword)
    {
        // Update the tweet data fields
        tweetContent = newText;
        tweetImageContent = newImage;
        tweetAccountImage = newAccountImage;
        tweetAccountName = newAccountName;
        tweetAccountID = newAccountID;
        IsKeyword = isKeyword;

        shouldLike = false;
        shouldRetweet = false;
        shouldBookmark = false;
        shouldReport = false;

        onReset.Invoke();
        // Update the UI elements
        UpdateUI();
    }

    // Optionally, automatically update the UI when the game starts
    void Start()
    {
        buttonFlag = ButtonFlag.None; // あなたのコードに合わせて初期化してください

        if (ruleChecker == null) { 
            
            Debug.LogError("RuleCheckerが見つかりませんでした！");
            ruleChecker = GameObject.Find("RuleChecker").GetComponent<RuleChecker>();
        }

        UpdateUI();
    }

    // Method to update the UI elements based on the tweet data
    [ContextMenu("Update UI")]
    private void UpdateUI()
    {

        // Update the tweet text
        tweetText.text = tweetContent;

        // Update the tweet image
        tweetImage.sprite = tweetImageContent;
        tweetImage.gameObject.SetActive(tweetImageContent != null);

        // Update the account name and ID
        accountName.text = tweetAccountName;
        accountID.text = "@" + tweetAccountID;
        accountImage.sprite = tweetAccountImage;

        RandomTweetInfo();
        // Adjust the size of the tweet container
        AdjustTweetSize();
    }

    // Method to adjust the size of the tweet container based on content
    [ContextMenu("Adjust Tweet Size")]
    private void AdjustTweetSize()
    {
        // Store the original anchored position
        Vector2 originalPosition = tweetContainer.anchoredPosition;

        // Force update the layout elements
        LayoutRebuilder.ForceRebuildLayoutImmediate(tweetText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tweetContainer);

        // Calculate the preferred height of the text
        float textHeight = LayoutUtility.GetPreferredHeight(tweetText.rectTransform);

        // Calculate the total height required
        float totalHeight = 230f;

        totalHeight += textHeight * 1.18f;

        // Calculate the preferred height of the image if it is active
        if (tweetImageContent != null)
        {
            // Add image height
            totalHeight += LayoutUtility.GetPreferredHeight(tweetImage.rectTransform) + 250f;
        }

        // Set the tweet container height
        tweetContainer.sizeDelta = new Vector2(tweetContainer.sizeDelta.x, totalHeight);

        // Restore the original anchored position to keep the bottom edge fixed
        tweetContainer.anchoredPosition = originalPosition + new Vector2(0, (totalHeight - tweetContainer.sizeDelta.y) / 2);
    }

    public void LikeButtonPressed()
    {
        shouldLike = !shouldLike;
    }

    public void RetweetButtonPressed()
    {
        shouldRetweet = !shouldRetweet;
    }

    public void BookmarkButtonPressed()
    {
        shouldBookmark = !shouldBookmark;
    }

    public void ReportButtonPressed()
    {
        if(!shouldReport)
        {
            shouldReport = true;
            //ruleChecker.CheckAction(buttonFlag, shouldLike, shouldRetweet, shouldBookmark, shouldReport); //現状ここはおいておく
        }
    }

    public void RandomTweetInfo()
    {
        // フォローしているかどうかをランダムに決定（50%の確率でフォロー）
        isFollowing = Random.value > 0.5f;

        // 投稿されてからの時間をランダムに決定（0から60分の間）
        minutesSincePosted = Random.Range(0f, 60f);

        // フォロー状態を表示
        IsFollow.SetActive(isFollowing);

        // 投稿時間を表示
        ElapsedTime.text = $"{Mathf.FloorToInt(minutesSincePosted)}分前";


        if (ruleChecker == null)
        {
            ruleChecker = GameObject.Find("RuleChecker").GetComponent<RuleChecker>();
        }

        TweetData tweetData = new TweetData(isFollowing, (int)minutesSincePosted, tweetContent);
        buttonFlag = ruleChecker.ApplyRules(tweetData);
    }

    public void TweetCheck()
    {
        ruleChecker.CheckAction(buttonFlag, shouldLike, shouldRetweet, shouldBookmark, shouldReport);
    }


}
