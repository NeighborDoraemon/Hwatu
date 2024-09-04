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


    [SerializeField] // ¸Ê ¸Å´ÏÀú #±èÀ±Çõ
    private Map_Manager map_Manager;

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
            Debug.Log("µð¹ö±× ·Î±× ½ÇÇè");
        }
    }

    void Move() // Ä³¸¯ÅÍ xÁÂÇ¥ ÀÌµ¿
    {
        movement.x = Input.GetAxisRaw("Horizontal");

        movement.Normalize();

        rb.velocity = new Vector2(movement.x * movementSpeed, rb.velocity.y);
    }

    void Jump() // Ä³¸¯ÅÍ yÁÂÇ¥ Á¡ÇÁ
    {
        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.AddForce(new Vector2(0, jumpPower), ForceMode2D.Impulse);
            isJumping = true;
            //Debug.Log("Á¡ÇÁ");
        }
    }

    void InterAction() // ÇÃ·¹ÀÌ¾î Ä³¸¯ÅÍ »óÈ£ÀÛ¿ë
    {
        if (Input.GetKeyDown(KeyCode.E) && currentItem != null)
        {
            
            if(currentItem.tag == "Card" && cardCount < cardInventory.Length)
            {
                Debug.Log("Ä«µå ½Àµæ");
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
            //Debug.Log("¾ÆÀÌÅÛ °¨Áö");
            currentItem = other.gameObject;
        }

        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == false) // Æ÷Å» ÁøÀÔ °¨Áö #±èÀ±Çõ
        {
            Debug.Log("Get In Portal");
            map_Manager.IsOnPortal = true;
            map_Manager.Which_Portal = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other) // Æ÷Å» ÅðÀå°¨Áö #±èÀ±Çõ
    {
        if (other.gameObject.tag == "Portal" && map_Manager.IsOnPortal == true)
        {
            map_Manager.IsOnPortal = false;
            Debug.Log("Get Out Portal");
        }
    }
}


