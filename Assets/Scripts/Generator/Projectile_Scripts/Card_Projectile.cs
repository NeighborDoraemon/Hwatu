using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class Card_Projectile : MonoBehaviour
{
    [HideInInspector] public PlayerCharacter_Controller player;
    [HideInInspector] public Card_Effect effect;

    [SerializeField] private int final_Damage;
    [SerializeField] private float life_Time = 3.0f;

    private SpriteRenderer sr;

    private Rigidbody2D rb;
    private bool isExpired;

    [HideInInspector] public bool is_Secondary_Spawn = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        CancelInvoke(nameof(Expire));
        Invoke(nameof(Expire), life_Time);

        Apply_Tint(effect);
    }

    public void Initialized(PlayerCharacter_Controller player,
                            Card_Effect effect,
                            Vector2 direction,
                            float speed,
                            bool isSecondary = false)
    {
        this.player = player;
        this.effect = effect;
        this.is_Secondary_Spawn = isSecondary;

        final_Damage = (player != null) ? player.Calculate_Damage() : final_Damage;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direction.normalized * speed;

        Apply_Tint(effect);

        effect?.OnShoot(this, player);
    }

    private void Apply_Tint(Card_Effect eff)
    {
        Color c = (eff != null) ? eff.tint : Color.gray;

        if (sr) sr.color = c;
    }

    public int Get_FinalDamage() => final_Damage;
    public void Set_Override_Damage(int dmg) => final_Damage = dmg;

    public void Retarget(Vector2 newDir, float speed)
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = newDir.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(final_Damage);
            }
            
            if (player != null)
            {
                if (player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
                {
                    Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
                }

                if (Random.value <= player.stun_Rate)
                {
                    Enemy_Stun_Interface enemy_Stun = other.GetComponent<Enemy_Stun_Interface>()
                                ?? other.GetComponentInParent<Enemy_Stun_Interface>()
                                ?? other.GetComponentInChildren<Enemy_Stun_Interface>();

                    enemy_Stun.Enemy_Stun(2.0f);
                    //other.GetComponentInChildren<Enemy_Stun_Interface>().Enemy_Stun(2.0f);
                }

                if (Random.value <= player.bleeding_Rate)
                {
                    other.GetComponent<Enemy_Basic>().Bleeding_Attack(final_Damage, 5, 1.1f);
                }

                player.Trigger_Enemy_Hit();
            }

            if (effect != null)
                effect?.OnHit(this, other, transform.position);
            else
                Expire();
        }
        else if (other.CompareTag("Walls"))
        {
            if (player != null && player.has_EarRing_Effect && player.earRing_Explosion_Prefab != null)
            {
                Instantiate(player.earRing_Explosion_Prefab, transform.position, transform.rotation);
            }

            effect?.OnExpire(this, transform.position);
            Expire();
        }
    }

    public void Expire()
    {
        if (isExpired) return;
        isExpired = true;

        Destroy(gameObject);
    }

    public (Vector2 dir, float speed) Get_Current_Flight()
    {
        if (rb == null || rb.velocity == Vector2.zero) return (Vector2.right, 0.0f);
        return (rb.velocity.normalized, rb.velocity.magnitude);
    }
}
