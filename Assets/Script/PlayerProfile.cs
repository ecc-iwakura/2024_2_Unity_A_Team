using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;
    public int score;

    public PlayerProfile(string playerName, int score)
    {
        this.playerName = playerName;
        this.score = score;
    }
}


