using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Mathematics;

// PlayerCharacter_Controller Created By JBJ, KYH
public class PlayerCharacter_Controller : PlayerChar_Inventory_Manager
{
    private Player_InputActions inputActions;
    [SerializeField]
    private GameObject inventoryPanel;
    private bool isInventory_Visible = false;
    public bool is_StatUI_Visible = false;

    public Rigidbody2D rb;
    GameObject current_Item;

    Vector2 movement = new Vector2();
    public bool isMoving;
    [HideInInspector] public int jumpCount = 0;
    public int maxJumpCount = 2;

    [Header("Teleport")]
    bool canTeleporting = true;
    private int cur_Teleport_Count;
    public float teleporting_Cooltime_Timer;

    [Header("DebugChest")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [HideInInspector] public bool isGrounded;
    [HideInInspector] private bool has_Jumped;
    [HideInInspector] public bool canAttack = true;

    [Header("Cinemachine")]
    private Camera_Manager camera_Manager;
    private Collider2D cur_Cinemachine_Collider;

    [Header("Weapon_Data")]
    private Weapon_Collision_Handler weapon_Handler;
    private bool is_AtkCoroutine_Running = false;
    public GameObject weapon_Prefab;    
    public SpriteRenderer effect_Render;
    public Animator effect_Animator;
    public Transform weapon_Anchor;
    public Transform effect_Anchor;
    public bool is_Facing_Right = true;
    public Animator weapon_Animator;

    public event Action On_Player_Damaged;
    public event Action On_Enemy_Hit;
    public event Action On_Enemy_Killed;
    public event Action<PlayerCharacter_Controller> On_Teleport;
    public bool isInvincible = false;

    private Item pending_SwapItem = null;
    public bool is_UI_Open = false;
    public bool is_Item_Change = false;

    [SerializeField] private float card_Change_Cooldown = 2.0f;
    private bool can_Card_Change = true;

    //--------------------------------------------------- Created By KYH
    [Header("Player_UI")]
    [SerializeField] private Image Player_Health_Bar;
    [SerializeField] private Pause_Manager pause_Manager;
    [SerializeField] private SpriteRenderer player_render;
    
    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;
    public bool use_Portal = false;
    
    //platform & Collider
    private GameObject current_Platform;
    [SerializeField] private BoxCollider2D player_Collider;
    private GameObject Now_New_Platform;
    private bool is_Down_Performed = false;    

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

    public enum Player_State
    {
        Normal,
        Dialogue,
        Dialogue_Choice,
        Event_Doing
    }

    public enum Event_State
    {
        None,
        Bird_Hunting
    }

    private Player_State Current_Player_State;
    private Event_State Current_Event_State;
    //---------------------------------------------------
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputActions = new Player_InputActions();
        
        inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();

        Set_Weapon(0);

        Current_Player_State = Player_State.Normal; // 플레이어 현재상태 초기화 KYH
        Current_Event_State = Event_State.None; //이벤트 상태 초기화
    }
    private void OnEnable()
    {
        inputActions.Player.Inventory.started += OnInventory_Pressed;
        inputActions.Player.Inventory.canceled += OnInventory_Released;
        inputActions.Player.Enable();
    }
    private void OnDisable()
    {
        inputActions.Player.Inventory.started -= OnInventory_Pressed;
        inputActions.Player.Inventory.canceled -= OnInventory_Released;
        inputActions.Player.Disable();
    }

