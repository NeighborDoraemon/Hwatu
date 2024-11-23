using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCharacter_Card_Manager : PlayerCharacter_Stat_Manager
{
    [Header("Card_Manager")]
    public Card_UI_Manager card_UI_Manager;
    [HideInInspector]
    public Card_Value card_Value;
    [HideInInspector]
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[2];
    protected int cardCount = 0;

    protected bool isCombDone = false;

    [HideInInspector] public bool is_Start_Spawn = true;

    public void AddCard(GameObject card)
    {
        isCombDone = false;

        if (cardCount == card_Inventory.Length)
        {
            if (card_Inventory[0] != null)
            {
                Card cardComponent = card_Inventory[0].GetComponent<Card>();
                if (cardComponent != null && cardComponent.selected_Sprite != null)
                {
                    Object_Manager.instance.Remove_Used_Sprite(cardComponent.selected_Sprite);
                }

                Destroy(card_Inventory[0]);
            }

            card_Inventory[0] = card_Inventory[1];
            card_Inventory[1] = card;

            //Debug.Log("Card Changed");
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("Ä«µå Ãß°¡" + card.name);

            if (cardCount == card_Inventory.Length)
            {
                is_Start_Spawn = false;
            }
        }

        UpdateCardUI();
        Card_Combination();
        
        if (Object_Manager.instance != null && !is_Start_Spawn)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Object_Manager.instance.Destroy_All_Cards(card);
            Object_Manager.instance.Remove_From_Spawned_Cards(card);
            Object_Manager.instance.Destroy_All_Items();

            card.SetActive(false);
        }
        else { Debug.LogWarning("Object Spawner instance is missing"); }
    }

    void UpdateCardUI()
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
                    Debug.LogError("No spriteRenderer in Card" + card_Inventory[i].name);
                }
            }
            else
            {
                cardSprites[i] = null;
            }
        }

        card_UI_Manager.UpdateCardUI(cardSprites);
    }

    public void Card_Combination()
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
                    Set_Weapon(21);
                    Debug.Log("1 3 ±¤¶¯");
                }
                else if ((card_1.Month == 11 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 11))
                {
                    Set_Weapon(15);
                    Debug.Log("1 8 ±¤¶¯");
                }
                else if ((card_1.Month == 13 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 13))
                {
                    Set_Weapon(15);
                    Debug.Log("3 8 ±¤¶¯");
                }
                isCombDone = true;
            }
            else if ((card_1.Month % 10) == (card_2.Month % 10)) // ¼­·Î °°Àº ´ÞÀÎÁö È®ÀÎ
            {
                switch (card_1.Month % 10)
                {
                    case 1:
                        Set_Weapon(4);
                        Debug.Log("1¶¯");
                        break;
                    case 2:
                        Set_Weapon(11);
                        Debug.Log("2¶¯");
                        break;
                    case 3:
                        Set_Weapon(12);
                        Debug.Log("3¶¯");
                        break;
                    case 4:
                        Set_Weapon(18);
                        Debug.Log("4¶¯");
                        break;
                    case 5:
                        Set_Weapon(20);
                        Debug.Log("5¶¯");
                        break;
                    case 6:
                        Set_Weapon(16);
                        Debug.Log("6¶¯");
                        break;
                    case 7:
                        Debug.Log("7¶¯");
                        break;
                    case 8:
                        Set_Weapon(10);
                        Debug.Log("8¶¯");
                        break;
                    case 9:
                        Set_Weapon(14);
                        Debug.Log("9¶¯");
                        break;
                    case 10:
                        Debug.Log("10¶¯");
                        break;
                    default:
                        Debug.Log("ÇØ´ç ¿ùÀÌ ¾øÀ½");
                        break;
                }
                isCombDone = true;
            }
            else if (card_1.Month != card_2.Month) // °°Àº ´ÞÀÌ ¾Æ´Ï¸é ÀÌÂÊÀ¸·Î ³Ñ¾î¿È
            {
                if ((card_1.Month + card_2.Month) % 10 >= 1 && (card_1.Month + card_2.Month) % 10 <= 8)
                {
                    if ((card_1.Month % 10 == 1 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month % 10 == 1))
                    {
                        Set_Weapon(7);
                        Debug.Log("µ¶»ç");
                    }
                    else if ((card_1.Month % 10 == 1 && card_2.Month == 2)
                        || (card_1.Month == 2 && card_2.Month % 10 == 1))
                    {
                        Set_Weapon(9);
                        Debug.Log("¾Ë¸®");
                    }
                    else if ((card_1.Month % 10 == 1 && card_2.Month == 10)
                        || (card_1.Month == 10 && card_2.Month % 10 == 1))
                    {                        
                        Debug.Log("Àå»æ");
                    }
                    else if ((card_1.Month == 10 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 10))
                    {
                        Set_Weapon(17);
                        Debug.Log("Àå»ç");
                    }                     
                    else if ((card_1.Month == 7 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 7))
                    {
                        Set_Weapon(13);
                        Debug.Log("¾ÏÇà¾î»ç");
                    }  
                    else if ((card_1.Month == 9 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 9))
                    {
                        Set_Weapon(19);
                        Debug.Log("49ÆÄÅä");
                    }
                    else if ((card_1.Month == 7 && card_2.Month == 3)
                        || (card_1.Month == 3 && card_2.Month == 7))
                    {                        
                        Debug.Log("49ÆÄÅä");
                    }                    
                    else
                    {
                        Set_Weapon(2);
                        Debug.Log((card_1.Month + card_2.Month) % 10 + "²ý");
                    }
                }
                else if ((card_1.Month + card_2.Month) % 10 == 9)
                {
                    Set_Weapon(3);
                    Debug.Log("°©¿À");
                }
                else
                {
                    if (((card_1.Month % 10) == 1 && card_2.Month == 9)
                        || (card_1.Month == 9 && (card_2.Month % 10) == 1))
                    {
                        Set_Weapon(8);
                        Debug.Log("±¸»æ");
                    }
                    else if ((card_1.Month == 4 && card_2.Month == 6)
                        || (card_1.Month == 6 && card_2.Month == 4))
                    {
                        Set_Weapon(5);
                        Debug.Log("¼¼·ú");
                    }
                    else
                    {
                        Set_Weapon(1);
                        Debug.Log("¸ÁÅë");
                    }                    
                }
                isCombDone = true;
            }
            else
            {
                Debug.Log("ÇØ´ç Á¶ÇÕ ¾øÀ½");
            }
        }
    }
}