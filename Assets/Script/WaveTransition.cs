//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class WaveTransition : MonoBehaviour
//{
//    public Material transitionMaterial;
//    public float duration = 1.0f;
//    public Color waveColor = Color.red; // デフォルトの波エフェクトの色を設定

//    private float progress = 0.0f;
//    private bool isTransitioning = false;

//    void Start()
//    {
//        // ゲーム開始時にマテリアルの色設定を行う
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
//                SceneManager.LoadScene("Ogura_Main"); // メインシーンに遷移
//            }
//        }
//    }

//    public void StartTransition()
//    {
//        // 波のエフェクトを開始する前にマテリアルの色設定を行う
//        transitionMaterial.SetColor("_WaveColor", waveColor);
//        isTransitioning = true;
//    }

//    //作成ボタンを押したときに呼び出されるメソッド
//    public void OnCreateButtonClicked()
//    {
//        // デフォルトの波エフェクトの色を設定し直す
//        transitionMaterial.SetColor("_WaveColor", waveColor);
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveTransition : MonoBehaviour
{
    public Material transitionMaterial;
    public float duration = 1.0f;
    public Color waveColor = Color.red; // デフォルトの波エフェクトの色を設定

    private float progress = 0.0f;
    private bool isTransitioning = false;

    void Start()
    {
        // ゲーム開始前にマテリアルの色を設定する
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
                SceneManager.LoadScene("Ogura_Main"); // メインシーンに遷移
            }
        }
    }

    public void StartTransition()
    {
        isTransitioning = true;
    }

    // 作成ボタンを押したときに呼び出されるメソッド
    public void OnCreateButtonClicked()
    {
        // デフォルトの波エフェクトの色を設定し直す
        transitionMaterial.SetColor("_WaveColor", waveColor);
    }
}

