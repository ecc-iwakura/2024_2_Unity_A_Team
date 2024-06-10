//using UnityEngine;

//public class KeywordChecker : MonoBehaviour
//{
//    public string[] keywords; // 判定するキーワードの配列

//    // ツイート文にキーワードが含まれているかどうかを判定するメソッド
//    public bool CheckForKeyword(string tweetContent)
//    {
//        foreach (string keyword in keywords)
//        {
//            // ツイート文にキーワードが含まれているかどうかを判定
//            if (tweetContent.ToLower().Contains(keyword.ToLower()))
//            {
//                Debug.Log($"キーワード '{keyword}' がツイート文に含まれています。");
//                return true; // キーワードが含まれている場合はtrueを返す
//            }
//        }
//        Debug.Log("キーワードがツイート文に含まれていません。");
//        return false; // キーワードが含まれていない場合はfalseを返す
//    }


//    void Start()
//    {
//        TestCheckForKeyword();
//    }
//    // テスト用のメソッド
//    public void TestCheckForKeyword()
//    {
//        string testTweet = "ツイート";
//        bool result = CheckForKeyword(testTweet);
//        Debug.Log($"ツイート文にキーワードが含まれているか: {result}");
//    }
//}



//using UnityEngine;
//using System.Globalization;
//using System.Text;

//public class KeywordChecker : MonoBehaviour
//{
//    public string[] keywords; // 判定するキーワードの配列

//    // ツイート文にキーワードが含まれているかどうかを判定するメソッド
//    public bool CheckForKeyword(string tweetContent)
//    {
//        // ツイート文をひらがなに変換
//        string normalizedTweetContent = ToHiragana(tweetContent);

//        foreach (string keyword in keywords)
//        {
//            // キーワードをひらがなに変換
//            string normalizedKeyword = ToHiragana(keyword);

//            // ツイート文にキーワードが含まれているかどうかを判定
//            if (normalizedTweetContent.Contains(normalizedKeyword))
//            {
//                Debug.Log($"キーワード '{keyword}' がツイート文に含まれています。");
//                return true; // キーワードが含まれている場合はtrueを返す
//            }
//        }
//        Debug.Log("キーワードがツイート文に含まれていません。");
//        return false; // キーワードが含まれていない場合はfalseを返す
//    }

//    // カタカナをひらがなに変換するメソッド
//    private string ToHiragana(string input)
//    {
//        StringBuilder sb = new StringBuilder();
//        foreach (char c in input)
//        {
//            if (c >= 'ァ' && c <= 'ン')
//            {
//                sb.Append((char)(c - 'ァ' + 'ぁ'));
//            }
//            else if (c == 'ヴ')
//            {
//                sb.Append('ゔ');
//            }
//            else
//            {
//                sb.Append(c);
//            }
//        }
//        return sb.ToString();
//    }

//    void Start()
//    {
//        TestCheckForKeyword();
//    }

//    // テスト用のメソッド
//    public void TestCheckForKeyword()
//    {
//        string testTweet = "ついーと";
//        bool result = CheckForKeyword(testTweet);
//        Debug.Log($"ツイート文にキーワードが含まれているか: {result}");
//    }
//}



//using UnityEngine;
//using System.Text;

//public class KeywordChecker : MonoBehaviour
//{
//    public string[] keywords; // 判定するキーワードの配列

//    // ツイート文にキーワードが含まれているかどうかを判定するメソッド
//    public bool CheckForKeyword(string tweetContent)
//    {
//        // ツイート文を小文字に変換し、ひらがなに変換
//        string normalizedTweetContent = ToHiragana(tweetContent.ToLower());

//        foreach (string keyword in keywords)
//        {
//            // キーワードを小文字に変換し、ひらがなに変換
//            string normalizedKeyword = ToHiragana(keyword.ToLower());

//            // ツイート文にキーワードが含まれているかどうかを判定
//            if (normalizedTweetContent.Contains(normalizedKeyword))
//            {
//                Debug.Log($"キーワード '{keyword}' がツイート文に含まれています。");
//                return true; // キーワードが含まれている場合はtrueを返す
//            }
//        }
//        Debug.Log("キーワードがツイート文に含まれていません。");
//        return false; // キーワードが含まれていない場合はfalseを返す
//    }

//    // カタカナをひらがなに変換するメソッド
//    private string ToHiragana(string input)
//    {
//        StringBuilder sb = new StringBuilder();
//        foreach (char c in input)
//        {
//            if (c >= 'ァ' && c <= 'ン')
//            {
//                sb.Append((char)(c - 'ァ' + 'ぁ'));
//            }
//            else if (c == 'ヴ')
//            {
//                sb.Append('ゔ');
//            }
//            else
//            {
//                sb.Append(c);
//            }
//        }
//        return sb.ToString();
//    }

//    void Start()
//    {
//        TestCheckForKeyword();
//    }

//    // テスト用のメソッド
//    public void TestCheckForKeyword()
//    {
//        string testTweet = "ツイート";
//        string convertedTweet = ToHiragana(testTweet.ToLower()); // カタカナをひらがなに変換

//        bool result = CheckForKeyword(convertedTweet);
//        Debug.Log($"ツイート文にキーワードが含まれているか: {result}");
//    }
//}


using UnityEngine;
using System.Text;
using TMPro;

public class KeywordChecker : MonoBehaviour
{
    public string[] keywords; // 判定するキーワードの配列
    public TMP_Text KeywordDisplayText;

    // ツイート文にキーワードが含まれているかどうかを判定するメソッド
    public bool CheckForKeyword(string tweetContent)
    {
        DisplayKeyword();
        // ツイート文を小文字に変換し、ひらがなに変換
        string normalizedTweetContent = ToHiragana(tweetContent.ToLower());

        foreach (string keyword in keywords)
        {
            // キーワードを小文字に変換し、ひらがなに変換
            string normalizedKeyword = ToHiragana(keyword.ToLower());

            // ツイート文にキーワードが含まれているかどうかを判定
            if (normalizedTweetContent.Contains(normalizedKeyword))
            {
                Debug.Log($"キーワード '{keyword}' がツイート文に含まれています。");
                return true; // キーワードが含まれている場合はtrueを返す
            }
        }
        Debug.Log("キーワードがツイート文に含まれていません。");
        return false; // キーワードが含まれていない場合はfalseを返す
    }

    // カタカナをひらがなに変換するメソッド
    private string ToHiragana(string input)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            if (c >= 'ァ' && c <= 'ン')
            {
                sb.Append((char)(c - 'ァ' + 'ぁ'));
            }
            else if (c == 'ヴ')
            {
                sb.Append('ゔ');
            }
            else if (char.IsUpper(c))
            {
                sb.Append(char.ToLower(c)); // アルファベットの大文字を小文字に変換
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    void Start()
    {
        TestCheckForKeyword();
    }

    [ContextMenu("キーワード表示")]
    public void DisplayKeyword()
    {
        string KeywordText = "";
        foreach (var keyword in keywords)
        {
            KeywordText += $" {keyword}\n"; // ここで keyword を直接使用する
        }
        KeywordDisplayText.text = KeywordText;
    }

    // テスト用のメソッド
    public void TestCheckForKeyword()
    {
        string testTweet = "ツイート";
        string convertedTweet = ToHiragana(testTweet.ToLower()); // カタカナをひらがなに変換

        bool result = CheckForKeyword(convertedTweet);
        Debug.Log($"ツイート文にキーワードが含まれているか: {result}");
    }
}