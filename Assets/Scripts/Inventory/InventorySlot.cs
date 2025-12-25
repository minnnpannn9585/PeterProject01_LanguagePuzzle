using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;

    private void Awake()
    {
        // If icon not manually assigned in inspector, try to find it:
        if (icon == null)
        {
            icon = GetComponent<Image>();
        }
    }

    public void SetItem(ItemData item)
    {

        if (item == null || item.itemIcon == null)
        {
            // Clear visual
            icon.sprite = null;
            icon.enabled = false;
            icon.gameObject.SetActive(false);
        }
        else
        {
            // Assign and ensure visible
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            icon.color = Color.white; // ensure not transparent
            icon.gameObject.SetActive(true);
            // Optional: make the Image match sprite native size
            icon.SetNativeSize();
        }
    }
}
