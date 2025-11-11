using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Mathematics;
using System.Linq;
using System.Xml.Schema;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using UnityEngine.Windows;
//using UnityEngine.UIElements;

// PlayerCharacter_Controller Created By JBJ, KYH
public class PlayerCharacter_Controller : PlayerChar_Inventory_Manager, ISaveable
{
    private Player_InputActions inputActions;
    private bool isInventory_Visible = false;
    [HideInInspector] public bool is_StatUI_Visible = false;

    public Rigidbody2D rb;
    GameObject current_Item;
    [SerializeField] private Animator player_Effect_Animator;

    public Vector2 movement = new Vector2();
    public bool isMoving;
    [HideInInspector] public int jumpCount = 0;
    public int maxJumpCount = 2;

    [Header("Teleport")]
    bool canTeleporting = true;
    private int cur_Teleport_Count;
    public float teleporting_Cooltime_Timer;

    [Header("DebugChest")] public GameObject chestPrefab;

    public Transform spawnPoint;
    /*[HideInInspector]*/ public bool isGrounded;
    [HideInInspector] private bool has_Jumped;
    [HideInInspector] public bool canAttack = true;
    [HideInInspector] public bool can_JumpAtk = true;
    private float combo_Deadline;

    [Header("Ground Check")]
    public Transform ground_Check_Point;
    public float ground_Check_Radius = 0.1f;
    public LayerMask ground_Layer;

    [Header("Cinemachine")]
    private Camera_Manager camera_Manager;
    private Collider2D cur_Cinemachine_Collider;

    [Header("Weapon_Data")]
    public GameObject weapon_Prefab;    
    public SpriteRenderer effect_Render;
    public Animator effect_Animator;
    public Transform weapon_Anchor;
    public Transform effect_Anchor;
    public bool is_Facing_Right = true;
    public Animator weapon_Animator;
    public LayerMask enemy_LayerMask;
    private Weapon_Collision_Handler weapon_Handler;
    private bool is_AtkCoroutine_Running = false;
    private float last_Combo_End_Time = -1.0f;
    [SerializeField] private float combo_Input_Lock = 0.05f;

    [Header("Audio/Sound")]
    [SerializeField] private PlayerChar_Audio_Proxy audio_Proxy;
    [SerializeField] private float base_Step_Interval = 0.45f;
    [SerializeField] private float min_Step_Interval = 0.18f;
    [SerializeField] private float footstep_MinSpeed = 0.05f;
    [SerializeField, Range(0.25f, 1.0f)]
    private float footstep_Interval_Scale = 0.75f;

    private float footstep_Timer;

    public enum Effect_Channel { Normal, Skill }

    [System.Serializable]
    public class Effect_Channel_Refs
    {
        public SpriteRenderer render;
        public Animator animator;
        public Transform root => render != null ? render.transform : null;
    }

    [Header("Weapon Effect Channels")]
    public Effect_Channel_Refs normal_Effect;
    public Effect_Channel_Refs skill_Effect;

    private Coroutine normal_Reset_CR;
    private Coroutine skill_Reset_CR;

    public event Action On_Player_Damaged;
    public event Action On_Enemy_Hit;
    public event Action On_Enemy_Killed;
    public event Action<PlayerCharacter_Controller> On_Teleport;
    public event Action On_Player_Use_Skill;
    public event System.Action<bool> On_Interactable_Changed;
    public event System.Action<bool> On_ItemChange_State_Changed;
    [HideInInspector] public bool isInvincible = false;
    private bool is_Near_Interactable = false;
    private HashSet<Collider2D> interactables = new HashSet<Collider2D>();

    private Item pending_SwapItem = null;
    //[HideInInspector] public bool is_UI_Open = false;
    [HideInInspector] public bool is_Item_Change = false;

    [SerializeField] private float card_Change_Cooldown = 2.0f;
    [HideInInspector] public bool can_Card_Change = true;

    [HideInInspector] public bool has_Cleared_Map = false;

    //--------------------------------------------------- Created By KYH
    [Header("Player UI")]
    [SerializeField] private Image Player_Health_Bar;
    [SerializeField] private Pause_Manager pause_Manager;
    [SerializeField] private SpriteRenderer player_render;

    [SerializeField] private Item_Preview_UI item_Preview_UI;
    [SerializeField] private Card_Preview_UI card_Preview_UI;

    [Header("Skill Cooltime UI")]
    public Image skill_Icon_Image;
    [SerializeField] private Image skill_Cooldown_Overlay;
    [SerializeField] private Image attack_Cooldown_Overlay;
    [SerializeField] private Image teleport_Cooldown_Overlay;
    private float teleport_Cooldown_Duration => teleporting_CoolTime * teleport_Cooltime_Mul;
    private bool is_Skill_Coolingdown = false;
    //public Color ready_Color = Color.blue;
    //public Color cooldown_Color = Color.red;
    
    [Header("Map_Manager")]
    [SerializeField] private Map_Manager map_Manager;
    public bool use_Portal = false;
    
    //platform & Collider
    private GameObject current_Platform;
    public Collider2D player_Platform_Collider;
    private GameObject Now_New_Platform;
    private bool is_Down_Performed = false;

    private List<GameObject> OneWays = new List<GameObject>();
    private List<GameObject> Platforms = new List<GameObject>();

    private bool is_Player_Dead = false;

    //NPC
    private bool is_Npc_Contack = false;
    private GameObject Now_Contact_Npc;

    //match_manager
    [SerializeField] private Match_Up_Manager match_manager;

    //Escape Minigame Values
    private int require_gauge = 100;
    private int decrease_rate = 5;
    private int increase_rate = 10;

    private int current_gauge = 0;
    private bool is_Minigame = false;

    [HideInInspector] public bool is_Knock_Back = false;

    //for Escape
    [HideInInspector] public bool is_Key_Setting = false;
    [SerializeField] private Canvas Key_Setting_Can;

    public enum Player_State
    {
        Normal,
        UI_Open,
        Dialogue,
        Dialogue_Choice,
        Event_Doing,
        Trap_Minigame,
        Player_Dead
    }

    public enum Event_State
    {
        None,
        Bird_Hunting
    }

    private Player_State Current_Player_State;
    private Event_State Current_Event_State;

    [SerializeField] private Stat_Panel_Object stat_Object;

    //---------------------------------------------------
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputActions = new Player_InputActions();
        
        inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();

        Set_Weapon(0);

        skill_Cooldown_Overlay.fillAmount = 0.0f;
        skill_Cooldown_Overlay.enabled = false;

        if (!audio_Proxy) audio_Proxy = GetComponent<PlayerChar_Audio_Proxy>();

