using UnityEngine;
using TMPro;

public class followplus : MonoBehaviour
{
    public NixieTube nixieTube;
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


    public TMP_Text changeInFollowersText;

    void Start()
    {
        if(nixieTube == null)
        {
            UnityEngine.Debug.LogError("nixieTubeがありません！");
        }
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
            ulong increaseAmountFromRate = (ulong)(followers * increaseRate); // 増加する割合部分のフォロワー数
            ulong fixedIncrease = (ulong)Random.Range((int)minFixedIncrease, (int)(maxFixedIncrease + 1)); // 1～10の固定値
            increaseAmount = (ulong)increaseAmountFromRate + (ulong)fixedIncrease; // 合計増加フォロワー数
            followers += (ulong)increaseAmount;
            Debug.LogWarning($"increaseRate {increaseAmountFromRate} (固定値: {fixedIncrease}) 合計加算値{increaseAmount}");
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

    private void UpdateUI(ulong changeAmount, bool isDecrease)
    {

        string formattedFollowers = FormatNumber(followers);
        nixieTube.UpdateDisplay(followers);

        if (!isDecrease)
        {
            string changeText = FormatNumber(changeAmount);
            changeInFollowersText.text = $"UP: +{changeText}";
            changeInFollowersText.color = Color.green; // 青色に設定
        }
        else
        {
            string changeText = FormatNumber(changeAmount);
            changeInFollowersText.text = $"DOWN: -{changeText}";
            changeInFollowersText.color = new Color(243f / 255f, 152f / 255f, 0f / 255f, 1f); // オレンジ色に設定
        }
    }


    // 数字をK, M, B, Tなどの表記にフォーマットする関数
    private string FormatNumber(ulong number)
    {
        // 1000以上の数に対して、適切な接尾辞を付けてフォーマットする
        if (number >= 1000000000000) // 兆 (T)
        {
            return $"{number / 1000000000000f:F1}T";
        }
        else if (number >= 1000000000) // 十億 (B)
        {
            return $"{number / 1000000000f:F1}B";
        }
        else if (number >= 1000000) // 百万 (M)
        {
            return $"{number / 1000000f:F1}M";
        }
        else if (number >= 1000) // 千 (K)
        {
            return $"{number / 1000f:F1}K";
        }
        else // 1000未満はそのまま表示
        {
            return number.ToString();
        }
    }
}
