using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite item_icon;
    public ItemEffect itemEffect;
    public bool isConsumable;
    
    public void ApplyEffect(PlayerCharacter_Controller player)
    {
        itemEffect.ApplyEffect(player);
    }
}

