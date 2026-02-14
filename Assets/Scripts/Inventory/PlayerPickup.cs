using System;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public GameObject puzzleCanvas;

    public bool insideItemArea = false;
    public bool insidePuzzleArea = false;
    public ItemPickup itemPickup;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            print(111);
            if (insidePuzzleArea)
            {
                puzzleCanvas.SetActive(true);
                
            }

            if (insideItemArea)
            {
                print(222);
                itemPickup.AddItem();
                itemPickup.ItemPicked();
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            insideItemArea = true;
            itemPickup = other.GetComponent<ItemPickup>();
        }

        if (other.CompareTag("Puzzle"))
        {
            insidePuzzleArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Puzzle"))
        {
            insidePuzzleArea = false;
        }

        if (other.CompareTag("Item"))
        {
            insideItemArea = false;
        }
    }
}