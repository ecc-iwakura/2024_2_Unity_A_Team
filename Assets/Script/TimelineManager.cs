using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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


public class TimelineManager : MonoBehaviour
{
    public GameObject tweetPrefab;         // �c�C�[�g�v���n�u
    public RectTransform timeline;         // �^�C�����C����RectTransform
    public Transform spawnPoint;           // �X�|�[���n�_
    public int maxTweets = 10;             // �ő�c�C�[�g��

    [SerializeField]
    private List<TweetObjectData> tweetObjectList = new List<TweetObjectData>(); // �c�C�[�g�I�u�W�F�N�g��TweetScript�̃Z�b�g�̃��X�g

    private float currentYPosition = 0f;   // ���݂�Y�ʒu
    public float tweetCooldown = 3f;       // �c�C�[�g�̊Ԋu�i�b�j

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
            if (!isTweetMoving)
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
        // �����_���ȃc�C�[�g�f�[�^�𐶐�
        (string text, Sprite image, string accountName, string accountID) = GenerateRandomTweetData();

        GameObject newTweet = null;
        RectTransform tweetRect = null;
        if (tweetObjectList.Count < maxTweets)
        {
            UnityEngine.Debug.Log("�V�i�����I");
            // �X�|�[���n�_�Ƀc�C�[�g�v���n�u�𐶐�
            newTweet = Instantiate(tweetPrefab, spawnPoint.position, Quaternion.identity, timeline);

            // �V�����c�C�[�g��TweetScript�R���|�[�l���g���擾
            TweetScript tweetScript = newTweet.GetComponent<TweetScript>();

            tweetRect = newTweet.GetComponent<RectTransform>();
            // �c�C�[�g�̓��e��ݒ�
            tweetScript.UpdateTweet(text, image, accountName, accountID);

            // �c�C�[�g�I�u�W�F�N�g��TweetScript�̃Z�b�g�����X�g�ɒǉ�
            tweetObjectList.Add(new TweetObjectData(newTweet, tweetScript, tweetRect));
        }
        else
        {
            UnityEngine.Debug.Log("�ė��p�I");
            // ���X�g�łP�ԌÂ��c�C�[�g���擾���čė��p
            var oldTweetObjectData = tweetObjectList[0];
            tweetObjectList.RemoveAt(0);

            // �c�C�[�g�̓��e���X�V
            oldTweetObjectData.tweetScript.UpdateTweet(text, image, accountName, accountID);

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

        // �^�C�����C�����c�C�[�g�̍������������Ɉړ�
        StartCoroutine(MoveTimeline(tweetHeight));
    }

    // �R���[�`���Ń^�C�����C����������艺�Ɉړ�
    private IEnumerator MoveTimeline(float tweetHeight)
    {
        isTweetMoving = true; // �c�C�[�g���ړ����ł��邱�Ƃ������t���O��ݒ�

        float duration = tweetCooldown; // �ړ��ɂ������{����

        // �ړ��ɂ����鎞�Ԃ������ɉ����Ē���
        float distanceFactor = Mathf.Clamp(tweetHeight / 100f, 0.5f, 2f);
        duration *= distanceFactor;

        float distance = tweetHeight; // �ړ�����

        Vector2 startPos = timeline.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, tweetHeight); // ���Ɉړ�

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            timeline.anchoredPosition = Vector2.Lerp(startPos, endPos, t * t); // �ړ�����芊�炩�ɂ��邽�߂� t * t ���g�p

            // �����ɉ����Ď��Ԃ𒲐�
            elapsed += Time.deltaTime;
            yield return null;
        }

        timeline.anchoredPosition = endPos;

        // ���݂�Y�ʒu���X�V
        currentYPosition += tweetHeight;
        timeline.sizeDelta = new Vector2(timeline.sizeDelta.x, currentYPosition);

        yield return new WaitForSeconds(tweetCooldown); // �N�[���_�E��

        isTweetMoving = false; // �c�C�[�g���ړ����łȂ����Ƃ������t���O�����Z�b�g
    }

    // �����_���ȃc�C�[�g�f�[�^�𐶐����郁�\�b�h
    private (string, Sprite, string, string) GenerateRandomTweetData()
    {
        string[] randomTexts = { "����̓e�X�g�c�C�[�g�ł�", "����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B", "����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B����̓e�X�g�c�C�[�g�ł��B" };
        string[] randomAccountNames = { "TestTweet", "TestTweet�ł��I", "TestTweet�����Ă�" };
        string[] randomAccountIDs = { "test_account", "random_account1", "another_account" };

        string text = randomTexts[Random.Range(0, randomTexts.Length)];
        string accountName = randomAccountNames[Random.Range(0, randomAccountNames.Length)];
        string accountID = randomAccountIDs[Random.Range(0, randomAccountIDs.Length)];

        // �����_���ȉ摜�𐶐��i�����null�̂܂܂ɂ��Ă����܂����A�摜��ǉ�����ꍇ�͓K�؂ɐݒ�j
        Sprite image = null; // �摜���K�v�ȏꍇ�̓����_���ɉ摜��I�����鏈����ǉ�

        return (text, image, accountName, accountID);
    }
}