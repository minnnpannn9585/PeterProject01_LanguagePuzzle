using System.Collections;
  using System.Collections.Generic;
  using UnityEngine;
  using TMPro;

  [RequireComponent(typeof(Rigidbody2D))]
  public class Player : MonoBehaviour
  {
      [SerializeField]
      private float moveSpeed = 5f;

      [SerializeField]
      private float jumpForce = 7f;

      [SerializeField]
      private Transform groundCheck;
      [SerializeField]
      private float groundCheckRadius = 0.1f;
      [SerializeField]
      private LayerMask groundLayer;

      public TMP_Text scoreText;

      public TMP_Text npcText;

      private Rigidbody2D rb;
      private bool jump2 = true;
      private float horizontalInput;
      private bool jumpPressed;

      void Awake()
      {
          rb = GetComponent<Rigidbody2D>();
          if (groundCheck == null)
              groundCheck = transform;
      }

      void Update()
      {
          horizontalInput = 0f;
          if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
          else if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

          if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() || Input.GetKeyDown(KeyCode.Space) && jump2)
          {
              if (IsGrounded())
              {
                  jump2 = true;
              }
              else
              {
                  jump2 = false;
              }
              jumpPressed = true;
          }
      }

      void FixedUpdate()
      {
          rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

          if (jumpPressed)
          {
              rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
              jumpPressed = false;
          }
      }

      private bool IsGrounded()
      {
          Vector2 origin = groundCheck != null ? (Vector2)groundCheck.position : rb.position;
          Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayer);
          return hit != null;
      }

      void OnDrawGizmosSelected()
      {
          if (groundCheck != null)
          {
              Gizmos.color = Color.yellow;
              Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
          }
      }

      void OnCollisionEnter2D(Collision2D collision)
      {
          var other = collision.gameObject;
          if (other.CompareTag("score"))
          {
              GMTest.playerScore++;
              Debug.Log("Score: " + GMTest.playerScore);
              scoreText.text = "Score: " + GMTest.playerScore;
              Destroy(other);
          }
          else if (other.CompareTag("kill"))
          {
              Destroy(gameObject);
          }
      }

      void OnTriggerEnter2D(Collider2D collider)
      {
          var other = collider.gameObject;
          if (other.CompareTag("score"))
          {
              GMTest.playerScore++;
              Debug.Log("Score: " + GMTest.playerScore);
              scoreText.text = "Score: " + GMTest.playerScore;
              Destroy(other);
          }
          else if (other.CompareTag("kill"))
          {
              Destroy(gameObject);
          }
          else if (other.CompareTag("npc"))
         {
            npcText.text = "NPC: HelloWorld";
        }
      }
  }