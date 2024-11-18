using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 100; // 스킬데미지

    public void Initialized(Rigidbody2D rb, Vector2 direction, float speed)
    {
        rb.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy_Basic>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
