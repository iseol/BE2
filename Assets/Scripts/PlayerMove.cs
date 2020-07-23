using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Video;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    public Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public CapsuleCollider2D capsuleCollider;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }


    void Update()
    {
        // Jump 
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            anim.SetBool("isJumping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }



        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0.005f * rigid.velocity.normalized.x, rigid.velocity.y);
        }

        // Direction Sprite
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
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

        rigid.AddForce(Vector2.right * h * 2147436, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxSpeed * (-1)) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Landing Platform
        if (Mathf.Abs(rigid.velocity.y) < 0.0001f)
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) // Attack
            {
                OnAttack(collision.transform);
            }
            else // Damaged
            {
                OnDamaged(collision.transform.position);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            // 코인 가격에 따른 점수 적립
            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 300;
            }

            // Deactive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            // Next Stage
            gameManager.NextStage();
        }
    }
    void OnAttack(Transform enemy)
    {
        // Point
        gameManager.stagePoint += 200;
        // Reaction Force
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
    void OnDamaged(Vector2 targetPos)
    {
        // Health Down
        gameManager.HealthDown();

        // Change Layer (Immortal Active)
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction Force
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1; // 자신의 위치가 장애물의 위치보다 오른쪽에 있으면 1, 아니면 -1
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        // Animation
        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 0.5f);
    }
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    public void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprite Flip Y
        spriteRenderer.flipY = true;

        // Collider Disable
        capsuleCollider.enabled = false;

        // Die Effect Jump
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        // Destroy
        Invoke("HidePlayer", 5);
    }
    void HidePlayer()
    {
        gameObject.SetActive(false);
    }
    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

}
