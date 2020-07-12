using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Video;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        // Jump 
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }



        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.005f * rigid.velocity.normalized.x, rigid.velocity.y); 
        }

        // Direction Sprite
        if (Input.GetButtonDown("Horizontal"))
        {
            if (rigid.velocity.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        // Animation
        if (Mathf.Abs(rigid.velocity.normalized.x) < 0.3)
        {
            anim.SetBool("isWalk", false);
        }
        else
        {
            anim.SetBool("isWalk", true);
        }
    }



    void FixedUpdate()
    {
        // Key Control (Move)
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h * 999, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform
        if (Mathf.Abs(rigid.velocity.y) < 0.001f)
        {
            anim.SetBool("isJumping", false);
        }
        else if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("플레이어가 맞았습니다!");
        }
    }
    void OnDamaged()
    {
        // Change Layer (Immortal Active)
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
    }
}
