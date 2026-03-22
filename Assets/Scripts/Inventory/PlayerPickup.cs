using System;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public GameObject puzzleCanvas;

    public bool insideItemArea = false;
    public bool insidePuzzleArea = false;
    public ItemPickup itemPickup;
    public GameObject hintText;
    public GameObject hintvfx;
    public GameObject unlockSound;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            print(111);
            if (insidePuzzleArea)
            {
                puzzleCanvas.SetActive(true);
                Instantiate(unlockSound, Vector3.zero, Quaternion.identity);
            }

            if (insideItemArea)
            {
                print(222);
                itemPickup.AddItem();
                itemPickup.ItemPicked();
                Instantiate(unlockSound, Vector3.zero, Quaternion.identity);
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        hintText.SetActive(true);
        hintvfx.SetActive(true);
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
        hintText.SetActive(false);
        hintvfx.SetActive(false);
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