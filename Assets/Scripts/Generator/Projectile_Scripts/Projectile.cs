using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    [SerializeField] private int final_Damage;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        final_Damage = player.Calculate_Damage();

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
            if (player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
            {
                Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
            }

            if (Random.value <= player.stun_Rate)
            {
                Enemy_Stun_Interface enemy = other.GetComponent<Enemy_Stun_Interface>()
                            ?? other.GetComponentInParent<Enemy_Stun_Interface>()
                            ?? other.GetComponentInChildren<Enemy_Stun_Interface>();

                enemy.Enemy_Stun(2.0f);
                //other.GetComponentInChildren<Enemy_Stun_Interface>().Enemy_Stun(2.0f);
            }

            if (Random.value <= player.bleeding_Rate)
            {
                other.GetComponent<Enemy_Basic>().Bleeding_Attack(final_Damage, 5, 1.1f);
            }

            player.Trigger_Enemy_Hit();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Walls"))
        {
            if (player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
            {
                Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
