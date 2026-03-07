using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData[] itemData;
    

    public void AddItem()
    {
        foreach (ItemData itemData in itemData)
        {
            InventoryManager.Instance.AddItem(itemData);
            //print(333);
        }
        
    }

    public void ItemPicked()
    {
        GetComponent<PolygonCollider2D>().enabled = false;
    }
    
}