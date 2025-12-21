using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 单例实例
    public static InventoryManager Instance;

    // 背包格子数量（可自定义）
    public int inventorySize = 20;
    public List<ItemData> inventorySlots = new List<ItemData>();

    // 物品添加后触发UI刷新事件
    public event Action OnInventoryChanged;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 跨场景保留
        }
        else
        {
            Destroy(gameObject);
        }

        // 初始化背包空槽位
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(null); // 初始所有槽位为空
        }
    }

    
    public bool AddItem(ItemData item)
    {
        // 遍历找第一个空槽位
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == null)
            {
                inventorySlots[i] = item;
                Debug.Log("拾取物品：" + item.itemName);
                OnInventoryChanged?.Invoke(); // 触发UI刷新
                return true;
            }
        }

        // 背包已满
        Debug.LogWarning("背包已满，无法拾取：" + item.itemName);
        return false;
    }
}