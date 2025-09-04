using MBT;
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
    [SerializeField] private LayerMask enemy_Mask;
    private readonly List<Enemy_Basic> hit_Enemies = new();

    private readonly Dictionary<Enemy_Basic, Transform> bt_Anchor_Cache = new();

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
            var enemy = other.GetComponentInParent<Enemy_Basic>();
            if (enemy != null && !hit_Enemies.Contains(enemy))
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

        Vector2 center = Get_Target_Position(enemy, fallback_To_RB: true);

        var next = Find_Next_Target(center);
        if (next != null)
        {
            Vector2 target_Pos = Get_Target_Position(next);
            Vector2 dir = (target_Pos - center).normalized;
            rb.velocity = dir * speed;
            Align_Rotation();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Enemy_Basic Find_Next_Target(Vector2 center)
    {
        Collider2D[] candidates = Physics2D.OverlapCircleAll(center, bounce_Range, enemy_Mask);

        Enemy_Basic closet = null;
        float min_Dist = float.MaxValue;

        foreach (var col in candidates)
        {
            var e = col.GetComponentInParent<Enemy_Basic>();
            if (e == null || hit_Enemies.Contains(e)) continue;

            Vector2 aim_Pos = Get_Target_Position(e);
            float dSq = (aim_Pos - center).sqrMagnitude;
            if (dSq < min_Dist)
            {
                min_Dist = dSq;
                closet = e;
            }
        }
        return closet;
    }

    private Vector2 Get_Target_Position(Enemy_Basic e, bool fallback_To_RB = false)
    {
        if (e == null) return rb != null && fallback_To_RB ? rb.position : (Vector2)transform.position;

        if (!bt_Anchor_Cache.TryGetValue(e, out var anchor) || anchor == null)
        {
            var bt = e.GetComponentInChildren<MonoBehaviourTree>(true);
            if (bt != null) anchor = bt.transform;
            bt_Anchor_Cache[e] = anchor;
        }

        if (anchor != null) return anchor.position;

        var col = e.GetComponentInChildren<Collider2D>();
        if (col != null) return col.bounds.center;

        return fallback_To_RB && rb != null ? rb.position : (Vector2)e.transform.position;
    }

    private void Align_Rotation()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, angle);
    }
}
