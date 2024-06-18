using UnityEngine;
using UnityEditor;
using System;
using System.IO;

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

        EditorGUILayout.Space();

        // エクスポートボタンを表示
        if (GUILayout.Button("Export to JSON"))
        {
            string filePath = EditorUtility.SaveFilePanel("Export Tweet Data to JSON", "", "TweetDatabase.json", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                tweetDatabase.ExportToJson(filePath);
            }
        }

        EditorGUILayout.Space();

        // インポートボタンを表示
        if (GUILayout.Button("Import from JSON"))
        {
            string filePath = EditorUtility.OpenFilePanel("Import Tweet Data from JSON", "", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                tweetDatabase.ImportFromJson(filePath);
            }
        }
    }

    private void GenerateTweetIDs(TweetDatabase tweetDatabase)
    {
        foreach (var accountInfo in tweetDatabase.accountList)
        {
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                if (string.IsNullOrEmpty(tweetInfo.tweetID))
                {
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
                tweetInfo.parentAccountID = accountInfo.accountID;
            }
        }

        Debug.Log("Parent Account IDs set for all tweets.");
    }
}