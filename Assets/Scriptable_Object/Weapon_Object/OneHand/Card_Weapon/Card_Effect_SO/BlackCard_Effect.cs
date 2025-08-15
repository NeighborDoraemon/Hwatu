using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlackCard", menuName = "Card_Effect/Black_Excute")]
public class BlackCard_Effect : Card_Effect
{
    [Header("Layer Settings")]
    public LayerMask normal_EnemyLayer;
    public LayerMask boss_Layer;

    [Header("Boss Damage Settings")]
    [Tooltip("보스에게 줄 추가 데미지 배율 (최종 데미지에 곱해짐)")]
    public float boss_Damage_Multiplier = 6.0f;

    [Tooltip("보스에게 줄 추가 고정 데미지 (배율 계산 뒤 더함)")]
    public int boss_FlatBonus = 0;

    [Header("Other")]
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

        int layer = target.gameObject.layer;
        bool isBoss = Is_In_Mask(layer, boss_Layer);
        bool isNormal = Is_In_Mask(layer, normal_EnemyLayer);

        if (isBoss)
        {
            int base_Dmg = proj.Get_FinalDamage();

            float target_Total = base_Dmg * boss_Damage_Multiplier + boss_FlatBonus;
            int extra = Mathf.Max(1, Mathf.RoundToInt(target_Total - base_Dmg));

            enemy.TakeDamage(extra);
        }
        else if (isNormal)
        {
            enemy.TakeDamage(999999);
        }
        else
        {
            proj.Expire();
        }

        if (expire_OnHit)
            proj.Expire();
    }

    private bool Is_In_Mask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }
}
