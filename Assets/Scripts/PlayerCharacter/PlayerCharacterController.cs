using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerCharacterController : PlayerCharacterCardManager
{
    Rigidbody2D rb;

    Vector2 movement = new Vector2();
    bool isJumping = false;

    GameObject currentItem;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();

        InterAction();

        if (!isCombDone)
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

        movement.Normalize();

        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    void Jump() // 캐릭터 y좌표 점프
    {
        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isJumping = true;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            isJumping = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Item" || other.gameObject.tag == "Card")
        {
            //Debug.Log("아이템 감지");
            currentItem = other.gameObject;
        }
    }
}


