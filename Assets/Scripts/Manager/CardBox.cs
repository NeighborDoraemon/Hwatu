using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBox : MonoBehaviour
{
    public Transform[] spawnPoints; // ī�� ��ȯ ��ġ

    public int number_Of_Cards_To_Spawn = 2; // ī�� ��ȯ ����

    public void Request_Spawn_Cards() // ī�� ��ȯ ��û �Լ�
    {
        if (Card_Spawner.instance == null)
        {
            Debug.LogError("Card Box���� Card_Spawner �ν��Ͻ� ����");
            return;
        }

        int spawnCount = Mathf.Min(number_Of_Cards_To_Spawn, spawnPoints.Length); // �� �� �� ���� ���� ����ī��Ʈ ��ȯ

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnPoints[i].position;
            Card_Spawner.instance.Spawn_Cards(spawnPos); // ī�� �������� ī�� ��ȯ �Լ� ȣ��
        }

        Destroy(gameObject);
    }
}
