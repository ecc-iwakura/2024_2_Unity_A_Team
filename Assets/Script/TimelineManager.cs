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
    public RectTransform tweetRect; // RectTransform��ǉ�

    public TweetObjectData(GameObject tweetObject, TweetScript tweetScript, RectTransform tweetRect)
    {
        this.tweetObject = tweetObject;
        this.tweetScript = tweetScript;
        this.tweetRect = tweetRect; // ���N�g��ǉ�
    }

    public void Deconstruct(out GameObject tweetObject, out TweetScript tweetScript, out RectTransform tweetRect)
    {
        tweetObject = this.tweetObject;
        tweetScript = this.tweetScript;
        tweetRect = this.tweetRect; // ���N�g��ǉ�
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
    public GameObject tweetPrefab;         // �c�C�[�g�v���n�u
    public RectTransform timeline;         // �^�C�����C����RectTransform
    public Transform spawnPoint;           // �X�|�[���n�_
    public TweetDatabase tweetDatabase;         // �^�C�����C����RectTransform
    public KeywordChecker keywordChecker;
    public RuleChecker ruleChecker;
    public int maxTweets = 10;             // �ő�c�C�[�g��
    public bool IsStop = false;

    public UnityEvent AddTweetSE;

    [SerializeField]
    private List<TweetObjectData> tweetObjectList = new List<TweetObjectData>(); // �c�C�[�g�I�u�W�F�N�g��TweetScript�̃Z�b�g�̃��X�g


    public List<AddRuleTweet> stackTweetIDs = new List<AddRuleTweet>(); // �X�^�b�N�c�C�[�gID���X�g

    private float currentYPosition = 0f;   // ���݂�Y�ʒu
    public float tweetCooldown = 3f;       // �c�C�[�g�̊Ԋu�i�b�j
    public float tweetSpeedTime = 3f;       // �c�C�[�g�̊Ԋu�i�b�j

    private bool isTweetMoving = false;    // �c�C�[�g���ړ������ǂ����������t���O

    // ���������ɊJ�n
    void Start()
    {
        StartCoroutine(GenerateTweets());
    }

    // �c�C�[�g�𐶐�����R���[�`��
    private IEnumerator GenerateTweets()
    {
        while (true)
        {
            if (!isTweetMoving && !IsStop)
            {
                AddTweet(); // �c�C�[�g��ǉ�
            }

            yield return new WaitForSeconds(0.1f); // �N�[���_�E��
        }
    }

    // Method to add a new tweet to the timeline
    [ContextMenu("Add Test Tweet")] // �C���X�y�N�^�[����Ăяo�����߂̃R���e�L�X�g���j���[
    public void AddTweet()
    {
        string text = "";
        Sprite image = null;
        Sprite accountImage = null;
        string accountName = "";
        string accountID = "";

        if (stackTweetIDs.Count > 0)
        {
            // �X�^�b�N�c�C�[�gID���X�g�Ƀc�C�[�gID������ꍇ�͂��̃c�C�[�gID���g���ăc�C�[�g�𐶐�
            AddRuleTweet stack = stackTweetIDs[0];
            string tweetID = stack.tweetID; // ���X�g�̐擪����c�C�[�gID���擾
            stackTweetIDs.RemoveAt(0); // ���X�g����폜
            (text, image, accountImage, accountName, accountID) = GenerateTweetData(tweetID);

            if (!string.IsNullOrEmpty(stack.ruleFunctionName) && stack.ruleFlag != null)
            {
                UnityEngine.Debug.LogError($"���B�I{stack.ruleFunctionName} {stack.ruleFlag}");
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
            UnityEngine.Debug.Log("�V�i�����I");
            // �X�|�[���n�_�Ƀc�C�[�g�v���n�u�𐶐�
            newTweet = Instantiate(tweetPrefab, spawnPoint.position, Quaternion.identity, timeline);

            newTweet.transform.rotation = timeline.rotation;

            // �V�����c�C�[�g��TweetScript�R���|�[�l���g���擾
            TweetScript tweetScript = newTweet.GetComponent<TweetScript>();

            tweetRect = newTweet.GetComponent<RectTransform>();

            isKeyword = keywordChecker.CheckForKeyword(text);

            
            // �c�C�[�g�̓��e��ݒ�
            tweetScript.UpdateTweet(text, image, accountImage, accountName, accountID, isKeyword, ruleChecker.selectedRules);

            // �c�C�[�g�I�u�W�F�N�g��TweetScript�̃Z�b�g�����X�g�ɒǉ�
            tweetObjectList.Add(new TweetObjectData(newTweet, tweetScript, tweetRect));
        }
        else
        {
            UnityEngine.Debug.Log("�ė��p�I");
            // ���X�g�łP�ԌÂ��c�C�[�g���擾���čė��p
            var oldTweetObjectData = tweetObjectList[0];

            oldTweetObjectData.tweetScript.TweetCheck();

            tweetObjectList.RemoveAt(0);

            isKeyword = keywordChecker.CheckForKeyword(text);
            // �c�C�[�g�̓��e���X�V
            oldTweetObjectData.tweetScript.UpdateTweet(text, image, accountImage, accountName, accountID, isKeyword, ruleChecker.selectedRules);

            oldTweetObjectData.tweetObject.transform.rotation = timeline.rotation;
            // �ė��p����c�C�[�g�I�u�W�F�N�g�̈ʒu��ݒ�
            oldTweetObjectData.tweetObject.transform.position = spawnPoint.position;

            // �c�C�[�g�I�u�W�F�N�g��TweetScript�̃Z�b�g�����X�g�̖����ɒǉ��i�ė��p�j
            tweetObjectList.Add(oldTweetObjectData);

            // newTweet�ϐ����ė��p���邽�߂ɏ�����
            newTweet = oldTweetObjectData.tweetObject;
            tweetRect = oldTweetObjectData.tweetRect;
        }

        // �c�C�[�g�̍������擾

        float tweetHeight = tweetRect.sizeDelta.y * 1.1f;

        AddTweetSE.Invoke();
        // �^�C�����C�����c�C�[�g�̍������������Ɉړ�
        StartCoroutine(MoveTimeline(tweetHeight));

    }

    // �R���[�`���Ń^�C�����C����������艺�Ɉړ�
    private IEnumerator MoveTimeline(float tweetHeight)
    {
        isTweetMoving = true; // �c�C�[�g���ړ����ł��邱�Ƃ������t���O��ݒ�

        float duration = tweetSpeedTime; // �ړ��ɂ������{����

        // �ړ��ɂ����鎞�Ԃ������ɉ����Ē���
        float distanceFactor = Mathf.Clamp(tweetHeight / 100f, 0.5f, 2f);
        duration *= distanceFactor;

        Vector2 startPos = timeline.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, tweetHeight); // ���Ɉړ�

        float elapsed = 0f;
        while (elapsed < duration)
        {
            // �A�j���[�V�����𒼐ڌv�Z
            float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            timeline.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);

            // �o�ߎ��Ԃ����Z
            elapsed += Time.deltaTime;
            yield return null; // ���̃t���[���܂őҋ@
        }

        timeline.anchoredPosition = endPos;

        // ���݂�Y�ʒu���X�V
        currentYPosition += tweetHeight;
        timeline.sizeDelta = new Vector2(0, 0);

        yield return new WaitForSeconds(tweetCooldown); // �N�[���_�E��

        Vector3[] childWorldPositions = new Vector3[timeline.childCount];
        for (int i = 0; i < timeline.childCount; i++)
        {
            childWorldPositions[i] = timeline.GetChild(i).position;
        }

        // �e�I�u�W�F�N�g�̈ʒu��ύX
        timeline.anchoredPosition = new Vector2(0.0f, 0.0f);

        // �q�I�u�W�F�N�g�̈ʒu�����̃��[���h���W�ɖ߂�
        for (int i = 0; i < timeline.childCount; i++)
        {
            RectTransform childRectTransform = timeline.GetChild(i).GetComponent<RectTransform>();
            childRectTransform.position = childWorldPositions[i];
        }

        isTweetMoving = false; // �c�C�[�g���ړ����łȂ����Ƃ������t���O�����Z�b�g
    }

    // GenerateRandomTweetData���\�b�h�̏C��
    private (string, Sprite, Sprite, string, string) GenerateRandomTweetData()
    {
        string randomID = tweetDatabase.GetRandomTweetID();

        if (randomID == null)
        {
            Debug.LogWarning("�����_���ȃc�C�[�gID���擾�ł��܂���ł����B");
            return ("null", null, null, "null", "null"); // �v�f����5�ɏC��
        }

        TweetInfo tweetInfo = tweetDatabase.GetTweetInfo(randomID);

        string accountID = tweetDatabase.GetParentAccountID(randomID);
        AccountInfo accountInfo = tweetDatabase.GetAccountInfo(accountID);

        if (tweetInfo == null)
        {
            Debug.LogWarning("�c�C�[�g��񂪎擾�ł��܂���ł����BID: " + randomID);
            return ("null", null, null, "null", "null"); // �v�f����5�ɏC��
        }

        string text = tweetInfo.tweetContent;
        Sprite image = tweetInfo.tweetImageContent;
        Sprite accountImage = accountInfo.accountImage; // �t�B�[���h���C��
        string accountName = accountInfo.accountName;

        return (text, image, accountImage, accountName, accountID);
    }

    private (string, Sprite, Sprite, string, string) GenerateTweetData(string ID)
    {
        TweetInfo tweetInfo = tweetDatabase.GetTweetInfo(ID);

        string accountID = tweetDatabase.GetParentAccountID(ID); // randomID����ID�ɕύX

        if(accountID == null) {
            Debug.LogWarning("�A�J�E���gID��Null�ł����I");
        }

        AccountInfo accountInfo = tweetDatabase.GetAccountInfo(accountID);

        if (tweetInfo == null)
        {
            Debug.LogWarning("�c�C�[�g��񂪎擾�ł��܂���ł����BID: " + ID); // randomID����ID�ɕύX
            return ("null", null, null, "null", "null");
        }

        string text = tweetInfo.tweetContent;
        Sprite image = tweetInfo.tweetImageContent;
        Sprite accountImage = accountInfo.accountImage;
        string accountName = accountInfo.accountName;

        return (text, image, accountImage, accountName, accountID);
    }
}