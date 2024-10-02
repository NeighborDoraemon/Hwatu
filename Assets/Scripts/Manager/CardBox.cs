using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBox : MonoBehaviour
{
    public Transform[] spawnPoints; // 카드 소환 위치

    public int number_Of_Cards_To_Spawn = 2; // 카드 소환 갯수

    public void Request_Spawn_Cards() // 카드 소환 요청 함수
    {
        if (Card_Spawner.instance == null)
        {
            Debug.LogError("Card Box에서 Card_Spawner 인스턴스 실종");
            return;
        }

        int spawnCount = Mathf.Min(number_Of_Cards_To_Spawn, spawnPoints.Length); // 둘 중 더 작은 수로 스폰카운트 반환

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnPoints[i].position;
            Card_Spawner.instance.Spawn_Cards(spawnPos); // 카드 스포너의 카드 소환 함수 호출
        }

        Destroy(gameObject);
    }
}
