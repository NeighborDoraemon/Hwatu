using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerCharacter_Card_Manager : PlayerCharacter_Stat_Manager
{
    [Header("Card_Manager")]
    public Card_UI_Manager card_UI_Manager;                         // 화투 UI 매니저 변수
    [HideInInspector]
    public Card_Value card_Value;                                   // 카드 스크립터블 오브젝트
    [HideInInspector]
    public SpriteRenderer sprite_Renderer;

    public GameObject[] card_Inventory = new GameObject[3];         // 화투 오브젝트 저장 공간 배열
    protected int cardCount = 0;                                    // 화투 갯수 카운트 변수

    protected bool isCombDone = false;                              // 화투 조합이 이루어졌는지 체크하는 변수

    private readonly HashSet<int> months_Exact = new();
    private readonly HashSet<int> months_Norm = new();

    private const int Snake_Wine_Weapon_ID = 7;

    private static int Normalize_Month(int m)
    {
        if (m == 10) return 10;
        return m % 10;
    }

    private void Rebuild_Month_Sets()
    {
        months_Exact.Clear();
        months_Norm.Clear();

        foreach(var go in card_Inventory)
        {
            if (go == null) continue;
            if (!go.TryGetComponent<Card>(out var card) || card.cardValue == null) continue;

            int exact = card.cardValue.Month;
            int norm = Normalize_Month(exact);

            months_Exact.Add(exact);
            months_Norm.Add(norm);
        }

        Debug.Log($"[Player CardManager] months_Exact={months_Exact},{months_Exact}, months_Norm={months_Norm}");
    }

    public void AddCard(GameObject card)
    {
        //isCombDone = false;

        if (cardCount == card_Inventory.Length)
        {
            if (card_Inventory[2] != null)
            {
                Card cardComponent = card_Inventory[2].GetComponent<Card>();
                if (cardComponent != null && cardComponent.selected_Sprite != null)
                {
                    Object_Manager.instance.Remove_Used_Sprite(cardComponent.selected_Sprite);
                }

                Destroy(card_Inventory[2]);
            }

            card_Inventory[2] = card;
        }
        else
        {
            card_Inventory[cardCount] = card;
            cardCount++;
            Debug.Log("카드 추가" + card.name);
        }

        Refresh_UI();

        Save_Manager.Instance.SaveAll();

        if (Object_Manager.instance != null)
        {
            Sprite collected_Sprite = card.GetComponent<SpriteRenderer>().sprite;

            Object_Manager.instance.Destroy_All_Cards(card);
            Object_Manager.instance.Remove_From_Spawned_Cards(card);
            Object_Manager.instance.Destroy_All_Items();

            card.SetActive(false);
        }
        else { Debug.LogWarning("Object Manager is missing"); }
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

    public void Recall_All_Cards()
    {
        // 1) 인벤토리에 들어있는 카드 정리 + 중복 방지 리스트에서 해제
        var sprites_To_Release = new List<Sprite>();
        for (int i = 0; i < card_Inventory.Length; i++)
        {
            var go = card_Inventory[i];
            if (!go) continue;

            var card = go.GetComponent<Card>();
            if (card != null && card.selected_Sprite != null)
            {
                sprites_To_Release.Add(card.selected_Sprite);
            }
        }

        for (int i = 0; i < card_Inventory.Length; i++)
        {
            if (card_Inventory[i] != null)
            {
                Destroy(card_Inventory[i]);
                card_Inventory[i] = null;
            }
        }
        cardCount = 0;

        // 2) 필드에 남아있는 카드 오브젝트 정리
        if (Object_Manager.instance != null)
        {
            Object_Manager.instance.Destroy_All_Cards(null);

            foreach (var sp in sprites_To_Release)
                Object_Manager.instance.Remove_Used_Sprite(sp);
        }
        else
        {
            Debug.LogWarning("[Card Manager] Object Manager is missing.");
        }

        // 3) 조합/무기/월 세트/UI 초기화
        isCombDone = false;
        Rebuild_Month_Sets();
        Set_Weapon(0);
        card_UI_Manager.Update_CombText("");
        UpdateCardUI();
    }

    public void Change_FirstAndThird_Card()
    {
        if (card_Inventory[2] != null)
            (card_Inventory[0], card_Inventory[2]) = (card_Inventory[2], card_Inventory[0]);
        //{
        //    GameObject temp = card_Inventory[0];
        //    card_Inventory[0] = card_Inventory[2];
        //    card_Inventory[2] = temp;
        //}

        Refresh_UI();
    }

    public void Change_SecondAndThird_Card()
    {
        if (card_Inventory[2] != null)
            (card_Inventory[1], card_Inventory[2]) = (card_Inventory[2], card_Inventory[1]);
        //{
        //    GameObject temp = card_Inventory[1];
        //    card_Inventory[1] = card_Inventory[2];
        //    card_Inventory[2] = temp;
        //}

        Refresh_UI() ;
    }

    public void Card_Combination()
    {
        isCombDone = false;
        string comb_Name = string.Empty;

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
                    comb_Name = "1 3 광땡";
                }
                else if ((card_1.Month == 11 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 11))
                {
                    Set_Weapon(15);
                    comb_Name = "1 8 광땡";
                }
                else if ((card_1.Month == 13 && card_2.Month == 18)
                    || (card_1.Month == 18 && card_2.Month == 13))
                {
                    Set_Weapon(6);
                    comb_Name = "3 8 광땡";
                }
                isCombDone = true;
            }
            else if ((card_1.Month % 10) == (card_2.Month % 10))
            {
                switch (card_1.Month % 10)
                {
                    case 1:
                        Set_Weapon(4);
                        comb_Name = "1 땡";
                        break;
                    case 2:
                        Set_Weapon(11);
                        comb_Name = "2 땡";
                        break;
                    case 3:
                        Set_Weapon(12);
                        comb_Name = "3 땡";
                        break;
                    case 4:
                        Set_Weapon(18);
                        comb_Name = "4 땡";
                        break;
                    case 5:
                        Set_Weapon(20);
                        comb_Name = "5 땡";
                        break;
                    case 6:
                        Set_Weapon(16);
                        comb_Name = "6 땡";
                        break;
                    case 7:
                        Set_Weapon(24);
                        comb_Name = "7 땡";
                        break;
                    case 8:
                        Set_Weapon(10);
                        comb_Name = "8 땡";
                        break;
                    case 9:
                        Set_Weapon(14);
                        comb_Name = "9 땡";
                        break;
                    case 0:
                        Set_Weapon(25);
                        comb_Name = "장땡";
                        break;
                    default:
                        Debug.Log("해당 월이 없음");
                        break;
                }
                isCombDone = true;
            }
            else if (card_1.Month != card_2.Month)
            {
                if ((card_1.Month + card_2.Month) % 10 >= 1 && (card_1.Month + card_2.Month) % 10 <= 8)
                {
                    if ((card_1.Month % 10 == 1 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month % 10 == 1))
                    {
                        Set_Weapon(7);
                        comb_Name = "독사";
                    }
                    else if ((card_1.Month % 10 == 1 && card_2.Month == 2)
                        || (card_1.Month == 2 && card_2.Month % 10 == 1))
                    {
                        Set_Weapon(9);
                        comb_Name = "알리";
                    }
                    else if ((card_1.Month % 10 == 1 && card_2.Month == 10)
                        || (card_1.Month == 10 && card_2.Month % 10 == 1))
                    {
                        Set_Weapon(22);
                        comb_Name = "장삥";
                    }
                    else if ((card_1.Month == 10 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 10))
                    {
                        Set_Weapon(17);
                        comb_Name = "장사";
                    }
                    else if ((card_1.Month == 7 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 7))
                    {
                        Set_Weapon(13);
                        comb_Name = "암행어사";
                    }
                    else if ((card_1.Month == 9 && card_2.Month == 4)
                        || (card_1.Month == 4 && card_2.Month == 9))
                    {
                        Set_Weapon(19);
                        comb_Name = "구사";
                    }
                    else
                    {
                        Set_Weapon(2);
                        comb_Name = ((card_1.Month + card_2.Month) % 10) + " 끗";
                        //comb_Text.text = ((card_1.Month + card_2.Month) % 10) + " " + cur_Weapon_Data.comb_Name;
                    }
                }
                else if ((card_1.Month + card_2.Month) % 10 == 9)
                {
                    Set_Weapon(3);
                    comb_Name = "갑오";
                }
                else
                {
                    if (((card_1.Month % 10) == 1 && card_2.Month == 9)
                        || (card_1.Month == 9 && (card_2.Month % 10) == 1))
                    {
                        Set_Weapon(8);
                        comb_Name = "구삥";
                    }
                    else if ((card_1.Month == 4 && card_2.Month == 6)
                        || (card_1.Month == 6 && card_2.Month == 4))
                    {
                        Set_Weapon(5);
                        comb_Name = "세륙";
                    }
                    else if ((card_1.Month == 7 && card_2.Month % 10 == 3)
                        || (card_1.Month % 10 == 3 && card_2.Month == 7))
                    {
                        Set_Weapon(23);
                        comb_Name = "땡잡이";
                    }
                    else
                    {
                        Set_Weapon(1);
                        comb_Name = "망통";
                    }
                }
                isCombDone = true;
            }
            else
            {
                Set_Weapon(0);
                Debug.Log("해당 조합 없음");
            }

            if (string.IsNullOrEmpty(comb_Name))
                comb_Name = "";

            card_UI_Manager.Update_CombText(comb_Name);
        }

        
    }

    public bool Has_All_Exact(params int[] months)
    {
        if (months == null || months.Length == 0) return false;
        foreach (var m in months)
            if (!months_Exact.Contains(m)) return false;
        return true;
    }
    public bool Has_Any_Exact(params int[] months)
    {
        if (months == null || months.Length == 0) return false;
        foreach (var m in months)
            if (months_Exact.Contains(m)) return true;
        return false;
    }

    public bool Has_All_Norm(params int[] months)
    {
        if (months == null || months.Length == 0) return false;
        foreach (var m in months)
            if (!months_Norm.Contains(Normalize_Month(m))) return false;
        return true;
    }
    public bool Has_Any_Norm(params int[] months)
    {
        if (months == null || months.Length == 0) return false;
        foreach (var m in months)
            if (months_Norm.Contains(Normalize_Month(m))) return true;
        return false;
    }

    public bool Has_Three_And_ThreeG() => Has_Any_Exact(3, 13);
    public bool Has_Four_And_Nine() => Has_All_Exact(4, 9);
    public bool Has_One_And_Four() => Has_All_Norm(1, 4);

    public Weapon_Data Compute_Weapon(Card_Value c1, Card_Value c2)
    {
        int weaponID = 1;

        if (c1.Month > 10 && c2.Month > 10 && c1.Month != c2.Month)
        {
            if ((c1.Month == 11 && c2.Month == 13)
                || (c1.Month == 13 && c2.Month == 11))
            {
                weaponID = 21;
            }
            else if ((c1.Month == 11 && c2.Month == 18)
                || (c1.Month == 18 && c2.Month == 11))
            {
                weaponID = 15;
            }
            else if ((c1.Month == 13 && c2.Month == 18)
                || (c1.Month == 18 && c2.Month == 13))
            {
                weaponID = 6;
            }
        }
        else if ((c1.Month % 10) == (c2.Month % 10))
        {
            switch (c1.Month % 10)
            {
                case 1:
                    weaponID = 4;
                    break;
                case 2:
                    weaponID = 11;
                    break;
                case 3:
                    weaponID = 12;
                    break;
                case 4:
                    weaponID = 18;
                    break;
                case 5:
                    weaponID = 20;
                    break;
                case 6:
                    weaponID = 16;
                    break;
                case 7:
                    weaponID = 24;
                    break;
                case 8:
                    weaponID = 10;
                    break;
                case 9:
                    weaponID = 14;
                    break;
                case 0:
                    weaponID = 25;
                    break;
                default:
                    Debug.Log("해당 월이 없음");
                    break;
            }
        }
        else if (c1.Month != c2.Month)
        {
            if ((c1.Month + c2.Month) % 10 >= 1 && (c1.Month + c2.Month) % 10 <= 8)
            {
                if ((c1.Month % 10 == 1 && c2.Month == 4)
                    || (c1.Month == 4 && c2.Month % 10 == 1))
                {
                    weaponID = 7;
                }
                else if ((c1.Month % 10 == 1 && c2.Month == 2)
                    || (c1.Month == 2 && c2.Month % 10 == 1))
                {
                    weaponID = 9;
                }
                else if ((c1.Month % 10 == 1 && c2.Month == 10)
                    || (c1.Month == 10 && c2.Month % 10 == 1))
                {
                    weaponID = 22;
                }
                else if ((c1.Month == 10 && c2.Month == 4)
                    || (c1.Month == 4 && c2.Month == 10))
                {
                    weaponID = 17;
                }
                else if ((c1.Month == 7 && c2.Month == 4)
                    || (c1.Month == 4 && c2.Month == 7))
                {
                    weaponID = 13;
                }
                else if ((c1.Month == 9 && c2.Month == 4)
                    || (c1.Month == 4 && c2.Month == 9))
                {
                    weaponID = 19;
                }
                else
                {
                    weaponID = 2;
                }
            }
            else if ((c1.Month + c2.Month) % 10 == 9)
            {
                weaponID = 3;
            }
            else
            {
                if (((c1.Month % 10) == 1 && c2.Month == 9)
                    || (c1.Month == 9 && (c2.Month % 10) == 1))
                {
                    weaponID = 8;
                }
                else if ((c1.Month == 4 && c2.Month == 6)
                    || (c1.Month == 6 && c2.Month == 4))
                {
                    weaponID = 5;
                }
                else if ((c1.Month == 7 && c2.Month % 10 == 3)
                    || (c1.Month % 10 == 3 && c2.Month == 7))
                {
                    weaponID = 23;
                }
                else
                {
                    weaponID = 1;
                }
            }
        }

        return weapon_Manager.Get_Weapon_Data(weaponID);
    }

    public void Refresh_UI()
    {
        Rebuild_Month_Sets();
        UpdateCardUI();

        Enforce_SnakeWine_SkillPolicy();

        Card_Combination();
    }

    private void Enforce_SnakeWine_SkillPolicy()
    {
        bool has_OneFour = Has_All_Norm(1, 4);

        var snake_Data = weapon_Manager.Get_Weapon_Data(Snake_Wine_Weapon_ID);
        var snake = snake_Data != null ? (snake_Data.attack_Strategy as SnakeWine_Attack_Strategy) : null;

        if (snake == null) return;

        if (!has_OneFour)
        {
            snake.skill_Count = 3;
        }
    }
}
