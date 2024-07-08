using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public Text highScoreText;

    public void ShowHighScore()
    {
        // 最高スコアを取得
        if (Score.instance != null)
        {
            int highScore = Score.instance.GetHighScore();

            // テキストを更新
            highScoreText.text = "High Score: " + highScore;
        }
        else
        {
            Debug.LogError("Score instance is null.");
        }
    }

    public void SaveAndShowScore(int newScore)
    {
        if (Score.instance != null)
        {
            Score.instance.SaveScore(newScore);
            ShowHighScore();
        }
        else
        {
            Debug.LogError("Score instance is null.");
        }
    }
}


