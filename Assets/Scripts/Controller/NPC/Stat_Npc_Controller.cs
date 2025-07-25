using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Stat_Npc_Controller : MonoBehaviour, Npc_Interface
{
    [SerializeField] private PlayerCharacter_Controller player;
    [SerializeField] private Canvas main_Can;

    [Header("Stat UI")]
    [SerializeField] private Canvas Stat_Manage_Can;
    [SerializeField] private Transform stat_Window_Panel;
    [SerializeField] private Image[] stat_Buttons;

    [Header("Stat Value")]
    public int health_Inc_Value = 10;
    public int atkDmg_Inc_Value = 1;
    public float moveSpeed_Inc_Value = 1.0f;
    public float critRate_Inc_Value = 0.1f;
    public float critDmg_Inc_Value = 0.1f;

    [Header("Attack_Dmg_Image")]


    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_start;
    private bool Dialogue_Once = false;

    private int cur_Index = 0;
    private bool is_StatUI_Open = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        Dialogue_Once = false;

        for (int i = 0; i < stat_Buttons.Length; i++)
        {
            var btn = stat_Buttons[i].gameObject;
            var hover = btn.GetComponent<Stat_Button_Hover>()
                ?? btn.AddComponent<Stat_Button_Hover>();
            hover.stat_Con = this;
            hover.index = i;
        }
    }

    public void UI_Start()
    {
        if (main_Can != null)
        {
            main_Can.gameObject.SetActive(false);
        }

        if (Stat_Manage_Can != null)
        {
            Stat_Manage_Can.gameObject.SetActive(true);
        }

        is_StatUI_Open = true;
        player.State_Change(PlayerCharacter_Controller.Player_State.UI_Open);
        player.is_StatUI_Visible = true;
        Time.timeScale = 0.0f;
        cur_Index = 0;
        Update_Select_Border();
    }

    public void Exit_UI()
    {
        if (!is_StatUI_Open) return;

        if (main_Can != null)
        {
            main_Can.gameObject.SetActive(true);
        }

        if (Stat_Manage_Can != null)
        {
            Stat_Manage_Can.gameObject.SetActive(false);
        }

        is_StatUI_Open = false;
        player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
        player.is_StatUI_Visible = false;
        Time.timeScale = 1.0f;
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

    public void On_Stat_Hover(int index)
    {
        if (!is_StatUI_Open) return;

        Transform prev = stat_Buttons[cur_Index].transform.Find("Select_Border");
        if (prev != null) { prev.gameObject.SetActive(false); }

        cur_Index = index;
        Transform next = stat_Buttons[cur_Index].transform.Find("Select_Border");
        if (next != null) { next.gameObject.SetActive(true); }
    }

    public void Select_Stat_By_Index(int index)
    {
        cur_Index = index;
        Confirm_Selection();
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
        string stat_Name = selected_Button.gameObject.name;

        switch (stat_Name)
        {
            case "Stat_AttackDamage":
                player?.Increase_AttackDamage();
                Update_Stat_Image(stat_Name, player.cur_AttackInc_Phase);
                break;
            case "Stat_Health":
                int old_Phase = player.cur_HealthInc_Phase;
                player?.Increase_Health();
                int phase_To_Show = old_Phase < player.cur_HealthInc_Phase
                                        ? player.cur_HealthInc_Phase : old_Phase;
                Update_Stat_Image(stat_Name, phase_To_Show);
                break;
            case "Stat_AttacSpeed":
                player?.Increase_AttackCoolTime();
                Update_Stat_Image(stat_Name, player.cur_AttackCoolTimeInc_Phase);
                break;
            case "Stat_MoveSpeed":
                player?.Increase_MoveSpeed();
                Update_Stat_Image(stat_Name, player.cur_MoveSpeedInc_Phase);
                break;
            default:
                Debug.Log($"[{stat_Name}] is null");
                break;
        }
    }

    private void Update_Stat_Image(string stat_Name, int phase)
    {
        Transform stat_Group = stat_Window_Panel.Find(stat_Name);
        if (stat_Group == null) return;
        
        var images = stat_Group.GetComponentsInChildren<Image>(true);

        foreach (var img in images)
        {
            if (img.name.StartsWith("First_Image"))
                img.gameObject.SetActive(phase >= 1);
            else if (img.name.StartsWith("Second_Image"))
                img.gameObject.SetActive(phase >= 2);
            else if (img.name.StartsWith("Third_Image"))
                img.gameObject.SetActive(phase >= 3);
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

    //Interface Method=====================================================================

    public void Npc_Interaction_Start()
    {
        if (player != null && !Dialogue_Once)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
            Dialogue_Manager.instance.Start_Dialogue(Interaction_start);
        }
        else if (player != null && Dialogue_Once)
        {
            Npc_Interaction_End();
        }
    }
    public void Event_Start()   //Not used
    {
    }
    public void Npc_Interaction_End()
    {
        if (player != null)
        {
            if(!Dialogue_Once)
            {
                player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
                player.Event_State_Change(PlayerCharacter_Controller.Event_State.None);
                Dialogue_Once = true;
            }
            UI_Start();
        }
    }

    public void Event_Move(InputAction.CallbackContext ctx) //Not used
    {
    }
    public void Event_Move_Direction(Vector2 dir) //Not used
    {
    }
    public void Event_Attack(InputAction.CallbackContext ctx)   //Not used
    {
    }
}
