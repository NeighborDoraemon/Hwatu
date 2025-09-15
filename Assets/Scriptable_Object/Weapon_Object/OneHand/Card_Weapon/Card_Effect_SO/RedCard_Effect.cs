using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RedCard", menuName = "Card_Effect/Red_Bleed")]
public class RedCard_Effect : Card_Effect
{
    [Header("Other")]
    [Tooltip("명중 시 투사체 소멸 여부")]
    public bool expire_OnHit = true;

    public override void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point)
    {
        if (proj == null || target == null) return;

        var enemy = target.GetComponent<Enemy_Basic>() ??
                    target.GetComponentInParent<Enemy_Basic>() ??
                    target.GetComponentInChildren<Enemy_Basic>();

        if (enemy == null)
        {
            if (expire_OnHit) proj.Expire();
            return;
        }

        var player = Object.FindObjectOfType<PlayerCharacter_Controller>();
        if (player == null)
        {
            Debug.LogWarning("[RedCard_Effect] Player not found.");
            return;
        }

        int tick_Dmg = Mathf.Max(1, player.bleed_Damage);
        int count = Mathf.Max(1, player.bleed_Count);
        float delay = Mathf.Max(0.01f, player.bleed_Delay);

        enemy.Bleeding_Attack(tick_Dmg, count, delay);

        if (expire_OnHit)
            proj.Expire();
    }
}
