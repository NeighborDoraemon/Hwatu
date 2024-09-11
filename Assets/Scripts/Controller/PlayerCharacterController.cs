using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerCharacterController : PlayerCharacterCardManager
{
    Rigidbody2D rb;

    Vector2 movement = new Vector2();
    int jumpCount = 0; // ���� Ƚ�� ī���� ����
    public int maxJumpCount = 2; // �ִ� ���� Ƚ�� ī���� ����

    [Header("Player_Teleport")]
    public float teleporting_Distance = 3.0f; // �����̵� �Ÿ� ����
    public float teleporting_CoolTime = 3.0f; // �����̵� ��Ÿ�� ����
    bool canTeleporting = true; // �����̵� ���� Ȯ�� ����

    GameObject current_Item; // ���� ������ Ȯ�� ����

    [Header("Player_Long_Range_Attack")] // ���Ÿ� ���� ���� ����
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10.0f; // �Ѿ� �ӵ�

    [Header("Player_Short_Range_Attack")]
    public float attackRange = 0.5f; // �÷��̾�� ���� ���� �� �Ÿ� ���� ����
    public float attackCooldown = 1.0f; // ���� ���� ��Ÿ�� ����
    private float last_Attack_Time = 0f; // ������ ���� ���� ��� ����

    [Header("Map_Manager")]
    [SerializeField]
    private Map_Manager map_Manager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(); // �̵�
        Jump();
        Teleportation();

        InterAction(); // ��ȣ�ۿ�

        Long_Range_Attack(); // ����
        Short_Range_Attack();

        if (!isCombDone) // ī�� ���� Ȯ�� ����
        {
            Card_Combination();
        }
    }

    void Move() // ĳ���� x��ǥ �̵�
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }

        movement.Normalize();
        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    void Jump() // ĳ���� y��ǥ ����
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void InterAction() // �÷��̾� ĳ���� ��ȣ�ۿ�
    {
        if (Input.GetKeyDown(KeyCode.E) && current_Item != null)
        {
            //Debug.Log("�ش� �����۰� ��ȣ�ۿ� : " + current_Item.name);

            if(current_Item.tag == "Card")
            {
                //Debug.Log("�ش� ī�� ȹ�� �õ� : " + current_Item.name);

                if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                {
                    Debug.Log("ī�� ����");
                    AddCard(current_Item);
                    current_Item.SetActive(false);
                }
            }
            else if (current_Item.tag == "Item")
            {

            }
            current_Item = null;
        }
    }

    void Teleportation() // �÷��̾� ĳ���� �����̵� �Լ�
    {
        if (Input.GetKeyDown(KeyCode.D) && canTeleporting)
        {
            if (movement.x < 0)
            {
                transform.Translate(Vector2.left * teleporting_Distance);
            }
            else
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

    void Short_Range_Attack()
    {
        if (Time.time >= last_Attack_Time + attackCooldown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // ���� ���� �ڽ� �ݶ��̴� ����
                Vector2 boxSize = new Vector2(0.2f, 0.4f);
                Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");

                Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, enemyLayer); // �� ���̾� ����

                // ������ ������ ������ ó��
                foreach (Collider2D enemy in hitEnemies)
                {
                    Debug.Log("���� ���� �� ����");

                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }

                last_Attack_Time = Time.time; // ������ ���� �ð� ���� �ð����� ����
            }
        }
    }

    void Long_Range_Attack() // ���Ÿ� ���� �Լ�
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

            Vector2 shootDirection = (transform.localScale.x < 0) ? Vector2.left : Vector2.right;
            projectileRb.velocity = shootDirection * projectileSpeed;

            Destroy(projectile, 5.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            jumpCount = 0;
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
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card")
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

        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card")
        {
            current_Item = null;
        }
    }

    private void OnDrawGizmosSelected() // ����׿� ���� ���� �׸���
    {
        Vector2 boxSize = new Vector2(0.2f, 0.4f);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}


