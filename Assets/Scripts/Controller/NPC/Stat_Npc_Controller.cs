using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum Stat_Kind { Attack, Health, AttackSpeed, MoveSpeed, Exit }

[System.Serializable]
public class Stat_Desc_Profile
{
    public string title;

    [TextArea] public string[] cur_Desc_Per_Phase = new string[3];
    [TextArea] public string[] next_Desc_Per_Phase = new string[3];

    public int[] token_Cost_Per_Phase = new int[3] { 1, 1, 1 };
}

public class Stat_Npc_Controller : MonoBehaviour, Npc_Interface
{
    [SerializeField] private PlayerCharacter_Controller player;
    [SerializeField] private Canvas main_Can;

    [Header("Stat UI")]
    [SerializeField] private Canvas Stat_Manage_Can;
    [SerializeField] private Transform stat_Window_Panel;
    [SerializeField] private Image[] stat_Buttons;
    [SerializeField] private TextMeshProUGUI stat_Desc_Text;

    [Header("Stat Value")]
    public int health_Inc_Value = 10;
    public int atkDmg_Inc_Value = 1;
    public float moveSpeed_Inc_Value = 1.0f;
    public float critRate_Inc_Value = 0.1f;
    public float critDmg_Inc_Value = 0.1f;

    [Header("Upgrade Token")]
    [SerializeField] private int token_Cost = 1;

    [Header("Description Text")]
    [SerializeField] private Stat_Desc_Profile attack_Profile;
    [SerializeField] private Stat_Desc_Profile health_Profile;
    [SerializeField] private Stat_Desc_Profile atkSpeed_Profile;
    [SerializeField] private Stat_Desc_Profile moveSpeed_Profile;

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
        Update_Stat_Description(GetKind_By_Index(cur_Index));
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

    private Stat_Kind GetKind_By_Index(int index)
    {
        var binder = stat_Buttons[index]?.GetComponent<Stat_Button_Binder>();
        return binder ? binder.kind : Stat_Kind.Exit;
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
        Update_Stat_Description(GetKind_By_Index(cur_Index));
    }

    public void On_Stat_Hover(int index)
    {
        if (!is_StatUI_Open) return;

        Transform prev = stat_Buttons[cur_Index].transform.Find("Select_Border");
        if (prev != null) { prev.gameObject.SetActive(false); }

        cur_Index = index;
        Transform next = stat_Buttons[cur_Index].transform.Find("Select_Border");
        if (next != null) { next.gameObject.SetActive(true); }

        Update_Stat_Description(GetKind_By_Index(cur_Index));
    }

    public void Select_Stat_By_Index(int index)
    {
        cur_Index = index;
        Confirm_Selection();
        Update_Stat_Description(GetKind_By_Index(cur_Index));
    }

    public void Confirm_Selection()
    {
        if (!is_StatUI_Open) return;

        if (cur_Index == stat_Buttons.Length - 1)
        {
            Exit_UI();
            return;
        }

        if (player == null)
        {
            Debug.LogError("[Stat Npc] Player is null");
            return;
        }

        if (player.i_Token < token_Cost)
        {
            Debug.LogWarning("[Stat Npc] Player has no token");
            return;
        }

        player.i_Token -= token_Cost;

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

        player.token_Text.text = player.i_Token.ToString();
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

    private Stat_Desc_Profile Get_Profile(Stat_Kind kind) => kind switch
    {
        Stat_Kind.Attack => attack_Profile,
        Stat_Kind.Health => health_Profile,
        Stat_Kind.AttackSpeed => atkSpeed_Profile,
        Stat_Kind.MoveSpeed => moveSpeed_Profile,
        _ => null
    };

    private int Get_Cur_Phase(Stat_Kind kind)
    {
        return kind switch
        {
            Stat_Kind.Attack => player?.cur_AttackInc_Phase ?? 0,
            Stat_Kind.Health => player?.cur_HealthInc_Phase ?? 0,
            Stat_Kind.AttackSpeed => player?.cur_AttackCoolTimeInc_Phase ?? 0,
            Stat_Kind.MoveSpeed => player?.cur_MoveSpeedInc_Phase ?? 0,
            _ => 0
        };
    }

    private void Update_Stat_Description(Stat_Kind kind)
    {
        if (!stat_Desc_Text) return;
        if (kind == Stat_Kind.Exit) { stat_Desc_Text.text = "강화를 종료합니다."; return; }

        var prof = Get_Profile(kind);
        if (prof == null || player == null) { stat_Desc_Text.text = ""; return; }

        int cur = Mathf.Clamp(Get_Cur_Phase(kind), 0, 3);
        int next = cur + 1;

        bool isMax = cur >= 3;
        string cur_Line = cur >= 1 && cur <= 3 && !string.IsNullOrEmpty(SafeGet(prof.cur_Desc_Per_Phase, cur - 1))
            ? $"현재 효과: {prof.cur_Desc_Per_Phase[cur - 1]}"
            : "";

        string next_Line = isMax
            ? "다음 효과: 최대 단계입니다."
            : $"다음효과(단계 {next}): {SafeGet(prof.next_Desc_Per_Phase, next - 1)}";

        //int need_Token = !isMax ? SafeGet(prof.token_Cost_Per_Phase, next-1, defalutValue: token_Cost) : 0;
        //string cost_Line = isMax ? "" : $"필요 재화: 깃발 {need_Token}개";

        stat_Desc_Text.text =
            $"<b>{prof.title}</b>\n" +
            $"현재 단계: {cur}/3\n" +
            $"{cur_Line}\n{next_Line}";
    }

    private static string SafeGet(string[] arr, int idx)
    {
        if (arr == null || idx < 0 || idx >= arr.Length) return "";
        return arr[idx] ?? "";
    }
    private static int SafeGet(int[] arr, int idx, int defalutValue = 1)
    {
        if (arr == null || idx < 0 || idx >= arr.Length) return defalutValue;
        return arr[idx];
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
