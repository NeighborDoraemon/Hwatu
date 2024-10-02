using System.Collections;
using System.Collections.Generic;
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
                    Card_Spawner.instance.Remove_Used_Sprite(cardComponent.selected_Sprite); // ī�� ���� �� ������ ī���� ��������Ʈ �ؽ��� ����
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
        if (Card_Spawner.instance != null)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Card_Spawner.instance.Destroy_All_Cards(card);
            Card_Spawner.instance.Remove_From_Spawned_Cards(card);

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

        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            Card_Value card_1 = card_Inventory[0].GetComponent<Card>().cardValue;
            Card_Value card_2 = card_Inventory[1].GetComponent<Card>().cardValue;

            if (card_1.Month == 11 && card_2.Month == 11) // ���� �� �˻� �ڵ带 ������ �����Դϴ�.
            {
                Debug.Log("�� ����");
            }
            if (card_1.Month == card_2.Month) // ���� ���� ������ Ȯ��
            {
                switch (card_1.Month)
                {
                    case 1:
                        movement_Speed_Change = 3;
                        movementSpeed += movement_Speed_Change;
                        Debug.Log("1�� �������� ���� �̵��ӵ� ����");
                        isCombDone = true;
                        break;
                    case 2:
                        jump_Power_Change = 3;
                        jumpPower += jump_Power_Change;
                        Debug.Log("2�� �������� ���� ������ ����");
                        isCombDone = true;
                        break;
                    case 3:
                        attack_Damage_Change = 25;
                        attackDamage += attack_Damage_Change;
                        Debug.Log("3�� �������� ���� ���ݷ� ����");
                        isCombDone = true;
                        break;
                    case 4:
                        Debug.Log("4�� ����");
                        isCombDone = true;
                        break;
                    case 5:
                        Debug.Log("5�� ����");
                        isCombDone = true;
                        break;
                    case 6:
                        Debug.Log("6�� ����");
                        isCombDone = true;
                        break;
                    case 7:
                        Debug.Log("7�� ����");
                        isCombDone = true;
                        break;
                    case 8:
                        Debug.Log("8�� ����");
                        isCombDone = true;
                        break;
                    case 9:
                        Debug.Log("9�� ����");
                        isCombDone = true;
                        break;
                    case 10:
                        Debug.Log("10�� ����");
                        isCombDone = true;
                        break;
                    default:
                        Debug.Log("�ش� ���� ����");
                        break;
                }
            }
            else if (card_1.Month != card_2.Month) // ���� ���� �ƴϸ� �������� �Ѿ��
            {
                //Debug.Log("��");
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
