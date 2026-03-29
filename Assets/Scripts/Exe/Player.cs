using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f; // 水平速度

    [SerializeField]
    private float jumpForce = 7f; // 跳跃脉冲力

    [SerializeField]
    private Transform groundCheck; // 地面检测点（建议放在角色脚下）
    [SerializeField]
    private float groundCheckRadius = 0.1f; // 检测半径
    [SerializeField]
    private LayerMask groundLayer; // 地面层

    private Rigidbody2D rb;
    private bool jump2 = true;
    private float horizontalInput;
    private bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (groundCheck == null)
            groundCheck = transform; // 回退到角色中心（建议在 Inspector 指定专用点）
    }

    void Update()
    {
        // 读取左右输入（A / D）
        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        // 跳跃按键（只在按下时记录，实际在 FixedUpdate 中应用）
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
        // 水平移动：直接设置速度以获得可控运动
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // 跳跃（物理脉冲），仅当按下且在地面时触发
        if (jumpPressed)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpPressed = false;
        }
    }

    // 2D 地面检测：OverlapCircle 更可靠于底部检测
    private bool IsGrounded()
    {
        Vector2 origin = groundCheck != null ? (Vector2)groundCheck.position : rb.position;
        Collider2D hit = Physics2D.OverlapCircle(origin, groundCheckRadius, groundLayer);
        return hit != null;
    }

    // 在 Scene 视图中可视化 groundCheck
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // 碰撞处理（实体碰撞）
    void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("score"))
        {
            Destroy(other);
        }
        else if (other.CompareTag("kill"))
        {
            Destroy(gameObject);
        }
    }

    // 触发处理（isTrigger = true 的情况）
    void OnTriggerEnter2D(Collider2D collider)
    {
        var other = collider.gameObject;
        if (other.CompareTag("score"))
        {
            Destroy(other);
        }
        else if (other.CompareTag("kill"))
        {
            Destroy(gameObject);
        }
    }
}
