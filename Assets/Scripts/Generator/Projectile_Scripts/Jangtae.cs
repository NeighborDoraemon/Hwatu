using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangtae : MonoBehaviour
{
    [SerializeField] private int base_Weapon_Damage;

    private PlayerCharacter_Controller player;
    private Rigidbody2D rb;
    private Collider2D self_Col;

    [Header("Ground Check")]
    [SerializeField] private float groundCheck_Distance = 0.25f;
    [SerializeField] private float topNormal_Threshold = 0.6f;
    [SerializeField] private float ground_SideOffset = 0.18f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        rb = GetComponent<Rigidbody2D>();

        if (player != null && player.cur_Weapon_Data != null)
            base_Weapon_Damage = player.cur_Weapon_Data.attack_Damage;
    }

    public int Calculate_Final_Damage()
    {
        int playerDamage = player != null ? player.attackDamage : 0;

        int totalDamage = base_Weapon_Damage + playerDamage;

        return totalDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            int finalDamage = Calculate_Final_Damage();
            other.GetComponent<Enemy_Basic>().TakeDamage(finalDamage);
        }
        //else if (other.CompareTag("Walls"))
        //{
        //    if (player.attack_Strategy is Jangtae_Attack_Startegy jt && jt.isRiding)
        //    {
        //        return;
        //    }
        //    Destroy(this.gameObject);
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        string tag = other.collider.tag;

        if (tag == "Walls")
        {
            //if (IsOn_Top_Of_OneWay())
            //    return;

            if (player != null && player.attack_Strategy is Jangtae_Attack_Startegy jt && jt.isRiding)
                return;

            Destroy(gameObject);
            return;
        }
        else if (tag == "OneWayPlatform")
        {
            bool hitFromTop = false;

            foreach (var contact in other.contacts)
            {
                if (contact.normal.y >= topNormal_Threshold)
                {
                    hitFromTop = true;
                    break;
                }
            }

            if (hitFromTop)
            {
                return;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
    }

    private bool IsOn_Top_Of_OneWay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheck_Distance);
        if (hit.collider != null && hit.collider.CompareTag("OneWayPlatform"))
        {
            return hit.normal.y >= topNormal_Threshold;
        }
        return false;
    }
}
