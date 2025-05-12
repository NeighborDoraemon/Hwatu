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
        Mang_Tong, TTang_Jab = 0,
        One_GG, Am_Haeng = 1,
        Two_GG,
        Three_GG, Gu_Sa = 3,
        Four_GG,
        Five_GG,
        Six_GG,
        Seven_GG,
        Eight_GG,
        Gab_Oh,    //°©¿À
        
        // 10 ~ 15
        Four_Six,  //¼¼·ú
        Four_Ten,  //Àå»ç
        One_Ten,   //Àå»æ
        One_Nine,  //±¸»æ
        One_Four,  //µ¶»ç
        One_Two,   //¾Ë¸®
        
        // 16 ~ 25
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

        // 26 ~ 28
        One_Three_Gwang,
        One_Eight_Gwang,
        Three_Eight_Gwang
    }

    Dictionary<(int, int), Match> Dict_Jokbo = new Dictionary<(int, int), Match>    //°ª
    {
        {(2,8),Match.Mang_Tong },   //¸ÁÅë
        {(2,18),Match.Mang_Tong },

        {(3,7),Match.TTang_Jab},    //¶¯ÀâÀÌ
        {(7,13),Match.TTang_Jab},

        {(2,9),Match.One_GG },      //ÇÑ²ý
        {(3,8),Match.One_GG },
        {(3,18),Match.One_GG },
        {(8,13),Match.One_GG },
        {(5,6),Match.One_GG },

        {(4,7),Match.Am_Haeng },    //¾ÏÇà¾î»ç

        {(2,10),Match.Two_GG },     //µÎ²ý
        {(3,9),Match.Two_GG },
        {(9,13),Match.Two_GG },
        {(4,8),Match.Two_GG },
        {(5,7),Match.Two_GG },

        {(3,10), Match.Three_GG},   //¼¼²ý
        {(10,13), Match.Three_GG},
        {(5,8), Match.Three_GG},
        {(5,18), Match.Three_GG},
        {(6,7), Match.Three_GG},

        {(4,9), Match.Gu_Sa},       //±¸»ç

        {(1,3),Match.Four_GG },     //³×²ý
        {(1,13),Match.Four_GG },
        {(3,11),Match.Four_GG },
        {(5,9),Match.Four_GG },
        {(6,8),Match.Four_GG },
            
        {(2,3),Match.Five_GG },     //´Ù¼¸²ý
        {(2,13),Match.Five_GG },
        {(5,10),Match.Five_GG },
        {(6,9),Match.Five_GG },
        {(7,8),Match.Five_GG },
        {(7,18),Match.Five_GG },

        {(1,5),Match.Six_GG },      //¿©¼¸²ý
        {(5,11),Match.Six_GG },
        {(2,4),Match.Six_GG },
        {(6,10),Match.Six_GG },
        {(7,9),Match.Six_GG },

        {(1,6),Match.Seven_GG },    //ÀÏ°ö²ý
        {(6,11),Match.Seven_GG },
        {(2,5),Match.Seven_GG },
        {(3,4),Match.Seven_GG },
        {(7,10),Match.Seven_GG },
        {(8,9),Match.Seven_GG },
        {(9,18),Match.Seven_GG },

        {(1,7),Match.Eight_GG },    //¿©´ü²ý
        {(7,11),Match.Eight_GG },
        {(2,6),Match.Eight_GG },
        {(3,5),Match.Eight_GG },
        {(5,13),Match.Eight_GG },
        {(8,10),Match.Eight_GG },
        {(10,18),Match.Eight_GG },

        {(1,8),Match.Gab_Oh },      //¾ÆÈ©²ý
        {(1,18),Match.Gab_Oh },
        {(8,11),Match.Gab_Oh },
        {(2,7),Match.Gab_Oh },
        {(3,6),Match.Gab_Oh },
        {(6,13),Match.Gab_Oh },
        {(4,5),Match.Gab_Oh },
        {(9,10),Match.Gab_Oh },

        {(4,6),Match.Four_Six },    //¼¼·ú

        {(4,10),Match.Four_Ten },   //Àå»ç

        {(1,10),Match.One_Ten },    //Àå»æ
        {(10,11),Match.One_Ten },

        {(1,9),Match.One_Nine },    //±¸»æ
        {(9,11),Match.One_Nine },

        {(1,4),Match.One_Four },    //µ¶»ç
        {(4,11),Match.One_Four },

        {(1,2),Match.One_Two },    //¾Ë¸®
        {(2,11),Match.One_Two },

        {(1,11),Match.One_TT },     //¶¯
        {(2,2),Match.Two_TT },
        {(3,13),Match.Three_TT },
        {(4,4),Match.Four_TT },
        {(5,5),Match.Five_TT },
        {(6,6),Match.Six_TT },
        {(7,7),Match.Seven_TT },
        {(8,8),Match.Eight_TT },
        {(9,9),Match.Nine_TT },
        {(10,10),Match.Jang_TT },

        {(11,13),Match.One_Three_Gwang },   //±¤
        {(11,18),Match.One_Eight_Gwang },
        {(13,18),Match.Three_Eight_Gwang },
    };


    private Match player_match;
    private Match map_match;

    [SerializeField] private PlayerCharacter_Controller p_controller;
    [SerializeField] private TextMeshPro Match_Text;

    private bool is_damage_up = false;
    private bool is_damage_down = false;

    public void Give_Player_Cards(GameObject card_01, GameObject card_02)
    {
        int i_First = card_01.GetComponent<Card>().cardValue.Month;
        int i_Second = card_02.GetComponent<Card>().cardValue.Month;

        (int, int) key = i_First > i_Second ? (i_Second, i_First) : (i_First, i_Second);

        if(Dict_Jokbo.ContainsKey(key))
        {
            player_match = Dict_Jokbo[key];
        }
        else
        {
            Debug.Log("Player Match Missing!");
        }
    }

    public void Give_Map_Cards(int card_01, int card_02)
    {
        (int, int) key = card_01 > card_02 ? (card_02, card_01) : (card_01, card_02);

        if (Dict_Jokbo.ContainsKey(key))
        {
            map_match = Dict_Jokbo[key];
        }
        else
        {
            Debug.Log("Map Match Missing!");
        }
    }

    public void Start_Match()
    {
        if (player_match == Match.TTang_Jab && ((int)map_match >= 16 && (int)map_match <= 24))
        {
            //¶¯ÀâÀÌ 
            player_Higher();
        }
        else if (player_match == Match.Am_Haeng && ((int)map_match >= 26 && (int)map_match <= 27))
        {
            //¾ÏÇà¾î»ç
            player_Higher();
        }
        else
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
                    p_controller.damage_Mul -= 0.3f;
                    is_damage_up = false;
                }
                else if(!is_damage_up && is_damage_down)
                {
                    p_controller.damage_Mul += 0.3f;
                    is_damage_down = false;
                }
                Print_Match(0);
            }
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
                    Match_Text.text = "»ó¼º ½Â¸®!\nµ¥¹ÌÁö°¡ 1.3¹è Áõ°¡ÇÕ´Ï´Ù";
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
}
