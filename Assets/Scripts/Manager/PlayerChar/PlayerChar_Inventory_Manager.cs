using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Item> player_Inventory = new List<Item>();

    public void AddItem(Item newItem)
    {
        if (!newItem.isConsumable)
        {
            player_Inventory.Add(newItem);  // 아이템 추가
            PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
            if (player != null) 
            {
                newItem.ApplyEffect(this.GetComponent<PlayerCharacter_Controller>());
            }
        }
    }

    // 아이템 획득 시 플레이어가 상호작용한 아이템을 제거하는 함수
    public void RemoveItem(Item item)
    {
        player_Inventory.Remove(item);  // 인벤토리에서 아이템 제거
        //Debug.Log($"{item.itemName} 인벤토리에서 제거됨.");
    }
}
