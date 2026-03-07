using UnityEngine;
using UnityEngine.UI;

public class ItemCanvasManager : MonoBehaviour
{
    public Image itemIcon;
    public Text itemNameText;

    // 更新 ItemCanvas 的内容
    public void UpdateItemInfo(ItemData itemData)
    {
        if (itemData == null)
        {
            itemIcon.sprite = null;
            itemNameText.text = null;
            return;
        }
        
        if (itemIcon != null)
        {
            itemIcon.sprite = itemData.itemIcon;
            itemIcon.SetNativeSize();
        }

        if (itemNameText != null)
        {
            itemNameText.text = itemData.itemName;
        }
    }
}