using UnityEngine;
using UnityEngine.UI;

// 拼图管理器：仅检测关键碎片是否全部就位
public class PuzzleManager : MonoBehaviour
{
    [Header("管理器配置")]
    public PuzzlePiece[] allPuzzlePieces; // 所有拼图碎片（10个：2个关键+8个干扰）
    public Text completionText; // 完成提示文本（可选）

    void Start()
    {
        // 初始化提示文本
        if (completionText != null)
        {
            completionText.text = "拼图中...";
            completionText.gameObject.SetActive(true);
        }
    }

    // 检测拼图是否全部完成（仅检查关键碎片）
    public void CheckPuzzleCompletion()
    {
        bool isAllComplete = true;

        // 遍历所有碎片，只检查关键碎片的吸附状态
        foreach (PuzzlePiece piece in allPuzzlePieces)
        {
            if (piece.IsRequiredPiece() && !piece.IsSnapped())
            {
                // 只要有一个关键碎片未就位，就判定未完成
                isAllComplete = false;
                break;
            }
        }

        // 所有关键碎片就位则提示成功
        if (isAllComplete)
        {
            if (completionText != null)
            {
                completionText.text = "拼图成功！";
            }
            Debug.Log("恭喜！拼图完成！");
        }
    }

    // 重新开始游戏（可选）
    public void RestartPuzzle()
    {
        foreach (PuzzlePiece piece in allPuzzlePieces)
        {
            piece.ResetPiece();
        }
        if (completionText != null)
        {
            completionText.text = "拼图中...";
        }
    }
}