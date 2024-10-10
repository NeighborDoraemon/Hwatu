using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter_Controller : PlayerChar_Inventory_Manager
{
    private Player_InputActions inputActions;
    [SerializeField]
    private GameObject inventoryPanel;
    private bool isInventory_Visible = false;

    Rigidbody2D rb;
    GameObject current_Item; // ���� ������ Ȯ�� ����

    Vector2 movement = new Vector2();
    int jumpCount = 0; // ���� Ƚ�� ī���� ����
    public int maxJumpCount = 2; // �ִ� ���� Ƚ�� ī���� ����
    bool canTeleporting = true; // �����̵� ���� Ȯ�� ����

    [Header("���� ��ȯ")]
    public GameObject chestPrefab;
    public Transform spawnPoint;

    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        inputActions = new Player_InputActions();

        inputActions.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => movement = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Teleportation.performed += ctx => Teleportation();

        inputActions.Player.Attack.performed += ctx => Perform_Attack();

        inputActions.Player.Skill.performed += ctx => Skill_Attack();

        inputActions.Player.InterAction.performed += ctx => InterAction();

        inputActions.Player.SpawnChest.performed += ctx => Spawn_Chest();

        attack_Strategy = new Base_Attack_Strategy(this);
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
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("�ν����� â�� �κ��丮 �г� ����");
        }
    }
    // Update is called once per frame
    void Update()
    {
        Move();

        if (!isCombDone) // ī�� ���� Ȯ�� ����
        {
            Card_Combination();
        }

        Update_Animation_Parameters();
        HandleCombo();
        Handle_Teleportation_Time();
    }
    void Update_Animation_Parameters() // �ִϸ��̼� ���� �Լ�
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

    void Move() // ĳ���� x��ǥ �̵�
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

    void InterAction() // �÷��̾� ĳ���� ��ȣ�ۿ�
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
                Item item = current_Item.GetComponent<Item_Prefab>().itemData;  // ������ ������ �������� (���� �������� ScriptableObject)

                if (item != null)
                {
                    // �÷��̾��� �κ��丮�� ������ �߰�
                    AddItem(item);

                    // �������� ȿ�� ����
                    item.ApplyEffect(this);

                    // �������� ������ ����
                    Destroy(current_Item);
                }
                else
                {
                    Debug.LogWarning("������ �����͸� ã�� �� �����ϴ�.");
                }
            }
            current_Item = null;
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
                Debug.Log("�����̵� ����");
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

    void HandleCombo() // �޺� ������ �Լ�
    {
        if (isAttacking)
        {
            if (Time.time - last_ComboAttck_Time > comboTime)
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

    public void ResetAttack() // �ִϸ��̼� ȣ��� �̺�Ʈ �Լ�
    {
        isAttacking = false;
        cur_AttackCount = 0;
        Debug.Log("���� ����");
    }
    public void ResetCombo() // �ִϸ��̼� ȣ��� �̺�Ʈ �Լ�
    {
        //Debug.Log("�����޺� �Լ� ȣ��");
        if (cur_AttackCount < max_AttackCount)
        {
            isAttacking = false;
            cur_AttackCount = 0;
            Debug.Log("���� ����");
        }
    }

    public void Check_Enemies_Collider(string hixBox_Values) // �ִϸ��̼� ȣ��� �� ���� �Լ�
    {
        string[] values = hixBox_Values.Split(',');
        float hitBox_x = float.Parse(values[0]);
        float hitBox_y = float.Parse(values[1]);

        // ���� ���� �ڽ� �ݶ��̴� ����
        Vector2 boxSize = new Vector2(hitBox_x, hitBox_y);
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

    void Skill_Attack() // ���Ÿ� ���� �Լ�
    {
        attack_Strategy.Skill();
    }

    void Spawn_Chest() // �ӽ� ����� �� ���� ��ȯ �Լ��Դϴ�.
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
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            isGrounded = false;
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

    private void OnDrawGizmosSelected() // ����׿� ���� ���� �׸���
    {
        Vector2 boxSize = new Vector2(hitCollider_x, hitCollider_y);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}


