using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class RankingManager : MonoBehaviour
{
    public List<PlayerProfile> playerProfiles = new List<PlayerProfile>();
    public TMP_Text rankText1;
    public TMP_Text rankText2;
    public TMP_Text rankText3;

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerProfiles.json");
        LoadPlayerProfiles();
        UpdateRanking();
    }

    // プレイヤープロファイルを追加してランキングを更新する
    public void AddPlayerProfile(string playerName, int score)
    {
        PlayerProfile profile = new PlayerProfile(playerName, score);
        playerProfiles.Add(profile);
        SavePlayerProfiles();
        UpdateRanking();
    }

    // プレイヤープロファイルを保存する
    private void SavePlayerProfiles()
    {
        string jsonData = JsonUtility.ToJson(new Wrapper<PlayerProfile> { Items = playerProfiles.ToArray() }, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Player profiles saved to: " + filePath);
    }

    // プレイヤープロファイルをロードする
    private void LoadPlayerProfiles()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            var loadedProfiles = JsonUtility.FromJson<Wrapper<PlayerProfile>>(jsonData).Items;
            playerProfiles = new List<PlayerProfile>(loadedProfiles);
            Debug.Log("Player profiles loaded from: " + filePath);
        }
        else
        {
            Debug.LogWarning("No saved player profiles found at: " + filePath);
        }
    }

    // ランキングを更新してテキストとして表示する
    public void UpdateRanking()
    {
        // スコアでソート
        playerProfiles.Sort((x, y) => y.score.CompareTo(x.score));

        // ランキングを更新
        UpdateRankingText();
    }

    // ランキングのテキストを更新する
    private void UpdateRankingText()
    {
        if (playerProfiles.Count > 0 && rankText1 != null)
        {
            rankText1.text = FormatRankingText(1, playerProfiles[0]);
        }
        else if (rankText1 != null)
        {
            rankText1.text = "1位: -";
        }

        if (playerProfiles.Count > 1 && rankText2 != null)
        {
            rankText2.text = FormatRankingText(2, playerProfiles[1]);
        }
        else if (rankText2 != null)
        {
            rankText2.text = "2位: -";
        }

        if (playerProfiles.Count > 2 && rankText3 != null)
        {
            rankText3.text = FormatRankingText(3, playerProfiles[2]);
        }
        else if (rankText3 != null)
        {
            rankText3.text = "3位: -";
        }
    }

    // ランキングテキストのフォーマットを行う
    private string FormatRankingText(int rank, PlayerProfile profile)
    {
        return $"{rank}位: {profile.playerName} - {profile.score}点";
    }

    // JSON シリアライズ/デシリアライズのためのラッパークラス
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
