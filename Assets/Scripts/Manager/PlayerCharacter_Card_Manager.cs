using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Card_Manager : PlayerCharacter_Stat_Manager
{
    [Header("Card_Manager")]
    public Card_UI_Manager card_UI_Manager; // 화투 UI 매니저 변수
    [HideInInspector]
    public Card_Value card_Value; // 카드 스크립터블 오브젝트
    [HideInInspector]
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[2]; // 화투 오브젝트 저장 공간 배열
    protected int cardCount = 0; // 화투 갯수 카운트 변수

    protected bool isCombDone = false; // 화투 조합이 이루어졌는지 체크하는 변수

    // 능력치 변화 저장 변수
    int movement_Speed_Change = 0;
    int jump_Power_Change = 0;
    int attack_Damage_Change = 0;
    int player_Health_Change = 0;

    public void AddCard(GameObject card) // 화투 저장 함수
    {
        Remove_Card_Comb_Effect(); // 기존 카드 효과 제거

        if (cardCount == card_Inventory.Length)
        {
            if (card_Inventory[0] != null) // 기존 화투 삭제함으로써 메모리 관리
            {
                Card cardComponent = card_Inventory[0].GetComponent<Card>();
                if (cardComponent != null && cardComponent.selected_Sprite != null)
                {
                    Card_Spawner.instance.Remove_Used_Sprite(cardComponent.selected_Sprite); // 카드 수집 시 수집된 카드의 스프라이트 해쉬에 저장
                }

                Destroy(card_Inventory[0]);
            }

            card_Inventory[0] = card_Inventory[1];
            card_Inventory[1] = card;

            //Debug.Log("카드 교체");
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("카드 추가" + card.name);
        }

        UpdateCardUI();
        Card_Combination();

        // 카드 수집 후 필드에 남아있는 카드 삭제
        if (Card_Spawner.instance != null)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Card_Spawner.instance.Destroy_All_Cards(card);
            Card_Spawner.instance.Remove_From_Spawned_Cards(card);

            card.SetActive(false);
        }
        else { Debug.LogWarning("Card Manager에서 Card Spawner 인스턴스 실종"); }
    }

    void UpdateCardUI() // 화투 UI 스프라이트 업데이트 함수
    {
        Sprite[] cardSprites = new Sprite[card_Inventory.Length];

        for (int i = 0; i < card_Inventory.Length; i++)
        {
            if (card_Inventory[i] != null)
            {
                SpriteRenderer card_Sprite_Renderer = card_Inventory[i].GetComponent<SpriteRenderer>();

                if (card_Sprite_Renderer != null)
                {
                    cardSprites[i] = card_Sprite_Renderer.sprite;
                }
                else
                {
                    Debug.LogError("카드에 스프라이트 렌더러 없음" + card_Inventory[i].name);
                }
            }
            else
            {
                cardSprites[i] = null;
            }
        }

        card_UI_Manager.UpdateCardUI(cardSprites);
    }

    public void Card_Combination() // 화투 조합을 통한 능력치 변경 함수
    {
        isCombDone = false;

        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            Card_Value card_1 = card_Inventory[0].GetComponent<Card>().cardValue;
            Card_Value card_2 = card_Inventory[1].GetComponent<Card>().cardValue;

            if (card_1.Month == 11 && card_2.Month == 11) // 추후 광 검사 코드를 제작할 예정입니다.
            {
                Debug.Log("광 조합");
            }
            if (card_1.Month == card_2.Month) // 서로 같은 달인지 확인
            {
                switch (card_1.Month)
                {
                    case 1:
                        movement_Speed_Change = 3;
                        movementSpeed += movement_Speed_Change;
                        Debug.Log("1월 조합으로 인한 이동속도 증가");
                        isCombDone = true;
                        break;
                    case 2:
                        jump_Power_Change = 3;
                        jumpPower += jump_Power_Change;
                        Debug.Log("2월 조합으로 인한 점프력 증가");
                        isCombDone = true;
                        break;
                    case 3:
                        attack_Damage_Change = 25;
                        attackDamage += attack_Damage_Change;
                        Debug.Log("3월 조합으로 인한 공격력 증가");
                        isCombDone = true;
                        break;
                    case 4:
                        Debug.Log("4월 조합");
                        isCombDone = true;
                        break;
                    case 5:
                        Debug.Log("5월 조합");
                        isCombDone = true;
                        break;
                    case 6:
                        Debug.Log("6월 조합");
                        isCombDone = true;
                        break;
                    case 7:
                        Debug.Log("7월 조합");
                        isCombDone = true;
                        break;
                    case 8:
                        Debug.Log("8월 조합");
                        isCombDone = true;
                        break;
                    case 9:
                        Debug.Log("9월 조합");
                        isCombDone = true;
                        break;
                    case 10:
                        Debug.Log("10월 조합");
                        isCombDone = true;
                        break;
                    default:
                        Debug.Log("해당 월이 없음");
                        break;
                }
            }
            else if (card_1.Month != card_2.Month) // 같은 달이 아니면 이쪽으로 넘어옴
            {
                //Debug.Log("끗");
            }
            else
            {
                Debug.Log("해당 조합 없음");
            }
        }
    }

    void Remove_Card_Comb_Effect() // 기존 조합 효과 제거 함수
    {
        if (isCombDone)
        {
            // 기존 조합으로 증가했던 능력치 제거
            movementSpeed -= movement_Speed_Change; 
            jumpPower -= jump_Power_Change;
            attackDamage -= attack_Damage_Change;
            player_Health_Change -= player_Health_Change;

            // 변화된 능력치 초기화
            movement_Speed_Change = 0;
            jump_Power_Change = 0;
            attack_Damage_Change = 0;
            player_Health_Change = 0;

            Debug.Log("기존 조합 효과 제거");
            isCombDone = false; // 조합 해제
        }

    }
}
