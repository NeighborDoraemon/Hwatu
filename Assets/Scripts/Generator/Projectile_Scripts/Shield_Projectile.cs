using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield_Projectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float return_Speed;
    private float max_Distance;
    private int damage;
    private Shield_Attack_Strategy attack_strategy;
    private PlayerCharacter_Controller player;

    private Vector3 origin;
    private bool returning = false;

    public void Initialize(Vector2 dir, float spd, float retSpd, float maxDist, int dmg, Shield_Attack_Strategy strat)
    {
        direction = dir;
        speed = spd;
        return_Speed = retSpd;
        max_Distance = maxDist;
        damage = dmg;
        attack_strategy = strat;

        origin = transform.position;
        player = FindObjectOfType<PlayerCharacter_Controller>();
    }

    private void Update()
    {
        if (!returning)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            if (Vector3.Distance(origin, transform.position) >= max_Distance)
                returning = true;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.weapon_Anchor.position,
                return_Speed * Time.deltaTime
                );

            if (Vector3.Distance(transform.position, player.weapon_Anchor.position) < 0.1f)
            {
                Debug.Log("[Shield_Projectile] º¹±Í ¿Ï·á");
                attack_strategy.On_Shield_Returned();
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!returning && other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<Enemy_Basic>();
            if (enemy != null) enemy.TakeDamage(damage);

            returning = true;
        }
    }
}
