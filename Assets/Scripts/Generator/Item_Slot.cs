using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item_Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject selection_Border;
    private Item stored_Item;

    [HideInInspector] public int slot_Index;
    private PlayerChar_Inventory_Manager player;

    private void Awake()
    {
        itemIcon.enabled = false;
        selection_Border.SetActive(false);
    }

    public void Initialized(int index, PlayerChar_Inventory_Manager player_Con)
    {
        slot_Index = index;
        player = player_Con;
    }

    public void Setup_Slot(Item newItem)
    {
        stored_Item = newItem;

        if (stored_Item == null)
        {
            itemIcon.enabled = false;
        }
        else
        {
            itemIcon.sprite = stored_Item.item_Icon;
            itemIcon.enabled = true;
        }

        Set_Selected(false);
    }

    public void Set_Selected(bool isSelected)
    {
        selection_Border.SetActive(isSelected);
    }

    public Item GetItem()
    {
        return stored_Item;
    }

    public void Hover_This_Slot()
    {
        Debug.Log($"Mouse entered slot {slot_Index}");
        player.On_Slot_Hover(slot_Index);
    }
}
