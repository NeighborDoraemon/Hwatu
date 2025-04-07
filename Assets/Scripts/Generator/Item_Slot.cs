using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private GameObject selection_Border;
    private Item stored_Item;

    public void Setup_Slot(Item newItem)
    {
        stored_Item = newItem;

        if (stored_Item != null)
        {
            itemIcon.sprite = stored_Item.item_Icon;
            itemIcon.enabled = true;
        }
        else
        {
            itemIcon.enabled = false;
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
}
