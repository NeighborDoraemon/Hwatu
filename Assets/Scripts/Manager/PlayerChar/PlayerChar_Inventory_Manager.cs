using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Inventory_Slot> inventory_Slots;
    public List<Item> player_Inventory = new List<Item>();
    
    public static bool is_Inventory_Full = false;

    public void AddItem(Item newItem)
    {
        foreach (var slot in inventory_Slots)
        {
            if (slot.GetItem() == null)
            {
                slot.AddItem(newItem);
                player_Inventory.Add(newItem);
                if (player_Inventory.Count >= inventory_Slots.Count) is_Inventory_Full = true;
                return;
            }
        }

        if (player_Inventory.Count >= inventory_Slots.Count)
        {
            PlayerCharacter_Controller player = GetComponent<PlayerCharacter_Controller>();
            if (player != null)
            {
                player.Open_Inventory_To_Swap(newItem);
            }
        }
        if (!newItem.isConsumable)
        {
            player_Inventory.Add(newItem);
            PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
            if (player != null) 
            {
                newItem.ApplyEffect(player);
            }
        }
    }
    
    public void RemoveItem(Item item)
    {
        foreach(var slot in inventory_Slots)
        {
            if (slot.GetItem() == item)
            {
                slot.Clear_Slot();
                player_Inventory.Remove(item);
                return;
            }
        }            
    }

    public void Update_Item_Effects()
    {
        PlayerCharacter_Controller player = GetComponent<PlayerCharacter_Controller>();

        if (player == null)
        {
            Debug.LogError("PlayerController is missing");
            return;
        }

        foreach (var item in player_Inventory)
        {
            item.ApplyEffect(player);
        }
    }
}
