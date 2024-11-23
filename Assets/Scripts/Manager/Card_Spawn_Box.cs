using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawn_Box : MonoBehaviour
{
    public Transform[] spawnPoints; // 카드 소환 위치

    public int number_Of_Objects_To_Spawn = 2; // 카드 소환 갯수

    public string itemName_To_Spawn;
    PlayerCharacter_Controller player;

    public void Request_Spawn_Cards() // 카드 소환 요청 함수
    {
        if (Object_Manager.instance == null)
        {
            Debug.LogError("Card Box에서 Card_Spawner 인스턴스 실종");
            return;
        }

        int spawnCount = Mathf.Min(number_Of_Objects_To_Spawn, spawnPoints.Length); // 둘 중 더 작은 수로 스폰카운트 반환

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnPoints[i].position;

            Object_Manager.instance.Spawn_Cards(spawnPos);            
        }

        Destroy(gameObject);
    }
}
