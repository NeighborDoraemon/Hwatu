using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;          // 아이템 이름
    public Sprite item_Icon;         // 아이템 아이콘 (스프라이트)
    public int item_Price;           // 아이템 가격
    public ItemEffect itemEffect;    // 아이템 효과 (스크립터블 오브젝트)
    public bool isConsumable;        // 소모성 아이템 유무
    public ItemRarity item_Rarity;  // 아이템 등급

    public void ApplyEffect(PlayerCharacter_Controller player)
    {
        itemEffect.ApplyEffect(player);
    }
}

