using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerCharacter_Controller : PlayerChar_Inventory_Manager
{
    private Player_InputActions inputActions;
    [SerializeField]
    private GameObject inventoryPanel;
    private bool isInventory_Visible = false;

    Rigidbody2D rb;
    GameObject current_Item; // 현재 아이템 확인 변수

    Vector2 movement = new Vector2();
    int jumpCount = 0; // 점프 횟수 카운팅 변수
    public int maxJumpCount = 2; // 최대 점프 횟수 카운팅 변수

    [Header("Player_UI")]
    [SerializeField] private Image Player_Health_Bar;
    [SerializeField] private Pause_Manager pause_Manager;
    [SerializeField] private SpriteRenderer player_render;

    [Header("텔레포트")]
    bool canTeleporting = true; // 순간이동 조건 확인 변수

    [Header("상자 소환")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;
    [HideInInspector]
    public bool isGrounded;


    //platform & Collider
    private GameObject current_Platform;
    [SerializeField]
    private CapsuleCollider2D player_Collider;
    private bool is_Down_Performed = false;

    private bool is_Player_Dead = false; // 사망처리

    //NPC
    private bool is_Npc_Contack = false; // NPC 접촉
    private GameObject Now_Contact_Npc;


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
    //private void OnEnable()
    //{
    //    inputActions.Player.Enable();
    //}
    //private void OnDisable()
    //{
    //    inputActions.Player.Disable();
    //}

    private void Start()
    {
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
            Move();
            if (!isCombDone) // 카드 조합 확인 조건
            {
                Card_Combination();

            }
            Update_Animation_Parameters();
            HandleCombo();
            Handle_Teleportation_Time();
        }
    }

    void Update_Animation_Parameters() // 애니메이션 관리 함수
    {
        bool isMoving = Mathf.Abs(movement.x) > 0.01f;
        animator.SetBool("isMove", isMoving);

        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded && rb.velocity.y <= 0.1f)
        {
            jumpCount = 0;
        }

        animator.SetFloat("vertical_Velocity", rb.velocity.y);
    }
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
            transform.localScale = new Vector3(-0.7f, 0.7f, 1f);
            
        }
        else if (movement.x > 0)
        {
            transform.localScale = new Vector3(0.7f, 0.7f, 1f);
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
                if (isAttacking) return;

                if (jumpCount < maxJumpCount)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
                    jumpCount++;
                }
            }
        }
    }

    void Jump() // 캐릭터 y좌표 점프
    {
        if (isAttacking) return; 

        if (jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    public void Input_Interaction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            map_Manager.Use_Portal();

            Debug.Log("상호작용 호출");
            if (is_Npc_Contack && Now_Contact_Npc != null)
            {
                if(Now_Contact_Npc.gameObject.name == "Start_Npc")
                {
                    Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().UI_Start();
                }
                else if(Now_Contact_Npc.gameObject.name == "Start_Card_Npc")
                {
                    Now_Contact_Npc.GetComponent<Start_Card_Npc>().Request_Spawn_Cards();
                }
                
            }

            if (current_Item != null)
            {
                Debug.Log("아이템 확인");
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
                    if (chest != null)
                    {
                        chest.Request_Spawn_Cards();
                    }
                }
                else if (current_Item.tag == "Item")
                {
                    // 아이템과 상호작용
                    Item item = current_Item.GetComponent<Item_Prefab>().itemData;  // 아이템 데이터 가져오기 (현재 아이템의 ScriptableObject)

                    if (item != null)
                    {
                        // 플레이어의 인벤토리에 아이템 추가
                        AddItem(item);

                        // 아이템의 효과 적용
                        item.ApplyEffect(this);

                        // 아이템을 씬에서 삭제
                        Destroy(current_Item);
                    }
                    else
                    {
                        Debug.LogWarning("아이템 데이터를 찾을 수 없습니다.");
                    }
                }
                current_Item = null;
            }
        }
    }

    void InterAction() // 플레이어 캐릭터 상호작용
    {
        if (current_Item != null)
        {
            if (current_Item.tag == "Card")
            {
                if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                {
                    AddCard(current_Item);
                }
            }
            else if (current_Item.tag == "Chest")
            {
                Spawn_Box chest = current_Item.GetComponent<Spawn_Box>();
                if (chest != null)
                {
                    chest.Request_Spawn_Cards();
                }
            }
            else if (current_Item.tag == "Item")
            {
                Item item = current_Item.GetComponent<Item_Prefab>().itemData;  // 아이템 데이터 가져오기 (현재 아이템의 ScriptableObject)

                if (item != null)
                {
                    // 플레이어의 인벤토리에 아이템 추가
                    AddItem(item);

                    // 아이템의 효과 적용
                    item.ApplyEffect(this);

                    // 아이템을 씬에서 삭제
                    Destroy(current_Item);
                }
                else
                {
                    Debug.LogWarning("아이템 데이터를 찾을 수 없습니다.");
                }
            }
            current_Item = null;
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
                canTeleporting = false;
            }
        }
    }

    void Teleportation() // 플레이어 캐릭터 순간이동 함수
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
            canTeleporting = false;
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

    /*public void Input_Short_Range_Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            if (!isGrounded) return;

            if (Time.time >= last_Attack_Time + attackCooldown)
            {
                animator.SetTrigger("Attack_1");
                isAttacking = true;
                attackPhase = 1;
                last_ComboAttck_Time = Time.time;
                last_Attack_Time = Time.time;
                //Debug.Log("첫 번째 공격 트리거");
            }
            else if (isAttacking && attackPhase == 1 && Time.time - last_ComboAttck_Time <= comboTime)
            {
                animator.SetTrigger("Attack_2");
                attackPhase = 2;
                last_ComboAttck_Time = Time.time;
                //Debug.Log("두 번째 공격 트리거");
            }
        }
    }*/
    public void Input_Perform_Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
        {
            Debug.Log("Attack Call");
            if (max_AttackCount > cur_AttackCount)
            {
                Debug.Log("Attack Start");
                attack_Strategy.Attack();
            }
            else
            {
                cur_AttackCount = 0;
            }
        }
    }


    void Perform_Attack()
    {
        if (max_AttackCount > cur_AttackCount)
        {
            attack_Strategy.Attack();
        }
        else
        {
            cur_AttackCount = 0;
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

    public void Set_Max_AttackCount(int count)
    {
        max_AttackCount = count;
        cur_AttackCount = 0;
    }
    public void Set_AttackCooldown(float time)
    {
        attack_Cooldown = time;
    }

    public void ResetAttack() // 애니메이션 호출용 이벤트 함수
    {
        if (cur_AttackCount >= max_AttackCount)
        {
            isAttacking = false;
            cur_AttackCount = 0;
            last_Attack_Time = Time.time;
        }
    }
    public void ResetCombo() // 애니메이션 호출용 이벤트 함수
    {
        //Debug.Log("리셋콤보 함수 호출");
        if (cur_AttackCount < max_AttackCount)
        {
            isAttacking = false;
            cur_AttackCount = 0;
            Debug.Log("공격 리셋");
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
            attack_Strategy.Skill();

            //if (!isGrounded) return;

            //if (Time.time >= lastSkillTime + skillCooldown)
            //{

            //    GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            //    Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            //    Vector2 shootDirection = (transform.localScale.x < 0) ? Vector2.left : Vector2.right;
            //    projectileRb.velocity = shootDirection * projectileSpeed;

            //    animator.SetTrigger("Attack_1");

            //    lastSkillTime = Time.time;
            //}
        }
    }

    void Skill_Attack() // 원거리 공격 함수
    {
        attack_Strategy.Skill();
    }

    void Spawn_Chest() // 임시 디버깅 용 상자 소환 함수입니다.
    {
        GameObject chest = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation);
    }

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
        }

        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card" || other.gameObject.tag == "Chest")
        {
            current_Item = null;
        }

        if(other.gameObject.tag == "NPC")
        {
            is_Npc_Contack = false;
            if (Now_Contact_Npc.gameObject.name == "Start_Npc")
            { 
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().Btn_Exit(); 
            } //NPC 추가되면 바뀌어야 함 / 아니면 현재 NPC코드를 부모로 두고 파생시켜서 호출해도 됨
        }
    }

    private void OnDrawGizmosSelected() // 디버그용 공격 범위 그리기
    {
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

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

    public void Input_Down_Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed && Time.timeScale == 1.0f)
        {
            Debug.Log("Down 감지");
            is_Down_Performed = true;
            //if (current_Platform != null)
            //{
            //    StartCoroutine(DisableCollision());
            //}
        }
        else if(ctx.phase == InputActionPhase.Canceled)
        {
            is_Down_Performed = false;
        }
    }

    private IEnumerator DisableCollision()
    {
        BoxCollider2D platform_Collider = current_Platform.GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(player_Collider, platform_Collider);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(player_Collider, platform_Collider, false);
    }

    public void Input_Game_Stop(InputAction.CallbackContext ctx)
    {
        if(ctx.phase == InputActionPhase.Started)
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
}


