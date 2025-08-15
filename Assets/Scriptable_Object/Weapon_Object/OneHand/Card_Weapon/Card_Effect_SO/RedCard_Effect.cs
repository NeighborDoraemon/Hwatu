using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RedCard", menuName = "Card_Effect/Red_Bleed")]
public class RedCard_Effect : Card_Effect
{
    [Header("Bleed Settings")]
    [Tooltip("true일 시 투사체 데미지 기반으로 틱 데미지 산출, false일 시 fixed_Tick_Damage 사용")]
    public bool tickDamage_From_Projectile = true;

    [Tooltip("투사체 최종 데미지에 곱해서 틱 데미지 산출 (예시 : 0.35 = 35%)")]
    [Range(0.0f, 2.0f)] public float tickDamage_Scale = 0.35f;

    [Tooltip("tickDamage_From_Projectile = false일 때 고정 틱 데미지")]
    public int fixed_Tick_Damage = 2;

    [Tooltip("틱 횟수")]
    public int tick_Count = 4;

    [Tooltip("틱 간격(초)")]
    public float tick_Delay = 0.5f;

    [Header("Other")]
    [Tooltip("명중 시 투사체 소멸 여부")]
    public bool expire_OnHit = true;

    public override void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point)
    {
        if (proj == null) return;

        var enemy = target.GetComponent<Enemy_Basic>() ??
                    target.GetComponentInParent<Enemy_Basic>() ??
                    target.GetComponentInChildren<Enemy_Basic>();

        if (enemy == null)
        {
            if (expire_OnHit) proj.Expire();
            return;
        }

        int tick_Dmg;
        if (tickDamage_From_Projectile)
        {
            tick_Dmg = Mathf.Max(1, Mathf.RoundToInt(proj.Get_FinalDamage() * tickDamage_Scale));
        }
        else
        {
            tick_Dmg = Mathf.Max(1, fixed_Tick_Damage);
        }

        enemy.Bleeding_Attack(tick_Dmg, Mathf.Max(1, tick_Count), Mathf.Max(0.01f, tick_Delay));

        if (expire_OnHit)
            proj.Expire();
    }
}
