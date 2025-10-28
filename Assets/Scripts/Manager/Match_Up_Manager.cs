using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.Examples;
using UnityEngine;

public class Match_Up_Manager : MonoBehaviour
{
    private enum Match  //Á·º¸
    {
        // 0 ~ 9
        Mang_Tong, 
        TTang_Jab = 1,
        One_GG, 
        Am_Haeng = 3,
        Two_GG,
        Three_GG, 
        Gu_Sa = 6,
        Four_GG,
        Five_GG,
        Six_GG,
        Seven_GG,
        Eight_GG,
        Gab_Oh,    //°©¿À
        
        // 10 ~ 15 -> 13~ 18
        Four_Six,  //¼¼·ú
        Four_Ten,  //Àå»ç
        One_Ten,   //Àå»æ
        One_Nine,  //±¸»æ
        One_Four,  //µ¶»ç
        One_Two,   //¾Ë¸®

        // 16 ~ 25 -> 19 ~ 28
        One_TT,
        Two_TT,
        Three_TT,
        Four_TT,
        Five_TT,
        Six_TT,
        Seven_TT,
        Eight_TT,
        Nine_TT,
        Jang_TT,

        // 26 ~ 28 -> 29 ~ 31
        One_Three_Gwang,
        One_Eight_Gwang,
        Three_Eight_Gwang
    }

    Dictionary<Tuple<int, int>, Match> Dict_Jokbo_Rev;

    public Match_Up_Manager()
    {
        Dict_Jokbo_Rev = new Dictionary<Tuple<int, int>, Match>
        {
        { Tuple.Create(2, 8), Match.Mang_Tong },   //¸ÁÅë
        { Tuple.Create(2, 18),Match.Mang_Tong },

        { Tuple.Create(3, 7),Match.TTang_Jab},    //¶¯ÀâÀÌ
        { Tuple.Create(7, 13),Match.TTang_Jab},

        { Tuple.Create(2, 9),Match.One_GG },      //ÇÑ²ý
        { Tuple.Create(3, 8),Match.One_GG },
        { Tuple.Create(3, 18),Match.One_GG },
        { Tuple.Create(8, 13),Match.One_GG },
        { Tuple.Create(5, 6),Match.One_GG },

        { Tuple.Create(4, 7),Match.Am_Haeng },    //¾ÏÇà¾î»ç

        { Tuple.Create(2, 10),Match.Two_GG },     //µÎ²ý
        { Tuple.Create(3, 9),Match.Two_GG },
        { Tuple.Create(9, 13),Match.Two_GG },
        { Tuple.Create(4, 8),Match.Two_GG },
        { Tuple.Create(5, 7),Match.Two_GG },

        { Tuple.Create(3, 10), Match.Three_GG},   //¼¼²ý
        { Tuple.Create(10, 13), Match.Three_GG},
        { Tuple.Create(5, 8), Match.Three_GG},
        { Tuple.Create(5, 18), Match.Three_GG},
        { Tuple.Create(6, 7), Match.Three_GG},

        { Tuple.Create(4, 9), Match.Gu_Sa},       //±¸»ç

        { Tuple.Create(1, 3),Match.Four_GG },     //³×²ý
        { Tuple.Create(1, 13),Match.Four_GG },
        { Tuple.Create(3, 11),Match.Four_GG },
        { Tuple.Create(5, 9),Match.Four_GG },
        { Tuple.Create(6, 8),Match.Four_GG },

        { Tuple.Create(2, 3),Match.Five_GG },     //´Ù¼¸²ý
        { Tuple.Create(2, 13),Match.Five_GG },
        { Tuple.Create(5, 10),Match.Five_GG },
        { Tuple.Create(6, 9),Match.Five_GG },
        { Tuple.Create(7, 8),Match.Five_GG },
        { Tuple.Create(7, 18),Match.Five_GG },

        { Tuple.Create(1, 5),Match.Six_GG },      //¿©¼¸²ý
        { Tuple.Create(5, 11),Match.Six_GG },
        { Tuple.Create(2, 4),Match.Six_GG },
        { Tuple.Create(6, 10),Match.Six_GG },
        { Tuple.Create(7, 9),Match.Six_GG },

        { Tuple.Create(1, 6),Match.Seven_GG },    //ÀÏ°ö²ý
        { Tuple.Create(6, 11),Match.Seven_GG },
        { Tuple.Create(2, 5),Match.Seven_GG },
        { Tuple.Create(3, 4),Match.Seven_GG },
        { Tuple.Create(7, 10),Match.Seven_GG },
        { Tuple.Create(8, 9),Match.Seven_GG },
        { Tuple.Create(9, 18),Match.Seven_GG },

        { Tuple.Create(1, 7),Match.Eight_GG },    //¿©´ü²ý
        { Tuple.Create(7, 11),Match.Eight_GG },
        { Tuple.Create(2, 6),Match.Eight_GG },
        { Tuple.Create(3, 5),Match.Eight_GG },
        { Tuple.Create(5, 13),Match.Eight_GG },
        { Tuple.Create(8, 10),Match.Eight_GG },
        { Tuple.Create(10, 18),Match.Eight_GG },

        { Tuple.Create(1, 8),Match.Gab_Oh },      //¾ÆÈ©²ý
        { Tuple.Create(1, 18),Match.Gab_Oh },
        { Tuple.Create(8, 11),Match.Gab_Oh },
        { Tuple.Create(2, 7),Match.Gab_Oh },
        { Tuple.Create(3, 6),Match.Gab_Oh },
        { Tuple.Create(6, 13),Match.Gab_Oh },
        { Tuple.Create(4, 5),Match.Gab_Oh },
        { Tuple.Create(9, 10),Match.Gab_Oh },

        { Tuple.Create(4, 6),Match.Four_Six },    //¼¼·ú

        { Tuple.Create(4, 10),Match.Four_Ten },   //Àå»ç

        { Tuple.Create(1, 10),Match.One_Ten },    //Àå»æ
        { Tuple.Create(10, 11),Match.One_Ten },

        { Tuple.Create(1, 9),Match.One_Nine },    //±¸»æ
        { Tuple.Create(9, 11),Match.One_Nine },

        { Tuple.Create(1, 4),Match.One_Four },    //µ¶»ç
        { Tuple.Create(4, 11),Match.One_Four },

        { Tuple.Create(1, 2),Match.One_Two },    //¾Ë¸®
        { Tuple.Create(2, 11),Match.One_Two },

        { Tuple.Create(1, 11),Match.One_TT },     //¶¯
        { Tuple.Create(2, 2),Match.Two_TT },
        { Tuple.Create(3, 13),Match.Three_TT },
        { Tuple.Create(4, 4),Match.Four_TT },
        { Tuple.Create(5, 5),Match.Five_TT },
        { Tuple.Create(6, 6),Match.Six_TT },
        { Tuple.Create(7, 7),Match.Seven_TT },
        { Tuple.Create(8, 8),Match.Eight_TT },
        { Tuple.Create(9, 9),Match.Nine_TT },
        { Tuple.Create(10, 10),Match.Jang_TT },

        { Tuple.Create(11, 13),Match.One_Three_Gwang },   //±¤
        { Tuple.Create(11, 18),Match.One_Eight_Gwang },
        { Tuple.Create(13, 18),Match.Three_Eight_Gwang },
        };
    }

