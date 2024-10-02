using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawner : MonoBehaviour
{
    public static Card_Spawner instance { get; private set; } // 싱글톤

    public GameObject card_Prefab;
    public Card_Value[] card_Values;

    [SerializeField]
    private HashSet<Sprite> used_Card_Sprite = new HashSet<Sprite>(); // 사용된 카드의 스프라이트 저장할 해쉬셋
    public List<GameObject> current_Spawned_Card = new List<GameObject>(); // 현재 상자에 소환된 카드를 저장하는 리스트

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject Making_Card(Vector2 spawnPosition, Card_Value selected_Card, Sprite selected_Sprite)
    {
        GameObject spawnedCard = Instantiate(card_Prefab, spawnPosition, Quaternion.identity); // 프리팹을 필드에 소환

        // 스크립터블 오브젝트에서 카드 값을 가져와서 카드에 적용
        SpriteRenderer spriteRenderer = spawnedCard.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = selected_Sprite; // 랜덤 스프라이트

        Card cardComponent = spawnedCard.GetComponent<Card>();
        cardComponent.cardValue = selected_Card;
        cardComponent.selected_Sprite = selected_Sprite;

        spawnedCard.tag = "Card"; // 소환 오브젝트 태그 카드로 설정

        current_Spawned_Card.Add(spawnedCard); // 현재 소환된 카드 리스트에 추가
        used_Card_Sprite.Add(selected_Sprite);

        return spawnedCard;
    }

    public void Spawn_Cards(Vector2 spawnPosition)
    {
        Card_Value selected_Card = null;
        Sprite selected_Sprite = null;
        bool found_Valid_Card = false;
        // 무한루프 방지 변수
        int attempts = 0;
        int maxAttempts = 100;

        while (!found_Valid_Card && attempts < maxAttempts)
        {
            int random_Card_Index = Random.Range(0, card_Values.Length); // 랜덤 스크립터블 오브젝트 지정
            selected_Card = card_Values[random_Card_Index]; // 선택된 카드에 저장
            selected_Sprite = selected_Card.GetRandomSprite(); // 저장된 카드에 랜덤 스프라이트 추출 후 선택된 스프라이트에 저장

            // 현재 저장된 스프라이트가 아닐 경우 탈출
            if (!used_Card_Sprite.Contains(selected_Sprite))
            {
                found_Valid_Card = true;
            }

            attempts++;
        }

        if (!found_Valid_Card)
        {
            Debug.LogWarning("더 이상 카드 탐색 불가능");
            return;
        }

        used_Card_Sprite.Add(selected_Sprite); // 사용된 스프라이트 해쉬에 저장

        Making_Card(spawnPosition, selected_Card, selected_Sprite); // 카드 제작 완료
    }

    public void Destroy_All_Cards(GameObject card_To_Keep = null) // 현재 소환된 카드 삭제 함수
    {
        for (int i = current_Spawned_Card.Count -1; i >= 0; i--)
        {
            GameObject card = current_Spawned_Card[i];
            if (card != null && card != card_To_Keep)
            {
                Sprite cardSprite = card.GetComponent<SpriteRenderer>().sprite;
                Remove_Used_Sprite(cardSprite); // 삭제되는 카드들의 스프라이트를 사용된 스프라이트 해쉬에서 삭제하여 중복 소환 방지
                Destroy(card);
                current_Spawned_Card.RemoveAt(i); // 소환된 카드 리스트 삭제 코드
            }
        }
    }

    public void Remove_From_Spawned_Cards(GameObject card)
    {
        if (current_Spawned_Card.Contains(card))
        {
            current_Spawned_Card.Remove(card);
        }
    }

    public void Add_To_Used_Sprites(Sprite sprite) // 사용된 스프라이트 해쉬에 저장하는 함수
    {
        if (used_Card_Sprite.Add(sprite))
        {
            Debug.Log("사용된 스프라이트 추가" + sprite.name);
        }
    }

    public void Remove_Used_Sprite(Sprite sprite) // 사용된 스프라이트 해쉬에서 삭제하는 함수
    {
        if (used_Card_Sprite.Contains(sprite))
        {
            used_Card_Sprite.Remove(sprite);
        }
    }

    // 사용된 카드 리스트 초기화 함수
    public void ResetUsedCards()
    {
        used_Card_Sprite.Clear();
    }

    public bool Is_Sprite_Used(Sprite sprite) // 스프라이트 검사 함수
    {
        return used_Card_Sprite.Contains(sprite);
    }
}
