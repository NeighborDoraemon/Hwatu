using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Stat_Npc_Controller : MonoBehaviour
{
    [SerializeField] private PlayerCharacter_Controller player;

    [Header("Stat UI")]
    [SerializeField] private Canvas Stat_Manage_Can;
    [SerializeField] private Image[] stat_Buttons;

    [Header("Stat Value")]
    public int health_Inc_Value = 10;
    public int atkDmg_Inc_Value = 1;
    public float moveSpeed_Inc_Value = 1.0f;
    public float critRate_Inc_Value = 0.1f;
    public float critDmg_Inc_Value = 0.1f;

    private int cur_Index = 0;
    private bool is_StatUI_Open = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
    }

    public void UI_Start()
    {
        if (Stat_Manage_Can != null)
        {
            Stat_Manage_Can.gameObject.SetActive(true);
        }

        is_StatUI_Open = true;
        player.is_UI_Open = true;
        player.is_StatUI_Visible = true;
        cur_Index = 0;
        Update_Select_Border();
    }

    public void Exit_UI()
    {
        if (!is_StatUI_Open) return;

        if (Stat_Manage_Can != null)
        {
            Stat_Manage_Can.gameObject.SetActive(false);
        }

        is_StatUI_Open = false;
        player.is_UI_Open = false;
        player.is_StatUI_Visible = false;
    }

    public void Navigate_Stats(Vector2 input_Dir)
    {
        if (!is_StatUI_Open) return;

        float x = input_Dir.x;
        if (Mathf.Abs(x) < 0.5f) return;

        int direction = (x > 0) ? 1 : -1;
        cur_Index += direction;

        if (cur_Index < 0)
            cur_Index = stat_Buttons.Length - 1;
        else if (cur_Index >= stat_Buttons.Length)
            cur_Index = 0;

        Update_Select_Border();
    }

    public void Confirm_Selection()
    {
        if (!is_StatUI_Open) return;

        if (cur_Index == stat_Buttons.Length - 1)
        {
            Exit_UI();
            return;
        }

        Image selected_Button = stat_Buttons[cur_Index];
        string button_Name = selected_Button.gameObject.name;

        switch (button_Name)
        {
            case "Stat_Health":
                player?.Increase_Health(health_Inc_Value);
                break;
            case "Stat_MoveSpeed":
                player?.Increase_MoveSpeed(moveSpeed_Inc_Value);
                break;
            case "Stat_AttackDamage":
                player?.Increase_AttackDamage(atkDmg_Inc_Value);
                break;
            case "Stat_CritRate":
                player?.Increase_CritRate(critRate_Inc_Value);
                break;
            case "Stat_CritDamage":
                player?.Increase_CritDamage(critDmg_Inc_Value);
                break;
            default:
                Debug.Log($"[{button_Name}] is null");
                break;
        }
    }

    private void Update_Select_Border()
    {
        for (int i = 0; i < stat_Buttons.Length; i++)
        {
            Transform border = stat_Buttons[i].transform.Find("Select_Border");
            if (border != null)
            {
                border.gameObject.SetActive(i == cur_Index);
            }
        }
    }

    public bool Is_StatUI_Open()
    {
        return is_StatUI_Open;
    }
}
