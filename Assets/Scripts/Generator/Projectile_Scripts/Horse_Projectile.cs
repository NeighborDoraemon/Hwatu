using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse_Projectile : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    [SerializeField] private int final_Damage;

    public delegate void Projectile_Event();
    public event Projectile_Event OnHitEnemy;
    public event Projectile_Event OnMiss;

    private Rigidbody2D rb;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        final_Damage = player.Calculate_Damage();
    }

    public void Initialized(HorseToken_Attack_Strategy attack_Strategy, Vector2 direction, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;

        OnHitEnemy += () =>
        {
            Debug.Log("Hit registered, increasing stack.");
            attack_Strategy.Increase_Stack();
            OnMiss = null;
        };
        OnMiss += () =>
        {
            Debug.Log("Miss registered, decreasing stack.");
            attack_Strategy.Decrease_Stack();
        };
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
            OnHitEnemy?.Invoke();
            Destroy(gameObject);
        }
        else if(other.CompareTag("Walls"))
        {
            OnMiss?.Invoke();
            if (player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
            {
                Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
            }
            Destroy(gameObject);
        }
    }
}
