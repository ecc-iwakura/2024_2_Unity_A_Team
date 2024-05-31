using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // パブリック関数 ToggleActive を定義
    public void ToggleActive()
    {
        // このオブジェクトのアクティブ状態をトグル（反転）する
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void SetActive(bool IsActive)
    {
        // このオブジェクトのアクティブ状態をトグル（反転）する
        gameObject.SetActive(IsActive);
    }
}
