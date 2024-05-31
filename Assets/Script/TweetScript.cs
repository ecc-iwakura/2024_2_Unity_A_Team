using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class TweetScript : MonoBehaviour
{
    // Public variables to be set in the Unity Editor
    public TMP_Text tweetText;          // �c�C�[�g�̃e�L�X�g��\������TextMeshPro�R���|�[�l���g
    public Image tweetImage;            // �c�C�[�g�̉摜��\������UI Image�R���|�[�l���g
    public TMP_Text accountName;        // �A�J�E���g����\������TextMeshPro�R���|�[�l���g
    public TMP_Text accountID;          // �A�J�E���gID��\������TextMeshPro�R���|�[�l���g
    public Image accountImage;            // �c�C�[�g�̉摜��\������UI Image�R���|�[�l���g
    public RectTransform tweetContainer; // �c�C�[�g�S�̂��͂�UI��RectTransform�R���|�[�l���g

    // Fields for tweet data
    [TextArea(3, 10)] 

    public string tweetContent;        // �c�C�[�g�̕���
    public Sprite tweetImageContent;   // �c�C�[�g�̉摜
    public Sprite tweetAccountImage;    // �A�J�E���g��
    public string tweetAccountName;    // �A�J�E���g��
    public string tweetAccountID;      // �A�J�E���gID

    [Space(10)] // 10�̃X�y�[�X��ǉ�

    public bool isFollowing;            // �t�H���[���Ă��邩�ǂ���
    public float minutesSincePosted;    // ���e����Ă���̎��ԁi���j
    public bool IsKeyword; //�L�[���[�h���܂܂�Ă��邩�ǂ���

    public TMP_Text ElapsedTime;        // �A�J�E���g����\������TextMeshPro�R���|�[�l���g
    public GameObject IsFollow;        // �A�J�E���g����\������TextMeshPro�R���|�[�l���g

    [Space(10)] // 10�̃X�y�[�X��ǉ�

    public UnityEvent onReset;
    private bool shouldLike;
    private bool shouldRetweet;
    private bool shouldBookmark;
    private bool shouldReport;

    [SerializeField]
    private ButtonFlag buttonFlag;

    [System.Flags]
    private enum ButtonFlag
    {
        None = 0,
        Like = 1 << 0,
        Retweet = 1 << 1,
        LikeAndRetweet = Like | Retweet, // �����˂ƃ��c�C�[�g�̗���
        Bookmark = 1 << 2,
        Report = 1 << 3
    }
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
            CheckAction(ButtonFlag.Report);
        }
    }

    // �����_���Ƀt�H���[��ԂƓ��e���Ԃ����肵�A�\������֐�
    public void RandomTweetInfo()
    {
        // �t�H���[���Ă��邩�ǂ����������_���Ɍ���i50%�̊m���Ńt�H���[�j
        isFollowing = Random.value > 0.5f;

        // ���e����Ă���̎��Ԃ������_���Ɍ���i0����60���̊ԁj
        minutesSincePosted = Random.Range(0f, 60f);

        // �t�H���[��Ԃ�\��
        IsFollow.SetActive(isFollowing);

        // ���e���Ԃ�\��
        ElapsedTime.text = $"{Mathf.FloorToInt(minutesSincePosted)}���O";

        // ���e���Ԃ�10���ȓ����ǂ����𔻒f
        bool isTime = minutesSincePosted <= 10f;

        // �������s�����t���O�Ŕ��f
        if (isFollowing && !isTime)
        {
            // ������
            buttonFlag = ButtonFlag.Like;
        }
        else if (!isFollowing && isTime)
        {
            // ���c�C�[�g
            buttonFlag = ButtonFlag.Retweet;
        }
        else if (isFollowing && isTime)
        {
            // �����˂ƃ��c�C�[�g
            buttonFlag = ButtonFlag.LikeAndRetweet;
        }
        else
        {
            // �u�b�N�}�[�N
            buttonFlag = ButtonFlag.Bookmark;
        }

        if (IsKeyword)
        {
            buttonFlag = ButtonFlag.Report;
        }

    }

    public void TweetCheck()
    {
        CheckAction(buttonFlag);
    }

    private void CheckAction(ButtonFlag action)
    {
        bool isCorrect = false;
        string logText = "";

        switch (action)
        {
            case ButtonFlag.Like:
                isCorrect = shouldLike && !shouldRetweet && !shouldBookmark && !shouldReport;
                logText = "Like";
                break;
            case ButtonFlag.Retweet:
                isCorrect = shouldRetweet && !shouldLike && !shouldBookmark && !shouldReport;
                logText = "Retweet";
                break;
            case ButtonFlag.Bookmark:
                isCorrect = shouldBookmark && !shouldLike && !shouldRetweet && !shouldReport;
                logText = "Bookmark";
                break;
            case ButtonFlag.Report:
                isCorrect = shouldReport;
                logText = "Report";
                break;
            case ButtonFlag.LikeAndRetweet:
                isCorrect = shouldLike && shouldRetweet && !shouldBookmark && !shouldReport;
                logText = "Like&Retweet";
                break;
        }

        string statusText = isCorrect ? "�������ł�" : "�������Ȃ��ł�";
        string buttonState = $" {(shouldRetweet ? "O" : "X")}{(shouldLike ? "O" : "X")}{(shouldBookmark ? "O" : "X")}{(shouldReport ? "O" : "X")}";
        Debug.Log($"{buttonState} => {logText} => {statusText}");
    }


}
