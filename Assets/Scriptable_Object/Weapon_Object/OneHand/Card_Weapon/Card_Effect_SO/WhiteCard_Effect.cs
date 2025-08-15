using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WhiteCard", menuName = "Card_Effect/White_LifeSteal")]
public class WhiteCard_Effect : Card_Effect
{
    [Header("Life Steal Settings")]
    [Range(0f, 1f)]
    public float lifeSteal_Rate = 0.2f;

    public bool expire_OnHit = true;

    public override void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point)
    {
        if (proj == null || proj.player == null)
        {
            if (expire_OnHit) proj.Expire();
            return;
        }

        int dealt_Damage_Approx = proj.Get_FinalDamage();

        int heal_Amount = Mathf.RoundToInt(dealt_Damage_Approx * lifeSteal_Rate);

        if (heal_Amount > 0)
            proj.player.Player_Take_Heal(heal_Amount);

        if (expire_OnHit)
            proj.Expire();
    }
}