        Current_Player_State = Player_State.Normal; // 플레이어 현재상태 초기화 KYH
        Current_Event_State = Event_State.None; //이벤트 상태 초기화
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        inputActions.Player.Inventory.started += OnInventory_Pressed;
        inputActions.Player.Inventory.canceled += OnInventory_Released;
        inputActions.Player.Enable();
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        inputActions.Player.Inventory.started -= OnInventory_Pressed;
        inputActions.Player.Inventory.canceled -= OnInventory_Released;
        inputActions.Player.Disable();
    }

    protected override void Start()
    {
        Save_Manager.Instance.Register(this);

        base.Start();

        camera_Manager = FindObjectOfType<Camera_Manager>();
        sprite_Renderer = GetComponent<SpriteRenderer>();

        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Inventory Panel is Missing!");
        }

        teleporting_Cooltime_Timer = teleporting_CoolTime * teleport_Cooltime_Mul;

        Current_Player_State = Player_State.Normal; // 플레이어 현재상태 초기화 KYH

        
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1.0f && !is_Player_Dead)
        {
            var state = animator.GetCurrentAnimatorStateInfo(0);
            bool on_Attack_State = state.IsTag("Attack_1") ||
            state.IsTag("Attack_2") ||
            state.IsTag("Attack_3");

            if (on_Attack_State && isGrounded && !cur_Weapon_Data.is_HoldAttack_Enabled)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                Move();
            }

            Update_Animation_Parameters();
            HandleCombo();
            Handle_Teleportation_Time();
            Update_WeaponAnchor_Position();

            if (attack_Cooldown_Overlay.gameObject.activeSelf)
                Update_Attack_CooldownUI();
            Update_Teleport_CooldownUI();
        }
    }
    private void FixedUpdate()
    {
        Update_Skill_Cooldown_UI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var data = Save_Manager.Instance.Get(d => d);
        if (data.is_Inventory_Saved)
            Load_From_Save(data);

        Refresh_UI();
    }

    private void OnDestroy()
    {
        Save_Manager.Instance.Unregister(this);
    }

    public void Save(SaveData data)
    {
        if (!data.is_Inventory_Saved) return;

        // Card Save
        data.saved_Card_IDs.Clear();
        foreach (var card_Obj in card_Inventory)
        {
            if (card_Obj != null)
            {
                var month = card_Obj.GetComponent<Card>().cardValue.Month;
                data.saved_Card_IDs.Add(month);
            }
            else
            {
                data.saved_Card_IDs.Add(0);
            }
        }

        // Item Save
        data.saved_Item_IDs.Clear();
        var item_DB_List = Object_Manager.instance.item_Database.all_Items;
        for (int i = 0; i < player_Inventory.Count; i++)
        {
            Item item_SO = player_Inventory[i];
            int idx = item_DB_List.IndexOf(item_SO);
            if (idx < 0)
            {
                Debug.LogWarning($"[Item Save] 아이템 {item_SO.itemName}을 데이터베이스 내에서 찾을 수 없습니다.");
            }
            data.saved_Item_IDs.Add(idx);
        }

        // Money Save
        data.player_Money = i_Money;

        //Player Stat Save
        data.health_Ratio = (max_Health > 0) ? Mathf.Clamp01((float)health / max_Health) : 0.0f;

        data.attack_Phase = cur_AttackInc_Phase;
        data.health_Phase = cur_HealthInc_Phase;
        data.atk_Cooltime_Phase = cur_AttackCoolTimeInc_Phase;
        data.move_Phase = cur_MoveSpeedInc_Phase;

        data.inc_AttackDamage = cur_Inc_AttackDamage;
        data.inc_Health = cur_Inc_Health;
        data.inc_Damage_Reduction = cur_Inc_DamageReduction;
        data.dec_AttackCooltime = cur_Dec_AttackCoolTime;
        data.inc_MoveSpeed = cur_Inc_MoveSpeed;

        data.dmg_Inc_To_Lost_Health = dmg_Inc_To_Lost_Health;
        data.card_Match_Dmg_Inc = card_Match_Dmg_Inc;
        data.skill_Coomtime_Has_Dec = skill_Cooltime_Has_Dec;
        data.money_Earned_Has_Inc = money_Earned_Has_Inc;
        data.invisible_Teleport = invicible_Teleport;

        data.is_Inventory_Saved = true;
    }

    private void Load_From_Save(SaveData data)
    {
        if (!data.is_Inventory_Saved) return;

        // Organize existing card and items
        for (int i = 0; i < card_Inventory.Length; i++)
        {
            if (card_Inventory[i] != null)
                Destroy(card_Inventory[i]);
            card_Inventory[i] = null;
        }
        cardCount = 0;
        isCombDone = false;

        foreach (var old_Item in player_Inventory.ToList())
            Remove_Item_Effect(old_Item);
        player_Inventory.Clear();

        // Restore card
        for (int i =0;
            i < data.saved_Card_IDs.Count && i < card_Inventory.Length;
            i++)
        {
            int month = data.saved_Card_IDs[i];
            if (month <= 0)
                continue;

            var card_SO = Object_Manager.instance.card_Values
                .FirstOrDefault(cv => cv.Month == month);
            if (card_SO == null)
                continue;

            Sprite sprite = card_SO.GetRandomSprite();

            GameObject go = Instantiate(
                Object_Manager.instance.card_Prefab,
                transform
                );

            var sr = go.GetComponent<SpriteRenderer>();
            sr.sprite = sprite;

            var card_Comp = go.GetComponent<Card>();
            card_Comp.cardValue = card_SO;
            card_Comp.selected_Sprite = sprite;

            AddCard(go);
        }

        // Load money and stat
        i_Money = data.player_Money;
        if (money_Text) money_Text.text = i_Money.ToString();

        cur_AttackInc_Phase = data.attack_Phase;
        cur_HealthInc_Phase = data.health_Phase;
        cur_AttackCoolTimeInc_Phase = data.atk_Cooltime_Phase;
        cur_MoveSpeedInc_Phase = data.move_Phase;

        cur_Inc_AttackDamage = data.inc_AttackDamage;
        cur_Inc_Health = data.inc_Health;
        cur_Inc_DamageReduction = data.inc_Damage_Reduction;
        cur_Dec_AttackCoolTime = data.dec_AttackCooltime;
        cur_Inc_MoveSpeed = data.inc_MoveSpeed;

        dmg_Inc_To_Lost_Health = data.dmg_Inc_To_Lost_Health;
        card_Match_Dmg_Inc = data.card_Match_Dmg_Inc;
        skill_Cooltime_Has_Dec = data.skill_Coomtime_Has_Dec;
        money_Earned_Has_Inc = data.money_Earned_Has_Inc;
        invicible_Teleport = data.invisible_Teleport;

        Recalculate_Stat_Without_Items(data.health_Ratio);

        // Restore items
        var item_DB_List = Object_Manager.instance.item_Database.all_Items;
        foreach (int idx in data.saved_Item_IDs)
        {
            if (idx >= 0 && idx < item_DB_List.Count)
            {
                Item item_SO = item_DB_List[idx];
                AddItem(item_SO);
            }
            else
            {
                Debug.LogWarning($"[Item Load] 잘못된 아이템 인덱스: {idx}");
            }
        }

        Refresh_UI();
    }

    private void Recalculate_Stat_Without_Items(float saved_HealthRatio)
    {
        movementSpeed = base_MovementSpeed;
        jumpPower = base_JumpPower;
        max_Health = base_Max_Health;
        attackDamage = 0;
        skill_Damage = 0;

        damage_Mul = 1.0f;
        takenDamage_Mul = 1.0f;
        movementSpeed_Mul = 1.0f;

        attack_Cooltime_Mul = 1.0f;
        skill_Cooltime_Mul = 1.0f;

        damage_Reduce_Min = 0;
        damage_Reduce_Max = 0;

        heal_Amount_Mul = 1.0f;
        money_Earned_Mul = 1.0f;

        max_Teleport_Count = 1;

        attackDamage += cur_Inc_AttackDamage;

        max_Health += cur_Inc_Health;
        damage_Reduce_Min += cur_Inc_DamageReduction;
        damage_Reduce_Max += cur_Inc_DamageReduction;

        movementSpeed += cur_Inc_MoveSpeed;
        movementSpeed = (float)System.Math.Round(movementSpeed, 1);

        attack_Cooltime_Mul = Mathf.Round((1.0f - (cur_Dec_AttackCoolTime * 0.1f)) * 100.0f) / 100.0f;
        attack_Cooltime_Mul = Mathf.Max(0.1f, attack_Cooltime_Mul);

        if (skill_Cooltime_Has_Dec)
        {
            skill_Cooltime_Mul -= inhance_Skillcooltime_Value;
            skill_Cooltime_Mul = Mathf.Max(0.1f, skill_Cooltime_Mul);
        }

        if (cur_HealthInc_Phase >= 3)
        {
            heal_Amount_Mul += 0.2f;
            heal_Amount_Mul = Mathf.Round(heal_Amount_Mul * 100.0f) / 100.0f;
        }

        if (money_Earned_Has_Inc)
            money_Earned_Mul += 0.2f;

        if (cur_AttackCoolTimeInc_Phase >= 3)
            max_Teleport_Count = 2;

        int new_Health = Mathf.RoundToInt(max_Health * Mathf.Clamp01(saved_HealthRatio));
        health = Mathf.Clamp(new_Health, 0, max_Health);
    }

    void Update_Animation_Parameters()
    {
        isMoving = Mathf.Abs(movement.x) > 0.01f;
        animator.SetBool("isMove", isMoving);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("vertical_Velocity", rb.velocity.y);

        if (weapon_Animator != null)
        {
            weapon_Animator.SetBool("isMove", isMoving);
        }

        //if (isGrounded && rb.velocity.y == 0 && this.gameObject.transform.position.y - 0.3f > Now_New_Platform.transform.position.y)
        //{
        //    jumpCount = 0;
        //}
    }

    // Move ===========================================================================================
    public void Input_Move(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //State Check
        {
            if (ctx.phase == InputActionPhase.Canceled)
            {
                movement = Vector2.zero;
            }
            //else if (ctx.phase == InputActionPhase.Started)
            //{
            //    movement = ctx.action.ReadValue<Vector2>();

            //    if (is_Minigame)
            //    {
            //        current_gauge += increase_rate;
            //        Debug.Log("게이지 증가" + current_gauge);
            //    }
            //}
            else
            {
                movement = ctx.action.ReadValue<Vector2>();
            }
        }
        else if (Current_Player_State == Player_State.UI_Open)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                Vector2 input = ctx.ReadValue<Vector2>();
                if (isInventory_Visible)
                {
                    if (Mathf.Abs(input.x) > 0.1f)
                    {
                        Navigate_Inventory((int)input.x);
                    }
                }
                else if (is_StatUI_Visible)
                {
                    Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Navigate_Stats(input);
                }
            }
        }
        else if (Current_Player_State == Player_State.Event_Doing)
        {
            Now_Contact_Npc.GetComponent<Npc_Interface>().Event_Move(ctx);
        }
        else if (Current_Player_State == Player_State.Trap_Minigame)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                movement = ctx.action.ReadValue<Vector2>();

                if (is_Minigame)
                {
                    current_gauge += increase_rate;
                    Debug.Log("게이지 증가" + current_gauge);
                }
            }
        }
    }

    void Move()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        bool on_Attack_State = state.IsTag("Attack_1") ||
            state.IsTag("Attack_2") ||
            state.IsTag("Attack_3");

        bool hold_Attack = cur_Weapon_Data != null && cur_Weapon_Data.is_HoldAttack_Enabled;

        if (on_Attack_State && isGrounded && !hold_Attack)
        {
            return;
        }

        if (movement.x < 0)
        {
            if (is_Facing_Right)
            {
                is_Facing_Right = false;
                sprite_Renderer.flipX = true;
            }                        
        }
        else if (movement.x > 0)
        {
            if (!is_Facing_Right)
            {
                is_Facing_Right = true;
                sprite_Renderer.flipX = false;
            }            
        }

        if (!is_Knock_Back)
        {
            //Vector2 normalized_Movement = movement.normalized;
            //float total_Speed = movementSpeed * movementSpeed_Mul;
            //rb.velocity = new Vector2(normalized_Movement.x * total_Speed, rb.velocity.y);

            float speed = movementSpeed * movementSpeed_Mul;
            rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        }

        Try_PlayFootstep_ByTimer();
    }

    private void Try_PlayFootstep_ByTimer()
    {
        if (!isGrounded || is_Knock_Back || Time.timeScale == 0.0f) return;

        float abs_VX = Mathf.Abs(rb.velocity.x);
        if (abs_VX < footstep_MinSpeed) return;

        float nominal_Max = Mathf.Max(0.01f, movementSpeed * movementSpeed_Mul);
        float speed01 = Mathf.Clamp01(abs_VX / nominal_Max);
        float interval = Mathf.Lerp(base_Step_Interval, min_Step_Interval, speed01);

        footstep_Timer -= Time.deltaTime;
        if (footstep_Timer > 0.0f) return;

        audio_Proxy.Play_Footstep();
        footstep_Timer = interval;
    }

    public void On_Move_Input(Vector2 input)
    {
        switch (Current_Player_State)
        {
            case Player_State.Normal:
                movement = input;
                break;
            case Player_State.UI_Open:
                if (isInventory_Visible && Mathf.Abs(input.x) > 0.1f)
                    Navigate_Inventory((int)Mathf.Sign(input.x));
                else if (is_StatUI_Visible)
                    Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Navigate_Stats(input);
                break;
            case Player_State.Event_Doing:
                Now_Contact_Npc.GetComponent<Npc_Interface>()?.Event_Move_Direction(input);
                break;
            case Player_State.Trap_Minigame:
                movement = input;
                if (is_Minigame)
                {
                    current_gauge += increase_rate;
                    Debug.Log("게이지 증가" + current_gauge);
                }
                break;
            default:
                return;
        }
    }

    public void On_Move_Stop()
    {
        switch (Current_Player_State)
        {
            case Player_State.Normal:
            case Player_State.Trap_Minigame:
                movement = Vector2.zero;
                rb.velocity = new Vector2(0, rb.velocity.y);
                break;
            case Player_State.Event_Doing:
                Now_Contact_Npc.GetComponent<Npc_Interface>()?.Event_Move_Direction(Vector2.zero);
                break;
            case Player_State.UI_Open:
                return;
            default:
                return;
        }
    }

    public void Input_Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            On_Jump_Button();
        }
    }

    private void Do_Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);

        jumpCount++;
        has_Jumped = true;
        isGrounded = false;

        if (jumpCount == 1)
            player_Effect_Animator.SetTrigger("Jump_Effect");
        else if (jumpCount == 2)
            player_Effect_Animator.SetTrigger("DoubleJump_Effect");
    }

    public void On_Jump_Button()
    {
        if (Current_Player_State == Player_State.Normal
        && Time.timeScale == 1.0f
        && !is_Player_Dead)
        {
            if (is_Down_Performed)
            {
                if (current_Platform != null)
                    StartCoroutine(DisableCollision());
            }
            else
            {
                if (jumpCount < maxJumpCount)
                    Do_Jump();
            }
        }
        // 2) UI 열려 있는 상태 처리 (기존과 동일)
        else if (Current_Player_State == Player_State.UI_Open)
        {
            if (is_StatUI_Visible)
            {
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Exit_UI();
            }
            else if (isInventory_Visible && is_Item_Change && pending_SwapItem != null)
            {
                Vector2 spawn_Pos = new Vector2(transform.position.x, transform.position.y - 0.2f);
                Object_Manager.instance.Spawn_Specific_Item(spawn_Pos, pending_SwapItem);
                pending_SwapItem = null;
                HideInventory();
                Cancel_Item_Change();
                Time.timeScale = 1.0f;
            }
        }
    }
    
    public void Input_Teleportation(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
            On_Teleport_Button();
    }

    public void On_Teleport_Button()
    {
        if (Current_Player_State != Player_State.Normal
            || is_Player_Dead
            || !canTeleporting
            || Time.timeScale != 1.0f)
            return;

        Vector2 direction = movement.x > 0.1f ? Vector2.right
                            : movement.x < -0.1f ? Vector2.left
                            : is_Facing_Right ? Vector2.right
                                                : Vector2.left;
        float max_Dist = teleporting_Distance;
        float adjusted_Dist = max_Dist;

        int mask = LayerMask.GetMask("Walls", "Platform");
        var col = GetComponent<Collider2D>();
        Vector2 size = col.bounds.size;

        Vector2 start_Pos = transform.position;
        Vector2 origin = (Vector2)transform.position + col.offset;

        RaycastHit2D hit = Physics2D.BoxCast(
            origin,
            size,
            0.0f,
            direction,
            max_Dist,
            mask);

        if (hit.collider != null)
            adjusted_Dist = hit.distance;

        Vector2 end_Pos = start_Pos + direction * adjusted_Dist;
        if (has_VineAmulet_Effect && adjusted_Dist > 0.01f)
        {
            Vector2 mid = (start_Pos + end_Pos) * 0.5f;

            Vector2 box_Size = new Vector2(size.x + adjusted_Dist, size.y);

            float angle_Z = transform.eulerAngles.z;

            Collider2D[] enemies = Physics2D.OverlapBoxAll(mid, box_Size, angle_Z, enemy_LayerMask);

            if (enemies != null && enemies.Length > 0)
            {
                HashSet<Collider2D> visited = new HashSet<Collider2D>();
                foreach (var e_Col in enemies)
                {
                    if (e_Col == null || visited.Contains(e_Col)) continue;
                    visited.Add(e_Col);

                    var enemy = e_Col.GetComponent<Enemy_Basic>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(10);
                    }
                }
            }
        }

        transform.Translate(direction * adjusted_Dist);

        animator.SetTrigger("Teleport");
        cur_Teleport_Count--;
        if (cur_Teleport_Count <= 0) canTeleporting = false;
        On_Teleport?.Invoke(this);

        if (invicible_Teleport)
            StartCoroutine(Invicible_After_Teleport());
    }

    private IEnumerator Invicible_After_Teleport()
    {
        isInvincible = true;
        yield return new WaitForSeconds(teleport_Invicible_Time);
        isInvincible = false;
    }

    void Handle_Teleportation_Time()
    {
        if (cur_Teleport_Count < max_Teleport_Count)
        {
            teleporting_Cooltime_Timer -= Time.deltaTime;
            if (teleporting_Cooltime_Timer <= 0.0f)
            {
                cur_Teleport_Count++;
                if (cur_Teleport_Count > 0)
                {
                    canTeleporting = true;
                }

                teleporting_Cooltime_Timer = teleporting_CoolTime * teleport_Cooltime_Mul;
            }
        }
    }

    private void Update_Teleport_CooldownUI()
    {
        if (cur_Teleport_Count == 0)
        {
            float t = Mathf.Clamp01(teleporting_Cooltime_Timer / teleport_Cooldown_Duration);

            if (!teleport_Cooldown_Overlay.gameObject.activeSelf)
                teleport_Cooldown_Overlay.gameObject.SetActive(true);

            teleport_Cooldown_Overlay.fillAmount = t;
        }
        else
        {
            if (teleport_Cooldown_Overlay.gameObject.activeSelf)
                teleport_Cooldown_Overlay.gameObject.SetActive(false);
        }
    }
    // Created By KYH ---------------------------------------------------------------
    public void Input_Down_Jump(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)
        {
            if (ctx.phase == InputActionPhase.Performed && Time.timeScale == 1.0f)
            {
                is_Down_Performed = true;
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                is_Down_Performed = false;
            }
        }
    }

    public void On_Down_Button_Down()
    {
        is_Down_Performed = true;
    }

    public void On_Down_Button_Up()
    {
        is_Down_Performed = false;
    }
    // ----------------------------------------------------------------------------
    // ======================================================================================================

    // InterAction ==========================================================================================
    public void Input_Interaction(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)
        {
            if (ctx.phase != InputActionPhase.Started || is_Player_Dead)
                return;

            map_Manager.Use_Portal(false);
            Handle_Npc_Interaction();
            Handle_Item_Interaction();
        }
        else if (Current_Player_State == Player_State.UI_Open)
        {
            if (ctx.phase != InputActionPhase.Started)
                return;

            if (isInventory_Visible && is_Item_Change && pending_SwapItem != null)
            {
                SwapItem(pending_SwapItem);
                pending_SwapItem = null;
                is_Item_Change = false;
                HideInventory();
                Time.timeScale = 1.0f;
                return;
            }
            else if (is_StatUI_Visible)
            {
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Confirm_Selection();
                return;
            }
        }
        else if (Current_Player_State == Player_State.Dialogue)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                Dialogue_Manager.instance.Print_Next_Dialogue();
            }
        }
        else if (Current_Player_State == Player_State.Dialogue_Choice)
        {
            if (ctx.phase == InputActionPhase.Started)
            {
                Dialogue_Manager.instance.Chose_Complete();
            }
        }
    }

    public void On_Interact_Button()
    {
        if (Current_Player_State == Player_State.Normal)
        {
            map_Manager.Use_Portal(false);
            Handle_Npc_Interaction();
            Handle_Item_Interaction();
        }
        else if (Current_Player_State == Player_State.UI_Open)
        {
            if (isInventory_Visible && is_Item_Change && pending_SwapItem != null)
            {
                SwapItem(pending_SwapItem);
                pending_SwapItem = null;
                HideInventory();
                Complete_Item_Change();
                Time.timeScale = 1.0f;
                return;
            }
            else if (is_StatUI_Visible)
            {
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Confirm_Selection();
            }
        }
        else if (Current_Player_State == Player_State.Dialogue)
        {
            Dialogue_Manager.instance.Print_Next_Dialogue();
        }
        else if (Current_Player_State == Player_State.Dialogue_Choice)
        {
            Dialogue_Manager.instance.Chose_Complete();
        }
    }

    private void Add_Interactable(Collider2D col)
    {
        if (interactables.Add(col))
            On_Interactable_Changed?.Invoke(true);
    }

    private void Remove_Interactable(Collider2D col)
    {
        if (interactables.Remove(col))
            On_Interactable_Changed?.Invoke(interactables.Count > 0);
    }

    private void Update_Interactable_Flag(bool flag)
    {
        if (is_Near_Interactable == flag) return;
        is_Near_Interactable = flag;

    }

    private void Handle_Npc_Interaction()
    {
        if (!is_Npc_Contack || Now_Contact_Npc == null)
            return;

        switch (Now_Contact_Npc.gameObject.name)
        {
            //case "Stat_Npc":
            //    Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.UI_Start();
            //    break;

            //case "Start_Card_Npc":
            //    Now_Contact_Npc.GetComponent<Start_Card_Npc>()?.Request_Spawn_Cards();
            //    break;

            default:
                {
                    if (Time.timeScale != 0.0f)
                    {
                        Now_Contact_Npc.GetComponent<Npc_Interface>().Npc_Interaction_Start();
                        Dialogue_Manager.instance.Get_Npc_Data(Now_Contact_Npc);
                        movement = Vector2.zero;
                    }
                    break;
                }
        }        
    }
    private void Handle_Item_Interaction()
    {
        if (current_Item == null)
            return;

        if (current_Item.TryGetComponent(out Spawn_Box chest))
        {
            chest.Request_Spawn_Cards();
        }
        else if (current_Item.TryGetComponent(out Card_Spawn_Box debug_Chest))
        {
            debug_Chest.Request_Spawn_Cards();
        }
        else if (current_Item.CompareTag("Card"))
        {
            AddCard(current_Item);

            //by KYH
            if (card_Inventory[0] != null && card_Inventory[1] != null)
            {
                match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
            }

            current_Item.gameObject.SetActive(false);
        }
        else if (current_Item.TryGetComponent(out Item_Prefab item_Prefab))
        {
            if (current_Item.CompareTag("Market_Item"))  // By KYH - Market Item
            {
                Debug.Log("Market Item Interacted!");
                int current_price = current_Item.GetComponent<Item_Prefab>().itemData.item_Price;

                if (current_price <= i_Money) // Player's Money is more than price
                {
                    i_Money -= current_price;
                    Debug.Log(i_Money);

                    Handle_Item(item_Prefab.itemData);

                    Debug.Log("After Call");
                    Market_Destroy_Manager.On_Item_Destroyed?.Invoke(item_Prefab.gameObject);
                }
                else
                {
                    Debug.Log("Not Enough Money!");
                }
                Update_Player_Money();
            }
            else // Default Item 
            {
                Handle_Item(item_Prefab.itemData);
            }
        }

        if (!is_Item_Change)
        {
            current_Item = null;
        }
    }
     
    private void Handle_Item(Item item)
    {
        if (item == null) return;

        if (item.isConsumable)
        {
            item.ApplyEffect(this);
        }
        else
        {
            if (player_Inventory.Count < max_Inventory_Size)
            {
                AddItem(item);
            }
            else
            {
                pending_SwapItem = item;
                Shift_Selected_Item();
            }
        }

        Destroy(current_Item);
        Object_Manager.instance.Destroy_All_Cards_And_Items();
    }

    void Spawn_Chest() // Spawn Debuging Card Chest
    {
        Vector2 spawn_Pos = new Vector2(transform.position.x, transform.position.y - 0.2f);
        GameObject chest = Instantiate(chestPrefab, spawn_Pos, Quaternion.identity);
    }

    public void Input_Change_FirstCard(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)
        {
            if (ctx.phase != InputActionPhase.Started || Time.timeScale != 1.0f || is_Player_Dead
            || !can_Card_Change || isAttacking || card_Inventory[2] == null)
                return;

            if (attack_Strategy is Jangtae_Attack_Startegy JT_Strategy && JT_Strategy.isRiding)
            {
                can_Card_Change = false;
                return;
            }

            Change_FirstAndThird_Card();

            if (card_Inventory[0] != null && card_Inventory[1] != null) //Match Up Call
            {
                match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
                match_manager.Match_Reset();
                match_manager.Start_Match();
            }

            StartCoroutine(Card_Change_Cooldown_Routine());
        }
    }

    public void Input_Change_SecondCard(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)
        {
            if (ctx.phase != InputActionPhase.Started || Time.timeScale != 1.0f || is_Player_Dead
            || !can_Card_Change || isAttacking || card_Inventory[2] == null)
                return;

            if (attack_Strategy is Jangtae_Attack_Startegy JT_Strategy && JT_Strategy.isRiding)
            {
                can_Card_Change = false;
                return;
            }

            Change_SecondAndThird_Card();

            if (card_Inventory[0] != null && card_Inventory[1] != null) //Match Up Call
            {
                match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
                match_manager.Match_Reset();
                match_manager.Start_Match();
            }

            StartCoroutine(Card_Change_Cooldown_Routine());
        }
    }

    public void On_Change_FirstCard_Button()
    {
        if (Current_Player_State != Player_State.Normal ||
           Time.timeScale != 1.0f ||
           is_Player_Dead ||
           !can_Card_Change ||
           isAttacking ||
           card_Inventory[2] == null)
            return;

        if (attack_Strategy is Jangtae_Attack_Startegy jt && jt.isRiding)
        {
            can_Card_Change = false;
            return;
        }

        Change_FirstAndThird_Card();
        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
            match_manager.Match_Reset();
            match_manager.Start_Match();
        }
        StartCoroutine(Card_Change_Cooldown_Routine());
    }

    public void On_Change_SecondCard_Button()
    {
        if (Current_Player_State != Player_State.Normal ||
            Time.timeScale != 1.0f ||
            is_Player_Dead ||
            !can_Card_Change ||
            isAttacking ||
            card_Inventory[2] == null)
            return;

        if (attack_Strategy is Jangtae_Attack_Startegy jt && jt.isRiding)
        {
            can_Card_Change = false;
            return;
        }

        Change_SecondAndThird_Card();
        if (card_Inventory[0] != null && card_Inventory[1] != null)
        {
            match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
            match_manager.Match_Reset();
            match_manager.Start_Match();
        }
        StartCoroutine(Card_Change_Cooldown_Routine());
    }

    private IEnumerator Card_Change_Cooldown_Routine()
    {
        can_Card_Change = false;
        yield return new WaitForSeconds(card_Change_Cooldown);
        can_Card_Change = true;
    }

    // ======================================================================================================

    // Weapon ===============================================================================================
    public override void Set_Weapon(int weaponIndex)
    {
        base.Set_Weapon(weaponIndex);

        if (cur_Weapon_Data == null)
        {            
            return;
        }

        if (cur_Weapon_Data.effect_Data == null)
        {            
            return;
        }
        
        if (attack_Strategy != null)
        {
            attack_Strategy.Reset_Stats();
        }

        Check_Es_Stack();

        if (weapon_Anchor.childCount > 0)
        {
            foreach(Transform child in weapon_Anchor)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (cur_Weapon_Data.weapon_Prefab != null)
        {
            GameObject new_Weapon = Instantiate(cur_Weapon_Data.weapon_Prefab, weapon_Anchor);
            new_Weapon.transform.localPosition = Vector3.zero;
            new_Weapon.transform.localRotation = Quaternion.identity;

            weapon_Prefab = new_Weapon;

            weapon_Handler = new_Weapon.GetComponent<Weapon_Collision_Handler>();

            weapon_Animator = new_Weapon.GetComponent<Animator>();            
            if (weapon_Animator != null && cur_Weapon_Data.weapon_overrideController != null)
            {
                weapon_Animator.runtimeAnimatorController = cur_Weapon_Data.weapon_overrideController;
                weapon_Animator.SetTrigger("Change");
            }
        }

        attack_Strategy = cur_Weapon_Data.attack_Strategy as IAttack_Strategy;

        if (attack_Strategy == null)
        {
            Debug.LogError($"'{cur_Weapon_Data.weapon_Name}' Weapon Equipe");            
            return;
        }
        
        Apply_Weapon_Data();

        Re_Apply_All_Effects();

        GameObject clone_Obj = GameObject.FindWithTag("PlayerClone");
        if (clone_Obj != null)
        {
            Player_Clone player_Clone = clone_Obj.GetComponent<Player_Clone>();
            if (player_Clone != null)
            {
                player_Clone.Copy_Player_Weapon();
            }
        }

        if (cur_Weapon_Data.skill_Icon != null)
        {
            skill_Icon_Image.sprite = cur_Weapon_Data.skill_Icon;
        }
    }

    private void Apply_Weapon_Data()
    {
        if (cur_Weapon_Data == null) return;

        attack_Strategy = cur_Weapon_Data.attack_Strategy as IAttack_Strategy;

        if (cur_Weapon_Data.overrideController != null)
        {
            animator.runtimeAnimatorController = cur_Weapon_Data.overrideController;
        }
        attack_Strategy.Initialize(this, cur_Weapon_Data);

        animator.SetInteger("cur_Max_AtkCount", cur_Weapon_Data.max_Attack_Count);

        animator.SetTrigger("Change_Weapon");
    }

    public void Check_Es_Stack()
    {
        if (!Has_Three_And_ThreeG())
        {
            es_Stack = 0;

            if (has_Es_Extra_Life)
            {
                if (player_Life > 0)
                {
                    player_Life -= 1;
                }
                
                has_Es_Extra_Life = false;
            }
        }
    }

    void Update_WeaponAnchor_Position()
    {
        if (cur_Weapon_Data == null)
        {
            Debug.LogWarning("cur_Weapon_Data is null.");
            return;
        }

        if (cur_Weapon_Data.animation_Pos_Data_List == null)
        {
            Debug.LogWarning("animation_Pos_Data_List is null.");
            return;
        }

        if (cur_Weapon_Data.animation_Pos_Data_List.Count == 0)
        {
            //Debug.LogWarning("animation_Pos_Data_List is empty.");
            return;
        }

        if (animator.IsInTransition(0))
            return;

        AnimatorStateInfo state_Info = animator.GetCurrentAnimatorStateInfo(0);
        string cur_Animation_Name = Get_Cur_Animation_Name(animator);

        var animation_Data = cur_Weapon_Data.animation_Pos_Data_List.Find(
            data => string.Equals(data.animation_Name, cur_Animation_Name, StringComparison.OrdinalIgnoreCase));

        if (animation_Data != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                AnimationClip cur_Clip = clipInfo[0].clip;
                int total_Frames = Mathf.RoundToInt(cur_Clip.length * cur_Clip.frameRate);

                float norm_Time = state_Info.normalizedTime;

                if (cur_Clip.isLooping)
                {
                    norm_Time = norm_Time % 1.0f;
                }
                else
                {
                    norm_Time = Mathf.Clamp01(norm_Time);
                }

                int cur_Frame = Mathf.FloorToInt(norm_Time * total_Frames);
                cur_Frame = Mathf.Clamp(cur_Frame, 0, total_Frames - 1);

                Vector3 new_Pos = animation_Data.Get_Position(cur_Frame);
                Quaternion new_Rotation = animation_Data.Get_Rotation(cur_Frame);
                Vector3 new_Scale = animation_Data.Get_Scale(cur_Frame);
                
                weapon_Anchor.localPosition = is_Facing_Right ? new_Pos : new Vector3(-new_Pos.x, new_Pos.y, new_Pos.z);
                weapon_Anchor.localRotation = is_Facing_Right ? new_Rotation : Quaternion.Inverse(new_Rotation);
                weapon_Anchor.localScale = is_Facing_Right ?
                    new_Scale : new Vector3(-new_Scale.x, new_Scale.y, new_Scale.z);
            }
        }
    }
    string Get_Cur_Animation_Name(Animator animator)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            return clipInfo[0].clip.name;
        }
        return "";
    }

    public void Activate_WeaponCollider(float duration)
    {
        //Debug.Log("Activate_WeaponCollider called");
        if (weapon_Handler != null)
        {
            weapon_Handler.Enable_Collider(duration);
        }
    }

    private static bool Has_Frame(Weapon_Effect_Data data, string motion, int frame)
    {
        if (data == null) return false;
        var effect_Info = data.Get_Effect_Info(motion);
        if (effect_Info == null || effect_Info.frame_Effects == null) return false;
        return effect_Info.frame_Effects.Exists(fe => fe.frame_Number == frame);
    }

    public void Show_Normal_Effect(string motion_Name_And_Frame)
    {
        var parts = motion_Name_And_Frame.Split(',');
        if (parts.Length < 2) return;
        string motion = parts[0].Trim();
        if (!int.TryParse(parts[1], out int frame)) return;

        var provider = attack_Strategy as IWeapon_Effect_Provider;

        var base_Data = provider?.Get_Normal_Attack_EffectData()
                       ?? (cur_Weapon_Data != null ? cur_Weapon_Data.effect_Data : null);

        var override_Data = provider?.Get_Normal_Attack_EffectData_WhileSkill();
        var data_To_Use = (override_Data != null && Has_Frame(override_Data, motion, frame))
                          ? override_Data
                          : base_Data;

        Show_Effect_Internal(motion_Name_And_Frame, Effect_Channel.Normal, data_To_Use);
    }

    public void Show_Skill_Effect(string motion_Name_And_Frame)
    {
        var data = cur_Weapon_Data != null ? cur_Weapon_Data.skill_Effect_Data : null;
        if (data == null) data = cur_Weapon_Data != null ? cur_Weapon_Data.effect_Data : null;

        Show_Effect_Internal(motion_Name_And_Frame, Effect_Channel.Skill, data);
    }

    private void Show_Effect_Internal(string motion_Name_And_Frame,
                                      Effect_Channel channel,
                                      Weapon_Effect_Data data)
    {
        if (data == null) return;

        var parts = motion_Name_And_Frame.Split(',');
        if (parts.Length < 2) return;

        string motion_Name = parts[0].Trim();
        if (!int.TryParse(parts[1], out int frame_Num)) return;

        var effect_Info = data.Get_Effect_Info(motion_Name);
        if (effect_Info == null) return;

        var frame_Effect_Info = effect_Info.frame_Effects.Find(fe => fe.frame_Number == frame_Num);
        if (frame_Effect_Info == null) return;

        var ch = (channel == Effect_Channel.Normal) ? normal_Effect : skill_Effect;
        if (ch == null || ch.render == null) return;

        Vector3 local_Pos = frame_Effect_Info.position_Offset;
        if (!is_Facing_Right) local_Pos.x = -local_Pos.x;

        if (ch.root != null)
        {
            ch.root.localPosition = local_Pos;
            ch.root.localScale = new Vector3(is_Facing_Right ? 1 : -1, 1, 1);
        }

        if (frame_Effect_Info.effect_Animator != null && ch.animator != null)
        {
            ch.render.enabled = true;
            ch.animator.runtimeAnimatorController = frame_Effect_Info.effect_Animator;
            ch.animator.Play("Effect_Start", 0, 0.0f);

            Stop_ResetCR(channel);
            Start_ResetCR(channel, Reset_Effect_After_Animation(channel, frame_Effect_Info.duration));
        }
        else if (frame_Effect_Info.effect_Sprites != null)
        {
            ch.render.sprite = frame_Effect_Info.effect_Sprites;
            ch.render.enabled = true;

            Stop_ResetCR(channel);
            Start_ResetCR(channel, HideEffect_After(channel, frame_Effect_Info.duration));
        }
    }

    private void Stop_ResetCR(Effect_Channel channel)
    {
        if (channel == Effect_Channel.Normal)
        {
            if (normal_Reset_CR != null) StopCoroutine(normal_Reset_CR);
        }
        else
        {
            if (skill_Reset_CR != null) StopCoroutine(skill_Reset_CR);
        }
    }

    private void Start_ResetCR(Effect_Channel channel, IEnumerator routine)
    {
        if (channel == Effect_Channel.Normal)
            normal_Reset_CR = StartCoroutine(routine);
        else
            skill_Reset_CR = StartCoroutine(routine);
    }

    private IEnumerator Reset_Effect_After_Animation(Effect_Channel channel, float duration)
    {
        yield return new WaitForSeconds(duration);
        var ch = (channel == Effect_Channel.Normal) ? normal_Effect : skill_Effect;
        if (ch != null && ch.animator != null)
            ch.animator.runtimeAnimatorController = null;
    }

    private IEnumerator HideEffect_After(Effect_Channel channel, float duration)
    {
        yield return new WaitForSeconds(duration);
        HideEffect(channel);
    }

    public void HideEffect(Effect_Channel channel)
    {
        var ch = (channel == Effect_Channel.Normal) ? normal_Effect : skill_Effect;
        if (ch != null && ch.render != null)
        {
            ch.render.enabled = false;
            ch.render.sprite = null;
        }
    }

    public void Hide_All_Effects()
    {
        HideEffect(Effect_Channel.Normal);
        HideEffect(Effect_Channel.Skill);
    }
    // ======================================================================================================

    // Attack And Skill =====================================================================================
    public void Input_Perform_Attack(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //평소에만 공격 가능하도록 설정
        {
            if (Time.timeScale != 1.0f || is_Player_Dead)
            {
                return;
            }

            if (attack_Strategy == null)
            {
                Debug.LogError("Attack Strategy Data is Missing");
                return;
            }

            if (cur_Weapon_Data.is_HoldAttack_Enabled)
            {
                switch (ctx.phase)
                {
                    case InputActionPhase.Started:
                        On_Attack_Button_Down();
                        break;
                    case InputActionPhase.Canceled:
                        On_Attack_Button_Up();
                        break;
                }
            }
            else
            {
                if (ctx.phase == InputActionPhase.Performed)
                {
                    if (!canAttack)
                        return;

                    On_Attack_Button_Click();
                }
            }
        }
        else if(Current_Player_State == Player_State.Event_Doing)
        {
            Now_Contact_Npc.GetComponent<Npc_Interface>().Event_Attack(ctx);
        }
    }

    public void On_Attack_Button_Down()
    {
        if (!cur_Weapon_Data.is_HoldAttack_Enabled) return;

        if (!is_AtkCoroutine_Running)
        {
            isAttacking = true;
            can_Card_Change = false;
            animator.SetBool("isHoldAtk", true);
            StartCoroutine(Continuous_Attack());
        }
    }

    public void On_Attack_Button_Up()
    {
        if (!cur_Weapon_Data.is_HoldAttack_Enabled)
            return;

        if (attack_Strategy is Bow_Attack_Strategy bow)
            bow.Release_Attack(this);
        
        isAttacking = false;
        animator.SetBool("isHoldAtk", false);
        can_Card_Change = true;
    }

    public void On_Attack_Button_Click()
    {
        if (Time.time - last_Combo_End_Time < combo_Input_Lock)
            return;

        if (!canAttack)
            return;

        if (cur_Weapon_Data.is_HoldAttack_Enabled)
            return;

        if (Current_Player_State != Player_State.Normal ||
            Time.timeScale != 1.0f ||
            is_Player_Dead ||
            attack_Strategy == null)
            return;

        if (Is_Last_Attack())
        {
            End_Attack();
            return;
        }

        if (Is_Cooldown_Complete() || Can_Combo_Attack())
        {
            Perform_Attack();
        }
        else if (Is_Combo_Complete())
        {
            End_Attack();
            return;
        }

        //Debug.Log($"Frame {Time.frameCount}: canAttack={canAttack}, last_Attack_Time={last_Attack_Time}");
    }

    private void Perform_Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            can_Card_Change = false;
        }

        if (isGrounded)
        {
            animator.SetBool("isAttacking", true);
            attack_Strategy.Attack(this, cur_Weapon_Data);

            Update_Attack_Timers();

            if (Is_Last_Attack())
            {
                End_Attack();
                return;
            }
        }
        else if (can_JumpAtk && !isGrounded)
        {
            animator.SetBool("Can_JumpAtk", true);
            attack_Strategy.Attack(this, cur_Weapon_Data);

            //audio_Proxy.Play_Attack();

            can_JumpAtk = false;
            StartCoroutine(Reset_JumpAtk_Param_Next_Frame());
        }
    }

    private IEnumerator Reset_JumpAtk_Param_Next_Frame()
    {
        yield return null;
        animator.SetBool("Can_JumpAtk", false);
    }

    public void End_Attack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        canAttack = false;
        can_Card_Change = true;

        StartCoroutine(Attack_Cooltime());
        Start_Attack_CooldownUI();

        last_Combo_End_Time = Time.time;

        Debug.Log($"Frame {Time.frameCount}: canAttack={canAttack}, last_Attack_Time={last_Attack_Time}");
    }

    private IEnumerator Attack_Cooltime()
    {
        float waitTime = cur_Weapon_Data.attack_Cooldown * attack_Cooltime_Mul;
        waitTime = Mathf.Max(0.1f, waitTime);

        yield return new WaitForSeconds(waitTime);
        canAttack = true;
    }

    private bool Is_Cooldown_Complete()
    {
        float modified_Cooldown = cur_Weapon_Data.attack_Cooldown * attack_Cooltime_Mul;
        modified_Cooldown = Mathf.Max(0.1f, modified_Cooldown);
        return Time.time >= last_Attack_Time + modified_Cooldown && canAttack == true;
    }
    private bool Can_Combo_Attack()
    {
        return isAttacking && Time.time <= combo_Deadline;
    }
    private bool Is_Combo_Complete()
    {
        return Time.time  > combo_Deadline;
    }
    private bool Is_Last_Attack()
    {
        int max_Atk_Count = animator.GetInteger("cur_Max_AtkCount");

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerChar_Attack_" + max_AttackCount))
        {
            Debug.Log("Last Atk True");
            return true;
        }

        return false;
    }

    private void Update_Attack_Timers()
    {
        last_Attack_Time = Time.time;
        combo_Deadline = Time.time + comboTime;
    }

    private void Start_Attack_CooldownUI()
    {
        Debug.Log("[플레이어 공격 UI] 현재 상태 True!");
        attack_Cooldown_Overlay.gameObject.SetActive(true);
        attack_Cooldown_Overlay.fillAmount = 1.0f;
    }

    private void Update_Attack_CooldownUI()
    {
        float cd = cur_Weapon_Data.attack_Cooldown * attack_Cooltime_Mul;
        float elapsed = Time.time - last_Attack_Time;
        float t = Mathf.Clamp01(elapsed / cd);

        if (t < 1.0f)
        {
            attack_Cooldown_Overlay.fillAmount = 1.0f - t;
        }
        else
        {
            attack_Cooldown_Overlay.gameObject.SetActive(false);
        }
    }

    private IEnumerator Continuous_Attack()
    {
        is_AtkCoroutine_Running = true;

        while (isAttacking)
        {
            if (attack_Strategy != null)
            {
                attack_Strategy.Attack(this, cur_Weapon_Data);
                can_Card_Change = false;
            }
            else
            {
                //Debug.LogError("Attack Strategy is missing for Hold_Attack");
                break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(cur_Weapon_Data.attack_Cooldown);

        is_AtkCoroutine_Running = false;
    }

    public void On_Last_Attack()
    {
        End_Attack();
    }

    public void Trigger_Enemy_Hit()
    {
        On_Enemy_Hit?.Invoke();
    }

    public void Enemy_Killed()
    {
        On_Enemy_Killed?.Invoke();
        Debug.Log("Enemy Killed Event has call");
    }

    public void On_Shoot_Projectile()
    {
        if (attack_Strategy != null)
        {
            attack_Strategy.Shoot(this, weapon_Anchor);
        }
        else
        {
            Debug.LogError("Weapon Projectile is Missing!");
        }
    }

    public void AE_PlayAttackSFX()
    {
        audio_Proxy?.Play_Attack();
    }

    void HandleCombo()
    {
        if (isAttacking)
        {
            if (cur_Weapon_Data != null && cur_Weapon_Data.is_HoldAttack_Enabled)
            {
                return;
            }

            if (Is_Combo_Complete())
            {
                End_Attack();
            }
        }
    }

    public void Input_Skill_Attack(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //상태조건
        {
            if (ctx.phase == InputActionPhase.Started
                && Time.timeScale == 1.0f
                && !is_Player_Dead
                && Is_Skill_Cooldown_Complete())
            {
                bool excuted = attack_Strategy.Skill(this, cur_Weapon_Data);
                if (!excuted) return;

                //attack_Strategy.Skill(this, cur_Weapon_Data);
                if (attack_Strategy is not Musket_Attack_Strategy musket)
                {
                    Update_Skill_Timer();
                }
                Start_Skill_Cooldown_UI();

                On_Player_Use_Skill?.Invoke();
            }
        }
    }

    public void On_Skill_Button()
    {
        if (Current_Player_State != Player_State.Normal
        || Time.timeScale != 1.0f
        || is_Player_Dead
        || !Is_Skill_Cooldown_Complete())
            return;

        bool executed = attack_Strategy.Skill(this, cur_Weapon_Data);
        if (!executed) return;

        if (attack_Strategy is not Musket_Attack_Strategy)
            Update_Skill_Timer();

        Start_Skill_Cooldown_UI();
        On_Player_Use_Skill?.Invoke();
    }

    public void Input_UpDown(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Current_Player_State == Player_State.Dialogue_Choice)
        {
            Dialogue_Manager.instance.Chose_Cursor_Move();
        }
    }

    private bool Is_Skill_Cooldown_Complete()
    {
        float modified_Cooldown = cur_Weapon_Data.skill_Cooldown * skill_Cooltime_Mul;
        return Time.time >= last_Skill_Time + modified_Cooldown;
    }

    public void Update_Skill_Timer()
    {
        last_Skill_Time = Time.time;
    }

    private void Start_Skill_Cooldown_UI()
    {
        is_Skill_Coolingdown = true;
        skill_Cooldown_Overlay.enabled = true;
        skill_Cooldown_Overlay.fillAmount = 1.0f;
    }

    private void Stop_Skill_Cooldown_UI()
    {
        is_Skill_Coolingdown = false;
        if (skill_Cooldown_Overlay != null)
        {
            skill_Cooldown_Overlay.enabled = false;
            skill_Cooldown_Overlay.fillAmount = 0.0f;
        }
    }

    private void Update_Skill_Cooldown_UI()
    {
        float cd = cur_Weapon_Data.skill_Cooldown * skill_Cooltime_Mul;
        float elapsed = Time.time - last_Skill_Time;
        float t = Mathf.Clamp01(elapsed / cd);

        if (t < 1.0f)
        {
            skill_Cooldown_Overlay.enabled = true;
            skill_Cooldown_Overlay.fillAmount = 1.0f - t;
        }
        else
        {
            skill_Cooldown_Overlay.enabled = false;
        }
    }

    public void Reset_Skill_Cooldown(bool refreshUI = true)
    {
        float modified = (cur_Weapon_Data ? cur_Weapon_Data.skill_Cooldown : 0.0f) * skill_Cooltime_Mul;

        last_Skill_Time = Time.time - modified;

        if (refreshUI) Stop_Skill_Cooldown_UI();
    }
    // ======================================================================================================

    // Player Character UI ==========================================================================================
    private bool Can_Close_Inventory()
    {
        bool isFull = player_Inventory.Count >= max_Inventory_Size;
        if (isInventory_Visible && isFull && is_Item_Change)
            return false;

        return true;
    }
    
    void OnInventory_Pressed(InputAction.CallbackContext context)
    {
        if (is_StatUI_Visible || Current_Player_State != Player_State.Normal || Current_Event_State == Event_State.Bird_Hunting)
        {
            return;
        }
        ShowInventory();
        stat_Object.Set_Stat_Panel();
        Time.timeScale = 0.0f;
    }
    void OnInventory_Released(InputAction.CallbackContext context)
    {
        if (!Can_Close_Inventory() ||
            Current_Event_State == Event_State.Bird_Hunting ||
            is_StatUI_Visible ||
            Current_Player_State == Player_State.Dialogue || 
            Current_Player_State == Player_State.Dialogue_Choice || 
            Current_Player_State == Player_State.Player_Dead)
        { 
            return; 
        }

        HideInventory();
        Time.timeScale = 1.0f;
        Current_Player_State = Player_State.Normal;
        movement = Vector2.zero;
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
    void ShowInventory()
    {
        Current_Player_State = Player_State.UI_Open;

        if (!isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            isInventory_Visible = true;
            //is_UI_Open = true;
            Update_Inventory();
        }
    }
    void HideInventory()
    {
        Current_Player_State = Player_State.Normal;

        if (isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isInventory_Visible = false;
            //is_UI_Open = false;
        }
    }

    public void Toggle_Inventory()
    {
        if (is_StatUI_Visible) return;

        if (isInventory_Visible)
        {
            if (!Can_Close_Inventory())
                return;

            HideInventory();
            Time.timeScale = 1.0f;
            Current_Player_State = Player_State.Normal;
            movement = Vector2.zero;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            ShowInventory();
            stat_Object.Set_Stat_Panel();
            Time.timeScale = 0.0f;
        }
    }

    private void Shift_Selected_Item()
    {
        is_Item_Change = true;
        On_ItemChange_State_Changed?.Invoke(true);

        Current_Player_State = Player_State.UI_Open;
        Time.timeScale = 0.0f;
        ShowInventory();
    }

    private void Complete_Item_Change()
    {
        is_Item_Change = false;
        On_ItemChange_State_Changed?.Invoke(false);
    }

    private void Cancel_Item_Change()
    {
        is_Item_Change = false;
        On_ItemChange_State_Changed?.Invoke(false);
    }
    // Created By KYH -------------------------------------------------------------------
    public void Update_Player_Health(float health_Multiplier)
    {
        if (health_Multiplier <= 0.0f)
        {
            Debug.LogWarning("health_Multiplier must be greater than 0.");
            return;
        }

        float ratio = (float)health / max_Health;
        int new_MaxHealth = Mathf.RoundToInt(max_Health * health_Multiplier);
        int new_CurHealth = Mathf.RoundToInt(new_MaxHealth * ratio);

        new_CurHealth = Mathf.Clamp(new_CurHealth, 0, new_MaxHealth);

        max_Health = new_CurHealth;
        health = new_CurHealth;

        Player_Health_Bar.fillAmount = (float)health / max_Health;
    }

    public void Player_Take_Damage(int Damage)
    {
        if (isInvincible || is_Player_Dead) return;

        if (attack_Strategy is Crow_Card_Attack_Strategy crow_Strategy && crow_Strategy.Crow_Is_Protecting())
        {
            Debug.Log("Crow blocked the damage!");
            crow_Strategy.End_Protection();
            return;
        }

        if (attack_Strategy is Shield_Attack_Strategy shield_Startegy && shield_Startegy.isParrying)
        {
            shield_Startegy.Detect_EnemyAttack(this);
            return;
        }

        if (UnityEngine.Random.value < defend_Attack_Rate)
        {
            Debug.Log("방어 성공, 데미지 차단.");
            return;
        }

        int damage_Reduction = UnityEngine.Random.Range(damage_Reduce_Min, damage_Reduce_Max + 1);
        int reduced_Damage = Mathf.Max(0, Damage - damage_Reduction);
        int total_Dmg = Mathf.RoundToInt(reduced_Damage * takenDamage_Mul);

        int old_Health = health;
        health = health - total_Dmg;

        if (dmg_Inc_To_Lost_Health)
        {
            int health_Lost = old_Health - health;
            Adjust_Damage_Multiplier(health_Lost);
        }

        Player_Health_Bar.fillAmount = (float)health / max_Health;

        if (health <= 0)
        {
            Player_Died();
        }
        else
        {
            On_Player_Damaged?.Invoke();
            audio_Proxy?.Play_Hurt();
        }

        camera_Manager.Shake_Camera();
    }

    public void Player_Take_Heal(int amount)
    {
        if (is_Player_Dead) return;

        int heal_Amount = Mathf.RoundToInt(amount * heal_Amount_Mul);

        int old_Health = health;
        health = Mathf.Clamp(health + heal_Amount, 0, max_Health);

        if (dmg_Inc_To_Lost_Health)
        {
            int health_Recoverd = health - old_Health;
            Adjust_Damage_Multiplier(health_Recoverd);
        }

        Player_Health_Bar.fillAmount = (float)health / max_Health;
    }

    private void Adjust_Damage_Multiplier(int health_Data)
    {
        float percent_Change = (float) health_Data / max_Health;
        damage_Mul += percent_Change;

        damage_Mul = Mathf.Round(damage_Mul * 100.0f) / 100.0f;

        damage_Mul = Mathf.Max(damage_Mul, 0.0f);
    }

    private void Player_Died()
    {
        if (player_Life > 0)
        {
            player_Life--;
            animator.SetTrigger("Player_Resurrection");

            is_Player_Dead = true;
            isInvincible = true;
            StartCoroutine(End_Invisible_After_Delay(2.4f));
            return;
        }

        is_Player_Dead = true;
        isInvincible = true;
        //player_render.enabled = false;
        animator.SetTrigger("Player_Dead");

        pause_Manager.Show_Result(true);
        Current_Player_State = Player_State.Player_Dead;
    }

    private IEnumerator End_Invisible_After_Delay(float delay)
    {
        yield return new WaitForSeconds(delay);

        health = max_Health / 5;
        Player_Health_Bar.fillAmount = (float)health / max_Health;
        isInvincible = false;
        is_Player_Dead = false;
    }

    public void Input_Game_Stop(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            if (Current_Player_State == Player_State.Normal)
            {
                if (Time.timeScale == 0.0f)
                {
                    if(!pause_Manager.is_Help_Activated() && !Key_Setting_Can.gameObject.activeSelf)
                    {
                        pause_Manager.Pause_Stop();
                    }
                    else
                    {
                        if (Key_Setting_Can.gameObject.activeSelf)
                        {
                            pause_Manager.Btn_Setting_Out();
                        }
                        else
                        {
                            pause_Manager.Btn_Help_Out();
                        }
                    }
                }
                else if (Time.timeScale == 1.0f)
                {
                    pause_Manager.Pause_Start();
                    //Time.timeScale = 0.0f;
                }
            }
        }
    }

    public void Open_Settings()
    {
        if (Current_Player_State != Player_State.Normal)
            return;

        if (Time.timeScale == 1.0f)
        {
            pause_Manager.Pause_Start();
        }

        Debug.Log("일시정지 호출됨");
    }
    // --------------------------------------------------------------------------------

    // ======================================================================================================

    // Player Collsion ======================================================================================

    private int i_platform = 0;

    private void OnCollisionEnter2D(Collision2D other)
    {
        foreach (ContactPoint2D contact in other.contacts)
        {
            Vector2 normal = contact.normal;

            if ((other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Trap_Platform")) && normal.y > 0.5f)
            {
                if(other.gameObject.CompareTag("Platform"))
                {
                    Platforms.Add(other.gameObject);
                }

                if (normal.y > 0.5f)
                {
                    bool was_Ground = isGrounded;
                    isGrounded = true;
                    i_platform++;

                    can_JumpAtk = true;
                    animator.SetBool("Can_JumpAtk", true);

                    //Now_New_Platform = other.gameObject; //Reset condition

                    if (!was_Ground)
                    {
                        has_Jumped = false;
                        jumpCount = 0;
                    }
                    player_Effect_Animator.SetTrigger("Land_Effect");
                }
            }
            else if (other.gameObject.CompareTag("OneWayPlatform"))
            {
                EdgeCollider2D edge = other.gameObject.GetComponent<EdgeCollider2D>() ? other.gameObject.GetComponent<EdgeCollider2D>() : null; 
                if (edge != null)
                {
                    OneWays.Add(other.gameObject);
                }
                
                if (normal.y > 0.1f)
                {
                    bool was_Ground = isGrounded;
                    isGrounded = true;
                    current_Platform = other.gameObject;

                    can_JumpAtk = true;
                    animator.SetBool("Can_JumpAtk", true);

                    if (!was_Ground)
                    {
                        has_Jumped = false;
                        jumpCount = 0;
                    }
                    player_Effect_Animator.SetTrigger("Land_Effect");

                    break;
                }
            }
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("OneWayPlatform"))
        {
            i_platform--;
            if (other.gameObject.CompareTag("OneWayPlatform"))
            {
                OneWays.Remove(other.gameObject);
            }
            else if(other.gameObject.CompareTag("Platform"))
            {
                Platforms.Remove(other.gameObject);
            }

            if (OneWays.Count == 0 && Platforms.Count == 0)
            {
                isGrounded = false;
                i_platform = 0;

                if (!has_Jumped)
                {
                    jumpCount = 1;
                }
            }

            if (other.gameObject.CompareTag("OneWayPlatform") && other.gameObject == current_Platform)
            {
                current_Platform = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == false)
        {
            map_Manager.IsOnPortal = true;
            use_Portal = true;
            Add_Interactable(other);
        }

        if (other.CompareTag("CM_Boundary"))
        {
            cur_Cinemachine_Collider = other;
            camera_Manager.Update_Confiner(cur_Cinemachine_Collider);
        }

        if (other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = true;
            Now_Contact_Npc = other.gameObject;
            Add_Interactable(other);
        }

        //if (other.CompareTag("Portal") && !map_Manager.IsOnPortal)
        //{
        //    map_Manager.IsOnPortal = true;
        //    use_Portal = true;
        //    Add_Interactable(other);
        //}
        //else if (other.CompareTag("NPC"))
        //{
        //    is_Npc_Contack = true;
        //    Now_Contact_Npc = other.gameObject;
        //    Add_Interactable(other);
        //}
        //else if (other.CompareTag("Card")
        //    || other.CompareTag("Item")
        //    || other.CompareTag("Market_Item")
        //    || other.CompareTag("Chest"))
        //{
        //    current_Item = other.gameObject;
        //    Add_Interactable(other);
        //}

        //if (other.CompareTag("CM_Boundary"))
        //{
        //    cur_Cinemachine_Collider = other;
        //    camera_Manager.Update_Confiner(cur_Cinemachine_Collider);
        //}
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Card"))
        {
            if (card_Inventory[0] != null && card_Inventory[1] != null)
            {
                if (card_Preview_UI == null)
                {
                    Debug.LogError("카드 프리뷰 UI 존재하지 않음"); return;
                }

                var hovered = other.GetComponent<Card>().cardValue;
                var a = card_Inventory[0].GetComponent<Card>().cardValue;
                var b = card_Inventory[1].GetComponent<Card>().cardValue;

                var preview_First = Compute_Weapon(a, hovered);
                var preview_Second = Compute_Weapon(b, hovered);

                card_Preview_UI.Show(preview_First, preview_Second);

                var ft = card_Preview_UI.GetComponent<Follow_Item>();
                ft.target = other.transform;
                ft.world_Offset = new Vector3(0, 1.2f, 0);
            }

            current_Item = other.gameObject;
            Add_Interactable(other);
        }
        else if ((other.CompareTag("Item") || other.CompareTag("Market_Item"))
            && other.TryGetComponent<Item_Prefab>(out var itemComp))
        {
            current_Item = other.gameObject;
            Add_Interactable(other);

            var item_Comp = other.GetComponent<Item_Prefab>();
            if (item_Comp != null)
            {
                item_Preview_UI.Show(item_Comp.itemData);
            }
            var ft = item_Preview_UI.GetComponent<Follow_Item>();
            ft.target = other.transform;
            ft.world_Offset = new Vector3(0, 1.2f, 0);
        }
        else if (other.CompareTag("Chest"))
        {
            current_Item = other.gameObject;
            Add_Interactable(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == true)
        {
            map_Manager.IsOnPortal = false;
            use_Portal = false;
            Remove_Interactable(other);
        }

        if (other.CompareTag("Card"))
        {
            current_Item = null;
            card_Preview_UI.Hide();
            Remove_Interactable(other);
        }
        else if (other.CompareTag("Item") || other.CompareTag("Market_Item"))
        {
            current_Item = null;
            item_Preview_UI.Hide();
            Remove_Interactable(other);
        }
        else if (other.CompareTag("Chest"))
        {
            current_Item = null;
            Remove_Interactable(other);
        }

        if (other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = false;
            Remove_Interactable(other);
        }

        //// ◼ Portal 탈출
        //if (other.CompareTag("Portal") && map_Manager.IsOnPortal)
        //{
        //    map_Manager.IsOnPortal = false;
        //    use_Portal = false;
        //    Remove_Interactable(other);
        //}
        //// ◼ NPC 탈출
        //else if (other.CompareTag("NPC"))
        //{
        //    is_Npc_Contack = false;
        //    Remove_Interactable(other);
        //}
        //// ◼ Card 탈출
        //else if (other.CompareTag("Card"))
        //{
        //    current_Item = null;
        //    card_Preview_UI.Hide();
        //    Remove_Interactable(other);
        //}
        //// ◼ Item / Market_Item 탈출
        //else if (other.CompareTag("Item") || other.CompareTag("Market_Item"))
        //{
        //    current_Item = null;
        //    item_Preview_UI.Hide();
        //    Remove_Interactable(other);
        //}
        //// ◼ Chest 탈출
        //else if (other.CompareTag("Chest"))
        //{
        //    current_Item = null;
        //    Remove_Interactable(other);
        //}
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platform_Collider = current_Platform.GetComponent<BoxCollider2D>();
        //CompositeCollider2D platform_Composite = current_Platform.GetComponent<CompositeCollider2D>();

        List<GameObject> Copy_Oneway = new List<GameObject>(OneWays);

        if (Copy_Oneway.Count != 0)
        {
            foreach (GameObject objects in Copy_Oneway)
            {
                EdgeCollider2D edges = objects.GetComponent<EdgeCollider2D>();
                Physics2D.IgnoreCollision(player_Platform_Collider, edges);
            }
        }

        if(platform_Collider != null)
        {
            Physics2D.IgnoreCollision(player_Platform_Collider, platform_Collider);
        }

        //if(platform_Composite != null)
        //{
        //    platform_Composite.isTrigger = true;
        //}
        yield return new WaitForSeconds(0.5f);

        if(platform_Collider != null)
        {
            Physics2D.IgnoreCollision(player_Platform_Collider, platform_Collider, false);
        }

        if (Copy_Oneway.Count != 0)
        {
            foreach (GameObject objects in Copy_Oneway)
            {
                EdgeCollider2D edges = objects.GetComponent<EdgeCollider2D>();
                Physics2D.IgnoreCollision(player_Platform_Collider, edges, false);
            }
        }
        //if(platform_Composite != null)
        //{
        //    platform_Composite.isTrigger = false;
        //}
    }

    public void Weak_Knock_Back(int Left_Num, float Knock_Back_time, float Power) //Left = 1, Right = -1
    {
        if(Current_Player_State == Player_State.Trap_Minigame)
        {
            return;
        }

        is_Knock_Back = true;

        Vector2 Knock_Back_Direction = new Vector2((float)Left_Num, 0.5f);

        rb.velocity = Vector2.zero;
        rb.AddForce(Knock_Back_Direction * Power, ForceMode2D.Impulse);

        Invoke("Knock_Back_End", Knock_Back_time);
    }

    private void Knock_Back_End()
    {
        if(rb.velocity.y <= 0.0f)
        {
            rb.velocity = Vector2.zero;
        }
        is_Knock_Back = false;
    }
    // =========================================================================================================

    public int Calculate_Damage()
    {
        int base_Dmg = cur_Weapon_Data.attack_Damage + attackDamage;
        int total_Dmg = Mathf.RoundToInt(base_Dmg * damage_Mul);

        bool is_Critical = UnityEngine.Random.value <= crit_Rate;
        if (is_Critical)
        {
            total_Dmg = Mathf.RoundToInt(total_Dmg * crit_Dmg);
        }
        
        return total_Dmg;
    }

    public int Calculate_Skill_Damage()
    {
        int base_Dmg = cur_Weapon_Data.skill_Damage + skill_Damage;
        int total_Dmg = Mathf.RoundToInt(base_Dmg * damage_Mul);

        bool is_Critical = UnityEngine.Random.value <= crit_Rate;
        if (is_Critical)
        {
            total_Dmg = Mathf.RoundToInt(total_Dmg * crit_Dmg);
        }

        return total_Dmg;
    }

    public void Add_Player_Money(int Income) // Money Method
    {
        i_Money += Income;

        if (i_Money <= 0)
        {
            i_Money = 0;
        }

        money_Text.text = i_Money.ToString();

        //Debug.Log("Player Money : " + i_Money);
    }

    public void Add_Player_Token(int income)
    {
        i_Token += income;

        if (i_Token <= 0)
        {
            i_Token = 0;
        }

        token_Text.text = i_Token.ToString();
    }

    public void Update_Player_Money()
    {
        money_Text.text = i_Money.ToString();
    }

    //=============== 속박, 기절계열
    public void Player_Bind(float Bind_Time)
    {
        is_Knock_Back = true;
        rb.velocity = Vector2.zero;

        Invoke("Knock_Back_End", Bind_Time);
    }

    public void Player_Stun()
    {

    }

    public void Player_Trap_Stun(bool is_Can_Jump, System.Action onComplete = null)
    {
        is_Knock_Back = true;
        rb.velocity = Vector2.zero;

        if(!is_Can_Jump)
        {
            isGrounded = false;
            jumpCount = 2;
        }

        //Invoke("Knock_Back_End", Bind_Time);
        StartCoroutine(Escape_Coroutine(onComplete));
    }

    public IEnumerator Escape_Coroutine(System.Action onComplete)
    {
        float time = 0.0f;
        current_gauge = 0;

        is_Minigame = true;
        Current_Player_State = Player_State.Trap_Minigame;

        while (current_gauge <= require_gauge)
        {
            time += Time.deltaTime;

            if (time >= 1.0f)
            {
                current_gauge -= decrease_rate;
                current_gauge = Mathf.Clamp(current_gauge, 0, require_gauge);

                time = 0.0f;
            }

            yield return null;
        }

        is_Minigame = false;
        Current_Player_State = Player_State.Normal;
        Knock_Back_End();
        onComplete?.Invoke();
    }

    public void Start_Tick_Damage(float Tick_Time, int Tick_Damage, int Tick_Count)
    {
        StartCoroutine(Player_Tick_Damage(Tick_Time, Tick_Damage, Tick_Count));
    }

    private IEnumerator Player_Tick_Damage(float Tick_Time, int Tick_Damage, int Tick_Count)
    {
        Debug.Log("Tick Damage Coroutine Start");
        for(int i = 0; i < Tick_Count; i++)
        {
            yield return new WaitForSeconds(Tick_Time);
            Player_Take_Damage(Tick_Damage);
        }
    }

    public void State_Change(Player_State state)
    {
        Current_Player_State = state;
    }

    public void Event_State_Change(Event_State state)
    {
        Current_Event_State = state;
    }

    public void Player_Vector_Stop()
    {
        movement = Vector2.zero;
    }

    public void custom_Give_Card()
    {
        if (card_Inventory[0] == null || card_Inventory[1] == null)
        {
            Debug.Log("카드가 없음");
            return;
        }
        match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
    }
}