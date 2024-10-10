using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter_Card_Manager : PlayerCharacter_Stat_Manager
{
    [Header("Card_Manager")]
    public Card_UI_Manager card_UI_Manager; // ȭ�� UI �Ŵ��� ����
    [HideInInspector]
    public Card_Value card_Value; // ī�� ��ũ���ͺ� ������Ʈ
    [HideInInspector]
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[2]; // ȭ�� ������Ʈ ���� ���� �迭
    protected int cardCount = 0; // ȭ�� ���� ī��Ʈ ����

    protected bool isCombDone = false; // ȭ�� ������ �̷�������� üũ�ϴ� ����

    // �ɷ�ġ ��ȭ ���� ����
    int movement_Speed_Change = 0;
    int jump_Power_Change = 0;
    int attack_Damage_Change = 0;
    int player_Health_Change = 0;

    public void AddCard(GameObject card) // ȭ�� ���� �Լ�
    {
        Remove_Card_Comb_Effect(); // ���� ī�� ȿ�� ����

        if (cardCount == card_Inventory.Length)
        {
            if (card_Inventory[0] != null) // ���� ȭ�� ���������ν� �޸� ����
            {
                Card cardComponent = card_Inventory[0].GetComponent<Card>();
                if (cardComponent != null && cardComponent.selected_Sprite != null)
                {
                    Object_Manager.instance.Remove_Used_Sprite(cardComponent.selected_Sprite); // ī�� ���� �� ������ ī���� ��������Ʈ �ؽ��� ����
                }

                Destroy(card_Inventory[0]);
            }

            card_Inventory[0] = card_Inventory[1];
            card_Inventory[1] = card;

            //Debug.Log("ī�� ��ü");
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("ī�� �߰�" + card.name);
        }

        UpdateCardUI();
        Card_Combination();

        // ī�� ���� �� �ʵ忡 �����ִ� ī�� ����
        if (Object_Manager.instance != null)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Object_Manager.instance.Destroy_All_Cards(card);
            Object_Manager.instance.Remove_From_Spawned_Cards(card);

            card.SetActive(false);
        }
        else { Debug.LogWarning("Card Manager���� Card Spawner �ν��Ͻ� ����"); }
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

        card_UI_Manager.UpdateCardUI(cardSprites);
    }

    public void Card_Combination() // ȭ�� ������ ���� �ɷ�ġ ���� �Լ�
    {
        isCombDone = false;

        PlayerCharacter_Controller player = GetComponent<PlayerCharacter_Controller>();

        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            Card_Value card_1 = card_Inventory[0].GetComponent<Card>().cardValue;
            Card_Value card_2 = card_Inventory[1].GetComponent<Card>().cardValue;

            if (card_1.Month > 10 && card_2.Month > 10 && card_1.Month != card_2.Month)
            {
                if ((card_1.Month == 11 && card_2.Month == 12)
                    || (card_1.Month == 12 && card_2.Month == 11))
                {
                    Debug.Log("1 3 ����");
                }
                else if ((card_1.Month == 11 && card_2.Month == 13)
                    || (card_1.Month == 13 && card_2.Month == 11))
                {
                    Debug.Log("1 8 ����");
                }
                else if ((card_1.Month == 12 && card_2.Month == 13)
                    || (card_1.Month == 13 && card_2.Month == 12))
                {
                    Change_Attack_Strategy(new Three_Eight_Attack_Strategy(player));
                    Debug.Log("3 8 ����");
                }
                isCombDone = true;
            }
            else if (card_1.Month == card_2.Month) // ���� ���� ������ Ȯ��
            {
                switch (card_1.Month)
                {
                    case 1:
                        Debug.Log("1��");
                        break;
                    case 2:
                        Debug.Log("2��");
                        break;
                    case 3:
                        Debug.Log("3��");
                        break;
                    case 4:
                        Debug.Log("4��");
                        break;
                    case 5:
                        Debug.Log("5��");
                        break;
                    case 6:
                        Debug.Log("6��");
                        break;
                    case 7:
                        Debug.Log("7��");
                        break;
                    case 8:
                        Debug.Log("8��");
                        break;
                    case 9:
                        Debug.Log("9��");
                        break;
                    case 10:
                        Debug.Log("10��");
                        break;
                    default:
                        Debug.Log("�ش� ���� ����");
                        break;
                }
                isCombDone = true;
            }
            else if (card_1.Month != card_2.Month) // ���� ���� �ƴϸ� �������� �Ѿ��
            {
                if (card_1.Month + card_2.Month >= 1 && card_1.Month + card_2.Month <= 8)
                {
                    if ((card_1.Month == 1 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 1))
                    {
                        Debug.Log("����");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 2)
                        || (card_1.Month == 2 && card_2.Month == 1))
                    {
                        Debug.Log("�˸�");
                    }
                    else
                    {
                        Change_Attack_Strategy(new Base_Two_Attack_Strategy(player));
                        Debug.Log((card_1.Month + card_2.Month) + "��");
                    }
                }
                else if (card_1.Month + card_2.Month == 9)
                {
                    Change_Attack_Strategy(new Base_Three_Attack_Strategy(player));
                    Debug.Log("����");
                }
                else
                {
                    if ((card_1.Month == 10 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 10))
                    {
                        Debug.Log("���");
                    }
                    else if ((card_1.Month == 4 && card_2.Month == 6)
                        || (card_1.Month == 6 && card_2.Month == 4))
                    {
                        Debug.Log("����");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 10)
                        || (card_1.Month == 10 && card_2.Month == 1))
                    {
                        Debug.Log("���");
                    }
                    else if ((card_1.Month == 1 && card_2.Month == 9)
                        || (card_1.Month == 9 && card_2.Month == 1))
                    {
                        Debug.Log("����");
                    }
                    else
                    {
                        Change_Attack_Strategy(new Base_Attack_Strategy(player));
                        Debug.Log("����");
                    }
                }
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
            // ���� �������� �����ߴ� �ɷ�ġ ����
            movementSpeed -= movement_Speed_Change;
            jumpPower -= jump_Power_Change;
            attackDamage -= attack_Damage_Change;
            player_Health_Change -= player_Health_Change;

            // ��ȭ�� �ɷ�ġ �ʱ�ȭ
            movement_Speed_Change = 0;
            jump_Power_Change = 0;
            attack_Damage_Change = 0;
            player_Health_Change = 0;

            Debug.Log("���� ���� ȿ�� ����");
            isCombDone = false; // ���� ����
        }

    }
}
