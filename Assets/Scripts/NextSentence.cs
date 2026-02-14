using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSentence : MonoBehaviour
{
    public GameObject[] sentences;
    public int index = 0;
    
    public void loadNextSentence()
    {
        if (index >= sentences.Length - 1)
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            sentences[index].SetActive(false);
            sentences[index + 1].SetActive(true);
            index++;
        }
    }
}
