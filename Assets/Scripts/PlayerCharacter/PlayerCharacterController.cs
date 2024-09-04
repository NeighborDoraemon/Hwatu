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

    public float teleportingDistance = 3.0f; // 순간이동 거리 변수
    public float teleportingCoolTime = 3.0f; // 순간이동 쿨타임 변수
    bool canTeleporting = true; // 순간이동 조건 확인 변수

    GameObject currentItem; // 현재 아이템 확인 변수

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
        Move(); // 이동
        Jump();
        Teleportation();

        InterAction(); // 상호작용

        LongRangeAttack(); // 공격
        ShortRangeAttack();

        if (!isCombDone) // 카드 조합 확인 조건
        {
            CardCombination();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("디버그 로그 실험");
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
            //Debug.Log("점프");
        }
    }

    void InterAction() // 플레이어 캐릭터 상호작용
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItem != null)
        {
            
            if(currentItem.tag == "Card" && cardCount < cardInventory.Length)
            {
                Debug.Log("카드 습득");
                currentItem.SetActive(false);
                AddCard(currentItem);
            }
            else if (currentItem.tag == "Item")
            {

            }
            currentItem = null;
        }
    }

    void Teleportation() // 플레이어 캐릭터 순간이동 함수
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
                Debug.Log("순간이동 가능");
            }
        }
    }

    void ShortRangeAttack()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {

        }
    }

    void LongRangeAttack() // 원거리 공격 함수
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
            //Debug.Log("아이템 감지");
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


