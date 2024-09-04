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
            Debug.Log("����� �α� ����");
        }
    }

    void Move() // ĳ���� x��ǥ �̵�
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        movement.Normalize();

        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    void Jump() // ĳ���� y��ǥ ����
    {
        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isJumping = true;
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
            //Debug.Log("������ ����");
            currentItem = other.gameObject;
        }
    }
}


