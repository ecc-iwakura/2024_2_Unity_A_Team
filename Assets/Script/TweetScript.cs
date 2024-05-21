using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TweetScript : MonoBehaviour
{
    // Public variables to be set in the Unity Editor
    public TMP_Text tweetText;          // ツイートのテキストを表示するTextMeshProコンポーネント
    public Image tweetImage;            // ツイートの画像を表示するUI Imageコンポーネント
    public TMP_Text accountName;        // アカウント名を表示するTextMeshProコンポーネント
    public TMP_Text accountID;          // アカウントIDを表示するTextMeshProコンポーネント
    public RectTransform tweetContainer; // ツイート全体を囲むUIのRectTransformコンポーネント

    // Fields for tweet data
    [TextArea(3, 10)] // TextArea attribute to make multiline text input easier in the Inspector
    public string tweetContent;        // ツイートの文面
    public Sprite tweetImageContent;   // ツイートの画像
    public string tweetAccountName;    // アカウント名
    public string tweetAccountID;      // アカウントID

    // Method to update the tweet content
    public void UpdateTweet(string newText, Sprite newImage, string newAccountName, string newAccountID)
    {
        // Update the tweet data fields
        tweetContent = newText;
        tweetImageContent = newImage;
        tweetAccountName = newAccountName;
        tweetAccountID = newAccountID;

        // Update the UI elements
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
        accountID.text = tweetAccountID;

        // Adjust the size of the tweet container
        AdjustTweetSize();
    }

    // Method to adjust the size of the tweet container based on content
    [ContextMenu("Adjust Tweet Size")]
    private void AdjustTweetSize()
    {
        // Force update the layout elements
        LayoutRebuilder.ForceRebuildLayoutImmediate(tweetText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tweetImage.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tweetContainer);

        // Calculate the preferred height of the text
        float textHeight = LayoutUtility.GetPreferredHeight(tweetText.rectTransform);

        // Calculate the preferred height of the image if it is active
        float imageHeight = tweetImage.gameObject.activeSelf ? LayoutUtility.GetPreferredHeight(tweetImage.rectTransform) : 0f;

        // Calculate the total height required
        float totalHeight = textHeight + imageHeight;

        // Add some padding
        float padding = 20f; // Adjust this value based on your UI design
        totalHeight += padding;

        // Set the tweet container height
        tweetContainer.sizeDelta = new Vector2(tweetContainer.sizeDelta.x, totalHeight);
    }

    // Optionally, automatically update the UI when the game starts
    void Start()
    {
        UpdateUI();
    }
}