    private Match player_match;
    private Match map_match;

    [SerializeField] private PlayerCharacter_Controller p_controller;
    [SerializeField] private TextMeshPro Match_Text;

    [Header("Map Card Display")]
    [SerializeField] private GameObject Map_Card_01;
    [SerializeField] private GameObject Map_Card_02;
    [SerializeField] private SpriteRenderer Map_Card_Image_01;    
    [SerializeField] private SpriteRenderer Map_Card_Image_02;
    [SerializeField] private TextMeshPro Map_Card_Text;

    [SerializeField] private List<Sprite> Map_Card_Sprites = new List<Sprite>();

    [Header("Others")]
    [SerializeField] private Weapon_Manager weapon_Manager;

    private bool is_damage_up = false;
    private bool is_damage_down = false;

    public void Give_Player_Cards(GameObject card_01, GameObject card_02)
    {
        int i_First = card_01.GetComponent<Card>().cardValue.Month;
        int i_Second = card_02.GetComponent<Card>().cardValue.Month;

        Tuple<int, int> key = i_First > i_Second ? Tuple.Create(i_Second, i_First) : Tuple.Create(i_First, i_Second);

        if(Dict_Jokbo_Rev.ContainsKey(key))
        {
            player_match = Dict_Jokbo_Rev[key];
        }
        else
        {
            Debug.Log("Player Match Missing!");
        }
    }

