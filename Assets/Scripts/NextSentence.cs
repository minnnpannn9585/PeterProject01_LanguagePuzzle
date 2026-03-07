using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 添加这个命名空间以使用Button

public class NextSentence : MonoBehaviour
{
    public GameObject[] sentences;
    public int index = 0;
    public GameObject cutsceneCam;
    public Button nextButton; // 添加一个公共的Button变量
    
    public void loadNextSentence()
    {
        if (index >= sentences.Length - 1)
        {
            transform.parent.gameObject.SetActive(false);
            GameManager.Instance.isGameStarted = true;
        }
        else
        {
            if(index == 2)
            {
                sentences[index].SetActive(false);
                cutsceneCam.SetActive(true);
                StartCoroutine(Waitsixsec());
            }
            else
            {
                sentences[index].SetActive(false);
                sentences[index + 1].SetActive(true);
                index++;
            }
                
        }
    }

    IEnumerator Waitsixsec()
    {
        nextButton.interactable = false; // 禁用按钮
        yield return new WaitForSeconds(6);
        cutsceneCam.SetActive(false);
        sentences[index + 1].SetActive(true);
        index++;
        nextButton.interactable = true; // 启用按钮
    }
}