using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Description : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI item_Name_Text;
    [SerializeField] private TextMeshProUGUI item_Description_Text;

    public void Show_Item_Info(Item item)
    {
        if (item != null)
        {
            if (item_Name_Text != null)
            {
                item_Name_Text.text = item.itemName;
            }

            if (item_Description_Text != null)
            {
                item_Description_Text.text = item.item_Description;
            }
        }
        else
        {
            if (item_Name_Text != null)
            {
                item_Name_Text.text = "";
            }

            if (item_Description_Text != null)
            {
                item_Description_Text.text = "";
            }
        }
    }
}
