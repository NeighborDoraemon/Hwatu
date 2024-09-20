using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBox : MonoBehaviour
{
    public Transform[] spawnPoints; // 스폰 포인트 2개
    public Card_Spawner card_Spawner; // 카드 스포너
    public int numberOfCardsToSpawn = 2; // 카드 소환 개수

    private bool isPlayerNearby = false; // 플레이어가 상자 근처에 있는지 체크

    void Update()
    {
        // 플레이어가 상자 근처에 있고, 액션 키 (예: E)를 눌렀을 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            SpawnCards();
        }
    }

    // 카드 스폰 함수
    public void SpawnCards()
    {
        // 카드 소환
        for (int i = 0; i < numberOfCardsToSpawn; i++)
        {
            if (i < spawnPoints.Length)
            {
                Vector2 spawnPosition = spawnPoints[i].position; // 스폰 포인트 위치 확인

                int randomCardIndex = Random.Range(0, card_Spawner.card_Values.Length); // 스크립터블 카드 오브젝트에서 랜덤 선택
                card_Spawner.Spawn_Card(spawnPosition, randomCardIndex); // 카드 소환
            }
        }

        Debug.Log("카드 소환 완료");
    }

    // 플레이어가 상자에 가까이 왔을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // 상자 근처에 있는 상태로 변경
        }
    }

    // 플레이어가 상자에서 멀어졌을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // 상자에서 벗어남
        }
    }
}
