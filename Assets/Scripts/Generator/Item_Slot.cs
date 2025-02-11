using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text item_Name_Text;
    [SerializeField] private GameObject selection_Border;
    private Item stored_Item;

    public void Setup_Slot(Item newItem)
    {
        stored_Item = newItem;

        if (stored_Item != null)
        {
            itemIcon.sprite = stored_Item.item_Icon;
            itemIcon.enabled = true;
            item_Name_Text.text = stored_Item.itemName;
        }
        else
        {
            itemIcon.enabled = false;
            item_Name_Text.text = "";
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
