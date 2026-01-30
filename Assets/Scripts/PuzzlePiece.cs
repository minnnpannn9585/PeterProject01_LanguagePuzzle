using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拼图配置")]
    public int pieceID;
    public bool isRequiredPiece = false;
    public Transform targetTransform;
    public float snapDistance = 300f;
    public PuzzleManager puzzleManager;

    private RectTransform rectTransform;
    private Canvas canvas; // 主画布（the canvas this piece belongs to）
    private bool isSnapped = false; // 是否已吸附到目标位置
    private Vector3 dragOffsetWorld; // world-space 鼠标与碎片的偏移量

    // 新增：记录拖拽前的原始状态（父物体+位置）
    private Transform originalParent; // 碎片原始父物体
    private Vector3 originalLocalPosition; // 碎片在原始父物体下的本地位置
    private Quaternion originalLocalRotation; // 原始旋转（防止旋转偏移）
    private Vector3 originalLocalScale; // 原始缩放（防止缩放偏移）

    // 新增：记录实际吸附到的目标（用于判断是否放到“正确”slot）
    private Transform snappedTarget;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Prefer the canvas this piece currently lives under to handle multiple canvases properly
        canvas = GetComponentInParent<Canvas>();

        // 记录拖拽前的原始状态（关键）
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;

        snappedTarget = null;
    }

    // 开始拖拽：临时转为Canvas子物体
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        if (isSnapped) isSnapped = false;
        snappedTarget = null;

        // choose the correct camera for this pointer/canvas
        Camera eventCamera = eventData.pressEventCamera ?? canvas.worldCamera;

        // 计算指针在 Canvas 上的 world 点，并记录世界空间偏移（防止跳动）
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3 pointerWorldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventCamera, out pointerWorldPos);

        // 保持世界位置的前提下，把碎片设为 Canvas 的子物体
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();

        // 记录世界空间的偏移量（碎片位置 - 指针位置）
        dragOffsetWorld = rectTransform.position - pointerWorldPos;
    }

    // 拖拽过程：精准跟随鼠标（使用 world 空间，避免坐标系不一致）
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Camera eventCamera = eventData.pressEventCamera ?? canvas.worldCamera;
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3 pointerWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventCamera, out pointerWorldPos))
        {
            rectTransform.position = pointerWorldPos + dragOffsetWorld;
        }
    }

    // 结束拖拽：判断是否吸附，决定是否放回原父物体
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Determine effective target RectTransform (explicit targetTransform preferred,
        // otherwise try to find the closest RectTransform in scene as fallback)
        RectTransform targetRect = GetEffectiveTargetRect();

        if (targetRect != null)
        {
            float distance = Vector2.Distance(rectTransform.position, targetRect.position);
            Debug.Log($"PuzzlePiece '{name}': checking snap to '{targetRect.name}', distance={distance}, threshold={snapDistance}");

            if (distance <= snapDistance)
            {
                // Snap: reparent to target and align locally
                transform.SetParent(targetRect.transform, false);
                // align to the target (reset local transform so it fits exactly)
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localRotation = Quaternion.identity;
                rectTransform.localScale = Vector3.one;

                isSnapped = true;
                snappedTarget = targetRect.transform;
                Debug.Log($"PuzzlePiece '{name}' snapped to '{targetRect.name}'.");
            }
            else
            {
                // Not close enough: return
                ResetToOriginalState();
                isSnapped = false;
                snappedTarget = null;
                Debug.Log($"PuzzlePiece '{name}' did not snap (too far).");
            }
        }
        else
        {
            // No target at all: return
            ResetToOriginalState();
            isSnapped = false;
            snappedTarget = null;
            Debug.Log($"PuzzlePiece '{name}': no target found, returned to original.");
        }

        puzzleManager.CheckPuzzleCompletion();
    }

    // Try to get the target RectTransform to snap to.
    // Priority:
    // 1) explicit targetTransform assigned in inspector
    // 2) nearest RectTransform in scene within snapDistance (fallback)
    private RectTransform GetEffectiveTargetRect()
    {
        if (targetTransform != null)
        {
            return targetTransform.GetComponent<RectTransform>();
        }

        // Optional: try GameObjects tagged "PuzzleTarget" first (useful if you tag targets)
        try
        {
            var tagged = GameObject.FindGameObjectsWithTag("PuzzleTarget");
            if (tagged != null && tagged.Length > 0)
            {
                RectTransform best = null;
                float bestDist = float.MaxValue;
                foreach (var go in tagged)
                {
                    var rt = go.GetComponent<RectTransform>();
                    if (rt == null) continue;
                    float d = Vector2.Distance(rectTransform.position, rt.position);
                    if (d < bestDist)
                    {
                        bestDist = d;
                        best = rt;
                    }
                }
                if (best != null) return best;
            }
        }
        catch { /* tag may not exist; ignore */ }

        // Fallback: find the nearest RectTransform in the scene
        RectTransform closest = null;
        float minDist = float.MaxValue;
        foreach (var rt in FindObjectsOfType<RectTransform>())
        {
            if (rt == rectTransform) continue;
            // optional heuristics: skip generic UI containers to reduce false positives
            if (rt.GetComponent<Canvas>() != null) continue; // skip Canvas root
            float d = Vector2.Distance(rectTransform.position, rt.position);
            if (d < minDist)
            {
                minDist = d;
                closest = rt;
            }
        }

        // Only accept if within snapDistance
        if (closest != null && minDist <= snapDistance) return closest;
        return null;
    }

    // 核心方法：将碎片恢复到拖拽前的原始状态（父物体+位置+旋转+缩放）
    private void ResetToOriginalState()
    {
        if (originalParent == null) return;

        // 改回原始父物体（保持世界位置不变）
        transform.SetParent(originalParent, true);

        // 恢复原始本地位置/旋转/缩放（避免偏差）
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        transform.localScale = originalLocalScale;

        snappedTarget = null;
    }

    // 重置碎片（重新开始游戏用）
    public void ResetPiece()
    {
        ResetToOriginalState();
        isSnapped = false;
        snappedTarget = null;
    }

    public bool IsSnapped() => isSnapped;
    public bool IsRequiredPiece() => isRequiredPiece;

    // 新增：是否吸附在“正确”目标上（用于判定胜利）
    public bool IsSnappedCorrectly()
    {
        return isSnapped && snappedTarget != null && targetTransform != null && snappedTarget == targetTransform;
    }
}