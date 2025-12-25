using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("Slot setup")]
    public Transform slotsParent;        // parent containing slot GameObjects (GridLayoutGroup)
    public GameObject slotPrefab;        // optional: prefab to instantiate if no children exist

    private List<InventorySlot> uiSlots = new List<InventorySlot>();

    private void Awake()
    {
        // Ensure InventoryManager.Instance exists (in case of ordering)
        if (InventoryManager.Instance == null)
        {
            var mgr = FindObjectOfType<InventoryManager>();
            if (mgr != null) InventoryManager.Instance = mgr;
        }

        if (slotsParent == null)
        {
            Debug.LogError("InventoryUI: slotsParent is not assigned.");
            return;
        }

        uiSlots.Clear();

        if (slotsParent.childCount > 0)
        {
            // Use existing children as slots
            foreach (Transform child in slotsParent)
            {
                InventorySlot s = child.GetComponent<InventorySlot>();
                if (s == null)
                {
                    Debug.LogWarning($"InventoryUI: child '{child.name}' missing InventorySlot -> adding one.");
                    s = child.gameObject.AddComponent<InventorySlot>();
                }
                uiSlots.Add(s);
            }
        }
        else if (slotPrefab != null && InventoryManager.Instance != null)
        {
            // Instantiate required number of slots
            int size = InventoryManager.Instance.inventorySize;
            for (int i = 0; i < size; i++)
            {
                GameObject go = Instantiate(slotPrefab, slotsParent);
                InventorySlot s = go.GetComponent<InventorySlot>();
                if (s == null) s = go.AddComponent<InventorySlot>();
                uiSlots.Add(s);
            }
        }

    }

    private void OnEnable()
    {
        TrySubscribe();
        RefreshUI(); // initial sync
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
            InventoryManager.Instance.OnSlotChanged -= OnSlotChangedHandler;
        }
    }

    private void Start()
    {
        // Ensure UI matches the inventory at scene start
        RefreshUI();
    }

    private void TrySubscribe()
    {
        if (InventoryManager.Instance == null)
        {
            var mgr = FindObjectOfType<InventoryManager>();
            if (mgr != null) InventoryManager.Instance = mgr;
        }

        if (InventoryManager.Instance != null)
        {
            // Avoid duplicate subscriptions
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;

            InventoryManager.Instance.OnSlotChanged -= OnSlotChangedHandler;
            InventoryManager.Instance.OnSlotChanged += OnSlotChangedHandler;

        }
    }

    private void RefreshUI()
    {
        if (InventoryManager.Instance == null) return;

        var slots = InventoryManager.Instance.inventorySlots;
        int count = Mathf.Min(uiSlots.Count, slots.Count);

        // Update visible slots
        for (int i = 0; i < count; i++)
        {
            uiSlots[i].SetItem(slots[i]);
            
        }

        // If UI has more slots than inventory, clear the rest
        for (int i = count; i < uiSlots.Count; i++)
        {
            uiSlots[i].SetItem(null);
            
        }
    }

    private void OnSlotChangedHandler(int index, ItemData item)
    {
        uiSlots[index].SetItem(item);
    }
}
