using UnityEngine;

public class TopDownSorting : MonoBehaviour
{
    [Tooltip("排序的基准点（物体的底部更符合视觉逻辑）")]
    public Transform sortPivot;
    [Tooltip("放大系数，避免小数导致排序误差")]
    public int sortMultiplier = 100;
    private SpriteRenderer _spriteRenderer;
    public bool movable;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (sortPivot == null)
        {
            sortPivot = transform;
        }
        UpdateSortingOrder();
    }

    private void Update()
    {
        if (movable)
        {
            UpdateSortingOrder();
        }
    }

    /// <summary>
    /// 动态更新渲染顺序
    /// 核心公式：Order = -（基准点Y值 * 放大系数） + 偏移值
    /// 负数保证Y越小，Order越大，渲染在上方
    /// </summary>
    private void UpdateSortingOrder()
    {
        float pivotY = sortPivot.position.y;
        int newOrder = -Mathf.RoundToInt(pivotY * sortMultiplier);
        _spriteRenderer.sortingOrder = newOrder;
    }
}