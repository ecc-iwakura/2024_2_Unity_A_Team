using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TweetDatabase))]
public class TweetDatabaseEditor : Editor
{
    private string generatedTweetID = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TweetDatabase tweetDatabase = (TweetDatabase)target;

        EditorGUILayout.Space();

        // UpdateDictionariesFromList 関数を実行するボタンを表示
        if (GUILayout.Button("Update Dictionaries"))
        {
            tweetDatabase.UpdateDictionariesFromList();
            EditorUtility.SetDirty(tweetDatabase);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();

        // ツイートIDを生成するボタンを表示
        if (GUILayout.Button("Generate Tweet IDs"))
        {
            GenerateTweetIDs(tweetDatabase);
            Debug.Log("Tweet IDs generated for all tweets.");
        }

        EditorGUILayout.Space();

        // 親のアカウントID を持つ各ツイートに自動的に設定するボタンを表示
        if (GUILayout.Button("Set Parent Account IDs"))
        {
            SetParentAccountIDs(tweetDatabase);
        }
    }

    private void GenerateTweetIDs(TweetDatabase tweetDatabase)
    {
        foreach (var accountInfo in tweetDatabase.accountList)
        {
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                // ツイートIDがまだ入力されていない場合のみ新しいIDを生成
                if (string.IsNullOrEmpty(tweetInfo.tweetID))
                {
                    // UUID形式のツイートIDを生成する
                    tweetInfo.tweetID = Guid.NewGuid().ToString("N");
                }
            }
        }
    }

    private void SetParentAccountIDs(TweetDatabase tweetDatabase)
    {
        foreach (var accountInfo in tweetDatabase.accountList)
        {
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                // 各ツイートに親のアカウントID を設定する
                tweetInfo.parentAccountID = accountInfo.accountID;
            }
        }

        Debug.Log("Parent Account IDs set for all tweets.");
    }
}
