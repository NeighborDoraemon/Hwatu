using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCardManager : PlayerCharacterStatManager
{
    public CardUIManager cardUIManager; // ȭ�� UI �Ŵ��� ����

    public GameObject[] cardInventory = new GameObject[2]; // ȭ�� ������Ʈ ���� ���� �迭
    public int cardCount = 0; // ȭ�� ���� ī��Ʈ ����

    public bool isCombDone = false; // ȭ�� ������ �̷�������� üũ�ϴ� ����

    public void AddCard(GameObject card) // ȭ�� ���� �Լ�
    {
        if (cardCount < cardInventory.Length)
        {
            cardInventory[cardCount] = card;
            cardCount++;
            Debug.Log("ī�� �߰�" + card.name);
        }
        else
        {
            Debug.Log("ī�� �� ��");
        }
    }

    void UpdateCardUI() // ȭ�� UI ��������Ʈ ������Ʈ �Լ�
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

    public void CardCombination() // ȭ�� ������ ���� �ɷ�ġ ���� �Լ�
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
                Debug.Log("�̵��ӵ� ����");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_2_Name == "Card_3M" && card_1_Name == "Card_1M"))
            {
                jumpPower += 5;
                Debug.Log("������ ����");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_2_Name == "Card_3M" && card_1_Name == "Card_2M"))
            {
                movementSpeed += 2;
                jumpPower += 2;
                Debug.Log("��� �ɷ�ġ ����");
                isCombDone = true;
            }
            else
            {
                Debug.Log("�ش� ���� ����");
            }
        }
        else
        {
            //Debug.Log("ī�� ����");
        }
    }
}
