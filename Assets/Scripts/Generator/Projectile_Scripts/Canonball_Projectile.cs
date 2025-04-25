using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canonball_Projectile : MonoBehaviour
{
    PlayerCharacter_Controller player;

    public int damage;
    public float explosion_Radius = 2.0f;
    public LayerMask enemy_Layer;
    public GameObject explosion_Prefab;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();

        damage = player.Calculate_Damage();
    }

    public void Initialized(Rigidbody2D rb, Vector2 direction, float speed)
    {
        rb.velocity = direction * speed;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Walls") || other.CompareTag("Platform") || other.CompareTag("OneWayPlatform"))
        {
            Instantiate(explosion_Prefab, transform.position, transform.rotation);

            Explode();
        }
    }

    private void Explode()
    {
        Collider2D[] hit_Enemies = Physics2D.OverlapCircleAll(transform.position, explosion_Radius, enemy_Layer);

        foreach (Collider2D enemy in hit_Enemies)
        {
            Enemy_Basic enemy_Script = enemy.GetComponent<Enemy_Basic>();
            if (enemy_Script != null)
            {
                enemy_Script.TakeDamage(damage);
            }

            if (Random.value <= player.stun_Rate)
            {
                Enemy_Stun_Interface enemy_Interface = enemy_Script.GetComponent<Enemy_Stun_Interface>()
                            ?? enemy_Script.GetComponentInParent<Enemy_Stun_Interface>()
                            ?? enemy_Script.GetComponentInChildren<Enemy_Stun_Interface>();

                enemy_Interface.Enemy_Stun(2.0f);
                //other.GetComponentInChildren<Enemy_Stun_Interface>().Enemy_Stun(2.0f);
            }

            if (Random.value <= player.bleeding_Rate)
            {
                enemy_Script.GetComponent<Enemy_Basic>().Bleeding_Attack(damage, 5, 1.1f);
            }

            player.Trigger_Enemy_Hit();
        }

        Destroy(gameObject);
    }
}
