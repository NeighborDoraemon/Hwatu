using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card_Preview_UI : MonoBehaviour
{
    [Header("Preview Panel Object")]
    [SerializeField] private GameObject preview_Panel;

    [Header("Child Panels")]
    [SerializeField] private Weapon_Preview_UI child_Panel_First;
    [SerializeField] private Weapon_Preview_UI child_Panel_Second;

    private void Awake()
    {
        preview_Panel.SetActive(false);
    }

    public void Show(Weapon_Data w1, Weapon_Data w2)
    {
        preview_Panel.SetActive(true);
        child_Panel_First.Show(w1);
        child_Panel_Second.Show(w2);
    }

    public void Hide()
    {
        preview_Panel.SetActive(false);
    }
}
