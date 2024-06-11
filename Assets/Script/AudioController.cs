using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public AudioSource audio_SE;
    public AudioSource audio_battleBGM;
    public AudioSource audio_themeBGM;
    public AudioClip[] audioClip_SE;

    public Slider SE_Slider;
    public Slider BGM_Slider;

    private float SE_Volume;
    private float BGM_Volume;

    private bool IsButton = false;

    void Start()
    {
        audio_SE.volume = PlayerPrefs.GetFloat("SE_Volume", 0.35f);
        audio_battleBGM.volume = PlayerPrefs.GetFloat("BGM_Volume", 0.10f);
        audio_themeBGM.volume = PlayerPrefs.GetFloat("BGM_Volume", 0.10f);
    }

    void Update()
    {

    }

    public void PlayOnSound(int Index)
    {
        audio_SE.PlayOneShot(audioClip_SE[Index]);
        UnityEngine.Debug.Log("効果音を再生しました: " + Index);
    }

    public void PlayBattleBGM(int play)
    {
        switch (play)
        {
            case 0:
                {
                    audio_battleBGM.Stop();
                    UnityEngine.Debug.Log("BGMを停止しました");
                    break;
                }
            case 1:
                {
                    audio_battleBGM.Play();
                    UnityEngine.Debug.Log("BGMを再生しました");
                    break;
                }
            case 2:
                {
                    audio_battleBGM.Pause();
                    UnityEngine.Debug.Log("BGMを一時停止しました");
                    break;
                }
            case 3:
                {
                    audio_battleBGM.UnPause();
                    UnityEngine.Debug.Log("BGMの一時停止を解除しました");
                    break;
                }
            default:
                {
                    UnityEngine.Debug.Log("不明な操作が行われました");
                    break;
                }
        }
    }

    public void PlayThemeBGM(int play)
    {
        switch (play)
        {
            case 0:
                {
                    audio_themeBGM.Stop();
                    UnityEngine.Debug.Log("BGMを停止しました");
                    break;
                }
            case 1:
                {
                    audio_themeBGM.Play();
                    UnityEngine.Debug.Log("BGMを再生しました");
                    break;
                }
            case 2:
                {
                    audio_themeBGM.Pause();
                    UnityEngine.Debug.Log("BGMを一時停止しました");
                    break;
                }
            case 3:
                {
                    audio_themeBGM.UnPause();
                    UnityEngine.Debug.Log("BGMの一時停止を解除しました");
                    break;
                }
            case 4:
                {
                    if (!IsButton)
                    {
                        audio_themeBGM.Play();
                        UnityEngine.Debug.Log("BGMを再生しました");
                        IsButton = true;
                    }
                    else
                    {
                        audio_themeBGM.Stop();
                        UnityEngine.Debug.Log("BGMを停止しました");
                        IsButton = false;
                    }
                    break;
                }
            default:
                {
                    UnityEngine.Debug.Log("不明な操作が行われました");
                    break;
                }
        }
    }

    public void SE_VolumeApply()
    {
        SE_Volume = SE_Slider.value;
        audio_SE.volume = SE_Volume;
    }

    public void BGM_VolumeApply()
    {
        BGM_Volume = BGM_Slider.value;
        audio_battleBGM.volume = BGM_Volume;
        audio_themeBGM.volume = BGM_Volume;
    }

    public void VolumeApply()
    {
        audio_SE.volume = SE_Volume;
        audio_battleBGM.volume = BGM_Volume;
        audio_themeBGM.volume = BGM_Volume;
        PlayerPrefs.SetFloat("SE_Volume", SE_Volume);
        PlayerPrefs.SetFloat("BGM_Volume", BGM_Volume);
    }

    public void SliderApply()
    {
        SE_Slider.value = PlayerPrefs.GetFloat("SE_Volume", 0.5f); ;
        BGM_Slider.value = PlayerPrefs.GetFloat("BGM_Volume", 0.5f); ;
    }
}