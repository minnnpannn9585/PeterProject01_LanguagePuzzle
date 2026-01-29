using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("拼图配置")]
    public int pieceID; 
    public bool isRequiredPiece = false; 
    public Transform targetTransform; 
    [Tooltip("吸附距离（建议先设为200测试）")]
    public float snapDistance = 200f; 
    public PuzzleManager puzzleManager; 

    private RectTransform rectTransform;
    private Canvas canvas; // 主画布
    private bool isSnapped = false; // 是否已吸附到目标位置
    private Vector2 dragOffset; // 鼠标与碎片的偏移量

    // 新增：记录拖拽前的原始状态（父物体+位置）
    private Transform originalParent; // 碎片原始父物体
    private Vector3 originalLocalPosition; // 碎片在原始父物体下的本地位置
    private Quaternion originalLocalRotation; // 原始旋转（防止旋转偏移）
    private Vector3 originalLocalScale; // 原始缩放（防止缩放偏移）

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindAnyObjectByType<Canvas>(); // 全局找Canvas（更稳定）
        if (canvas == null)
        {
            Debug.LogError("场景中未找到Canvas！", this);
            return;
        }

        // 记录拖拽前的原始状态（关键）
        originalParent = transform.parent;
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        originalLocalScale = transform.localScale;
    }

    // 开始拖拽：临时转为Canvas子物体
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        
        if (isSnapped) isSnapped = false;

        // 1. 计算鼠标与碎片的偏移量（保证拖拽跟随精准）
        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out canvasLocalPos
        );
        dragOffset = rectTransform.anchoredPosition - canvasLocalPos;

        // 2. 临时将碎片设为Canvas的直接子物体（消除层级坐标干扰）
        // worldPositionStays=true：保持世界位置不变，仅改父物体
        transform.SetParent(canvas.transform, true);
        // 确保层级在最前（避免被遮挡）
        transform.SetAsLastSibling();
    }

    // 拖拽过程：精准跟随鼠标
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Vector2 canvasLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out canvasLocalPos
        );
        rectTransform.anchoredPosition = canvasLocalPos + dragOffset;
    }

    // 结束拖拽：判断是否吸附，决定是否放回原父物体
    public void OnEndDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // 关键碎片：判断是否吸附
        if (isRequiredPiece && targetTransform != null)
        {
            RectTransform targetRect = targetTransform.GetComponent<RectTransform>();
            if (targetRect == null)
            {
                Debug.LogError("目标位置缺少RectTransform组件！", this);
                ResetToOriginalState(); // 放回原位置
                puzzleManager.CheckPuzzleCompletion();
                return;
            }

            // 计算碎片与目标的距离（统一Canvas参考系）
            float distance = Vector2.Distance(rectTransform.anchoredPosition, targetRect.anchoredPosition);
            Debug.Log($"碎片{gameObject.name}到目标距离：{distance} | 吸附阈值：{snapDistance}", this);

            if (distance <= snapDistance)
            {
                // 吸附成功：留在Canvas下，定位到目标位置
                rectTransform.anchoredPosition = targetRect.anchoredPosition;
                isSnapped = true;
                Debug.Log($"碎片{gameObject.name}吸附成功！", this);
            }
            else
            {
                // 吸附失败：放回原始父物体和位置
                ResetToOriginalState();
                isSnapped = false;
            }
        }
        else
        {
            // 干扰项：直接放回原始父物体和位置
            ResetToOriginalState();
            isSnapped = false;
        }

        puzzleManager.CheckPuzzleCompletion();
    }

    // 核心方法：将碎片恢复到拖拽前的原始状态（父物体+位置+旋转+缩放）
    private void ResetToOriginalState()
    {
        if (originalParent == null) return;

        // 1. 改回原始父物体（保持世界位置不变）
        transform.SetParent(originalParent, true);
        // 2. 强制恢复原始本地位置/旋转/缩放（避免偏差）
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        transform.localScale = originalLocalScale;
    }

    // 重置碎片（重新开始游戏用）
    public void ResetPiece()
    {
        ResetToOriginalState();
        isSnapped = false;
    }

    public bool IsSnapped() => isSnapped;
    public bool IsRequiredPiece() => isRequiredPiece;
}