using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card_UI_Manager : MonoBehaviour
{
    [Header("PC 슬롯 Image")]
    [SerializeField ]private Image[] pc_CardSlots;
    [Header("모바일 슬롯 Image")]
    [SerializeField] private Image[] mobile_CardSlots;

    [Header("PC 조합 이름 텍스트")]
    [SerializeField] private TMP_Text pc_Comb_Text;
    [Header("모바일 조합 이름 텍스트")]
    [SerializeField] private TMP_Text mobile_Comb_Text;

    public void UpdateCardUI(Sprite[] cardSprites)
    {
        for (int i = 0; i < pc_CardSlots.Length; i++)
        {
            if (i < cardSprites.Length && cardSprites[i] != null)
            {
                pc_CardSlots[i].sprite = cardSprites[i];
                pc_CardSlots[i].color = Color.white;
            }
            else
            {
                pc_CardSlots[i].sprite = null;
                pc_CardSlots[i].color = new Color(1, 1, 1, 0);
            }
        }

        for (int i = 0; i < mobile_CardSlots.Length; i++)
        {
            if (i < cardSprites.Length && cardSprites[i] != null)
            {
                mobile_CardSlots[i].sprite = cardSprites[i];
                mobile_CardSlots[i].color = Color.white;
            }
            else
            {
                mobile_CardSlots[i].sprite = null;
                mobile_CardSlots[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void Update_CombText(string comb_Name)
    {
        pc_Comb_Text.text = comb_Name;
        mobile_Comb_Text.text = comb_Name;
    }
}
