using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Mathematics;


// 플레이어 캐릭터 조작 제작자 : 전병준 // 김윤혁이 제작한 부분은 따로 표시
public class PlayerCharacter_Controller : PlayerChar_Inventory_Manager
{
    private Player_InputActions inputActions;
    [SerializeField]
    private GameObject inventoryPanel;
    private bool isInventory_Visible = false;

    public Rigidbody2D rb;
    GameObject current_Item; // 현재 아이템 확인 변수

    Vector2 movement = new Vector2();
    int jumpCount = 0; // 점프 횟수 카운팅 변수
    public int maxJumpCount = 2; // 최대 점프 횟수 카운팅 변수

    [Header("텔레포트")]
    bool canTeleporting = true; // 순간이동 조건 확인 변수

    [Header("상자 소환")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool canAttack = true;
    
    [Header("Cinemachine")]
    private Camera_Manager camera_Manager;
    private Collider2D cur_Boundary_Collider;

    [Header("Weapon_Data")]
    public SpriteRenderer weapon_Render;
    public GameObject weapon_Prefab;
    private Weapon_Collision_Handler weapon_Handler;    
    public SpriteRenderer effect_Render;
    public Transform weapon_Anchor;
    public Transform effect_Anchor;
    private bool is_Facing_Right = true;

    //--------------------------------------------------- 김윤혁 제작
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
    [SerializeField]
    private BoxCollider2D player_Collider;
    private bool is_Down_Performed = false;    

    private bool is_Player_Dead = false; // 사망처리

    //NPC
    private bool is_Npc_Contack = false; // NPC 접촉
    private GameObject Now_Contact_Npc;
    //---------------------------------------------------

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputActions = new Player_InputActions();
        
        //inputActions.Player.Teleportation.performed += ctx => Teleportation();

        //inputActions.Player.Attack.performed += ctx => Perform_Attack();

        //inputActions.Player.InterAction.performed += ctx => InterAction();

        inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();

        Set_Weapon(0);
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
            Debug.LogError("인스펙터 창에 인벤토리 패널 없음");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1.0f && !is_Player_Dead) // 일시정지 조건 추가
        {
            if (isAttacking && isGrounded)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
            {
                Move();
            }

            if (!isCombDone) // 카드 조합 확인 조건
            {
                Card_Combination();
            }
            Update_Animation_Parameters();
            HandleCombo();
            Handle_Teleportation_Time();
        }
    }
    private void LateUpdate()
    {
        Update_WeaponAnchor_Position();
    }

    void Update_Animation_Parameters() // 애니메이션 관리 함수
    {
        bool isMoving = Mathf.Abs(movement.x) > 0.01f;
        animator.SetBool("isMove", isMoving);

        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded && rb.velocity.y == 0)
        {
            jumpCount = 0;
        }

        animator.SetFloat("vertical_Velocity", rb.velocity.y);
    }

    // 이동 조작 ===========================================================================================
    public void Input_Move(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Canceled)
        {
            movement = Vector2.zero;
        }
        else
        {
            movement = ctx.action.ReadValue<Vector2>();
        }
    }

    void Move() // 캐릭터 x좌표 이동
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

        movement.Normalize();
        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    public void Input_Jump(InputAction.CallbackContext ctx)
    {
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
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
                    jumpCount++;
                }
            }
        }
    }
    

    public void Input_Teleportation(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            if (canTeleporting)
            {
                if (movement.x < 0)
                {
                    transform.Translate(Vector2.left * teleporting_Distance);
                }
                else if (movement.x > 0)
                {
                    transform.Translate(Vector2.right * teleporting_Distance);
                }
                animator.SetTrigger("Teleport");
                canTeleporting = false;
            }
        }
    }
    void Handle_Teleportation_Time()
    {
        if (!canTeleporting)
        {
            teleporting_CoolTime -= Time.deltaTime;
            if (teleporting_CoolTime <= 0.0f)
            {
                teleporting_CoolTime = 3.0f;
                canTeleporting = true;
                Debug.Log("순간이동 가능");
            }
        }
    }
    // 김윤혁 제작 ---------------------------------------------------------------
    public void Input_Down_Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed && Time.timeScale == 1.0f)
        {
            Debug.Log("Down 감지");
            is_Down_Performed = true;
            if (current_Platform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            is_Down_Performed = false;
        }
    }
    // ----------------------------------------------------------------------------
    // ======================================================================================================

    // 상호작용 조작 ========================================================================================
    public void Input_Interaction(InputAction.CallbackContext ctx)
    {
        // 김윤혁 제작 -------------------------------------------------------------------------------------
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            map_Manager.Use_Portal();

            //Debug.Log("상호작용 호출");
            if (is_Npc_Contack && Now_Contact_Npc != null)
            {
                if(Now_Contact_Npc.gameObject.name == "Stat_Npc")
                {
                    Debug.Log("Start_Npc");
                    Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().UI_Start();
                }
                else if(Now_Contact_Npc.gameObject.name == "Start_Card_Npc")
                {
                    Now_Contact_Npc.GetComponent<Start_Card_Npc>().Request_Spawn_Cards();
                }                
            }
        // ---------------------------------------------------------------------------------------------------
            if (current_Item != null)
            {
                //Debug.Log("아이템 확인");
                if (current_Item.tag == "Card")
                {
                    if(cardCount == 0) // 시작 첫 카드 획득 시 획득한 카드 꺼지도록 하기 (윤혁 임시)
                    {
                        AddCard(current_Item);
                        current_Item.gameObject.SetActive(false);
                    }
                    else if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                    {
                        AddCard(current_Item);
                    }
                }
                else if (current_Item.tag == "Chest")
                {
                    Debug.Log("상자와 상호작용");
                    Spawn_Box chest = current_Item.GetComponent<Spawn_Box>();
                    Card_Spawn_Box debug_Chest = current_Item.GetComponent<Card_Spawn_Box>();
                    if (chest != null)
                    {
                        chest.Request_Spawn_Cards();
                    }
                    else if (debug_Chest != null)
                    {
                        debug_Chest.Request_Spawn_Cards();
                    }
                }
                else if (current_Item.tag == "Item")
                {
                    // 아이템과 상호작용
                    Debug.Log("아이템 사용 시도");
                    Item item = current_Item.GetComponent<Item_Prefab>().itemData;  // 아이템 데이터 가져오기 (현재 아이템의 ScriptableObject)

                    if (item != null)
                    {
                        Debug.Log($"아이템 적용 {item.name}");
                        if (item.isConsumable)
                        {                            
                            if (this != null) 
                            {
                                item.ApplyEffect(this);
                                Debug.Log($"아이템 효과 적용");
                            }
                            Destroy(current_Item);
                        }
                        else
                        {
                            AddItem(item);

                            Destroy(current_Item);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("아이템 데이터를 찾을 수 없습니다.");
                    }
                    Object_Manager.instance.Destroy_All_Cards_And_Items();
                }
                current_Item = null;
            }
        }
    }

    void Spawn_Chest() // 임시 디버깅 용 상자 소환 함수입니다.
    {
        GameObject chest = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation);
    }
    // ======================================================================================================

    // 무기 =================================================================================================
    public override void Set_Weapon(int weaponIndex)
    {
        base.Set_Weapon(weaponIndex);

        if (cur_Weapon_Data == null)
        {
            Debug.LogError("무기 데이터가 올바르게 설정되지 않았습니다.");
            return;
        }

        if (cur_Weapon_Data.effect_Data == null)
        {
            Debug.LogError("무기 이펙트 데이터가 설정되지 않았습니다.");
            return;
        }
        
        if (weapon_Anchor.childCount > 0)
        {
            foreach(Transform child in weapon_Anchor)
            {
                Destroy(child.gameObject);
            }
        }
        
        if (cur_Weapon_Data.weapon_Prefab)
        {
            weapon_Prefab = cur_Weapon_Data.weapon_Prefab;

            GameObject new_Weapon = Instantiate(cur_Weapon_Data.weapon_Prefab, weapon_Anchor);
            new_Weapon.transform.localPosition = Vector3.zero;
            new_Weapon.transform.localRotation = Quaternion.identity;

            weapon_Handler = new_Weapon.GetComponent<Weapon_Collision_Handler>();

            if (weapon_Handler != null)
            {
                weapon_Handler.Set_Damage(cur_Weapon_Data.attack_Damage);
                Debug.Log($"무기 데미지 설정 완료 : {cur_Weapon_Data.attack_Damage}");
            }
            else
            {
                Debug.LogError("Weapon_Collision_Handler가 프리팹에 없습니다");
            }
        }     

        attack_Strategy = cur_Weapon_Data.attack_Strategy as IAttack_Strategy;

        if (attack_Strategy == null)
        {
            Debug.LogError($"'{cur_Weapon_Data.weapon_Name}'에 대한 공격 전략이 유효하지 않습니다.");            
            return;
        }

        Debug.Log($"공격 전략 '{attack_Strategy.GetType().Name}' 설정 완료.");
        
        Apply_Weapon_Data();
    }

    private void Apply_Weapon_Data()
    {
        if (cur_Weapon_Data != null)
        {
            attack_Strategy = cur_Weapon_Data.attack_Strategy as IAttack_Strategy;

            if (cur_Weapon_Data.overrideController != null)
            {
                animator.runtimeAnimatorController = cur_Weapon_Data.overrideController;
                Debug.Log($"애니메이터 컨트롤러 변경 : {cur_Weapon_Data.overrideController}");
            }

            attackDamage = cur_Weapon_Data.attack_Damage;
            attack_Cooldown = cur_Weapon_Data.attack_Cooldown;
            max_AttackCount = cur_Weapon_Data.max_Attack_Count;

            isAttacking = false;
            cur_AttackCount = 0;

            animator.SetTrigger("Change_Weapon");
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

        AnimatorStateInfo state_Info = animator.GetCurrentAnimatorStateInfo(0);
        string cur_Animation_Name = Get_Cur_Animation_Name(animator);

        var animation_Data = cur_Weapon_Data.animation_Pos_Data_List.Find(
            data => string.Equals(data.animation_Name, cur_Animation_Name, StringComparison.OrdinalIgnoreCase)
        );

        //Debug.Log($"Looking for Animation Name: '{cur_Animation_Name}'");
        //foreach (var data in cur_Weapon_Data.animation_Pos_Data_List)
        //{
        //    Debug.Log($"Comparing with: '{data.animation_Name}'");
        //    if (data.animation_Name == cur_Animation_Name)
        //    {
        //        Debug.Log("Match found!");
        //    }
        //}

        if (animation_Data != null)
        {
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                AnimationClip cur_Clip = clipInfo[0].clip;
                int total_Frames = Mathf.RoundToInt(cur_Clip.length * cur_Clip.frameRate);
                int cur_Frame = Mathf.FloorToInt(state_Info.normalizedTime * total_Frames) % total_Frames;

                //Debug.Log("현재 프레임 : " + cur_Frame);

                Vector3 new_Pos = animation_Data.Get_Position(cur_Frame);
                Quaternion new_Rotation = animation_Data.Get_Rotation(cur_Frame);
                

                //Debug.Log("새 위치 : " + new_Pos + "새 회전값" + new_Rotation);

                // 위치와 회전을 강제 적용
                weapon_Anchor.localPosition = is_Facing_Right ? new_Pos : new Vector3(-new_Pos.x, new_Pos.y, new_Pos.z);
                weapon_Anchor.localRotation = is_Facing_Right ? new_Rotation : Quaternion.Inverse(new_Rotation);
                weapon_Anchor.localScale = is_Facing_Right ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            }
            else
            {
                //Debug.LogWarning("No animation clip info found.");
            }
        }
        else
        {
            //Debug.LogWarning("No matching animation data found for current animation.");
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
        Debug.Log("Activate_WeaponCollider called");
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
        if (!int.TryParse(parts[1], out frame_Num))
        {
            return;
        }

        var effect_Info = cur_Weapon_Data.effect_Data.Get_Effect_Info(motion_Name);
        if (effect_Info != null)
        {
            var frame_Effect_Info = effect_Info.frame_Effects.Find(fe => fe.frame_Number == frame_Num);
            if (frame_Effect_Info != null)
            {
                effect_Render.sprite = frame_Effect_Info.effect_Sprites;

                Vector3 effect_Pos = frame_Effect_Info.position_Offset;
                effect_Render.transform.position = is_Facing_Right ? (transform.position + effect_Pos) : (transform.position + new Vector3(-effect_Pos.x, effect_Pos.y, effect_Pos.z));                

                effect_Render.enabled = true;

                Invoke("HideEffect", frame_Effect_Info.duration);
            }
        }
    }

    public void HideEffect()
    {
        effect_Render.enabled = false;
        effect_Render.sprite = null;
    }
    // ======================================================================================================

    // 공격 및 스킬 =========================================================================================
    public void Input_Perform_Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed || Time.timeScale != 1.0f || is_Player_Dead)
        {
            return;
        }

        if (attack_Strategy == null)
        {
            Debug.LogError("공격 전략이 설정되지 않았습니다.");
            return;
        }

        if (cur_AttackCount >= max_AttackCount)
        {
            Debug.Log("최대 공격 횟수 도달");
            return;
        }

        attack_Strategy.Attack(this, cur_Weapon_Data);
    }

    public void On_Shoot_Projectile(GameObject projectile_Prefab)
    {
        Debug.Log("투사체 발사 이벤트 호출");

        if (attack_Strategy != null)
        {
            attack_Strategy.Shoot(this, projectile_Prefab, firePoint);
            Debug.Log("총알 발사 완료");
        }
        else
        {
            Debug.LogError("공격 전략이 설정되지 않음");
        }
    }

    void HandleCombo() // 콤보 관리용 함수
    {
        if (isAttacking)
        {
            if (Time.time - last_ComboAttack_Time > comboTime)
            {
                isAttacking = false;
                cur_AttackCount = 0;
            }
        }
    }

    public void ResetAttack() // 애니메이션 호출용 이벤트 함수
    {
        isAttacking = false;
        cur_AttackCount = 0;        
    }
    public void ResetCombo() // 애니메이션 호출용 이벤트 함수
    {
        //Debug.Log("리셋콤보 함수 호출");
        if (cur_AttackCount < max_AttackCount)
        {
            isAttacking = false;
            cur_AttackCount = 0;
            //Debug.Log("공격 리셋");
        }
    }

    public void Check_Enemies_Collider(string hixBox_Values) // 애니메이션 호출용 적 감지 함수
    {
        string[] values = hixBox_Values.Split(',');
        float hitBox_x = float.Parse(values[0]);
        float hitBox_y = float.Parse(values[1]);

        // 공격 범위 박스 콜라이더 설정
        Vector2 boxSize = new Vector2(hitBox_x, hitBox_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, enemyLayer); // 적 레이어 설정
        
        // 감지된 적에게 데미지 처리
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("근접 공격 적 감지");

            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }
    
    public void Input_Skill_Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            attack_Strategy.Skill(this, cur_Weapon_Data);
        }
    }
    // ======================================================================================================

    // 플레이어 UI ==========================================================================================
    void OnInventory_Pressed(InputAction.CallbackContext context)
    {
        ShowInventory();
    }
    void OnInventory_Released(InputAction.CallbackContext context)
    {
        HideInventory();
    }
    void ShowInventory()
    {
        if (!isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(true);
            isInventory_Visible = true;
        }
    }
    void HideInventory()
    {
        if (isInventory_Visible && inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
            isInventory_Visible = false;
        }
    }

    // 김윤혁 제작 -------------------------------------------------------------------
    public void Player_Take_Damage(int Damage)
    {
        Debug.Log("플레이어 데미지 계산");
        health = health - Damage;
        Player_Health_Bar.fillAmount = (float)health / max_Health;
        Debug.Log("계산 완료");

        if (health <= 0)
        {
            //사망처리
            Player_Died();
        }
    }

    private void Player_Died()
    {
        is_Player_Dead = true;
        player_render.enabled = false;

        pause_Manager.Show_Result();
    }

    public void Input_Game_Stop(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
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

    // 충돌 관련 코드 =======================================================================================
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }
        else if (other.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = true;
            current_Platform = other.gameObject;
        }
    }


    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
        }
        else if (other.gameObject.CompareTag("OneWayPlatform"))
        {
            isGrounded = false;
            current_Platform = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == false)
        {
            map_Manager.IsOnPortal = true;
            map_Manager.Which_Portal = other.gameObject;
            map_Manager.v_Now_Portal = other.transform.position;

            use_Portal = true;
        }

        if (other.CompareTag("Boundary"))
        {
            cur_Boundary_Collider = other;
            camera_Manager.Update_Confiner(cur_Boundary_Collider);
            Debug.Log("맵 안에 있음");
        }

        if(other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = true;
            Now_Contact_Npc = other.gameObject;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card" || other.gameObject.tag == "Chest")
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

        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card" || other.gameObject.tag == "Chest")
        {
            current_Item = null;
        }

        if (other.CompareTag("Boundary"))
        {
            if (use_Portal)
            {
                return;
            }

            Debug.Log("맵 벗어나려 함! 가까운 위치로 이동");
            Vector2 closet_Point = Get_Closet_Point(transform.position);
            transform.position = closet_Point;
        }

        if(other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = false;
            if (Now_Contact_Npc.gameObject.name == "Stat_Npc")
            { 
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().Btn_Exit(); 
            } //NPC 추가되면 바뀌어야 함 / 아니면 현재 NPC코드를 부모로 두고 파생시켜서 호출해도 됨
        }
    }

    private Vector2 Get_Closet_Point(Vector2 position)
    {
        if (cur_Boundary_Collider != null)
        {
            return cur_Boundary_Collider.ClosestPoint(position);
        }
        return position;
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platform_Collider = current_Platform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(player_Collider, platform_Collider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(player_Collider, platform_Collider, false);
    }

    private void OnDrawGizmosSelected() // 디버그용 공격 범위 그리기
    {
        Vector2 boxSize = new Vector2(0.3f, 0.5f);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
    // =========================================================================================================
}


