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
    }

    private void GenerateTweetIDs(TweetDatabase tweetDatabase)
    {
        foreach (var accountInfo in tweetDatabase.accountList)
        {
            foreach (var tweetInfo in accountInfo.tweetList)
            {
                // �c�C�[�gID���܂����͂���Ă��Ȃ��ꍇ�̂ݐV����ID�𐶐�
                if (string.IsNullOrEmpty(tweetInfo.tweetID))
                {
                    // UUID�`���̃c�C�[�gID�𐶐�����
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
                // �e�c�C�[�g�ɐe�̃A�J�E���gID ��ݒ肷��
                tweetInfo.parentAccountID = accountInfo.accountID;
            }
        }

        Debug.Log("Parent Account IDs set for all tweets.");
    }
}
