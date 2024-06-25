using UnityEngine;
using TMPro;

public class followplus : MonoBehaviour
{
    public ulong followers = 0; // フォロワー数を管理する変数
    public ulong maxFollowers = 0; // 最高到達点のフォロワー数
    private bool firstCorrectAction = true; // 初めて正しい行動が行われたかどうかを管理するフラグ

    [Header("フォロワー増加設定")]
    [Tooltip("フォロワー数が増加する最小割合")]
    public float minIncreaseRate = 0.05f; // 最小増加率
    [Tooltip("フォロワー数が増加する最大割合")]
    public float maxIncreaseRate = 0.10f; // 最大増加率
    [Tooltip("フォロワー数が増加する最小固定値")]
    public int minFixedIncrease = 1; // 最小固定増加値
    [Tooltip("フォロワー数が増加する最大固定値")]
    public int maxFixedIncrease = 10; // 最大固定増加値

    [Header("フォロワー減少設定")]
    [Tooltip("最高到達点のフォロワー数から減少する最小割合")]
    public float minDecreaseRate = 0.40f; // 最小減少率
    [Tooltip("最高到達点のフォロワー数から減少する最大割合")]
    public float maxDecreaseRate = 0.50f; // 最大減少率

    // UIテキストコンポーネントへの参照
    public TMP_Text totalFollowersText;
    public TMP_Text changeInFollowersText;

    void Start()
    {
        UpdateUI(0, false); // 初期状態を更新
    }

    // 正しい行動をした時に呼び出される関数
    [ContextMenu("CorrectAction")]
    public void CorrectAction()
    {
        ulong increaseAmount = 0;

        if (firstCorrectAction)
        {
            followers += 1; // 初めての正しい行動の場合、フォロワーが固定で1増加
            firstCorrectAction = false; // 初めての正しい行動が行われたフラグをfalseに設定
            increaseAmount = 1;
        }
        else
        {
            // フォロワーが5%～10%で増加し、さらに1～10の固定値を追加
            float increaseRate = Random.Range(minIncreaseRate, maxIncreaseRate); // 5%～10%の増加率
            ulong increaseAmountFromRate = (ulong)Mathf.CeilToInt(followers * increaseRate); // 増加する割合部分のフォロワー数
            ulong fixedIncrease = (ulong)Random.Range((int)minFixedIncrease, (int)(maxFixedIncrease + 1)); // 1～10の固定値
            increaseAmount = (ulong)increaseAmountFromRate + (ulong)fixedIncrease; // 合計増加フォロワー数

            followers += (ulong)increaseAmount;
        }

        // 最高到達点の更新
        if (followers > maxFollowers)
        {
            maxFollowers = followers;
        }

        Debug.Log($"正しい行動が実行されました！フォロワー数: {followers} (増加数: {increaseAmount})");
        UpdateUI((ulong)increaseAmount,false);
    }

    // 間違った行動をした時に呼び出される関数
    [ContextMenu("IncorrectAction")]
    public void IncorrectAction()
    {
        // 最高到達点のフォロワー数の40%～50%で減少
        float decreaseRate = Random.Range(minDecreaseRate, maxDecreaseRate); // 40%～50%の減少率
        ulong decreaseAmount = (ulong)Mathf.CeilToInt(maxFollowers * decreaseRate); // 減少するフォロワー数
        if (decreaseAmount > followers)
        {
            decreaseAmount = followers; // フォロワー数より減少量が多い場合は全て減らす
        }
        followers -= (ulong)decreaseAmount;

        Debug.Log($"間違った行動が実行されました...フォロワー数: {followers} (減少数: {decreaseAmount})");
        UpdateUI((ulong)decreaseAmount, true);
    }

    // 行動を評価する関数
    public void EvaluateAction(bool isCorrect)
    {
        if (isCorrect)
        {
            CorrectAction();
        }
        else
        {
            IncorrectAction();
        }
    }

    // UIを更新する関数
    private void UpdateUI(ulong changeAmount,bool m)
    {
        totalFollowersText.text = $"{followers}";
        if (!m)
        {
            changeInFollowersText.text = $"UP: +{changeAmount}";
            changeInFollowersText.color = Color.blue; // 青色に設定
        }
        else
        {
            changeInFollowersText.text = $"DOWN: -{changeAmount}";
            changeInFollowersText.color = Color.red; // 赤色に設定
        }
    }
}
