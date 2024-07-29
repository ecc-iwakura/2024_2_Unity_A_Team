using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class RankingManager : MonoBehaviour
{
    public List<PlayerProfile> playerProfiles = new List<PlayerProfile>();
    public TMP_Text rank1;
    public TMP_Text rank2;
    public TMP_Text rank3;

    public followplus followPlusScript; // followplusのインスタンスを参照する変数

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerProfiles.json");
        LoadPlayerProfiles();
        UpdateRanking();
        Debug.Log("Player profiles saved to: " + filePath); // ここでファイルパスをログに出力
    }

    // プレイヤープロファイルを追加してランキングを更新する
    public void AddPlayerProfile(string playerName, ulong score)
    {
        PlayerProfile existingProfile = playerProfiles.Find(profile => profile.playerName == playerName);

        if (existingProfile != null)
        {
            // 既存のプロファイルがある場合はスコアを更新
            if (score > existingProfile.score)
            {
                existingProfile.score = score;
            }
        }
        else
        {
            // 新しいプロファイルを追加
            PlayerProfile profile = new PlayerProfile(playerName, score);
            playerProfiles.Add(profile);
        }

        SavePlayerProfiles();
        UpdateRanking();
    }

    public void SaveCurrentScore(ulong currentFollowers, string playerName = "Player")
    {
        AddPlayerProfile(playerName, currentFollowers);
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
        if (rank1 != null)
        {
            rank1.text = playerProfiles.Count > 0 ? FormatRankingText(1, playerProfiles[0]) : "1位: -";
        }

        if (rank2 != null)
        {
            rank2.text = playerProfiles.Count > 1 ? FormatRankingText(2, playerProfiles[1]) : "2位: -";
        }

        if (rank3 != null)
        {
            rank3.text = playerProfiles.Count > 2 ? FormatRankingText(3, playerProfiles[2]) : "3位: -";
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

    [System.Serializable]
    public class PlayerProfile
    {
        public string playerName;
        public ulong score;

        public PlayerProfile(string playerName, ulong score)
        {
            this.playerName = playerName;
            this.score = score;
        }
    }
}
