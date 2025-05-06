using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Weapon_Preview_UI : MonoBehaviour
{
    [SerializeField] private Image weapon_Icon;
    [SerializeField] private TMP_Text comb_Name_Text, weapon_Name_Text, desc_Text;

    public void Show(Weapon_Data weapon)
    {
        weapon_Icon.sprite = weapon.weapon_Icon;
        comb_Name_Text.text = weapon.comb_Name;
        weapon_Name_Text.text= weapon.weapon_Name;
        
    }
}
