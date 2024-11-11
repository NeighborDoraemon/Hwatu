using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse_Projectile : MonoBehaviour
{
    public int damage = 5; // 스킬데미지
    public delegate void Projectile_Event();
    public event Projectile_Event OnHitEnemy;
    public event Projectile_Event OnMiss;

    private Rigidbody2D rb;

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
            other.GetComponent<Enemy_Basic>().TakeDamage(damage);
            OnHitEnemy?.Invoke();
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible() // 화면 밖으로 나갈 경우 자동 삭제
    {
        if (OnMiss != null)
        {
            OnMiss?.Invoke();
        }        
        Destroy(gameObject);
    }
}
