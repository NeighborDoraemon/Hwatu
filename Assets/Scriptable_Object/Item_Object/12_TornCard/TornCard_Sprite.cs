using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TornCard_Effect", menuName = "ItemEffects/TornCard_Effect")]
public class TornCard_Sprite : ItemEffect
{
    public int teleport_Damage = 5;
    public float damage_Radius = 5f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.On_Teleport += Teleport_With_Damage;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.On_Teleport -= Teleport_With_Damage;
    }

    private void Teleport_With_Damage(PlayerCharacter_Controller player)
    {
        Collider2D[] hit_Colliders = Physics2D.OverlapCircleAll(player.transform.position, damage_Radius);
        foreach (Collider2D collider in hit_Colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Enemy_Basic enemy = collider.GetComponent<Enemy_Basic>();
                if (enemy != null)
                {
                    enemy.TakeDamage(Mathf.RoundToInt(teleport_Damage));
                }
            }
        }
    }
}
