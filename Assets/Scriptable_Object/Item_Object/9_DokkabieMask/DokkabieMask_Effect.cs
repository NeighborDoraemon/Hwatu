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

    private float cur_Gauge = 0f;
    private bool is_Berserk_Active = false;
    private bool is_Cooldown = false;
    private PlayerCharacter_Controller cur_Player;

    public float damage_Value = 0.5f;
    public float taken_Damage_Value = 0.5f;
    public float atk_Cooltime_Value = 0.3f;
    public float moveSpeed_Value = 0.2f;

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

        cur_Player.damage_Mul += damage_Value;
        cur_Player.takenDamage_Mul += taken_Damage_Value;
        cur_Player.attack_Cooltime_Mul -= atk_Cooltime_Value;
        cur_Player.movementSpeed_Mul += moveSpeed_Value;

        yield return new WaitForSeconds(berserk_Duration);

        cur_Player.damage_Mul -= damage_Value;
        cur_Player.takenDamage_Mul -= taken_Damage_Value;
        cur_Player.attack_Cooltime_Mul += atk_Cooltime_Value;
        cur_Player.movementSpeed_Mul -= moveSpeed_Value;

        is_Berserk_Active = false;
        is_Cooldown = true;
        cur_Gauge = 0f;

        yield return new WaitForSeconds(cooldown_Duration);

        is_Cooldown = false;
    }
}
