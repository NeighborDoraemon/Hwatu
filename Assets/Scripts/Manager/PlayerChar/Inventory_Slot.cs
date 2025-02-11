using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Slot : MonoBehaviour
{
    public Image item_Image;
    public Item cur_Items;
    public GameObject highlight_Border;

    public void Highlight(bool is_Highlighted)
    {
        highlight_Border.SetActive(is_Highlighted);
    }

    public void AddItem(Item item)
    {
        cur_Items = item;
        item_Image.sprite = item.item_Icon;
        item_Image.enabled = true;
    }

    public void Clear_Slot()
    {
        cur_Items = null;
        item_Image = null;
        item_Image.enabled = false;
    }

    public Item GetItem()
    {
        return cur_Items;
    }
}
