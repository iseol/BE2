﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Rigidbody2D rigid;
    public Animator anim;
    public SpriteRenderer spriteRenderer;
    public CapsuleCollider2D capsuleCollider;
    public int nextMove;
    public bool isDrawRay = true;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // Platform Check
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);

        if (isDrawRay == true)
        {
            Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider == null)
            {
                Turn();
            }
        }

    }
    // 재귀 함수
    void Think()
    {
        // Set Next Active
        nextMove = Random.Range(-1, 2);

        // Sprite Animation
        anim.SetInteger("WalkSpeed", nextMove);

        // Flip Sprite
        if (nextMove != 0)
        {
            Turn();
        }
        // Recursive
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }
    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }
    public void OnDamaged()
    {
        if (!gameObject.name.Contains("Spike"))
        {
            // Sprite Alpha
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);

            // Sprite Flip Y
            spriteRenderer.flipY = true;

            // Stop Drawray
            isDrawRay = false;

            // Collider Disable
            capsuleCollider.enabled = false;

            // Die Effect Jump
            rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

            // Destroy
            Invoke("DeActive", 5);
        }
    }
    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
