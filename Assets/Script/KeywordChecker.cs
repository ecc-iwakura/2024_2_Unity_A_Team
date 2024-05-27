using UnityEngine;

public class KeywordChecker : MonoBehaviour
{
    public string[] keywords; // 判定するキーワードの配列

    // ツイート文にキーワードが含まれているかどうかを判定するメソッド
    public bool CheckForKeyword(string tweetContent)
    {
        foreach (string keyword in keywords)
        {
            // ツイート文にキーワードが含まれているかどうかを判定
            if (tweetContent.ToLower().Contains(keyword.ToLower()))
            {
                return true; // キーワードが含まれている場合はtrueを返す
            }
        }
        return false; // キーワードが含まれていない場合はfalseを返す
    }
}
