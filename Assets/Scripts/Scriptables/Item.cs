using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;        // 아이템 이름
    public Sprite icon;            // 아이템 아이콘 (스프라이트)
    public ItemEffect itemEffect;  // 아이템 효과 (스크립터블 오브젝트)
    public bool isConsumable;      // 소모성 아이템 유무 

    // 아이템의 효과를 적용하는 함수
    public void ApplyEffect(PlayerCharacter_Controller player)
    {
        itemEffect.ApplyEffect(player);
    }
}

