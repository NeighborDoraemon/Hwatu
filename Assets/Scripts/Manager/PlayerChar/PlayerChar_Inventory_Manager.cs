using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Item> player_Inventory = new List<Item>();
    [SerializeField] private List<Item_Slot> item_Slots;

    private int selected_Slot_Index = 0;

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

        Update_Inventory();
    }
    
    public void RemoveItem(Item item)
    {
        player_Inventory.Remove(item);
        Update_Inventory();
    }

    public void Update_Inventory()
    {
        for (int i = 0; i < item_Slots.Count; i++)
        {
            if (i < player_Inventory.Count)
            {
                item_Slots[i].Setup_Slot(player_Inventory[i]);
            }
            else
            {
                item_Slots[i].Setup_Slot(null);
            }

            item_Slots[i].Set_Selected(i == selected_Slot_Index);
        }
    }

    public void Navigate_Inventory(int direction)
    {
        if (player_Inventory.Count == 0) return;

        item_Slots[selected_Slot_Index].Set_Selected(false);

        selected_Slot_Index += direction;

        if (selected_Slot_Index < 0)
            selected_Slot_Index = player_Inventory.Count - 1;
        else if (selected_Slot_Index >= player_Inventory.Count)
            selected_Slot_Index = 0;

        item_Slots[selected_Slot_Index].Set_Selected(true);
    }


    //By KYH    Give inventory data to Market Stall
    public Item[] Give_Inventory_Data()
    {
        return player_Inventory.ToArray();
    }
}