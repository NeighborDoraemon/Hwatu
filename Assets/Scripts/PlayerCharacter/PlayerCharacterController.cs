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

    public float teleportingDistance = 3.0f; // �����̵� �Ÿ� ����
    public float teleportingCoolTime = 3.0f; // �����̵� ��Ÿ�� ����
    bool canTeleporting = true; // �����̵� ���� Ȯ�� ����

    GameObject currentItem; // ���� ������ Ȯ�� ����

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10.0f;

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

        LongRangeAttack(); // ����
        ShortRangeAttack();

        if (!isCombDone) // ī�� ���� Ȯ�� ����
        {
            CardCombination();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("����� �α� ����");
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
            //Debug.Log("����");
        }
    }

    void InterAction() // �÷��̾� ĳ���� ��ȣ�ۿ�
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItem != null)
        {
            
            if(currentItem.tag == "Card" && cardCount < cardInventory.Length)
            {
                Debug.Log("ī�� ����");
                currentItem.SetActive(false);
                AddCard(currentItem);
            }
            else if (currentItem.tag == "Item")
            {

            }
            currentItem = null;
        }
    }

    void Teleportation() // �÷��̾� ĳ���� �����̵� �Լ�
    {
        if (Input.GetKeyDown(KeyCode.D) && canTeleporting)
        {
            if (movement.x < 0)
            {
                transform.Translate(Vector2.left * teleportingDistance);
            }
            else
            {
                transform.Translate(Vector2.right * teleportingDistance);
            }
            canTeleporting = false;
        }

        if (!canTeleporting)
        {
            teleportingCoolTime -= Time.deltaTime;
            if (teleportingCoolTime <= 0.0f)
            {
                teleportingCoolTime = 3.0f;
                canTeleporting = true;
                Debug.Log("�����̵� ����");
            }
        }
    }

    void ShortRangeAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

        }
    }

    void LongRangeAttack() // ���Ÿ� ���� �Լ�
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
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card")
        {
            //Debug.Log("������ ����");
            currentItem = other.gameObject;
        }

        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == false)
        {
            map_Manager.IsOnPortal = true;
            map_Manager.Which_Portal = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == true)
        {
            map_Manager.IsOnPortal = false;
        }
    }
}


