using UnityEngine;

public class KeywordChecker : MonoBehaviour
{
    public string[] keywords; // ���肷��L�[���[�h�̔z��

    // �c�C�[�g���ɃL�[���[�h���܂܂�Ă��邩�ǂ����𔻒肷�郁�\�b�h
    public bool CheckForKeyword(string tweetContent)
    {
        foreach (string keyword in keywords)
        {
            // �c�C�[�g���ɃL�[���[�h���܂܂�Ă��邩�ǂ����𔻒�
            if (tweetContent.ToLower().Contains(keyword.ToLower()))
            {
                return true; // �L�[���[�h���܂܂�Ă���ꍇ��true��Ԃ�
            }
        }
        return false; // �L�[���[�h���܂܂�Ă��Ȃ��ꍇ��false��Ԃ�
    }
}
