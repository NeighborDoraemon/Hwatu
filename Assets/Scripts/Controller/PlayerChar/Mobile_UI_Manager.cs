using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Mobile_UI_Manager : MonoBehaviour
{
    [Header("플레이어 스크립트")]
    [SerializeField] private PlayerCharacter_Controller player;

    [Header("기본 조작 버튼")]
    [SerializeField] private Button btn_Attack;
    [SerializeField] private Button btn_Left;
    [SerializeField] private Button btn_Right;
    [SerializeField] private Button btn_Down;
    [SerializeField] private Button btn_Jump;
    [SerializeField] private Button btn_Teleport;
    [SerializeField] private Button btn_Skill;

    [Header("카드 슬롯 버튼")]
    [SerializeField] private Button btn_CardSlot_First;
    [SerializeField] private Button btn_CardSlot_Second;

    [Header("인벤토리 및 설정 버튼")]
    [SerializeField] private Button btn_Inventory;
    [SerializeField] private Button btn_Settings;

    [Header("공격 및 상호작용 아이콘")]
    [SerializeField] private Sprite icon_Attack;
    [SerializeField] private Sprite icon_Interact;

    EventTrigger attack_Trigger, left_Trigger, right_Trigger, down_Trigger;

    private bool can_Interact = false;
    private bool is_Changing = false;

    private void Awake()
    {
        // 공격 버튼 할당
        btn_Attack.onClick.AddListener(player.On_Attack_Button_Click);
        attack_Trigger = Ensure_Event_Trigger(btn_Attack.gameObject);
        Add_Trigger(attack_Trigger, EventTriggerType.PointerDown, _ => player.On_Attack_Button_Down());
        Add_Trigger(attack_Trigger, EventTriggerType.PointerUp, _ => player.On_Attack_Button_Up());

        // 좌우 이동 할당
        left_Trigger = Ensure_Event_Trigger(btn_Left.gameObject);
        right_Trigger = Ensure_Event_Trigger(btn_Right.gameObject);
        Add_Trigger(left_Trigger, EventTriggerType.PointerDown, _ => player.On_Move_Input(Vector2.left));
        Add_Trigger(left_Trigger, EventTriggerType.PointerUp, _ => player.On_Move_Stop());
        Add_Trigger(right_Trigger, EventTriggerType.PointerDown, _ => player.On_Move_Input(Vector2.right));
        Add_Trigger(right_Trigger, EventTriggerType.PointerUp, _ => player.On_Move_Stop());

        // 점프 할당
        btn_Jump.onClick.AddListener(player.On_Jump_Button);
        // 텔레포트 할당
        btn_Teleport.onClick.AddListener(player.On_Teleport_Button);

        // 아래 방향 입력 할당
        down_Trigger = Ensure_Event_Trigger(btn_Down.gameObject);
        Add_Trigger(down_Trigger, EventTriggerType.PointerDown, _ => player.On_Down_Button_Down());
        Add_Trigger(down_Trigger, EventTriggerType.PointerUp, _ => player.On_Down_Button_Up());

        // 플레이어의 이벤트 구독
        player.On_Interactable_Changed += Handle_Interactable_Changed;
        player.On_ItemChange_State_Changed += Handle_ItemChange_StateChanged;

        // 카드 슬롯 버튼 할당
        btn_CardSlot_First.onClick.AddListener(player.On_Change_FirstCard_Button);
        btn_CardSlot_Second.onClick.AddListener(player.On_Change_SecondCard_Button);

        // 스킬 버튼 할당
        btn_Skill.onClick.AddListener(player.On_Skill_Button);

        // 인벤토리 토글 버튼 할당
        btn_Inventory.onClick.AddListener(player.Toggle_Inventory);
        // 설정 버튼 할당
        btn_Settings.onClick.AddListener(() =>
        {
            Debug.Log("설정 버튼 클릭됨.");
            player.Open_Settings();
        });
    }

    private void Start()
    {
        Handle_Interactable_Changed(false);
    }

    private void Add_Trigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private EventTrigger Ensure_Event_Trigger(GameObject go)
    {
        var trigger = go.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = go.AddComponent<EventTrigger>();
        return trigger;
    }

    private void Handle_Interactable_Changed(bool can_Interact_Now)
    {
        can_Interact = can_Interact_Now;
        Refresh_AttackButton();
    }

    private void Handle_ItemChange_StateChanged(bool is_Now_Changing)
    {
        is_Changing = is_Now_Changing;
        Refresh_AttackButton();
    }

    private void Refresh_AttackButton()
    {
        btn_Attack.onClick.RemoveAllListeners();

        if (is_Changing || can_Interact)
        {
            btn_Attack.onClick.AddListener(player.On_Interact_Button);
            btn_Attack.image.sprite = icon_Interact;
        }
        else
        {
            btn_Attack.onClick.AddListener(player.On_Attack_Button_Click);
            btn_Attack.image.sprite = icon_Attack;
        }
    }

    private void OnDestroy()
    {
        btn_Attack.onClick.RemoveListener(player.On_Attack_Button_Click);
        btn_Jump.onClick.RemoveListener(player.On_Jump_Button);
        btn_Teleport.onClick.RemoveListener(player.On_Teleport_Button);
        btn_CardSlot_First.onClick.RemoveListener(player.On_Change_FirstCard_Button);
        btn_CardSlot_Second.onClick.RemoveListener(player.On_Change_SecondCard_Button);
        btn_Skill.onClick.RemoveListener(player.On_Skill_Button);
        btn_Inventory.onClick.RemoveListener(player.Toggle_Inventory);
        btn_Settings.onClick.RemoveListener(player.Open_Settings);

        attack_Trigger.triggers.Clear();
        left_Trigger.triggers.Clear();
        right_Trigger.triggers.Clear();
        down_Trigger.triggers.Clear();

        player.On_Interactable_Changed -= Handle_Interactable_Changed;
        player.On_ItemChange_State_Changed -= Handle_ItemChange_StateChanged;
    }
}
