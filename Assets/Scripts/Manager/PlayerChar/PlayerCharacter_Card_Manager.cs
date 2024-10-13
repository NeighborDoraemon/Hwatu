using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [HideInInspector] public bool is_Start_Spawn = true; // 시작 지급인지 확인하는 변수 (윤혁)

    public void AddCard(GameObject card) // 화투 저장 함수
    {
        isCombDone = false;

        if (cardCount == card_Inventory.Length)
        {
            if (card_Inventory[0] != null) // 기존 화투 삭제함으로써 메모리 관리
            {
                Card cardComponent = card_Inventory[0].GetComponent<Card>();
                if (cardComponent != null && cardComponent.selected_Sprite != null)
                {
                    Object_Manager.instance.Remove_Used_Sprite(cardComponent.selected_Sprite); // 카드 수집 시 수집된 카드의 스프라이트 해쉬에 저장
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

            if (cardCount == card_Inventory.Length) // 첫 획득 시 카드가 완전히 사라지지 않도록 1회성 방지 (윤혁, 임시)
            {
                is_Start_Spawn = false;
            }
        }

        UpdateCardUI();
        Card_Combination();

        // 카드 수집 후 필드에 남아있는 카드 삭제
        if (Object_Manager.instance != null && !is_Start_Spawn)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Object_Manager.instance.Destroy_All_Cards(card);
            Object_Manager.instance.Remove_From_Spawned_Cards(card);

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

        PlayerCharacter_Controller player = GetComponent<PlayerCharacter_Controller>();

        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            Card_Value card_1 = card_Inventory[0].GetComponent<Card>().cardValue;
            Card_Value card_2 = card_Inventory[1].GetComponent<Card>().cardValue;

            if (card_1.Month > 10 && card_2.Month > 10 && card_1.Month != card_2.Month)
            {
                if ((card_1.Month == 11 && card_2.Month == 13)
                    || (card_1.Month == 13 && card_2.Month == 11))
                {
                    Debug.Log("1 3 광땡");
                }
                else if ((card_1.Month == 11 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 11))
                {
                    Debug.Log("1 8 광땡");
                }
                else if ((card_1.Month == 13 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 13))
                {
                    Set_Weapon(6);
                    Debug.Log("3 8 광땡");
                }
                isCombDone = true;
            }
            else if ((card_1.Month % 10) == (card_2.Month % 10)) // 서로 같은 달인지 확인
            {
                switch (card_1.Month % 10)
                {
                    case 1:
                        Set_Weapon(4);
                        Debug.Log("1땡");
                        break;
                    case 2:
                        Debug.Log("2땡");
                        break;
                    case 3:
                        Debug.Log("3땡");
                        break;
                    case 4:
                        Debug.Log("4땡");
                        break;
                    case 5:
                        Debug.Log("5땡");
                        break;
                    case 6:
                        Debug.Log("6땡");
                        break;
                    case 7:
                        Debug.Log("7땡");
                        break;
                    case 8:
                        Debug.Log("8땡");
                        break;
                    case 9:
                        Debug.Log("9땡");
                        break;
                    case 10:
                        Debug.Log("10땡");
                        break;
                    default:
                        Debug.Log("해당 월이 없음");
                        break;
                }
                isCombDone = true;
            }
            else if (card_1.Month != card_2.Month) // 같은 달이 아니면 이쪽으로 넘어옴
            {
                if ((card_1.Month + card_2.Month) % 10 >= 1 && (card_1.Month + card_2.Month) % 10 <= 8)
                {
                    if ((card_1.Month == 1 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 1))
                    {
                        Debug.Log("독사");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 2)
                        || (card_1.Month == 2 && card_2.Month == 1))
                    {
                        Debug.Log("알리");
                    }
                    else
                    {
                        Set_Weapon(2);
                        Debug.Log((card_1.Month + card_2.Month) % 10 + "끗");
                    }
                }
                else if (card_1.Month + card_2.Month == 9)
                {
                    Set_Weapon(3);
                    Debug.Log("갑오");
                }
                else
                {
                    if ((card_1.Month == 10 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 10))
                    {
                        Debug.Log("장사");
                    }
                    else if ((card_1.Month == 4 && card_2.Month == 6)
                        || (card_1.Month == 6 && card_2.Month == 4))
                    {
                        Set_Weapon(5);
                        Debug.Log("세륙");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 10)
                        || (card_1.Month == 10 && card_2.Month == 1))
                    {
                        Debug.Log("장삥");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 9)
                        || (card_1.Month == 9 && card_2.Month == 1))
                    {
                        Debug.Log("구삥");
                    }
                    else
                    {
                        Set_Weapon(1);
                        Debug.Log("망통");
                    }
                }
                isCombDone = true;
            }
            else
            {
                Debug.Log("해당 조합 없음");
            }
        }
    }
}
