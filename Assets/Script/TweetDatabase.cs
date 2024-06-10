using UnityEngine;
using System.Collections.Generic;

// �c�C�[�g����\���N���X
[System.Serializable]
public class TweetInfo
{
    public string tweetContent;         // �c�C�[�g�̕���
    public Sprite tweetImageContent;    // �c�C�[�g�̉摜
    public string tweetID;              // �c�C�[�gID
    public string parentAccountID;      // �e�̃A�J�E���gID

    // �R���X�g���N�^�Ń����_���ȃc�C�[�gID�𐶐�
    public TweetInfo(string content, Sprite image, string parentID)
    {
        tweetContent = content;
        tweetImageContent = image;
        tweetID = GenerateRandomTweetID();
        parentAccountID = parentID;
    }

    // �����_���ȃc�C�[�gID�𐶐�����֐�
    private string GenerateRandomTweetID()
    {
        // �����_���ȕ�����𐶐��i�����ł͒P���� GUID ���g���܂��j
        return System.Guid.NewGuid().ToString();
    }
}

// �A�J�E���g����\���N���X
[System.Serializable]
public class AccountInfo
{
    public string accountID;                // �A�J�E���gID
    public string accountName;              // �A�J�E���g��
    public Sprite accountImage;             // �A�J�E���g�摜
    public bool IsExclusion;
    public List<TweetInfo> tweetList;      // �c�C�[�g���X�g


    public AccountInfo(string id, string name, Sprite image)
    {
        accountID = id;
        accountName = name;
        accountImage = image;
        tweetList = new List<TweetInfo>();
    }
}

// �A�J�E���g�����Ǘ�����N���X
[System.Serializable]
public class TweetDatabase : MonoBehaviour
{
    // �A�J�E���g���̃��X�g
    public List<AccountInfo> accountList = new List<AccountInfo>();

    // �A�J�E���gID���L�[�Ƃ����A�J�E���g���̎���
    public Dictionary<string, AccountInfo> accountDictionary = new Dictionary<string, AccountInfo>();

    // �c�C�[�gID���L�[�Ƃ����c�C�[�g���̎���
    public Dictionary<string, TweetInfo> tweetDictionary = new Dictionary<string, TweetInfo>();

    // ���X�g�̏����A�J�E���g�����ƃc�C�[�g�����ɍX�V����֐�
    public void UpdateDictionariesFromList()
    {
        // �A�J�E���g�������N���A
        accountDictionary.Clear();

        // �c�C�[�g�������N���A
        tweetDictionary.Clear();

        // �A�J�E���g���X�g�̊e�v�f������
        foreach (var accountInfo in accountList)
        {
            // �A�J�E���g�����A�J�E���g�����ɒǉ�
            if (!accountDictionary.ContainsKey(accountInfo.accountID))
            {
                accountDictionary.Add(accountInfo.accountID, accountInfo);
            }
            else
            {
                Debug.LogWarning("�A�J�E���gID " + accountInfo.accountID + " �͊��ɑ��݂��܂��B");
            }

            // �c�C�[�g�����c�C�[�g�����ɒǉ�
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                if (!tweetDictionary.ContainsKey(tweetInfo.tweetID))
                {
                    tweetDictionary.Add(tweetInfo.tweetID, tweetInfo);
                }
                else
                {
                    Debug.LogWarning("�c�C�[�gID " + tweetInfo.tweetID + " �͊��ɑ��݂��܂��B");
                }
            }
        }
    }

    public void Start()
    {
        UpdateDictionariesFromList();
    }

    // �c�C�[�gID����TweetInfo���擾����֐�
    public TweetInfo GetTweetInfo(string tweetID)
    {
        // �c�C�[�gID�����݂��邩�m�F
        if (tweetDictionary.ContainsKey(tweetID))
        {
            // �c�C�[�g����Ԃ�
            return tweetDictionary[tweetID];
        }
        else
        {
            Debug.LogWarning("�c�C�[�gID " + tweetID + " ��������܂���B");
            return null;
        }
    }

    public AccountInfo GetAccountInfo(string accountID)
    {
        // �A�J�E���gID�����݂��邩�m�F
        if (accountDictionary.ContainsKey(accountID))
        {
            // �A�J�E���g����Ԃ�
            return accountDictionary[accountID];
        }
        else
        {
            Debug.LogWarning("�A�J�E���gID " + accountID + " ��������܂���B");
            return null;
        }
    }

    public string GetParentAccountID(string tweetID)
    {
        TweetInfo tweet = GetTweetInfo(tweetID);
        if (tweet != null)
        {
            return tweet.parentAccountID;
        }
        else
        {
            Debug.LogWarning("�c�C�[�gID " + tweetID + " �ɑΉ�����c�C�[�g��񂪌�����܂���B");
            return null;
        }
    }

    public string GetRandomTweetID()
    {
        if (tweetDictionary.Count > 0)
        {
            List<string> keys = new List<string>(tweetDictionary.Keys); // KeyCollection �� List �ɕϊ�
            int maxAttempts = 10; // �ő厎�s�񐔂�ݒ�
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                int randomIndex = GenerateRandomIndex(keys.Count); // �����_���ȃC���f�b�N�X�𐶐�
                string randomID = keys[randomIndex];

                // �e�A�J�E���g�� IsExclusion �� true �̏ꍇ�͍ēx���s
                string parentAccountID = GetParentAccountID(randomID);
                if (parentAccountID != null)
                {
                    AccountInfo parentAccount = GetAccountInfo(parentAccountID);
                    if (parentAccount != null && parentAccount.IsExclusion)
                    {
                        attempt++;
                        continue;
                    }
                }

                //Debug.Log("�����_���ɑI�΂ꂽ�c�C�[�gID: " + randomID);
                return randomID;
            }

            Debug.LogWarning("�K�؂ȃc�C�[�gID��������܂���ł����B");
            return null;
        }
        else
        {
            Debug.LogWarning("�c�C�[�g��������ł��B");
            return null;
        }
    }

    // �����̃C���f�b�N�X�𐶐�����֐�
    private int GenerateRandomIndex(int count)
    {
        // �{�b�N�X���~�����[�@��p���Đ��K���z���痐���𐶐�
        float u1 = 1.0f - Random.value;
        float u2 = 1.0f - Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        // ���ς𒆐S�Ƃ��ĕW���΍���5�{�܂ł͈̔͂ŗ����𐶐�
        float mean = count / 2.0f;
        float stdDev = count / 10.0f; // 10�͔C�ӂ̒l�ŁA�������K�v�ȏꍇ�͕ύX�\
        int randomIndex = Mathf.RoundToInt(mean + stdDev * randStdNormal);

        // �C���f�b�N�X���͈͊O�̏ꍇ�͔͈͓��Ɏ��߂�
        randomIndex = Mathf.Clamp(randomIndex, 0, count - 1);

        return randomIndex;
    }
}
