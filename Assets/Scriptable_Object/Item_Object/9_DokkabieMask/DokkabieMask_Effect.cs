using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DokkabieMask_Effect", menuName = "ItemEffects/DokkabieMask_Effect")]
public class DokkabieMask_Effect : ItemEffect
{
    public float gauge_Inc_Per_Hit = 20f;
    public float gauge_Threshold = 100f;

    public float berserk_Duration = 30f;
    public float cooldown_Duration = 60f;

    public float damage_Mul = 1.5f;
    public float takenDamage_Mul = 1.5f;
    public float attack_Cooldown_Mul = 0.7f;
    public float moveSpeed_Mul = 1.2f;

    private float cur_Gauge = 0f;
    private bool is_Berserk_Active = false;
    private bool is_Cooldown = false;
    private PlayerCharacter_Controller cur_Player;

    private float og_Damage_Mul;
    private float og_TakenDamage_Mul;
    private float og_AttackCooldown;
    private float og_MoveSpeed;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        cur_Player = player;
        cur_Gauge = 0f;
        is_Berserk_Active = false;
        is_Cooldown = false;

        player.On_Enemy_Hit += Handle_EnemyHit;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.On_Enemy_Hit -= Handle_EnemyHit;
        cur_Player = null;
    }

    private void Handle_EnemyHit()
    {
        if (is_Berserk_Active || is_Cooldown)
        {
            return;
        }

        cur_Gauge += gauge_Inc_Per_Hit;

        if (cur_Gauge >= gauge_Threshold)
        {
            cur_Player.StartCoroutine(BerserkRoutine());
        }
    }

    private IEnumerator BerserkRoutine()
    {
        is_Berserk_Active = true;

        og_MoveSpeed = cur_Player.movementSpeed;
        og_Damage_Mul = cur_Player.damage_Mul;
        og_TakenDamage_Mul = cur_Player.takenDamage_Mul;
        og_AttackCooldown = cur_Player.attack_Cooldown;

        cur_Player.movementSpeed = og_MoveSpeed * moveSpeed_Mul;
        cur_Player.damage_Mul = og_Damage_Mul * damage_Mul;
        cur_Player.takenDamage_Mul = og_TakenDamage_Mul * takenDamage_Mul;
        cur_Player.attack_Cooldown = og_AttackCooldown * attack_Cooldown_Mul;

        yield return new WaitForSeconds(berserk_Duration);

        cur_Player.movementSpeed = og_MoveSpeed;
        cur_Player.damage_Mul = og_Damage_Mul;
        cur_Player.takenDamage_Mul = og_TakenDamage_Mul;
        cur_Player.attack_Cooldown = og_AttackCooldown;

        is_Berserk_Active = false;
        is_Cooldown = true;
        cur_Gauge = 0f;

        yield return new WaitForSeconds(cooldown_Duration);

        is_Cooldown = false;
    }
}
