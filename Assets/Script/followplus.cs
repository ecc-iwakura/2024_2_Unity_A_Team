using UnityEngine;

public class followplus : MonoBehaviour
{
    public int followers = 0; // フォロワー数を管理する変数
    private bool firstCorrectAction = true; // 初めて正しい行動が行われたかどうかを管理するフラグ

    // 正しい行動をした時に呼び出される関数
    [ContextMenu("CorrectAction")]
    public void CorrectAction()
    {
        if (firstCorrectAction)
        {
            followers += 1; // 初めての正しい行動の場合、フォロワーが固定で1増加
            Debug.Log($"初めての正しい行動が実行されました！フォロワー数: {followers}");
            firstCorrectAction = false; // 初めての正しい行動が行われたフラグをfalseに設定
        }
        else
        {
            // フォロワーが5%～10%で増加し、さらに1～10の固定値を追加
            float increaseRate = Random.Range(0.05f, 0.10f); // 5%～10%の増加率
            int increaseAmountFromRate = Mathf.CeilToInt(followers * increaseRate); // 増加する割合部分のフォロワー数
            int fixedIncrease = Random.Range(1, 11); // 1～10の固定値
            int totalIncrease = increaseAmountFromRate + fixedIncrease; // 合計増加フォロワー数

            followers += totalIncrease;
            Debug.Log($"正しい行動が実行されました！フォロワー数: {followers} (増加率: {increaseRate:P0}, 増加数: {increaseAmountFromRate}, 固定増加: {fixedIncrease}, 合計増加: {totalIncrease})");
        }

        // 正しい行動に対する追加の処理をここに記述
    }


    // 間違った行動をした時に呼び出される関数
    [ContextMenu("IncorrectAction")]
    public void IncorrectAction()
    {
        // フォロワー数の40%～50%で減少
        float decreaseRate = Random.Range(0.40f, 0.50f); // 40%～50%の減少率
        int decreaseAmount = Mathf.CeilToInt(followers * decreaseRate); // 減少するフォロワー数
        followers -= decreaseAmount;
        if (followers < 0) followers = 0; // フォロワー数が負にならないようにする
        Debug.Log($"間違った行動が実行されました...フォロワー数: {followers} (減少率: {decreaseRate:P0}, 減少数: {decreaseAmount})");

        // 間違った行動に対する追加の処理をここに記述
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
}