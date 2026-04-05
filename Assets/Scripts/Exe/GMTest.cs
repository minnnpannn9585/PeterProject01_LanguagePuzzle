using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GMTest : MonoBehaviour
{
    public static int playerScore = 0;
    public GameObject winText;

    void Update()
    {
        if (playerScore >= 3)
        {
            winText.SetActive(true);
        }
    }
}
