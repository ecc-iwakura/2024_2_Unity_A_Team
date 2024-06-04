//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class WaveTransition : MonoBehaviour
//{
//    public Material transitionMaterial;
//    public float duration = 1.0f;
//    public Color waveColor = Color.red; // �f�t�H���g�̔g�G�t�F�N�g�̐F��ݒ�

//    private float progress = 0.0f;
//    private bool isTransitioning = false;

//    void Start()
//    {
//        // �Q�[���J�n���Ƀ}�e���A���̐F�ݒ���s��
//        transitionMaterial.SetColor("_WaveColor", waveColor);
//    }

//    void Update()
//    {
//        if (isTransitioning)
//        {
//            progress += Time.deltaTime / duration;
//            transitionMaterial.SetFloat("_Progress", progress);

//            if (progress >= 1.0f)
//            {
//                isTransitioning = false;
//                SceneManager.LoadScene("Ogura_Main"); // ���C���V�[���ɑJ��
//            }
//        }
//    }

//    public void StartTransition()
//    {
//        // �g�̃G�t�F�N�g���J�n����O�Ƀ}�e���A���̐F�ݒ���s��
//        transitionMaterial.SetColor("_WaveColor", waveColor);
//        isTransitioning = true;
//    }

//    //�쐬�{�^�����������Ƃ��ɌĂяo����郁�\�b�h
//    public void OnCreateButtonClicked()
//    {
//        // �f�t�H���g�̔g�G�t�F�N�g�̐F��ݒ肵����
//        transitionMaterial.SetColor("_WaveColor", waveColor);
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveTransition : MonoBehaviour
{
    public Material transitionMaterial;
    public float duration = 1.0f;
    public Color waveColor = Color.red; // �f�t�H���g�̔g�G�t�F�N�g�̐F��ݒ�

    private float progress = 0.0f;
    private bool isTransitioning = false;

    void Start()
    {
        // �Q�[���J�n�O�Ƀ}�e���A���̐F��ݒ肷��
        transitionMaterial.SetColor("_WaveColor", waveColor);
    }

    void Update()
    {
        if (isTransitioning)
        {
            progress += Time.deltaTime / duration;
            transitionMaterial.SetFloat("_Progress", progress);

            if (progress >= 1.0f)
            {
                isTransitioning = false;
                SceneManager.LoadScene("Ogura_Main"); // ���C���V�[���ɑJ��
            }
        }
    }

    public void StartTransition()
    {
        isTransitioning = true;
    }

    // �쐬�{�^�����������Ƃ��ɌĂяo����郁�\�b�h
    public void OnCreateButtonClicked()
    {
        // �f�t�H���g�̔g�G�t�F�N�g�̐F��ݒ肵����
        transitionMaterial.SetColor("_WaveColor", waveColor);
    }
}

