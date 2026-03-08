using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameBtn : MonoBehaviour
{
    public GameObject uiClickSound;
    public void LoadGameScene()
    {
        Instantiate(uiClickSound, Vector3.zero, Quaternion.identity);
        Invoke("LoadGame", 0.5f);
        
    }
    
    private void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
