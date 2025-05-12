using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubberBullet_Projectile : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    private Rigidbody2D rb;

    [Header("Damage & Speed")]
    [SerializeField] private int final_Damage;
    [SerializeField] private float speed = 15.0f;

    [Header("Bounce Settings")]
    [SerializeField] private int max_Bounces = 3;
    private int bounce_Count = 0;
    [SerializeField] private float bounce_Range = 6.0f;
    private List<Enemy_Basic> hit_Enemies = new List<Enemy_Basic>();

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        rb = GetComponent<Rigidbody2D>();
        final_Damage = player.Calculate_Skill_Damage();
        Destroy(gameObject, 5.0f);
    }

    private void Update()
    {
        if (rb.velocity.sqrMagnitude > 0.01f)
            Align_Rotation();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<Enemy_Basic>(out var enemy)
                && !hit_Enemies.Contains(enemy))
            {
                Hit_And_Bounce(enemy);
            }
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

    private void Hit_And_Bounce(Enemy_Basic enemy)
    {
        hit_Enemies.Add(enemy);
        enemy.TakeDamage(final_Damage);

        if (player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
        {
            Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
        }

        if (Random.value <= player.stun_Rate)
        {
            var stun_Comp = enemy.GetComponent<Enemy_Stun_Interface>()
                            ?? enemy.GetComponentInParent<Enemy_Stun_Interface>()
                            ?? enemy.GetComponentInChildren<Enemy_Stun_Interface>();

            stun_Comp.Enemy_Stun(2.0f);
        }

        if (Random.value <= player.bleeding_Rate)
        {
            enemy.Bleeding_Attack(final_Damage, 5, 1.1f);
        }

        player.Trigger_Enemy_Hit();

        bounce_Count++;
        if (bounce_Count >= max_Bounces)
        {
            Destroy(gameObject);
            return;
        }

        var next = Find_Next_Target();
        if (next != null)
        {
            Vector2 dir = ((Vector2)next.transform.position - rb.position).normalized;
            rb.velocity = dir * speed;
            Align_Rotation();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Enemy_Basic Find_Next_Target()
    {
        int mask = LayerMask.GetMask("Enemy", "Boss_Enemy");
        Collider2D[] candidates = Physics2D.OverlapCircleAll(transform.position, bounce_Range, mask);

        Enemy_Basic closet = null;
        float min_Dist = float.MaxValue;
        foreach (var col in candidates)
        {
            if (col.TryGetComponent<Enemy_Basic>(out var e) && !hit_Enemies.Contains(e))
            {
                float d = Vector2.Distance(transform.position, e.transform.position);
                if (d < min_Dist)
                {
                    min_Dist = d;
                    closet = e;
                }
            }
        }
        return closet;
    }

    private void Align_Rotation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}