    public void Give_Map_Cards(int card_01, int card_02)
    {
        Debug.Log("Map_Card Recieved");

        Tuple<int, int> key = card_01 > card_02 ? Tuple.Create(card_02, card_01) : Tuple.Create(card_01, card_02);

        if (card_01 == 13 || card_01 == 18 || card_02 == 13 || card_02 == 18)
        {
            //¸Ê Ä«µå ÀÌ¹ÌÁö ¼³Á¤
            if (card_01 == 13) //±¤
            {
                Map_Card_Image_01.sprite = Map_Card_Sprites[11];
            }
            else if (card_01 == 18) //±¤
            {
                Map_Card_Image_01.sprite = Map_Card_Sprites[12];
            }

            if (card_02 == 13) //±¤
            {
                Map_Card_Image_02.sprite = Map_Card_Sprites[11];
            }
            else if (card_02 == 18) //±¤
            {
                Map_Card_Image_02.sprite = Map_Card_Sprites[12];
            }
        }
        else
        {
            Map_Card_Image_01.sprite = Map_Card_Sprites[card_01 - 1];
            Map_Card_Image_02.sprite = Map_Card_Sprites[card_02 - 1];
        }

        Map_Card_Text.text = Compute_Weapon(card_01, card_02).comb_Name;

        if (Dict_Jokbo_Rev.ContainsKey(key))
        {
            map_match = Dict_Jokbo_Rev[key];
        }
        else
        {
            Debug.Log("Map Match Missing!");
        }
    }

    public void Start_Match()
    {
        if (player_match == Match.TTang_Jab)
        {
            if (map_match >= Match.One_TT && map_match <= Match.Nine_TT)
            {//¶¯ÀâÀÌ 
                player_Higher();
            }
            else if(map_match == Match.TTang_Jab || map_match == Match.Mang_Tong)
            {//µ¿ÀÏ
                Match_Reset();
                Print_Match(0);
            }
            else
            {
                Match_Normal();
            }
        }
        else if (player_match == Match.Am_Haeng)
        {
            if (map_match == Match.One_Eight_Gwang || map_match == Match.One_Three_Gwang)
            {//¾ÏÇà¾î»ç
                player_Higher();
            }
            else if (map_match == Match.Am_Haeng || map_match == Match.One_GG)
            {//µ¿ÀÏ
                Match_Reset();
                Print_Match(0);
            }
            else
            {
                Match_Normal();
            }
        }
        else if (player_match >= Match.One_TT && player_match <= Match.Nine_TT)
        {
            if(map_match == Match.TTang_Jab)
            {
                map_Higher();
            }
            else
            {
                Match_Normal();
            }
        }
        else if (player_match == Match.One_Eight_Gwang || player_match == Match.One_Three_Gwang)
        {
            if (map_match == Match.Am_Haeng)
            {
                map_Higher();
            }
            else
            {
                Match_Normal();
            }
        }
        else if(player_match == Match.Gu_Sa || player_match == Match.Three_GG)
        {
            if(map_match == Match.Three_GG || map_match == Match.Gu_Sa)
            {
                Match_Reset();
                Print_Match(0);
            }
            else
            {
                Match_Normal();
            }
        }
        else
        {
            Match_Normal();
        }

        Debug.Log("Map Match: " + map_match.ToString());
        Debug.Log("Player Match: " + player_match.ToString());

        //Print_Map_Card();
    }

    private void Match_Normal()
    {
        if ((int)player_match != (int)map_match)    //Á·º¸°¡ °°Àº µî±ÞÀÌ¸é ¾ÈµÊ
        {
            if ((int)player_match > (int)map_match)
            {
                player_Higher();
            }
            else
            {
                if (p_controller.has_Dice_Effect)
                {
                    player_Higher();
                    Debug.Log("Dice Effect has enabled!");
                    return;
                }
                map_Higher();
            }
        }
        else    //µ¥¹ÌÁö Á¤»óÈ­
        {
            if (is_damage_up && !is_damage_down)
            {
                p_controller.damage_Mul -= 0.5f;
                is_damage_up = false;
            }
            else if (!is_damage_up && is_damage_down)
            {
                p_controller.damage_Mul += 0.3f;
                is_damage_down = false;
            }
            Print_Match(0);
        }
    }

    private void player_Higher()
    {
        Debug.Log("Player's Match is Higher!");
        if (p_controller.card_Match_Dmg_Inc)
        {
            p_controller.damage_Mul += 0.5f;
        }
        else
        {
            p_controller.damage_Mul += 0.3f;
        }
        Print_Match(1);
        is_damage_up = true;
    }

    private void map_Higher()
    {
        Debug.Log("Map's Match is Higher!");
        p_controller.damage_Mul -= 0.3f;
        Print_Match(2);
        is_damage_down = true;
    }

    public void Match_Reset()
    {
        if (is_damage_up && !is_damage_down)
        {
            if (p_controller.card_Match_Dmg_Inc)
            {
                p_controller.damage_Mul -= 0.5f;
            }
            else
            {
                p_controller.damage_Mul -= 0.3f;
            }
            
            is_damage_up = false;
        }
        else if (!is_damage_up && is_damage_down)
        {
            p_controller.damage_Mul += 0.3f;
            is_damage_down = false;
        }
    }

