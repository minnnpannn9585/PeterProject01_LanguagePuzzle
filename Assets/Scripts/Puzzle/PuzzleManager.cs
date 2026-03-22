using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    [Header("UI/生成配置")]
    public PuzzlePiece piecePrefab;              // 拼图碎片预制体（必须带 PuzzlePiece 组件）
    public RectTransform piecesRoot;             // 生成碎片的父节点（拼图面板中的容器）
    public bool buildOnStart = true;             // 是否在 Start 时自动生成
    public bool clearChildrenBeforeBuild = true; // 生成前是否清空旧的子物体

    [Header("管理器配置")]
    public Text completionText;                  // 完成提示文本（可选）

    [Header("运行时数据（自动填充）")]
    public PuzzlePiece[] allPuzzlePieces;        // 不再手动拖拽，改为运行时生成后填充

    private void Start()
    {
        if (completionText != null)
        {
            completionText.text = "拼图中...";
            completionText.gameObject.SetActive(true);
        }

        if (buildOnStart)
        {
            BuildPiecesFromInventory();
        }
    }

    /// <summary>
    /// 根据玩家背包（InventoryManager.Instance.inventorySlots）自动创建拼图碎片
    /// \- 碎片本体不放图片
    /// \- 名字写到碎片的子物体 TMP\_Text 上显示
    /// </summary>
    public void BuildPiecesFromInventory()
    {
        if (piecePrefab == null || piecesRoot == null)
        {
            Debug.LogWarning("[PuzzleManager] piecePrefab 或 piecesRoot 未设置，无法生成拼图碎片。");
            return;
        }

        if (InventoryManager.Instance == null || InventoryManager.Instance.inventorySlots == null)
        {
            Debug.LogWarning("[PuzzleManager] InventoryManager 或 inventorySlots 为空，无法生成拼图碎片。");
            allPuzzlePieces = new PuzzlePiece[0];
            return;
        }

        if (clearChildrenBeforeBuild)
        {
            for (int i = piecesRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(piecesRoot.GetChild(i).gameObject);
            }
        }

        var created = new List<PuzzlePiece>();

        foreach (var item in InventoryManager.Instance.inventorySlots)
        {
            if (item == null) continue;

            PuzzlePiece piece = Instantiate(piecePrefab, piecesRoot);
            piece.puzzleManager = this;

            // 碎片 id \= 物品 id（用于和 PuzzleTarget.targetID 匹配）
            piece.pieceID = item.itemID;

            // 名字显示：写到子物体上的 TMP_Text（不依赖 Image）
            TMP_Text nameText = piece.GetComponentInChildren<TMP_Text>(true);
            if (nameText != null)
            {
                nameText.text = item.itemName;
            }
            else
            {
                Debug.LogWarning($"[PuzzleManager] 未在 PuzzlePiece 预制体子物体中找到 TMP_Text，无法显示名字。pieceID={piece.pieceID}");
            }

            created.Add(piece);
        }

        allPuzzlePieces = created.ToArray();

        Debug.Log($"[PuzzleManager] BuildPiecesFromInventory: createdPieces={allPuzzlePieces.Length}");
    }

    public void CheckPuzzleCompletion()
    {
        if (allPuzzlePieces == null || allPuzzlePieces.Length == 0)
            return;

        bool isAllComplete = true;

        foreach (PuzzlePiece piece in allPuzzlePieces)
        {
            if (piece == null) continue;

            if (piece.IsRequiredPiece() && !piece.IsSnappedCorrectly())
            {
                isAllComplete = false;
                break;
            }
        }

        if (isAllComplete && completionText != null)
        {
            completionText.text = "拼图成功！";
        }
    }

    public void RestartPuzzle()
    {
        if (allPuzzlePieces != null)
        {
            foreach (PuzzlePiece piece in allPuzzlePieces)
            {
                if (piece != null) piece.ResetPiece();
            }
        }

        if (completionText != null)
        {
            completionText.text = "拼图中...";
        }
    }
}
