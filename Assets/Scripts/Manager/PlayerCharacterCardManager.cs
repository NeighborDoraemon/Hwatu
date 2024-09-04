using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCardManager : PlayerCharacterStatManager
{
    public CardUIManager cardUIManager; // 화투 UI 매니저 변수

    public GameObject[] cardInventory = new GameObject[2]; // 화투 오브젝트 저장 공간 배열
    public int cardCount = 0; // 화투 갯수 카운트 변수

    public bool isCombDone = false; // 화투 조합이 이루어졌는지 체크하는 변수

    public void AddCard(GameObject card) // 화투 저장 함수
    {
        if (cardCount < cardInventory.Length)
        {
            cardInventory[cardCount] = card;
            cardCount++;
            Debug.Log("카드 추가" + card.name);
        }
        else
        {
            Debug.Log("카드 꽉 참");
        }
    }

    void UpdateCardUI() // 화투 UI 스프라이트 업데이트 함수
    {
        Sprite[] cardSprites = new Sprite[cardInventory.Length];
        for (int i = 0; i < cardInventory.Length; i++)
        {
            if (cardInventory[i] != null)
            {
                cardSprites[i] = cardInventory[i].GetComponent<SpriteRenderer>().sprite;
            }
        }

        cardUIManager.UpdateCardUI(cardSprites);
    }

    public void CardCombination() // 화투 조합을 통한 능력치 변경 함수
    {
        if (isCombDone)
        {
            return;
        }

        if (cardInventory[0] != null && cardInventory[1] != null)
        {
            string card_1_Name = cardInventory[0].name;
            string card_2_Name = cardInventory[1].name;

            if ((card_1_Name == "Card_1M" && card_2_Name == "Card_2M") ||
                    (card_2_Name == "Card_2M" && card_1_Name == "Card_1M"))
            {
                movementSpeed += 5;
                Debug.Log("이동속도 증가");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_2_Name == "Card_3M" && card_1_Name == "Card_1M"))
            {
                jumpPower += 5;
                Debug.Log("점프력 증가");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_2_Name == "Card_3M" && card_1_Name == "Card_2M"))
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
        else
        {
            //Debug.Log("카드 부족");
        }
    }
}
