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
    private ItemData itemData;
    private GameObject itemCanvas;
    private ItemCanvasManager itemCanvasManager;
    public int slotIndex;

    private void Awake()
    {
        if (icon == null)
            icon = GetComponent<Image>();
        if (slotButton == null)
            slotButton = GetComponent<Button>();
        if (itemCanvas == null)
            itemCanvas = GameObject.FindGameObjectWithTag("ItemUI");
        if (itemCanvas != null)
        {
            if (itemUI == null)
                itemUI = itemCanvas.transform.GetChild(0).gameObject;
            if (itemCanvasManager == null)
                itemCanvasManager = itemCanvas.GetComponent<ItemCanvasManager>();
        }
        if (slotButton != null)
        {
            slotButton.onClick.RemoveAllListeners();
            slotButton.onClick.AddListener(OnSlotClicked);
        }
    }

    public void SetItem(ItemData item)
    {
        itemData = item;
        if (icon == null) return;

        if (item != null)
        {
            TMP_Text textComp = icon.transform.GetChild(0).GetComponent<TMP_Text>();
            if (textComp != null)
                textComp.text = item.itemName;
            //icon.sprite = item.itemIcon;
            icon.enabled = true;
            icon.color = Color.white;
            icon.gameObject.SetActive(true);
            //icon.SetNativeSize();
        }
        else
        {
            TMP_Text textComp = icon.transform.GetChild(0).GetComponent<TMP_Text>();
            if (textComp != null)
                textComp.text = null;
            //icon.sprite = null;
            itemData = null;
        }

        TMP_Text debugTextComp = icon != null ? icon.transform.GetChild(0).GetComponent<TMP_Text>() : null;
        Sprite itemIconSprite = item != null ? item.itemIcon : null;
        Sprite slotIconSprite = icon != null ? icon.sprite : null;

        Debug.Log(
            $"[InventorySlot.SetItem] " +
            $"slotIndex={slotIndex}, " +
            $"itemIsNull={item == null}, " +
            $"cachedItemIsNull={itemData == null}, " +
            $"itemName={(item != null ? item.itemName : "null")}, " +
            $"itemId={(item != null ? item.itemID.ToString() : "null")}, " +
            $"itemIconIsNull={(itemIconSprite == null)}, " +
            $"itemIconName={(itemIconSprite != null ? itemIconSprite.name : "null")}, " +
            $"cachedItemName={(itemData != null ? itemData.itemName : "null")}, " +
            $"cachedItemId={(itemData != null ? itemData.itemID.ToString() : "null")}, " +
            $"iconIsNull={icon == null}, " +
            $"iconSpriteIsNull={(slotIconSprite == null)}, " +
            $"iconSpriteName={(slotIconSprite != null ? slotIconSprite.name : "null")}, " +
            $"iconEnabled={(icon != null ? icon.enabled.ToString() : "null")}, " +
            $"iconActive={(icon != null ? icon.gameObject.activeSelf.ToString() : "null")}, " +
            $"iconText={(debugTextComp != null ? debugTextComp.text : "null")}, " +
            $"slotObjectPath={GetTransformPath(transform)}"
        );
    }

    private void OnSlotClicked()
    {
        if (itemUI != null)
            itemUI.SetActive(true);
        if (itemCanvasManager == null)
        {
            if (itemCanvas == null)
                itemCanvas = GameObject.FindGameObjectWithTag("ItemUI");
            if (itemCanvas != null)
                itemCanvasManager = itemCanvas.GetComponent<ItemCanvasManager>();
        }
        if (itemCanvasManager == null) return;

        ItemData currentItem = null;
        //if (InventoryManager.Instance != null &&
        //    InventoryManager.Instance.inventorySlots != null &&
        //    slotIndex >= 0 && slotIndex < InventoryManager.Instance.inventorySlots.Count)
        {
            currentItem = InventoryManager.Instance.inventorySlots[slotIndex];
        }

        Sprite dataIconSprite = currentItem != null ? currentItem.itemIcon : null;
        Debug.Log(
            $"[InventorySlot.OnSlotClicked] " +
            $"slotIndex={slotIndex}, " +
            $"managerIsNull={InventoryManager.Instance == null}, " +
            $"currentItemIsNull={currentItem == null}, " +
            $"currentItemName={(currentItem != null ? currentItem.itemName : "null")}, " +
            $"currentItemId={(currentItem != null ? currentItem.itemID.ToString() : "null")}, " +
            $"currentItemIconIsNull={(dataIconSprite == null)}, " +
            $"currentItemIconName={(dataIconSprite != null ? dataIconSprite.name : "null")}"
        );

        itemCanvasManager.UpdateItemInfo(currentItem);
    }

    public void TurnOnItemUI()
    {
        OnSlotClicked();
    }

    private string GetTransformPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
