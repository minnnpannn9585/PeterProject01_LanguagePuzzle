using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupNew : MonoBehaviour
{
    public GameObject puzzleCanvas;

    bool insideItemArea = false;
    bool insidePuzzleArea = false;
    
    public ClueItem clue;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (insidePuzzleArea)
            {
                puzzleCanvas.SetActive(true);
            }

            if (insideItemArea)
            {
                
                
            }
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            insideItemArea = true;
            clue = other.gameObject.GetComponent<ClueItem>();
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
