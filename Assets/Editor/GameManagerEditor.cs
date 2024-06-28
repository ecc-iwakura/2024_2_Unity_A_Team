using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager gameManager = (GameManager)target;

        GUILayout.Space(20);

        // Save Game Data ボタン
        if (GUILayout.Button("Save Game Data"))
        {
            string filePath = EditorUtility.SaveFilePanel("Save Game Data", Application.persistentDataPath, "gameData.json", "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                gameManager.SaveGameData(filePath);
            }
        }

        // Load Game Data ボタン
        if (GUILayout.Button("Load Game Data"))
        {
            string filePath = EditorUtility.OpenFilePanel("Load Game Data", Application.persistentDataPath, "json");
            if (!string.IsNullOrEmpty(filePath))
            {
                gameManager.LoadGameData(filePath);
            }
        }
    }
}
