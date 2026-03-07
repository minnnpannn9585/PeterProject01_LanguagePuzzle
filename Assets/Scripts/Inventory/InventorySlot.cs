using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public GameObject itemUI;

    private Button slotButton;
    // 仅用于当前小图标显示的缓存；点击详情用 InventoryManager + slotIndex 查
    private ItemData itemData;

    // 仍然在 Awake 里通过 Tag 查找
    private GameObject itemCanvas;
    private ItemCanvasManager itemCanvasManager;

    // 由 InventoryUI 赋值，表示这是第几个槽位
    public int slotIndex;

    private void Awake()
    {
        // 获取自身组件
        if (icon == null)
            icon = GetComponent<Image>();

        if (slotButton == null)
            slotButton = GetComponent<Button>();

        // 通过 Tag 找到物品详情 UI 所在的 Canvas
        if (itemCanvas == null)
            itemCanvas = GameObject.FindGameObjectWithTag("ItemUI");

        if (itemCanvas != null)
        {
            // 按你原来的约定：第 0 个子物体是物品信息面板
            if (itemUI == null)
                itemUI = itemCanvas.transform.GetChild(0).gameObject;

            if (itemCanvasManager == null)
                itemCanvasManager = itemCanvas.GetComponent<ItemCanvasManager>();
        }

        // 为按钮绑定统一点击逻辑（只绑定一次）
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    // 刷新这个槽位的小图标显示
    public void SetItem(ItemData item)
    {
        itemData = item; // 仅作为当前显示缓存

        if (icon == null) return;

        if (item != null)
        {
            // 更新小图标文字
            TMP_Text textComp = icon.transform.GetChild(0).GetComponent<TMP_Text>();
            if (textComp != null)
                textComp.text = item.itemName;

            icon.enabled = true;
            icon.color = Color.white;
            icon.gameObject.SetActive(true);
            icon.SetNativeSize();
        }
        else
        {
            // 槽位为空：清文字 + 清缓存
            TMP_Text textComp = icon.transform.GetChild(0).GetComponent<TMP_Text>();
            if (textComp != null)
                textComp.text = null;

            itemData = null;
        }
    }

    // 槽位被点击：根据 slotIndex 从 InventoryManager 读取当前物品，并更新详情面板
    private void OnSlotClicked()
    {
        // 打开物品信息 UI 面板
        if (itemUI != null)
            itemUI.SetActive(true);

        // 确认有有效的 ItemCanvasManager（兜底一次，防止场景时序问题）
        if (itemCanvasManager == null)
        {
            if (itemCanvas == null)
                itemCanvas = GameObject.FindGameObjectWithTag("ItemUI");

            if (itemCanvas != null)
                itemCanvasManager = itemCanvas.GetComponent<ItemCanvasManager>();
        }

        if (itemCanvasManager == null) return;

        // 使用当前 slotIndex 从 InventoryManager 读取“此刻”的 ItemData
        ItemData currentItem = null;
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.inventorySlots != null &&
            slotIndex >= 0 && slotIndex < InventoryManager.Instance.inventorySlots.Count)
        {
            currentItem = InventoryManager.Instance.inventorySlots[slotIndex];
        }

        itemCanvasManager.UpdateItemInfo(currentItem);
    }

    // 兼容旧代码的公共方法
    public void TurnOnItemUI()
    {
        OnSlotClicked();
    }
}
