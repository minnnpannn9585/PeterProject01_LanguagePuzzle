using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检测碰撞对象是否为物品
        if (other.CompareTag("Item"))
        {
            ItemPickup itemPickup = other.GetComponent<ItemPickup>();
            if (itemPickup != null && itemPickup.itemData != null)
            {
                
                bool isAdded = InventoryManager.Instance.AddItem(itemPickup.itemData);
                if (isAdded)
                {
                    // 拾取成功后销毁物品对象
                    Destroy(other.gameObject);
                }
            }
        }
    }
}