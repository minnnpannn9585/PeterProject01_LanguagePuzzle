using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartGameBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject uiClickSound;

    private Image buttonImage;
    private Color originalColor;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
        }
    }

    public void LoadGameScene()
    {
        Instantiate(uiClickSound, Vector3.zero, Quaternion.identity);
        Invoke("LoadGame", 0.5f);
        
    }
    
    private void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage == null) return;

        buttonImage.color = new Color(1f, 1f, 1f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage == null) return;

        buttonImage.color = originalColor;
    }
}
