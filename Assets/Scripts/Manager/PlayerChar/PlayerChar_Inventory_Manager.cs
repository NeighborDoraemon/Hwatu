using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Item> player_Inventory = new List<Item>();
    [SerializeField] private List<Item_Slot> item_Slots;

    private int selected_Slot_Index = 0;

    private Dictionary<ItemEffect, int> active_Effects = new Dictionary<ItemEffect, int>();

    public bool has_BowSheath_Effect = false;
    public bool has_EarRing_Effect = false;

    [HideInInspector] public GameObject earRing_Explosion_Prefab;

    private void Start()
    {
        
    }

    public void AddItem(Item newItem)
    {
        if (!newItem.isConsumable)
        {
            player_Inventory.Add(newItem);
            Apply_Item_Effect(newItem);
        }

        Update_Inventory();
    }
    
    public void RemoveItem(Item item)
    {
        player_Inventory.Remove(item);
        Update_Inventory();
    }

    private void Apply_Item_Effect(Item new_Item)
    {
        PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
        if (player == null || new_Item.itemEffect == null) return;

        if (!active_Effects.ContainsKey(new_Item.itemEffect))
        {
            active_Effects[new_Item.itemEffect] = 1;
            new_Item.itemEffect.ApplyEffect(player);
        }
        else
        {
            active_Effects[new_Item.itemEffect]++;
        }
    }

    private void Remove_Item_Effect(Item item)
    {
        PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
        if (player == null || item.itemEffect == null) return;

        if (active_Effects.ContainsKey(item.itemEffect))
        {
            active_Effects[item.itemEffect]--;

            if (active_Effects[item.itemEffect] <= 0)
            {
                active_Effects.Remove(item.itemEffect);
                item.itemEffect.RemoveEffect(player);
            }
        }
    }

    public void Re_Apply_All_Effects()
    {
        PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
        if (player == null) return;
        
        foreach (var effect in active_Effects.Keys)
        {
            effect.ApplyEffect(player);
        }
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