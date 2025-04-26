using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Projectile : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    private int final_Damage;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        final_Damage = player.Calculate_Skill_Damage();

        Destroy(gameObject, 3.0f);
    }

    public void Initialized(Rigidbody2D rb, Vector2 direction, float speed)
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy_Basic>().TakeDamage(final_Damage);
            //other.GetComponentInChildren<Enemy_Basic>().TakeDamage(final_Damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls")/* || other.CompareTag("Platform") || other.CompareTag("OneWayPlatform")*/)
        {
            Destroy(gameObject);
        }
    }
}
