using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerCharacter_Controller : PlayerCharacter_Card_Manager
{
    private Player_InputActions inputActions;

    Rigidbody2D rb;

    Vector2 movement = new Vector2();
    int jumpCount = 0; // 점프 횟수 카운팅 변수
    public int maxJumpCount = 2; // 최대 점프 횟수 카운팅 변수

    [Header("텔레포트")]
    public float teleporting_Distance = 3.0f; // 순간이동 거리 변수
    public float teleporting_CoolTime = 3.0f; // 순간이동 쿨타임 변수
    bool canTeleporting = true; // 순간이동 조건 확인 변수

    GameObject current_Item; // 현재 아이템 확인 변수

    [Header("스킬 공격")] // 원거리 공격 변수 관리
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10.0f; // 총알 속도
    public float skillCooldown = 1.0f;
    public float lastSkillTime = 0f;

    [Header("근접 공격")]
    public float attackRange = 0.5f; // 플레이어와 공격 범위 간 거리 설정 변수
    public float comboTime = 0.5f; // 콤보 공격 제한시간
    public float attackCooldown = 1.0f; // 근접 공격 쿨타임 변수
    private float last_Attack_Time = 0f; // 마지막 공격 시점 기록 변수
    private float last_ComboAttck_Time = 0f; // 마지막 콤보어택 시점 기록 변수
    bool isAttacking = false;
    int attackPhase = 0; // 공격 단계 변수
    public float hitCollider_x = 0.2f;
    public float hitCollider_y = 0.4f;

    [Header("상자 소환")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;

    private Animator animator;

    [Header("애니메이션 - 점프")]
    public Transform groundCheck;
    public float gorundCheck_Radius = 0.1f;
    public LayerMask groundLayer;
    bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputActions = new Player_InputActions();

        inputActions.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movement = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Teleportation.performed += ctx => Teleportation();

        inputActions.Player.Attack.performed += ctx => Short_Range_Attack();

        inputActions.Player.Skill.performed += ctx => Skill_Attack();

        inputActions.Player.InterAction.performed += ctx => InterAction();

        inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
    }
    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (!isCombDone) // 카드 조합 확인 조건
        {
            Card_Combination();
        }

        Update_Animation_Parameters();
        HandleCombo();
    }

    void Update_Animation_Parameters() // 애니메이션 관리 함수
    {
        bool isMoving = Mathf.Abs(movement.x) > 0.01f;
        animator.SetBool("isMove", isMoving);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, gorundCheck_Radius, groundLayer);
        if (isGrounded)
        {
            jumpCount = 0;
        }    
        animator.SetBool("isGrounded", isGrounded);

        animator.SetFloat("vertical_Velocity", rb.velocity.y);
    }

    void Move() // 캐릭터 x좌표 이동
    {
        if (!isAttacking)
        {
            if (movement.x < 0)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (movement.x > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }

            movement.Normalize();
            rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
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

    void InterAction() // 플레이어 캐릭터 상호작용
    {
        Debug.Log("상호작용 호출");

        if (current_Item != null)
        {
            Debug.Log("아이템 확인");
            if (current_Item.tag == "Card")
            {
                if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                {
                    AddCard(current_Item);
                }
            }
            else if (current_Item.tag == "Chest")
            {
                Debug.Log("상자와 상호작용");
                CardBox chest = current_Item.GetComponent<CardBox>();
                if (chest != null)
                {
                    chest.Request_Spawn_Cards();
                }
            }
            else if (current_Item.tag == "Item")
            {
                // 아이템과 상호작용
            }
            current_Item = null;
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

    void Short_Range_Attack()
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

    void HandleCombo() // 콤보 관리용 함수
    {
        if (isAttacking)
        {
            if (Time.time - last_ComboAttck_Time > comboTime)
            {
                isAttacking = false;
                attackPhase = 0;
                //Debug.Log("콤보 리셋");
            }
        }
    }

    public void ResetAttack() // 애니메이션 호출용 이벤트 함수
    {
        isAttacking = false;
        attackPhase = 0;

    }
    public void ResetCombo() // 애니메이션 호출용 이벤트 함수
    {
        //Debug.Log("리셋콤보 함수 호출");
        if (attackPhase < 2)
        {
            isAttacking = false;
            attackPhase = 0;
            animator.ResetTrigger("Attack_2");
        }
    }

    public void Check_Enemies_Collider() // 애니메이션 호출용 적 감지 함수
    {
        // 공격 범위 박스 콜라이더 설정
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
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

    void Skill_Attack() // 원거리 공격 함수
    {
        if (!isGrounded) return;

        if (Time.time >= lastSkillTime + skillCooldown)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            Vector2 shootDirection = (transform.localScale.x < 0) ? Vector2.left : Vector2.right;
            projectileRb.velocity = shootDirection * projectileSpeed;

            //animator.SetTrigger("Attack_1");

            lastSkillTime = Time.time;
        }
    }

    void Spawn_Chest() // 임시 디버깅 용 상자 소환 함수입니다.
    {
        GameObject chest = Instantiate(chestPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == false)
        {
            map_Manager.IsOnPortal = true;
            map_Manager.Which_Portal = other.gameObject;
            map_Manager.v_Now_Portal = other.transform.position;
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
    }

    private void OnDrawGizmosSelected() // 디버그용 공격 범위 그리기
    {
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}


