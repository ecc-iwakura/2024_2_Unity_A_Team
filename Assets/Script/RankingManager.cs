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

    public followplus followPlusScript; // 追加: followplusのインスタンスを参照する変数

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerProfiles.json");
        LoadPlayerProfiles();
        UpdateRanking();
    }

    // プレイヤープロファイルを追加してランキングを更新する
    public void AddPlayerProfile(string playerName)
    {
        // followplusからmaxFollowersを取得
        ulong maxFollowers = followPlusScript != null ? followPlusScript.maxFollowers : 0;

        PlayerProfile profile = new PlayerProfile(playerName, (int)maxFollowers);
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
        // デフォルトで全てのランキングを「-」で表示
        string[] rankings = { "1位: -", "2位: -", "3位: -" };

        // プレイヤープロファイルが存在する場合、ランキングを設定
        for (int i = 0; i < playerProfiles.Count && i < 3; i++)
        {
            rankings[i] = FormatRankingText(i + 1, playerProfiles[i]);
        }

        // テキストに表示を更新
        if (rankText1 != null)
            rankText1.text = rankings[0];
        if (rankText2 != null)
            rankText2.text = rankings[1];
        if (rankText3 != null)
            rankText3.text = rankings[2];
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
