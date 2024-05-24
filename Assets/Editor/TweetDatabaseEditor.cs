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

        // UpdateDictionariesFromList �֐������s����{�^����\��
        if (GUILayout.Button("Update Dictionaries"))
        {
            tweetDatabase.UpdateDictionariesFromList();
            EditorUtility.SetDirty(tweetDatabase);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.Space();

        // �c�C�[�gID�𐶐�����{�^����\��
        if (GUILayout.Button("Generate Tweet ID"))
        {
            generatedTweetID = GenerateTweetID();
            Debug.Log("Generated Tweet ID: " + generatedTweetID);
        }

        // �������ꂽ�c�C�[�gID��\��
        EditorGUILayout.LabelField("Generated Tweet ID:");
        EditorGUILayout.TextArea(generatedTweetID);
    }

    private string GenerateTweetID()
    {
        // UUID�`���̃c�C�[�gID�𐶐�����
        return Guid.NewGuid().ToString("N");
    }
}