    private void Start()
    {
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
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1.0f && !is_Player_Dead)
        {
            if (isAttacking && isGrounded && !cur_Weapon_Data.is_HoldAttack_Enabled)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (is_UI_Open)
            {
                rb.velocity = Vector2.zero;
            }
            else
            {
                Move();
            }

            if (!isCombDone)
            {
                Card_Combination();
            }

            Update_Animation_Parameters();
            HandleCombo();
            Handle_Teleportation_Time();
        }
    }
    private void FixedUpdate()
    {
        Update_WeaponAnchor_Position();
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

        if (isGrounded && rb.velocity.y == 0 && this.gameObject.transform.position.y - 0.3f > Now_New_Platform.transform.position.y)
        {
            //jumpCount = 0;
        }
    }

    // Move ===========================================================================================
    public void Input_Move(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //State Check
        {
            if (is_UI_Open)
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
                        return;
                    }
                }
                return;
            }

            if (ctx.phase == InputActionPhase.Canceled)
            {
                movement = Vector2.zero;
            }
            else if (ctx.phase == InputActionPhase.Started)
            {
                movement = ctx.action.ReadValue<Vector2>();

                if (is_Minigame)
                {
                    current_gauge += increase_rate;
                    Debug.Log("게이지 증가" + current_gauge);
                }
            }
            else
            {
                movement = ctx.action.ReadValue<Vector2>();
            }
        }
        else if(Current_Player_State == Player_State.Event_Doing)
        {
            Now_Contact_Npc.GetComponent<Npc_Interface>().Event_Move(ctx);
        }
    }

    void Move()
    {
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

        Vector2 normalized_Movement = movement.normalized;

        if (!is_Knock_Back)
        {
            rb.velocity = new Vector2(normalized_Movement.x * movementSpeed, rb.velocity.y);
        }
    }

    public void Input_Jump(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //상태조건
        {
            if (is_UI_Open)
            {
                return;
            }

            if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
            {
                if (is_Down_Performed)
                {
                    if (current_Platform != null)
                    {
                        StartCoroutine(DisableCollision());
                    }
                }
                else
                {
                    if (jumpCount < maxJumpCount)
                    {
                        Do_Jump();
                    }
                }
            }
        }
    }

    private void Do_Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);

        jumpCount++;
        has_Jumped = true;
        isGrounded = false;
    }
    
    public void Input_Teleportation(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //상태조건
        {
            if (is_UI_Open || is_Player_Dead) return;

            if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && canTeleporting)
            {
                float adjusted_Distance = teleporting_Distance;

                Vector2 top_Pos = new Vector2(transform.position.x, transform.position.y + GetComponent<Collider2D>().bounds.extents.y);
                Vector2 bottom_Pos = new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y);

                Vector2 direction = is_Facing_Right ? Vector2.right : Vector2.left;

                RaycastHit2D topHit = Physics2D.Raycast(top_Pos, direction, teleporting_Distance, LayerMask.GetMask("Walls"));
                RaycastHit2D bottomHit = Physics2D.Raycast(bottom_Pos, direction, teleporting_Distance, LayerMask.GetMask("Walls"));

                if (topHit.collider != null)
                {
                    adjusted_Distance = Mathf.Min(adjusted_Distance, topHit.distance);
                }
                if (bottomHit.collider != null)
                {
                    adjusted_Distance = Mathf.Min(adjusted_Distance, bottomHit.distance);
                }

                if (!is_Facing_Right)
                {
                    transform.Translate(Vector2.left * adjusted_Distance);
                }
                else
                {
                    transform.Translate(Vector2.right * adjusted_Distance);
                }

                animator.SetTrigger("Teleport");
                cur_Teleport_Count--;

                if (cur_Teleport_Count <= 0)
                {
                    canTeleporting = false;
                }

                On_Teleport?.Invoke(this);
            }
        }
    }

    void Handle_Teleportation_Time()
    {
        if (cur_Teleport_Count < max_Teleport_Count)
        {
            teleporting_Cooltime_Timer -= Time.deltaTime;
            if (teleporting_Cooltime_Timer <= 0.0f)
            {
                teleporting_Cooltime_Timer = teleporting_CoolTime;
                cur_Teleport_Count++;

                if (cur_Teleport_Count > 0)
                {
                    canTeleporting = true;
                }
            }
        }
    }
    // Created By KYH ---------------------------------------------------------------
    public void Input_Down_Jump(InputAction.CallbackContext ctx)
    {
        if (is_UI_Open) return;

        if (ctx.phase == InputActionPhase.Performed && Time.timeScale == 1.0f)
        {
            is_Down_Performed = true;
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            is_Down_Performed = false;
        }
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

            if (isInventory_Visible && is_Item_Change && pending_SwapItem != null)
            {
                SwapItem(pending_SwapItem);
                pending_SwapItem = null;
                is_Item_Change = false;
                HideInventory();
                return;
            }

            if (is_StatUI_Visible)
            {
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>()?.Confirm_Selection();
                return;
            }

            if (is_UI_Open) return;

            map_Manager.Use_Portal();
            Handle_Npc_Interaction();
            Handle_Item_Interaction();
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
        GameObject chest = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void Input_Change_FirstCard(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started || Time.timeScale != 1.0f || is_Player_Dead || !can_Card_Change || isAttacking)
            return;

        Change_FirstAndThird_Card();

        if (card_Inventory[0] != null && card_Inventory[1] != null) //Match Up Call
        {
            match_manager.Give_Player_Cards(card_Inventory[0], card_Inventory[1]);
            match_manager.Match_Reset();
            match_manager.Start_Match();
        }

        StartCoroutine(Card_Change_Cooldown_Routine());
    }

    public void Input_Change_SecondCard(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started || Time.timeScale != 1.0f || is_Player_Dead || !can_Card_Change || isAttacking)
            return;

        Change_SecondAndThird_Card();

        if (card_Inventory[0] != null && card_Inventory[1] != null) //Match Up Call
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
        
        if (attack_Strategy != null && attack_Strategy is Tiger_Attack_Strategy tiger_Strategy)
        {
            tiger_Strategy.Reset_Stats();
        }
        else if (attack_Strategy != null && attack_Strategy is Crow_Card_Attack_Strategy crow_Strategy)
        {
            crow_Strategy.Reset_Stats();
        }

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
                int cur_Frame = Mathf.FloorToInt(state_Info.normalizedTime * total_Frames) % total_Frames;                

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

    public void Show_Effect(string motion_Name_And_Frame)
    {
        var parts = motion_Name_And_Frame.Split(',');
        if (parts.Length < 2) return;

        string motion_Name = parts[0];
        int frame_Num;
        if (!int.TryParse(parts[1], out frame_Num)) return;

        var effect_Info = cur_Weapon_Data.effect_Data.Get_Effect_Info(motion_Name);
        if (effect_Info != null)
        {
            var frame_Effect_Info = effect_Info.frame_Effects.Find(fe => fe.frame_Number == frame_Num);
            if (frame_Effect_Info != null)
            {
                Vector3 effect_Pos = frame_Effect_Info.position_Offset;
                if (!is_Facing_Right)
                {
                    effect_Pos.x = -effect_Pos.x;
                }
                effect_Render.transform.localPosition = effect_Pos;
                effect_Render.transform.localScale = new Vector3(is_Facing_Right ? 1 : -1, 1, 1);
                //Debug.Log($"Saved Offset: {effect_Pos}");

                if (frame_Effect_Info.effect_Animator != null)
                {
                    effect_Animator.runtimeAnimatorController = frame_Effect_Info.effect_Animator;
                    effect_Render.enabled = true;
                    effect_Animator.Play("Effect_Start");
                    StartCoroutine(Reset_Effect_After_Animation(frame_Effect_Info.duration));
                }
                else if (frame_Effect_Info.effect_Sprites != null)
                {
                    effect_Render.sprite = frame_Effect_Info.effect_Sprites;
                    effect_Render.enabled = true;
                    Invoke("HideEffect", frame_Effect_Info.duration);
                }
            }
        }
    }

    private IEnumerator Reset_Effect_After_Animation(float duration)
    {
        yield return new WaitForSeconds(duration);
        effect_Animator.runtimeAnimatorController = null;
    }

    public void HideEffect()
    {
        effect_Render.enabled = false;
        effect_Render.sprite = null;
    }
    // ======================================================================================================

    // Attack And Skill =====================================================================================
    public void Input_Perform_Attack(InputAction.CallbackContext ctx)
    {
        if (Current_Player_State == Player_State.Normal)    //평소에만 공격 가능하도록 설정
        {
            if (is_UI_Open)
            {
                return;
            }

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
                        if (!is_AtkCoroutine_Running)
                        {
                            isAttacking = true;
                            can_Card_Change = false;
                            animator.SetBool("isHoldAtk", true);
                            StartCoroutine(Continuous_Attack());
                        }
                        break;
                    case InputActionPhase.Canceled:
                        if (attack_Strategy is Bow_Attack_Strategy bowAttack)
                        {
                            bowAttack.Release_Attack(this);
                        }
                        isAttacking = false;
                        animator.SetBool("isHoldAtk", false);
                        can_Card_Change = true;
                        break;
                }
            }
            else
            {
                if (ctx.phase == InputActionPhase.Performed)
                {
                    if (Is_Last_Attack())
                    {
                        End_Attack();
                        return;
                    }

                    if (Is_Cooldown_Complete())
                    {
                        Perform_Attack();
                    }
                    else if (Can_Combo_Attack())
                    {
                        Perform_Attack();
                    }
                    else if (Is_Combo_Complete())
                    {
                        End_Attack();
                    }
                }
            }
        }
        else if(Current_Player_State == Player_State.Event_Doing)
        {
            Now_Contact_Npc.GetComponent<Npc_Interface>().Event_Attack(ctx);
        }
    }

    private void Perform_Attack()
    {
        isAttacking = true;
        animator.SetBool("isAttacking", true);
        attack_Strategy.Attack(this, cur_Weapon_Data);
        Update_Attack_Timers();

        if (Is_Last_Attack())
        {
            End_Attack();
        }
    }

    public void End_Attack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
        canAttack = false;

        StartCoroutine(Attack_Cooltime());
    }

    private IEnumerator Attack_Cooltime()
    {
        yield return new WaitForSeconds(cur_Weapon_Data.attack_Cooldown);

        canAttack = true;
    }

    private bool Is_Cooldown_Complete()
    {
        return Time.time >= last_Attack_Time + attack_Cooldown && canAttack == true;
    }
    private bool Can_Combo_Attack()
    {
        return isAttacking && Time.time - last_Attack_Time <= last_ComboAttack_Time;
    }
    private bool Is_Combo_Complete()
    {
        return Time.time - last_ComboAttack_Time > comboTime;
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
        last_ComboAttack_Time = Time.time;
        last_Attack_Time = Time.time;
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
            if (is_UI_Open) return;

            if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f
                && !is_Player_Dead && Is_Skill_Cooldown_Complete())
            {
                animator.SetTrigger("Skill");
                attack_Strategy.Skill(this, cur_Weapon_Data);
                Update_Skill_Timer();
            }
        }
    }

    public void Input_UpDown(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Started && Current_Player_State == Player_State.Dialogue_Choice)
        {
            Dialogue_Manager.instance.Chose_Cursor_Move();
        }
    }

    private bool Is_Skill_Cooldown_Complete()
    {
        return Time.time >= last_Skill_Time + skill_Cooldown;
    }

    private void Update_Skill_Timer()
    {
        last_Skill_Time = Time.time;
    }
    // ======================================================================================================

    // Player Character UI ==========================================================================================
    void OnInventory_Pressed(InputAction.CallbackContext context)
    {
        ShowInventory();
        Time.timeScale = 0.0f;
    }
    void OnInventory_Released(InputAction.CallbackContext context)
    {
        HideInventory();
        Time.timeScale = 1.0f;
    }
    void ShowInventory()
    {
        if (!isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            isInventory_Visible = true;
            is_UI_Open = true;
        }
    }
    void HideInventory()
    {
        if (isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isInventory_Visible = false;
            is_UI_Open = false;
        }
    }

    private void Shift_Selected_Item()
    {
        is_Item_Change = true;
        ShowInventory();
    }

    // Created By KYH -------------------------------------------------------------------
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
        health = health - total_Dmg;
        Player_Health_Bar.fillAmount = (float)health / max_Health;

        if (health <= 0)
        {
            Player_Died();
        }
        else
        {
            On_Player_Damaged?.Invoke();
        }
    }

    private void Player_Died()
    {
        is_Player_Dead = true;
        player_render.enabled = false;

        pause_Manager.Show_Result(true);
    }

    public void Input_Game_Stop(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Current_Player_State == Player_State.Normal)
            {
                if (Time.timeScale == 0.0f)
                {
                    pause_Manager.Pause_Stop();
                    Time.timeScale = 1.0f;
                }
                else if (Time.timeScale == 1.0f)
                {
                    pause_Manager.Pause_Start();
                    Time.timeScale = 0.0f;
                }
            }
        }
    }
    // --------------------------------------------------------------------------------

    // ======================================================================================================

    // Player Collsion ======================================================================================

    private int i_platform = 0;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("OneWayPlatform"))
        {
            bool was_Ground = isGrounded;

            isGrounded = true;
            i_platform++;

            if (other.gameObject.CompareTag("OneWayPlatform"))
            {
                current_Platform = other.gameObject;
            }

            Now_New_Platform = other.gameObject; //Reset condition

            if (!was_Ground)
            {
                has_Jumped = false;
                jumpCount = 0;
            }
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("OneWayPlatform"))
        {
            i_platform--;

            if (i_platform <= 0)
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
        }

        if (other.CompareTag("CM_Boundary"))
        {
            cur_Cinemachine_Collider = other;
            camera_Manager.Update_Confiner(cur_Cinemachine_Collider);
        }

        if(other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = true;
            Now_Contact_Npc = other.gameObject;
        }        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card" || other.gameObject.tag == "Chest" || other.gameObject.tag == "Market_Item")
        {
            current_Item = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == true)
        {
            map_Manager.IsOnPortal = false;

            use_Portal = false;
        }

        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card" || other.gameObject.tag == "Chest" || other.gameObject.tag == "Market_Item")
        {
            current_Item = null;
        }

        if (other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = false;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platform_Collider = current_Platform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(player_Collider, platform_Collider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(player_Collider, platform_Collider, false);
    }

    public void Weak_Knock_Back(int Left_Num, float Knock_Back_time, float Power) //Left = 1, Right = -1
    {
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
}


