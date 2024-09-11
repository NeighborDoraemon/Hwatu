using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerCharacterController : PlayerCharacterCardManager
{
    Rigidbody2D rb;

    Vector2 movement = new Vector2();
    int jumpCount = 0; // 점프 횟수 카운팅 변수
    public int maxJumpCount = 2; // 최대 점프 횟수 카운팅 변수

    [Header("Player_Teleport")]
    public float teleporting_Distance = 3.0f; // 순간이동 거리 변수
    public float teleporting_CoolTime = 3.0f; // 순간이동 쿨타임 변수
    bool canTeleporting = true; // 순간이동 조건 확인 변수

    GameObject current_Item; // 현재 아이템 확인 변수

    [Header("Player_Long_Range_Attack")] // 원거리 공격 변수 관리
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10.0f; // 총알 속도

    [Header("Player_Short_Range_Attack")]
    public float attackRange = 0.5f; // 플레이어와 공격 범위 간 거리 설정 변수
    public float attackCooldown = 1.0f; // 근접 공격 쿨타임 변수
    private float last_Attack_Time = 0f; // 마지막 공격 시점 기록 변수

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
        Move(); // 이동
        Jump();
        Teleportation();

        InterAction(); // 상호작용

        Long_Range_Attack(); // 공격
        Short_Range_Attack();

        if (!isCombDone) // 카드 조합 확인 조건
        {
            Card_Combination();
        }
    }

    void Move() // 캐릭터 x좌표 이동
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        if (movement.x != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movement.x), 1, 1);
        }

        movement.Normalize();
        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    void Jump() // 캐릭터 y좌표 점프
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            jumpCount++;
        }
    }

    void InterAction() // 플레이어 캐릭터 상호작용
    {
        if (Input.GetKeyDown(KeyCode.E) && current_Item != null)
        {
            //Debug.Log("해당 아이템과 상호작용 : " + current_Item.name);

            if(current_Item.tag == "Card")
            {
                //Debug.Log("해당 카드 획득 시도 : " + current_Item.name);

                if (cardCount < card_Inventory.Length || cardCount == card_Inventory.Length)
                {
                    Debug.Log("카드 습득");
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

    void Teleportation() // 플레이어 캐릭터 순간이동 함수
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
                Debug.Log("순간이동 가능");
            }
        }
    }

    void Short_Range_Attack()
    {
        if (Time.time >= last_Attack_Time + attackCooldown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                // 공격 범위 박스 콜라이더 설정
                Vector2 boxSize = new Vector2(0.2f, 0.4f);
                Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");

                Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, enemyLayer); // 적 레이어 설정

                // 감지된 적에게 데미지 처리
                foreach (Collider2D enemy in hitEnemies)
                {
                    Debug.Log("근접 공격 적 감지");

                    enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
                }

                last_Attack_Time = Time.time; // 마지막 공격 시간 현재 시간으로 갱신
            }
        }
    }

    void Long_Range_Attack() // 원거리 공격 함수
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

    private void OnDrawGizmosSelected() // 디버그용 공격 범위 그리기
    {
        Vector2 boxSize = new Vector2(0.2f, 0.4f);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}


