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

    private void Awake()
    {
        // If icon not manually assigned in inspector, try to find it:
        if (icon == null)
        {
            icon = GetComponent<Image>();
        }
        
        itemUI = GameObject.FindGameObjectWithTag("ItemUI").transform.GetChild(0).gameObject;
    }

    public void SetItem(ItemData item)
    {
        //print(icon.gameObject.name);
        if (item!=null)
        {
            icon.transform.GetChild(0).GetComponent<TMP_Text>().text = item.itemName;
            // Assign and ensure visible
            //icon.sprite = item.itemIcon;
            icon.enabled = true;
            icon.color = Color.white; // ensure not transparent
            icon.gameObject.SetActive(true);
            // Optional: make the Image match sprite native size
            icon.SetNativeSize();
        }
        else
        {
            icon.transform.GetChild(0).GetComponent<TMP_Text>().text = null;
        }
        

    }

    public void TurnOnItemUI()
    {
        itemUI.SetActive(true);
    }
}
