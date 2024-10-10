using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PlayerCharacter_Controller : PlayerCharacter_Card_Manager
{
    private Player_InputActions inputActions;

    Rigidbody2D rb;

    Vector2 movement = new Vector2();
    int jumpCount = 0; // ���� Ƚ�� ī���� ����
    public int maxJumpCount = 2; // �ִ� ���� Ƚ�� ī���� ����

    [Header("Player_UI")]
    [SerializeField] private Image Player_Health_Bar;
    [SerializeField] private Pause_Manager pause_Manager;
    [SerializeField] private SpriteRenderer player_render;

    [Header("�ڷ���Ʈ")]
    public float teleporting_Distance = 3.0f; // �����̵� �Ÿ� ����
    public float teleporting_CoolTime = 3.0f; // �����̵� ��Ÿ�� ����
    bool canTeleporting = true; // �����̵� ���� Ȯ�� ����

    GameObject current_Item; // ���� ������ Ȯ�� ����

    [Header("��ų ����")] // ���Ÿ� ���� ���� ����
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10.0f; // �Ѿ� �ӵ�
    public float skillCooldown = 1.0f;
    public float lastSkillTime = 0f;

    [Header("���� ����")]
    public float attackRange = 0.5f; // �÷��̾�� ���� ���� �� �Ÿ� ���� ����
    public float comboTime = 0.5f; // �޺� ���� ���ѽð�
    public float attackCooldown = 1.0f; // ���� ���� ��Ÿ�� ����
    private float last_Attack_Time = 0f; // ������ ���� ���� ��� ����
    private float last_ComboAttck_Time = 0f; // ������ �޺����� ���� ��� ����
    bool isAttacking = false;
    int attackPhase = 0; // ���� �ܰ� ����
    public float hitCollider_x = 0.2f;
    public float hitCollider_y = 0.4f;

    [Header("���� ��ȯ")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;

    private Animator animator;

    [Header("�ִϸ��̼� - ����")]
    public Transform groundCheck;
    public float gorundCheck_Radius = 0.1f;
    public LayerMask groundLayer;
    bool isGrounded;


    //platform & Collider
    private GameObject current_Platform;
    [SerializeField]
    private CapsuleCollider2D player_Collider;
    private bool is_Down_Performed = false;

    private bool is_Player_Dead = false; // ���ó��

    //NPC
    private bool is_Npc_Contack = false; // NPC ����
    private GameObject Now_Contact_Npc;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        //inputActions = new Player_InputActions();

        //inputActions.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        //inputActions.Player.Move.canceled += ctx => movement = Vector2.zero;

        //inputActions.Player.Jump.performed += ctx => Jump();

        //inputActions.Player.Teleportation.performed += ctx => Teleportation();

        //inputActions.Player.Attack.performed += ctx => Short_Range_Attack();

        //inputActions.Player.Skill.performed += ctx => Skill_Attack();

        //inputActions.Player.InterAction.performed += ctx => InterAction();

        //inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();
    }
    //private void OnEnable()
    //{
    //    inputActions.Player.Enable();
    //}
    //private void OnDisable()
    //{
    //    inputActions.Player.Disable();
    //}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1.0f && !is_Player_Dead) // �Ͻ����� ���� �߰�
        {
            Move();
            if (!isCombDone) // ī�� ���� Ȯ�� ����
            {
                Card_Combination();
            }

            Update_Animation_Parameters();
            HandleCombo();
        }
    }

    void Update_Animation_Parameters() // �ִϸ��̼� ���� �Լ�
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

    void Move() // ĳ���� x��ǥ �̵�
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

    void Jump() // ĳ���� y��ǥ ����
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

            Debug.Log("��ȣ�ۿ� ȣ��");
            if (is_Npc_Contack && Now_Contact_Npc != null)
            {
                Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().UI_Start();
            }

            if (current_Item != null)
            {
                Debug.Log("������ Ȯ��");
                if (current_Item.tag == "Card")
                {
                    if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                    {
                        AddCard(current_Item);
                    }
                }
                else if (current_Item.tag == "Chest")
                {
                    Debug.Log("���ڿ� ��ȣ�ۿ�");
                    CardBox chest = current_Item.GetComponent<CardBox>();
                    if (chest != null)
                    {
                        chest.Request_Spawn_Cards();
                    }
                }
                else if (current_Item.tag == "Item")
                {
                    // �����۰� ��ȣ�ۿ�
                }
                current_Item = null;
            }
        }
    }

    void InterAction() // �÷��̾� ĳ���� ��ȣ�ۿ�
    {
        Debug.Log("��ȣ�ۿ� ȣ��");

        if (current_Item != null)
        {
            Debug.Log("������ Ȯ��");
            if (current_Item.tag == "Card")
            {
                if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                {
                    AddCard(current_Item);
                }
            }
            else if (current_Item.tag == "Chest")
            {
                Debug.Log("���ڿ� ��ȣ�ۿ�");
                CardBox chest = current_Item.GetComponent<CardBox>();
                if (chest != null)
                {
                    chest.Request_Spawn_Cards();
                }
            }
            else if (current_Item.tag == "Item")
            {
                // �����۰� ��ȣ�ۿ�
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

            if (!canTeleporting)
            {
                teleporting_CoolTime -= Time.deltaTime;
                if (teleporting_CoolTime <= 0.0f)
                {
                    teleporting_CoolTime = 3.0f;
                    canTeleporting = true;
                    Debug.Log("�����̵� ����");
                }
            }
        }
    }

    void Teleportation() // �÷��̾� ĳ���� �����̵� �Լ�
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
                Debug.Log("�����̵� ����");
            }
        }
    }

    public void Input_Short_Range_Attack(InputAction.CallbackContext ctx)
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
                //Debug.Log("ù ��° ���� Ʈ����");
            }
            else if (isAttacking && attackPhase == 1 && Time.time - last_ComboAttck_Time <= comboTime)
            {
                animator.SetTrigger("Attack_2");
                attackPhase = 2;
                last_ComboAttck_Time = Time.time;
                //Debug.Log("�� ��° ���� Ʈ����");
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
            //Debug.Log("ù ��° ���� Ʈ����");
        }
        else if (isAttacking && attackPhase == 1 && Time.time - last_ComboAttck_Time <= comboTime)
        {
            animator.SetTrigger("Attack_2");
            attackPhase = 2;
            last_ComboAttck_Time = Time.time;
            //Debug.Log("�� ��° ���� Ʈ����");
        }
    }

    void HandleCombo() // �޺� ������ �Լ�
    {
        if (isAttacking)
        {
            if (Time.time - last_ComboAttck_Time > comboTime)
            {
                isAttacking = false;
                attackPhase = 0;
                //Debug.Log("�޺� ����");
            }
        }
    }

    public void ResetAttack() // �ִϸ��̼� ȣ��� �̺�Ʈ �Լ�
    {
        isAttacking = false;
        attackPhase = 0;

    }
    public void ResetCombo() // �ִϸ��̼� ȣ��� �̺�Ʈ �Լ�
    {
        //Debug.Log("�����޺� �Լ� ȣ��");
        if (attackPhase < 2)
        {
            isAttacking = false;
            attackPhase = 0;
            animator.ResetTrigger("Attack_2");
        }
    }

    public void Check_Enemies_Collider() // �ִϸ��̼� ȣ��� �� ���� �Լ�
    {
        // ���� ���� �ڽ� �ݶ��̴� ����
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, enemyLayer); // �� ���̾� ����

        // ������ ������ ������ ó��
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("���� ���� �� ����");

            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }
    
    public void Input_Skill_Attack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started && Time.timeScale == 1.0f && !is_Player_Dead)
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
    }

    void Skill_Attack() // ���Ÿ� ���� �Լ�
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

    void Spawn_Chest() // �ӽ� ����� �� ���� ��ȯ �Լ��Դϴ�.
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
            Now_Contact_Npc.GetComponent<Stat_Npc_Controller>().Btn_Exit(); //NPC �߰��Ǹ� �ٲ��� �� / �ƴϸ� ���� NPC�ڵ带 �θ�� �ΰ� �Ļ����Ѽ� ȣ���ص� ��
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            current_Platform = collision.gameObject;

            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWayPlatform"))
        {
            current_Platform = null;
        }
    }

    private void OnDrawGizmosSelected() // ����׿� ���� ���� �׸���
    {
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    public void Player_Take_Damage(int Damage)
    {
        Debug.Log("�÷��̾� ������ ���");
        health = health - Damage;
        Player_Health_Bar.fillAmount = (float)health / Max_Health;
        Debug.Log("��� �Ϸ�");

        if (health <= 0)
        {
            //���ó��
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
            Debug.Log("Down ����");
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