    private void Print_Match(int case_index)
    {
        switch (case_index)
        {
            case 0:
                {
                    Match_Text.text = "»ó¼º µ¿ÀÏ!\nµ¥¹ÌÁö°¡ º¯ÇÏÁö ¾Ê½À´Ï´Ù";
                    StartCoroutine(Fade_Sprite(Match_Text, 1.0f, 1.0f, 0.0f));
                    break;
                }
            case 1:
                {
                    Match_Text.text = "»ó¼º ½Â¸®!\nµ¥¹ÌÁö°¡ 1.5¹è Áõ°¡ÇÕ´Ï´Ù";
                    StartCoroutine(Fade_Sprite(Match_Text, 1.0f, 1.0f, 0.0f));
                    break;
                }
            case 2:
                {
                    Match_Text.text = "»ó¼º ÆÐ¹è...\nµ¥¹ÌÁö°¡ 1.3¹è °¨¼ÒÇÕ´Ï´Ù";
                    StartCoroutine(Fade_Sprite(Match_Text, 1.0f, 1.0f, 0.0f));
                    break;
                }
        }
    }

    private void Print_Map_Card()
    {
        Map_Card_01.SetActive(true);
        Map_Card_02.SetActive(true);
        Map_Card_Text.gameObject.SetActive(true);

        Map_Card_01.GetComponent<DOTweenAnimation>().DOPlay();
        Map_Card_02.GetComponent<DOTweenAnimation>().DOPlay();
    }

    public void Hide_Map_Card()
    {
        Map_Card_01.GetComponent<DOTweenAnimation>().DORewind();
        Map_Card_02.GetComponent<DOTweenAnimation>().DORewind();
        Map_Card_01.SetActive(false);
        Map_Card_02.SetActive(false);
        Map_Card_Text.gameObject.SetActive(false);
    }

    private IEnumerator Fade_Sprite(TextMeshPro sprite, float targetAlpha, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Color color = sprite.color;
        //float startAlpha = color.a;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0.0f, targetAlpha, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(targetAlpha, 0.0f, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = 0.0f;
        sprite.color = color;
    }

    private Weapon_Data Compute_Weapon(int c1, int c2)
    {
        int weaponID = 1;

        if (c1 > 10 && c2 > 10 && c1 != c2)
        {
            if ((c1 == 11 && c2 == 13)
                || (c1 == 13 && c2 == 11))
            {
                weaponID = 21;
            }
            else if ((c1 == 11 && c2 == 18)
                || (c1 == 18 && c2 == 11))
            {
                weaponID = 15;
            }
            else if ((c1 == 13 && c2 == 18)
                || (c1 == 18 && c2 == 13))
            {
                weaponID = 6;
            }
        }
        else if ((c1 % 10) == (c2 % 10))
        {
            switch (c1 % 10)
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
                    Debug.Log("ÇØ´ç ¿ùÀÌ ¾øÀ½");
                    break;
            }
        }
        else if (c1 != c2)
        {
            if ((c1 + c2) % 10 >= 1 && (c1 + c2) % 10 <= 8)
            {
                if ((c1 % 10 == 1 && c2 == 4)
                    || (c1 == 4 && c2 % 10 == 1))
                {
                    weaponID = 7;
                }
                else if ((c1 % 10 == 1 && c2 == 2)
                    || (c1 == 2 && c2 % 10 == 1))
                {
                    weaponID = 9;
                }
                else if ((c1 % 10 == 1 && c2 == 10)
                    || (c1 == 10 && c2 % 10 == 1))
                {
                    weaponID = 22;
                }
                else if ((c1 == 10 && c2 == 4)
                    || (c1 == 4 && c2 == 10))
                {
                    weaponID = 17;
                }
                else if ((c1 == 7 && c2 == 4)
                    || (c1 == 4 && c2 == 7))
                {
                    weaponID = 13;
                }
                else if ((c1 == 9 && c2 == 4)
                    || (c1 == 4 && c2 == 9))
                {
                    weaponID = 19;
                }
                else
                {
                    weaponID = 2;
                }
            }
            else if ((c1 + c2) % 10 == 9)
            {
                weaponID = 3;
            }
            else
            {
                if (((c1 % 10) == 1 && c2 == 9)
                    || (c1 == 9 && (c2 % 10) == 1))
                {
                    weaponID = 8;
                }
                else if ((c1 == 4 && c2 == 6)
                    || (c1 == 6 && c2 == 4))
                {
                    weaponID = 5;
                }
                else if ((c1 == 7 && c2 % 10 == 3)
                    || (c1 % 10 == 3 && c2 == 7))
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
}
