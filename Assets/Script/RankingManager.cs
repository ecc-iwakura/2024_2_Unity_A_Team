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

    public followplus followPlusScript; // followplus�̃C���X�^���X���Q�Ƃ���ϐ�

    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "playerProfiles.json");
        LoadPlayerProfiles();
        UpdateRanking();
        Debug.Log("Player profiles saved to: " + filePath); // �����Ńt�@�C���p�X�����O�ɏo��
    }

    // �v���C���[�v���t�@�C����ǉ����ă����L���O���X�V����
    public void AddPlayerProfile(string playerName, ulong score)
    {
        PlayerProfile existingProfile = playerProfiles.Find(profile => profile.playerName == playerName);

        if (existingProfile != null)
        {
            // �����̃v���t�@�C��������ꍇ�̓X�R�A���X�V
            if (score > existingProfile.score)
            {
                existingProfile.score = score;
            }
        }
        else
        {
            // �V�����v���t�@�C����ǉ�
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

    // �v���C���[�v���t�@�C����ۑ�����
    private void SavePlayerProfiles()
    {
        string jsonData = JsonUtility.ToJson(new Wrapper<PlayerProfile> { Items = playerProfiles.ToArray() }, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Player profiles saved to: " + filePath);
    }

    // �v���C���[�v���t�@�C�������[�h����
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

    // �����L���O���X�V���ăe�L�X�g�Ƃ��ĕ\������
    public void UpdateRanking()
    {
        // �X�R�A�Ń\�[�g
        playerProfiles.Sort((x, y) => y.score.CompareTo(x.score));

        // �����L���O���X�V
        UpdateRankingText();
    }

    // �����L���O�̃e�L�X�g���X�V����
    private void UpdateRankingText()
    {
        if (rank1 != null)
        {
            rank1.text = playerProfiles.Count > 0 ? FormatRankingText(1, playerProfiles[0]) : "1��: -";
        }

        if (rank2 != null)
        {
            rank2.text = playerProfiles.Count > 1 ? FormatRankingText(2, playerProfiles[1]) : "2��: -";
        }

        if (rank3 != null)
        {
            rank3.text = playerProfiles.Count > 2 ? FormatRankingText(3, playerProfiles[2]) : "3��: -";
        }
    }

    // �����L���O�e�L�X�g�̃t�H�[�}�b�g���s��
    private string FormatRankingText(int rank, PlayerProfile profile)
    {
        return $"{rank}��: {profile.playerName} - {profile.score}�_";
    }

    // JSON �V���A���C�Y/�f�V���A���C�Y�̂��߂̃��b�p�[�N���X
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
