using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion_By_EarRing : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    private int explosion_Damage;

    public float explosion_Radius = 5f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        explosion_Damage = player.Calculate_Damage();

        Explosion();
    }

    private void Explosion()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, explosion_Radius, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            Enemy_Basic enemyController = enemy.GetComponent<Enemy_Basic>();
            if (enemyController != null)
            {
                enemyController.TakeDamage(explosion_Damage);
            }
        }

        Destroy(this.gameObject, 0.7f);
    }
}
