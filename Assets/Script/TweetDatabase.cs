using UnityEngine;
using System.Collections.Generic;

// ツイート情報を表すクラス
[System.Serializable]
public class TweetInfo
{
    public string tweetContent;         // ツイートの文面
    public Sprite tweetImageContent;    // ツイートの画像
    public string tweetID;              // ツイートID
    public string parentAccountID;      // 親のアカウントID

    // コンストラクタでランダムなツイートIDを生成
    public TweetInfo(string content, Sprite image, string parentID)
    {
        tweetContent = content;
        tweetImageContent = image;
        tweetID = GenerateRandomTweetID();
        parentAccountID = parentID;
    }

    // ランダムなツイートIDを生成する関数
    private string GenerateRandomTweetID()
    {
        // ランダムな文字列を生成（ここでは単純に GUID を使います）
        return System.Guid.NewGuid().ToString();
    }
}

// アカウント情報を表すクラス
[System.Serializable]
public class AccountInfo
{
    public string accountID;                // アカウントID
    public string accountName;              // アカウント名
    public Sprite accountImage;             // アカウント画像
    public bool IsExclusion;
    public List<TweetInfo> tweetList;      // ツイートリスト


    public AccountInfo(string id, string name, Sprite image)
    {
        accountID = id;
        accountName = name;
        accountImage = image;
        tweetList = new List<TweetInfo>();
    }
}

// アカウント情報を管理するクラス
[System.Serializable]
public class TweetDatabase : MonoBehaviour
{
    // アカウント情報のリスト
    public List<AccountInfo> accountList = new List<AccountInfo>();

    // アカウントIDをキーとしたアカウント情報の辞書
    public Dictionary<string, AccountInfo> accountDictionary = new Dictionary<string, AccountInfo>();

    // ツイートIDをキーとしたツイート情報の辞書
    public Dictionary<string, TweetInfo> tweetDictionary = new Dictionary<string, TweetInfo>();

    // リストの情報をアカウント辞書とツイート辞書に更新する関数
    public void UpdateDictionariesFromList()
    {
        // アカウント辞書をクリア
        accountDictionary.Clear();

        // ツイート辞書をクリア
        tweetDictionary.Clear();

        // アカウントリストの各要素を処理
        foreach (var accountInfo in accountList)
        {
            // アカウント情報をアカウント辞書に追加
            if (!accountDictionary.ContainsKey(accountInfo.accountID))
            {
                accountDictionary.Add(accountInfo.accountID, accountInfo);
            }
            else
            {
                Debug.LogWarning("アカウントID " + accountInfo.accountID + " は既に存在します。");
            }

            // ツイート情報をツイート辞書に追加
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                if (!tweetDictionary.ContainsKey(tweetInfo.tweetID))
                {
                    tweetDictionary.Add(tweetInfo.tweetID, tweetInfo);
                }
                else
                {
                    Debug.LogWarning("ツイートID " + tweetInfo.tweetID + " は既に存在します。");
                }
            }
        }
    }

    public void Start()
    {
        UpdateDictionariesFromList();
    }

    // ツイートIDからTweetInfoを取得する関数
    public TweetInfo GetTweetInfo(string tweetID)
    {
        // ツイートIDが存在するか確認
        if (tweetDictionary.ContainsKey(tweetID))
        {
            // ツイート情報を返す
            return tweetDictionary[tweetID];
        }
        else
        {
            Debug.LogWarning("ツイートID " + tweetID + " が見つかりません。");
            return null;
        }
    }

    public AccountInfo GetAccountInfo(string accountID)
    {
        // アカウントIDが存在するか確認
        if (accountDictionary.ContainsKey(accountID))
        {
            // アカウント情報を返す
            return accountDictionary[accountID];
        }
        else
        {
            Debug.LogWarning("アカウントID " + accountID + " が見つかりません。");
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
            Debug.LogWarning("ツイートID " + tweetID + " に対応するツイート情報が見つかりません。");
            return null;
        }
    }

    public string GetRandomTweetID()
    {
        if (tweetDictionary.Count > 0)
        {
            List<string> keys = new List<string>(tweetDictionary.Keys); // KeyCollection を List に変換
            int maxAttempts = 10000; // 最大試行回数を設定
            int attempt = 0;

            while (attempt < maxAttempts)
            {
                int randomIndex = GenerateRandomIndex(keys.Count); // ランダムなインデックスを生成
                string randomID = keys[randomIndex];

                // 親アカウントが IsExclusion が true の場合は再度試行
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

                //Debug.Log("ランダムに選ばれたツイートID: " + randomID);
                return randomID;
            }

            Debug.LogWarning("適切なツイートIDが見つかりませんでした。");
            return null;
        }
        else
        {
            Debug.LogWarning("ツイート辞書が空です。");
            return null;
        }
    }

    // 乱数のインデックスを生成する関数
    private int GenerateRandomIndex(int count)
    {
        return Random.Range(0, count);
    }
}
