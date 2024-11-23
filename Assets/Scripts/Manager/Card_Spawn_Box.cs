using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawn_Box : MonoBehaviour
{
    public Transform[] spawnPoints; // ī�� ��ȯ ��ġ

    public int number_Of_Objects_To_Spawn = 2; // ī�� ��ȯ ����

    public string itemName_To_Spawn;
    PlayerCharacter_Controller player;

    public void Request_Spawn_Cards() // ī�� ��ȯ ��û �Լ�
    {
        if (Object_Manager.instance == null)
        {
            Debug.LogError("Card Box���� Card_Spawner �ν��Ͻ� ����");
            return;
        }

        int spawnCount = Mathf.Min(number_Of_Objects_To_Spawn, spawnPoints.Length); // �� �� �� ���� ���� ����ī��Ʈ ��ȯ

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnPoints[i].position;

            Object_Manager.instance.Spawn_Cards(spawnPos);            
        }

        Destroy(gameObject);
    }
}
