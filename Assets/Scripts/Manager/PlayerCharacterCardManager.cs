using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterCardManager : PlayerCharacterStatManager
{
    [Header("Card_Manager")]
    public CardUIManager card_UIManager; // ȭ�� UI �Ŵ��� ����
    public Card_Value card_Value; // ī�� ��ũ���ͺ� ������Ʈ
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[2]; // ȭ�� ������Ʈ ���� ���� �迭
    protected int cardCount = 0; // ȭ�� ���� ī��Ʈ ����

    protected bool isCombDone = false; // ȭ�� ������ �̷�������� üũ�ϴ� ����

    public void AddCard(GameObject card) // ȭ�� ���� �Լ�
    {
        Remove_Card_Comb_Effect(); // ���� ī�� ȿ�� ����

        if (cardCount == card_Inventory.Length)
        {
            card_Inventory[0] = card_Inventory[1];

            card_Inventory[1] = card;

            Debug.Log("ī�� ��ü");
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("ī�� �߰�" + card.name);
        }

        UpdateCardUI();
        Card_Combination();
    }

    void UpdateCardUI() // ȭ�� UI ��������Ʈ ������Ʈ �Լ�
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
                    Debug.LogError("ī�忡 ��������Ʈ ������ ����" + card_Inventory[i].name);
                }
            }
            else
            {
                cardSprites[i] = null;
            }
        }

        card_UIManager.UpdateCardUI(cardSprites);
    }

    public void Card_Combination() // ȭ�� ������ ���� �ɷ�ġ ���� �Լ�
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
                Debug.Log("�̵��ӵ� ����");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_1M"))
            {
                jumpPower += 5;
                Debug.Log("������ ����");
                isCombDone = true;
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_2M"))
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
    }

    void Remove_Card_Comb_Effect() // ���� ���� ȿ�� ���� �Լ�
    {
        if (isCombDone)
        {
            string card_1_Name = card_Inventory[0].name;
            string card_2_Name = card_Inventory[1].name;

            if ((card_1_Name == "Card_1M" && card_2_Name == "Card_2M") ||
                (card_1_Name == "Card_2M" && card_2_Name == "Card_1M"))
            {
                movementSpeed -= 5;
                Debug.Log("�̵��ӵ� ����");
            }
            else if ((card_1_Name == "Card_1M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_1M"))
            {
                jumpPower -= 5;
                Debug.Log("������ ����");
            }
            else if ((card_1_Name == "Card_2M" && card_2_Name == "Card_3M") ||
                    (card_1_Name == "Card_3M" && card_2_Name == "Card_2M"))
            {
                movementSpeed -= 2;
                jumpPower -= 2;
                Debug.Log("��� �ɷ�ġ ����");
            }
            else
            {
                Debug.Log("�ش� ���� ����");
            }

            isCombDone = false; // ���� ����
        }

    }
}
