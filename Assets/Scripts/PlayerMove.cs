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

    void Start(){

        rb = GetComponent<Rigidbody2D>();
        bagUI.SetActive(false);
    }
    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(inputX, inputY).normalized;

        if (Input.GetKeyDown(KeyCode.E))
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
        
    }
}