using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Universal Render Pipelineを使用している場合

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

    [Header("ビネット設定")]
    public Volume volume; // Volumeを参照
    private Vignette vignette; // Vignetteエフェクト

    public TMP_Text changeInFollowersText;
    public TMP_Text changeTypeText; // UPやDOWNを表示するテキスト
    public Animator textAnimator; // アニメーターを参照するための変数

    void Start()
    {
        if (nixieTube == null)
        {
            UnityEngine.Debug.LogError("nixieTubeがありません！");
        }

        if (volume != null)
        {
            volume.profile.TryGet(out vignette); // Vignetteの設定を取得
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
        SetImageTransparency(CalculateScaling(followers, maxFollowers));
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
            followers = 0;
        }
        else
        {
            followers -= decreaseAmount;
        }
        Debug.LogWarning($"間違った行動が実行されました！フォロワー数: {followers} (減少数: {decreaseAmount})");
        UpdateUI(decreaseAmount, true);
    }

    // フォロワー数のUIを更新する
    private void UpdateUI(ulong changeAmount, bool isDecrease)
    {
        changeInFollowersText.text = changeAmount.ToString();
       

        if (textAnimator != null)
        {
            textAnimator.SetTrigger("Animate");
        }
    }


    // 画像の透過度を設定する
    private void SetImageTransparency(float value)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = value;
            image.color = color;
        }
    }

    // スケーリング値を計算する
    private float CalculateScaling(ulong currentFollowers, ulong maxFollowers)
    {
        if (maxFollowers == 0)
            return 0f;
        return Mathf.Clamp01((float)currentFollowers / maxFollowers);
    }

    internal (int intValue, string unit) FormatNumber(ulong value)
    {
        // 数値が百万以上の場合
        if (value >= 1000000)
        {
            // 百万単位でフォーマットし、単位 "M" を付ける
            return ((int)(value / 1000000), "M");
        }
        // 数値が千以上の場合
        else if (value >= 1000)
        {
            // 千単位でフォーマットし、単位 "K" を付ける
            return ((int)(value / 1000), "K");
        }
        // 数値が千未満の場合
        else
        {
            // 単位なしでそのまま返す
            return ((int)value, "");
        }
    }

    internal void EvaluateAction(bool v)
    {
        if (v)
        {
            CorrectAction();
        }
        else
        {
            IncorrectAction();
        }
    }
}
