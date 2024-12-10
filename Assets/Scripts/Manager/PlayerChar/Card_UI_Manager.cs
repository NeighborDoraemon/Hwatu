using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_UI_Manager : MonoBehaviour
{
    public Image[] cardSlots;

    public void UpdateCardUI(Sprite[] cardSprites)
    {
        for (int i = 0; i < cardSlots.Length; i++)
        {
            if (i < cardSprites.Length && cardSprites[i] != null)
            {
                cardSlots[i].sprite = cardSprites[i];
                cardSlots[i].color = Color.white;
            }
            else
            {
                cardSlots[i].sprite = null;
                cardSlots[i].color = new Color(1, 1, 1, 0);
            }
        }

    }
}
