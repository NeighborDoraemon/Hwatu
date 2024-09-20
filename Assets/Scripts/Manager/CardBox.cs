using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBox : MonoBehaviour
{
    public Transform[] spawnPoints; // ���� ����Ʈ 2��
    public Card_Spawner card_Spawner; // ī�� ������
    public int numberOfCardsToSpawn = 2; // ī�� ��ȯ ����

    private bool isPlayerNearby = false; // �÷��̾ ���� ��ó�� �ִ��� üũ

    void Update()
    {
        // �÷��̾ ���� ��ó�� �ְ�, �׼� Ű (��: E)�� ������ ��
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            SpawnCards();
        }
    }

    // ī�� ���� �Լ�
    public void SpawnCards()
    {
        // ī�� ��ȯ
        for (int i = 0; i < numberOfCardsToSpawn; i++)
        {
            if (i < spawnPoints.Length)
            {
                Vector2 spawnPosition = spawnPoints[i].position; // ���� ����Ʈ ��ġ Ȯ��

                int randomCardIndex = Random.Range(0, card_Spawner.card_Values.Length); // ��ũ���ͺ� ī�� ������Ʈ���� ���� ����
                card_Spawner.Spawn_Card(spawnPosition, randomCardIndex); // ī�� ��ȯ
            }
        }

        Debug.Log("ī�� ��ȯ �Ϸ�");
    }

    // �÷��̾ ���ڿ� ������ ���� ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = true; // ���� ��ó�� �ִ� ���·� ����
        }
    }

    // �÷��̾ ���ڿ��� �־����� ��
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false; // ���ڿ��� ���
        }
    }
}
