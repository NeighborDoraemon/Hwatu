using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCardManager : PlayerCharacterStatManager
{
    [Header("Card_Manager")]
    public CardUIManager card_UIManager; // 화투 UI 매니저 변수
    public Card_Value card_Value; // 카드 스크립터블 오브젝트
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[2]; // 화투 오브젝트 저장 공간 배열
    protected int cardCount = 0; // 화투 갯수 카운트 변수

    protected bool isCombDone = false; // 화투 조합이 이루어졌는지 체크하는 변수

    public void AddCard(GameObject card) // 화투 저장 함수
    {
        Remove_Card_Comb_Effect(); // 기존 카드 효과 제거

        if (cardCount == card_Inventory.Length)
        {
            card_Inventory[0] = card_Inventory[1];

            card_Inventory[1] = card;

            Debug.Log("카드 교체");
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("카드 추가" + card.name);
        }

        UpdateCardUI();
        Card_Combination();
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

        card_UIManager.UpdateCardUI(cardSprites);
    }

    public void Card_Combination() // 화투 조합을 통한 능력치 변경 함수
    {
        isCombDone = false;

        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            string card_1_Name = card_Inventory[0].name;
            string card_2_Name = card_Inventory[1].name;

            if ((card_1_Name == "Card_1M" && card_2_Name == "Card_2M") ||
                (card_1_Name == "Card_2M" && card_2_Name == "Card_1M"))
            {
                movementSpeed += 5;
                Debug.Log("이동속도 증가");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_1M"))
            {
                jumpPower += 5;
                Debug.Log("점프력 증가");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_2M"))
            {
                movementSpeed += 2;
                jumpPower += 2;
                Debug.Log("모든 능력치 증가");
                isCombDone = true;
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
            string card_1_Name = card_Inventory[0].name;
            string card_2_Name = card_Inventory[1].name;

            if ((card_1_Name == "Card_1M" && card_2_Name == "Card_2M") ||
                (card_1_Name == "Card_2M" && card_2_Name == "Card_1M"))
            {
                movementSpeed -= 5;
                Debug.Log("이동속도 감소");
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_1M"))
            {
                jumpPower -= 5;
                Debug.Log("점프력 감소");
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_2M"))
            {
                movementSpeed -= 2;
                jumpPower -= 2;
                Debug.Log("모든 능력치 감소");
            }
            else
            {
                Debug.Log("해당 조합 없음");
            }

            isCombDone = false; // 조합 해제
        }

    }
}
