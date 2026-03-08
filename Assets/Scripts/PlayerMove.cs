using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;

    public bool isOpen = false;
    public GameObject bagUI;

    public GameObject uiClickSound;

    void Start(){

        rb = GetComponent<Rigidbody2D>();
        bagUI.SetActive(false);
    }
    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
        {
            return;
        }

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(inputX, inputY).normalized;
        if (inputX > 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = true;
        }

        if (inputX < 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            TurnOnOffBag();
        }
        
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    public void TurnOnOffBag()
    { 
        isOpen = !isOpen; 
        bagUI.SetActive(isOpen);
        Instantiate(uiClickSound, Vector3.zero, Quaternion.identity);
    }
}