
using UnityEngine;
using UnityEngine.UI;

public class ItemCanvasManager : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;

    public void UpdateItemInfo(ItemData itemData)
    {
        if (itemData == null)
        {
            if (itemIcon != null) itemIcon.sprite = null;
            if (itemNameText != null) itemNameText.text = null;
            Debug.Log("[ItemCanvasManager.UpdateItemInfo] itemData is null, clear UI.");
            return;
        }

        Sprite dataIcon = itemData.itemIcon;
        if (itemIcon != null)
        {
            itemIcon.sprite = dataIcon;
            itemIcon.SetNativeSize();
        }
        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }
        Sprite finalIcon = itemIcon != null ? itemIcon.sprite : null;

        Debug.Log(
            $"[ItemCanvasManager.UpdateItemInfo] " +
            $"itemName={itemData.itemName}, " +
            $"itemId={itemData.itemID}, " +
            $"dataIconIsNull={(dataIcon == null)}, " +
            $"dataIconName={(dataIcon != null ? dataIcon.name : "null")}, " +
            $"finalIconIsNull={(finalIcon == null)}, " +
            $"finalIconName={(finalIcon != null ? finalIcon.name : "null")}"
        );
    }
}
