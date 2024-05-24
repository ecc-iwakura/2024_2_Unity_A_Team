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
        if (GUILayout.Button("Generate Tweet ID"))
        {
            generatedTweetID = GenerateTweetID();
            Debug.Log("Generated Tweet ID: " + generatedTweetID);
        }

        // 生成されたツイートIDを表示
        EditorGUILayout.LabelField("Generated Tweet ID:");
        EditorGUILayout.TextArea(generatedTweetID);
    }

    private string GenerateTweetID()
    {
        // UUID形式のツイートIDを生成する
        return Guid.NewGuid().ToString("N");
    }
}