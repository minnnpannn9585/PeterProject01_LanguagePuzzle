using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI拼图碎片组件
/// </summary>
public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拼图配置")] public int pieceID; // 碎片编号
    public bool isRequired; // 是否为关键碎片（可移除）
    public float snapDistance = 300f; // 吸附距离阈值
    public PuzzleManager puzzleManager; // 拼图管理器

    private RectTransform rectTransform; // 本地RectTransform组件
    private Canvas canvas; // 所在画布
    private Vector3 dragOffsetWorld; // 拖拽时的偏移量
    private Transform originalParent; // 原始父对象
    private Vector3 originalLocalPosition; // 原始本地位置
    private Vector3 originalLocalScale; // 原始本地缩放
    private bool isSnapped = false; // 是否已吸附
    private bool isMatched = false; // 是否id匹配

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalScale = transform.localScale;
    }

    // 开始拖拽时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        Camera eventCamera = eventData.pressEventCamera ?? canvas.worldCamera;
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3 pointerWorldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventCamera,
            out pointerWorldPos);

        // 将碎片移到画布顶层，便于拖拽
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
        dragOffsetWorld = rectTransform.position - pointerWorldPos;
        isSnapped = false;
        isMatched = false;
    }

    // 拖拽过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        Camera eventCamera = eventData.pressEventCamera ?? canvas.worldCamera;
        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector3 pointerWorldPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRect, eventData.position, eventCamera,
                out pointerWorldPos))
        {
            // 更新碎片位置
            rectTransform.position = pointerWorldPos + dragOffsetWorld;
        }
    }

    // 拖拽结束时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // 查找最近的PuzzleTarget目标
        PuzzleTarget nearestTarget = null;
        float minDistance = float.MaxValue;
        Vector3 piecePos = rectTransform.position;

        foreach (PuzzleTarget target in FindObjectsOfType<PuzzleTarget>())
        {
            RectTransform targetRect = target.GetComponent<RectTransform>();
            float distance = Vector2.Distance(piecePos, targetRect.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        // 判断是否在吸附距离内
        if (nearestTarget != null && minDistance <= snapDistance)
        {
            // 吸附到目标
            RectTransform targetRect = nearestTarget.GetComponent<RectTransform>();
            transform.SetParent(targetRect.transform, false);
            rectTransform.localPosition = Vector3.zero;
            rectTransform.localScale = Vector3.one;
            isSnapped = true;
            isMatched = (pieceID == nearestTarget.targetID); // 判断id是否匹配
        }
        else
        {
            // 未吸附，恢复原状态
            ResetToOriginalState();
        }

        // 检查拼图完成状态
        puzzleManager?.CheckPuzzleCompletion();
    }

    // 恢复碎片到初始状态
    private void ResetToOriginalState()
    {
        transform.SetParent(originalParent, true);
        transform.localPosition = originalLocalPosition;
        transform.localScale = originalLocalScale;
        isSnapped = false;
        isMatched = false;
    }

    // 外部调用，重置碎片
    public void ResetPiece()
    {
        ResetToOriginalState();
    }

    // 判断是否为关键碎片（可移除）
    public bool IsRequiredPiece()
    {
        return isRequired;
    }

    // 判断碎片是否正确吸附到目标（吸附且id匹配）
    public bool IsSnappedCorrectly()
    {
        return isSnapped && isMatched;
    }
}
