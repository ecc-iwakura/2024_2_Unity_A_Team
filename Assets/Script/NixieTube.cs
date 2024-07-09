using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DigitObjectArray
{
    public GameObject[] digitObjects; // 0から9までのオブジェクトを格納する配列
}

public class NixieTube : MonoBehaviour
{
    public DigitObjectArray[] digitArrays; // 各桁の1から9、0までのオブジェクトを格納する配列の配列
    public GameObject[] unitObjects;      // 単位（K、M、B、Tなど）のオブジェクトを格納する配列

    public ulong currentValue = 0; // 現在表示している値
    private Coroutine currentCoroutine; // 現在実行中のコルーチン

    // Start is called before the first frame update
    void Start()
    {
        UpdateDisplay(1);
    }

    // Update is called once per frame
    void Update()
    {
        // 例えば、値が変化するタイミングで更新する場合はここに処理を追加する
    }

    // 数値に応じて表示を更新するメソッド
    [ContextMenu("UpdateDisplay")]
    public void UpdateDisplay(ulong valueToShow)
    {
        // 既存のコルーチンが実行中であれば停止する
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 新しい値でコルーチンを開始して、値を順番に更新する
        currentCoroutine = StartCoroutine(AnimateDisplayChange(valueToShow));
    }

    // 数値の変更をアニメーションで行うコルーチン
    private IEnumerator AnimateDisplayChange(ulong targetValue)
    {
        while (currentValue != targetValue)
        {
            // 目標値と現在の値の差に基づいて待ち時間と増加量を調整
            ulong valueDifference = targetValue > currentValue ? targetValue - currentValue : currentValue - targetValue;
            int magnitudeDifference = Mathf.FloorToInt(Mathf.Log10((float)valueDifference));
            int divisionFactor = magnitudeDifference / 3;
            int remainderFactor = magnitudeDifference % 3 + 1;

            ulong increment = (ulong)Mathf.Pow(10, 3 * divisionFactor);
            float waitTime = Mathf.Pow(0.1f, remainderFactor);

            // 現在の値を適切な増加量で目標値に近づける
            if (currentValue < targetValue)
            {
                currentValue = (ulong)Mathf.Min(currentValue + increment, targetValue);
            }
            else if (currentValue > targetValue)
            {
                currentValue = (ulong)Mathf.Max(currentValue - increment, targetValue);
            }

            // 現在の値を表示する
            DisplayValue(currentValue);

            // 少し待ってから次の更新を行う
            yield return new WaitForSeconds(waitTime);
        }
    }


    // 実際に表示を更新するメソッド
    private void DisplayValue(ulong valueToShow)
    {
        // 表示する単位のインデックスを初期化
        int unitIndex = 0;
        ulong displayValue = valueToShow;

        // 数値が1000を超える場合、適切な単位に切り替える
        while (displayValue >= 1000 && unitIndex < unitObjects.Length)
        {
            displayValue /= 1000;
            unitIndex++;
        }

        // 値を文字列に変換して、3桁になるように0でパディングする
        string valueString = displayValue.ToString().PadLeft(3, '0');

        // 各桁のオブジェクトを表示する
        for (int i = 0; i < 3; i++)
        {
            int digit = int.Parse(valueString[i].ToString());
            // 対応する桁のオブジェクトをアクティブにする
            for (int j = 0; j < digitArrays[i].digitObjects.Length; j++)
            {
                digitArrays[i].digitObjects[j].SetActive(j == digit);
            }
        }

        // 単位を表示する
        DisplayUnits(unitIndex);
    }

    // 単位を表示するメソッド
    public void DisplayUnits(int index)
    {
        // 単位のオブジェクトをアクティブにする
        for (int i = 0; i < unitObjects.Length; i++)
        {
            if (unitObjects[i] != null)
            {
                unitObjects[i].SetActive(i == index);
            }
        }
    }
}