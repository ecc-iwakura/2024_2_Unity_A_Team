using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using static RuleChecker;

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


    [Tooltip("�V�[������ 'RuleChecker' �I�u�W�F�N�g�ւ̎Q�ƁB")]
    public RuleChecker ruleChecker;
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
        buttonFlag = ButtonFlag.None; // ���Ȃ��̃R�[�h�ɍ��킹�ď��������Ă�������

        if (ruleChecker == null) { 
            
            Debug.LogError("RuleChecker��������܂���ł����I");
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
            //ruleChecker.CheckAction(buttonFlag, shouldLike, shouldRetweet, shouldBookmark, shouldReport); //���󂱂��͂����Ă���
        }
    }

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
