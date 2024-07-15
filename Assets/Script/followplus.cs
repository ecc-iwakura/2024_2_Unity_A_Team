using System.Collections; // これを追加
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class followplus : MonoBehaviour
{
    public NixieTube nixieTube;
    public ulong followers = 0; // フォロワー数を管理する変数
    public ulong maxFollowers = 0; // 最高到達点のフォロワー数
    private bool firstCorrectAction = true; // 初めて正しい行動が行われたかどうかを管理するフラグ
    public Image image;

    [Header("HPバー")]
    public Image orangeHPBar; // オレンジのHPバー
    public Image greenHPBar; // 緑のHPバー
    public float updateDuration = 0.5f; // バーの更新にかける時間

    [Header("フォロワー増加設定")]
    [Tooltip("フォロワー数が増加する最小割合")]
    public float minIncreaseRate = 0.05f; // 最小増加率
    [Tooltip("フォロワー数が増加する最大割合")]
    public float maxIncreaseRate = 0.10f; // 最大増加率
    [Tooltip("フォロワー数が増加する最小固定値")]
    public int minFixedIncrease = 1; // 最小固定増加値
    [Tooltip("フォロワー数が増加する最大固定値")]
    public int maxFixedIncrease = 10; // 最大固定増加値
    [Tooltip("最高到達点のフォロワー数の割合で増加する値")]
    public float maxFollowersIncreaseRate = 0.02f; // maxFollowersの割合で増加する割合

    [Header("フォロワー減少設定")]
    [Tooltip("最高到達点のフォロワー数から減少する最小割合")]
    public float minDecreaseRate = 0.40f; // 最小減少率
    [Tooltip("最高到達点のフォロワー数から減少する最大割合")]
    public float maxDecreaseRate = 0.35f; // 最大減少率


    public TMP_Text changeInFollowersText;
    public TMP_Text changeTypeText; // UPやDOWNを表示するテキスト
    public Animator textAnimator; // アニメーターを参照するための変数


    void Start()
    {

        if (nixieTube == null)
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
            // フォロワーが5%～10%で増加し、さらに1～10の固定値を追加し、maxFollowersの2%を追加
            float increaseRate = Random.Range(minIncreaseRate, maxIncreaseRate); // 5%～10%の増加率
            ulong increaseAmountFromRate = (ulong)(followers * increaseRate); // 増加する割合部分のフォロワー数
            ulong fixedIncrease = (ulong)Random.Range((int)minFixedIncrease, (int)(maxFixedIncrease + 1)); // 1～10の固定値
            ulong increaseAmountFromMaxFollowers = (ulong)(maxFollowers * maxFollowersIncreaseRate); // maxFollowersの割合で増加するフォロワー数
            increaseAmount = increaseAmountFromRate + fixedIncrease + increaseAmountFromMaxFollowers; // 合計増加フォロワー数
            followers += increaseAmount;
            Debug.LogWarning($"increaseRate {increaseAmountFromRate} (固定値: {fixedIncrease}) maxFollowers増加: {increaseAmountFromMaxFollowers} 合計加算値{increaseAmount}");
        }

        // 最高到達点の更新
        if (followers > maxFollowers)
        {
            maxFollowers = followers;
        }

        Debug.Log($"正しい行動が実行されました！フォロワー数: {followers} (増加数: {increaseAmount})");
        UpdateUI(increaseAmount, false);
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

        SetImageTransparency(CalculateScaling(followers, maxFollowers));
    }

    private void UpdateUI(ulong changeAmount, bool isDecrease)
    {
        (int intValue, string unit) = FormatNumber(changeAmount);
        nixieTube.UpdateDisplay(followers);
        float changeRatio = (float)changeAmount / followers; // 変化したフォロワー数の割合

        if (!isDecrease)
        {
            if (changeTypeText != null)
            {
                changeTypeText.text = "UP"; // UPを表示
                changeTypeText.color = Color.green; // 青色に設定
            }

            changeInFollowersText.text = $"+{intValue}{unit}";
            changeInFollowersText.color = Color.green; // 青色に設定

        }
        else
        {
            if (changeTypeText != null)
            {
                changeTypeText.text = "DOWN"; // DOWNを表示
                changeTypeText.color = new Color(243f / 255f, 152f / 255f, 0f / 255f, 1f); // オレンジ色に設
            }
            changeInFollowersText.text = $"-{intValue}{unit}";
            changeInFollowersText.color = new Color(243f / 255f, 152f / 255f, 0f / 255f, 1f); // オレンジ色に設


        }

        float weight = Mathf.Clamp01(changeRatio); // 変化したフォロワーの割合を0〜1にクランプする
        textAnimator.SetLayerWeight(1, weight); // レイヤー1のウェイトを1に設定
        textAnimator.SetTrigger("Change"); // アニメーションのトリガーを実行
        StartCoroutine(UpdateHPBars());
    }

    private IEnumerator UpdateHPBars()
    {
        if (orangeHPBar != null && greenHPBar != null)
        {
            float elapsedTime = 0f;
            float currentOrangeFill = orangeHPBar.fillAmount;
            float targetOrangeFill = (float)followers / maxFollowers;

            float currentGreenFill = greenHPBar.fillAmount;
            float adjustedFollowers = followers * (1 - maxDecreaseRate);
            float targetGreenFill = adjustedFollowers / maxFollowers;

            while (elapsedTime < updateDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / updateDuration;

                orangeHPBar.fillAmount = Mathf.Lerp(currentOrangeFill, targetOrangeFill, t);
                greenHPBar.fillAmount = Mathf.Lerp(currentGreenFill, targetGreenFill, t);

                yield return null;
            }

            // 最終的に目標値を設定
            orangeHPBar.fillAmount = targetOrangeFill;
            greenHPBar.fillAmount = targetGreenFill;
        }
    }

    // 数字をK, M, B, Tなどの表記にフォーマットする関数
    private (int, string) FormatNumber(ulong number)
    {
        if (number >= 1000000000000) // 兆 (T)
        {
            float value = number / 1000000000000f;
            return ((int)value, "T");
        }
        else if (number >= 1000000000) // 十億 (B)
        {
            float value = number / 1000000000f;
            return ((int)value, "B");
        }
        else if (number >= 1000000) // 百万 (M)
        {
            float value = number / 1000000f;
            return ((int)value, "M");
        }
        else if (number >= 1000) // 千 (K)
        {
            float value = number / 1000f;
            return ((int)value, "K");
        }
        else // 1000未満はそのまま表示
        {
            return ((int)number, "");
        }
    }

    float CalculateScaling(float currentFollowers, float maxFollowers)
    {
        float maxOpacity = 100.0f; // 最高透明度
        float minOpacity = 0.0f; // 最低透明度
        float minRatio = 0.3f; // 最小比率
        float maxRatio = 1.0f; // 最大比率

        float ratio = currentFollowers / maxFollowers;

        // ratioがmaxRatioを超える場合は最低透明度を返す
        if (ratio >= maxRatio)
        {
            return minOpacity;
        }

        // minRatioからmaxRatioの間で線形に透明度を計算し、範囲を設定する
        return Mathf.Clamp(maxOpacity * (1.0f - (ratio - minRatio) / (maxRatio - minRatio)), minOpacity, maxOpacity);
    }

    void SetImageTransparency(float transparency)
    {
        if (image != null)
        {
            // Imageのカラーを取得して、透明度を変更します
            Color color = image.color;
            color.a = transparency / 100.0f; // スケーリング値を透明度に変換します
            image.color = color;
        }
    }
}
