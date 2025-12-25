using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            ItemPickup itemPickup = other.GetComponent<ItemPickup>();

            InventoryManager.Instance.AddItem(itemPickup.itemData);

            Destroy(other.gameObject);
                
        }
    }
}