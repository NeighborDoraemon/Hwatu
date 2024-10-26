using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear_Attack", menuName = "Weapon/Attack Strategy/Spear")]
public class Spear_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;
        player.attackDamage = weapon_Data.attack_Power;
        player.attack_Cooldown = weapon_Data.skillCooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skillCooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        //Debug.Log("카드 공격 실행");
        if (Is_Cooldown_Complete(player))
        {
            Start_Attack(player, weapon_Data);
        }
        else if (Is_Combo_Complete(player, weapon_Data))
        {
            End_Attack(player);
        }
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger(weapon_Data.attack_Trigger);
        player.isAttacking = true;

        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);
    }

    private void End_Attack(PlayerCharacter_Controller player)
    {
        player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
