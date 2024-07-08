using UnityEngine;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public Text highScoreText;

    public void ShowHighScore()
    {
        // �ō��X�R�A���擾
        if (Score.instance != null)
        {
            int highScore = Score.instance.GetHighScore();

            // �e�L�X�g���X�V
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


