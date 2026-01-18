using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueItem : MonoBehaviour
{
    public int index;
    public Transform bag;

    public void TurnOnClue()
    {
        bag.GetChild(index).gameObject.SetActive(true);
    }
}
