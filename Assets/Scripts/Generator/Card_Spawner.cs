using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawner : MonoBehaviour
{
    public GameObject card_Prefab;
    public Card_Value[] card_Values;

    public void Spawn_Card(Vector2 spawnPosition, int cardIndex)
    {
        GameObject spawnedCard = Instantiate(card_Prefab, spawnPosition, Quaternion.identity); // �������� �ʵ忡 ��ȯ

        // ��ũ���ͺ� ������Ʈ���� ī�� ���� �����ͼ� ī�忡 ����
        SpriteRenderer spriteRenderer = spawnedCard.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card_Values[cardIndex].GetRandomSprite(); // ���� ��������Ʈ

        spawnedCard.GetComponent<Card>().cardValue = card_Values[cardIndex]; // ī�� ������Ʈ�� ī�� ������ ������ �� �ֵ��� ��ũ���ͺ� ������Ʈ �����͸� �ѱ�
    }
}
