using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class Item_Preview_UI : MonoBehaviour
{
    [Header("Preview Panel")]
    public GameObject preview_Panel;
    [SerializeField] Image item_Icon;
    [SerializeField] TMP_Text name_Text, desc_Text;

    private void Awake()
    {
        preview_Panel.SetActive(false);
    }

    public void Show(Item item)
    {
        item_Icon.sprite = item.item_Icon;
        name_Text.text = item.itemName;
        desc_Text.text = item.item_Description;
        preview_Panel.SetActive(true);
    }

    public void Hide() => preview_Panel.SetActive(false);
}
