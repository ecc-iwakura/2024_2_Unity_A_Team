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

        // UpdateDictionariesFromList �֐������s����{�^����\��
        if (GUILayout.Button("Update Dictionaries"))
        {
            tweetDatabase.UpdateDictionariesFromList();
            EditorUtility.SetDirty(tweetDatabase);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();

        // �c�C�[�gID�𐶐�����{�^����\��
        if (GUILayout.Button("Generate Tweet IDs"))
        {
            GenerateTweetIDs(tweetDatabase);
            Debug.Log("Tweet IDs generated for all tweets.");
        }

        EditorGUILayout.Space();

        // �e�̃A�J�E���gID �����e�c�C�[�g�Ɏ����I�ɐݒ肷��{�^����\��
        if (GUILayout.Button("Set Parent Account IDs"))
        {
            SetParentAccountIDs(tweetDatabase);
        }

        EditorGUILayout.Space();

        // �G�N�X�|�[�g�{�^����\��
        if (GUILayout.Button("Export to JSON"))
        {
            string filePath = EditorUtility.SaveFilePanel("Export Tweet Data to JSON", "", "TweetDatabase.json", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                tweetDatabase.ExportToJson(filePath);
            }
        }

        EditorGUILayout.Space();

        // �C���|�[�g�{�^����\��
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