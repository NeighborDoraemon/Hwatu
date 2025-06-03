using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Card_Value cardValue;
    public Sprite selected_Sprite;
    private SpriteRenderer sp_Render;

    private void Awake()
    {
        sp_Render = GetComponent<SpriteRenderer>();
        if (sp_Render == null)
            Debug.LogError("[Card] There is no SpriteRenderer on the Card");
    }

    public void Render_Sprite(Sprite sprite)
    {
        sp_Render.sprite = sprite;
    }
}
