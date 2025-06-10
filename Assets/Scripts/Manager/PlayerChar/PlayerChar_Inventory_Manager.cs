using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public const int max_Inventory_Size = 6;

    public List<Item> player_Inventory = new List<Item>();
    [SerializeField] private List<Item_Slot> item_Slots = new List<Item_Slot>();
    [SerializeField] private Item_Description item_Description_UI;

    public GameObject inventoryPanel;

    private int selected_Slot_Index = 0;

    private Dictionary<ItemEffect, int> active_Effects = new Dictionary<ItemEffect, int>();

    [HideInInspector] public bool has_BowSheath_Effect = false;
    [HideInInspector] public bool has_EarRing_Effect = false;
    [HideInInspector] public bool has_Dice_Effect = false;

    [HideInInspector] public GameObject earRing_Explosion_Prefab;

    protected virtual void Start()
    {
        var slot_Array = inventoryPanel
            .GetComponentsInChildren<Item_Slot>(true);
        item_Slots = new List<Item_Slot>(slot_Array);

        var player_Con = GetComponent<PlayerCharacter_Controller>();

        for (int i = 0; i < item_Slots.Count; i++)
        {
            item_Slots[i].Initialized(i, player_Con);
        }

        Update_Inventory();
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

    public void SwapItem(Item newItem)
    {
        if (player_Inventory.Count >= max_Inventory_Size)
        {
            Item old_Item = player_Inventory[selected_Slot_Index];
            Remove_Item_Effect(old_Item);

            PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
            Vector2 drop_Pos = new Vector2(player.transform.position.x, player.transform.position.y - 0.2f);
            Object_Manager.instance.Spawn_Specific_Item(drop_Pos, old_Item);

            player_Inventory[selected_Slot_Index] = newItem;
            Apply_Item_Effect(newItem);

            Update_Inventory();
        }
        else
        {
            Debug.LogWarning("Inventory is not full, can't swap the item.");
        }
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

    protected void Remove_Item_Effect(Item item)
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
        var player = this.GetComponent<PlayerCharacter_Controller>();
        if (player == null) return;
        
        var effects = active_Effects.Keys.ToList();
        foreach (var effect in effects)
        {
            if (effect is Scroll_Effect)
                continue;
            effect.RemoveEffect(player);
        }

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

        Update_ItemDescription();
    }

    public void Navigate_Inventory(int direction)
    {
        if (item_Slots.Count == 0) return;

        item_Slots[selected_Slot_Index].Set_Selected(false);

        selected_Slot_Index += direction;

        if (selected_Slot_Index < 0)
            selected_Slot_Index = player_Inventory.Count - 1;
        else if (selected_Slot_Index >= player_Inventory.Count)
            selected_Slot_Index = 0;

        item_Slots[selected_Slot_Index].Set_Selected(true);

        Update_ItemDescription();
    }

    public void On_Slot_Hover(int index)
    {
        //if (index < 0 || index >= item_Slots.Count)
        //    return;

        item_Slots[selected_Slot_Index].Set_Selected(false);
        selected_Slot_Index = index;

        item_Slots[selected_Slot_Index].Set_Selected(true);
        Update_ItemDescription();
    }

    private void Update_ItemDescription()
    {
        if (item_Description_UI != null)
        {
            if (player_Inventory.Count > 0)
                item_Description_UI.Show_Item_Info(player_Inventory[selected_Slot_Index]);
            else
                item_Description_UI.Show_Item_Info(null);
        }
    }

    //By KYH    Give inventory data to Market Stall
    public Item[] Give_Inventory_Data()
    {
        return player_Inventory.ToArray();
    }
}