using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawner : MonoBehaviour
{
    public GameObject card_Prefab;
    public Card_Value[] card_Values;

    public void Spawn_Card(Vector2 spawnPosition, int cardIndex)
    {
        GameObject spawnedCard = Instantiate(card_Prefab, spawnPosition, Quaternion.identity); // 프리팹을 필드에 소환

        // 스크립터블 오브젝트에서 카드 값을 가져와서 카드에 적용
        SpriteRenderer spriteRenderer = spawnedCard.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = card_Values[cardIndex].GetRandomSprite(); // 랜덤 스프라이트

        spawnedCard.GetComponent<Card>().cardValue = card_Values[cardIndex]; // 카드 오브젝트에 카드 정보를 저장할 수 있도록 스크립터블 오브젝트 데이터를 넘김
    }
}
