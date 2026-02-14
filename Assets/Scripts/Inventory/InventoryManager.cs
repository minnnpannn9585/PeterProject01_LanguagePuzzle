using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance
    public static InventoryManager Instance;

    // Inventory configuration
    public int inventorySize = 20;
    public List<ItemData> inventorySlots;

    // Events for UI / listeners
    public event Action OnInventoryChanged;
    public event Action<int, ItemData> OnSlotChanged;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        inventorySlots = new List<ItemData>(inventorySize);

        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(null);
        }

        if (Instance == null)
        {
            Instance = this;   
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Adds an item to the first empty slot. Returns true when added.
    public bool AddItem(ItemData item)
    {
        if (item == null) return false;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = item;
                OnSlotChanged?.Invoke(i, item);
                OnInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
}