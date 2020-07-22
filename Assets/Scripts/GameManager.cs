using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public PlayerMove player;

    public void NextStage()
    {
        stageIndex++; // 스테이지 인덱스 넘겨주기

        totalPoint += stagePoint; // 최총 포인트에 더해주기
        stagePoint = 0; // 초기화
    }
    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
        }
        else
        {
            // Player Die Effect
            player.OnDie();

            // Result UI
            Debug.Log("죽었습니다!");

            // Retry Button UI
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Player Reposition
            if (health > 1)
            {
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(-3.5f, -0.5f, 1);
            }

            // Health Down
            HealthDown();
        }
        
    }
}
