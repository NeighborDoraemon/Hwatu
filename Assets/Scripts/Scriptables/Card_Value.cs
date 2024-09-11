using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCard_Values", menuName = "Cards/Value")]
public class Card_Value : ScriptableObject
{
    public int Month; // �� ����
    public bool isGwang; // �� ����
    public Sprite[] sprites = new Sprite[4];
    public bool[] isGwangSprites = new bool[4];

    public Sprite GetRandomSprite()
    {
        if (sprites.Length > 0)
        {
            int randomIndex = Random.Range(0,sprites.Length);

            isGwang = isGwangSprites[randomIndex];
            return sprites[randomIndex];
        }
        return null;
    }
}
