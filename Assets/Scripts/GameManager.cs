using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public GameObject[] Stages;
    public PlayerMove player;

    public void NextStage()
    {
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);

            stageIndex++; // 스테이지 인덱스 넘겨주기

            Stages[stageIndex].SetActive(true);

            PlayerReposition(); // 플레이어 위치 초기화
        }
        else // Game Clear
        {
            // Player Control Lock
            Time.timeScale = 0;
            // Result UI
            Debug.Log("게임 클리어!");
            // Restat Button UI
        }

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
                PlayerReposition();
            }

            // Health Down
            HealthDown();
        }
        
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-2.5f, -0.5f, 1);
        player.VelocityZero();
    }
}
