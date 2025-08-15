using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "BlueCard", menuName = "Card_Effect/Blue_DoubleShot")]
public class BlueCard_Effect : Card_Effect
{
    [Header("Double Shot Settings")]
    public float second_Delay = 0.07f;
    public float second_Damage_Scale = 1.0f;

    [Header("Separation")]
    [Tooltip("두 번째 카드를 첫 카드의 진행 방향 반대쪽으로 떨어뜨려서 소환")]
    public float min_Spawn_Gap = 0.35f;

    [Header("Other")]
    public bool expire_OnHit = true;

    public override void OnShoot(Card_Projectile proj, PlayerCharacter_Controller player)
    {
        if (proj.is_Secondary_Spawn) return;

        proj.StartCoroutine(Spawn_Second(proj));
    }

    public override void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point)
    {
        if (expire_OnHit)
            proj.Expire();
    }

    private IEnumerator Spawn_Second(Card_Projectile first)
    {
        if (second_Delay > 0.0f)
            yield return new WaitForSeconds(second_Delay);

        if (first == null) yield break;

        var (dir, speed) = first.Get_Current_Flight();
        if (dir == Vector2.zero)
            dir = (first.player != null && first.player.is_Facing_Right) ? Vector2.right : Vector2.left;

        float gap_By_Delay = Mathf.Max(0.0f, speed * second_Delay);
        float gap = Mathf.Max(min_Spawn_Gap, gap_By_Delay);

        Vector3 spawn_Pos = first.transform.position - (Vector3)dir * gap;

        GameObject cloneObj = Object.Instantiate(first.gameObject, spawn_Pos, Quaternion.identity);
        var clone = cloneObj.GetComponent<Card_Projectile>();
        if (clone == null) yield break;

        clone.Initialized(first.player, first.effect, dir, speed, isSecondary: true);

        if (Mathf.Abs(second_Damage_Scale -1.0f) > 0.01f)
        {
            int dmg = Mathf.RoundToInt(clone.Get_FinalDamage() * second_Damage_Scale);
            clone.Set_Override_Damage(Mathf.Max(1, dmg));
        }
    }
}
