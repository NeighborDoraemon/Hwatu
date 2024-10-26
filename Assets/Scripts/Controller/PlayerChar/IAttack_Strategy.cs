using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack_Strategy
{
    void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data);
    void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data);
}

//public class Weapon_Attack_Strategy : IAttack_Strategy
//{
//    private PlayerCharacter_Controller player;
//    private Weapon_Data weapon_Data;

//    public Weapon_Attack_Strategy(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
//    {
//        this.player = player;
//        this.weapon_Data = weapon_Data;

//        player.animator.runtimeAnimatorController = weapon_Data.overrideController;

//        player.attack_Cooldown = weapon_Data.attackCooldown;
//        player.max_AttackCount = weapon_Data.max_Attack_Count;
//        player.skill_Cooldown = weapon_Data.skillCooldown;

//    }

//    public void Attack() 
//    {
//        //if (!player.isGrounded) return;

//        if (Time.time >= player.last_Attack_Time + player.attack_Cooldown)
//        {
//            player.animator.SetTrigger(weapon_Data.attack_Trigger);
//            player.isAttacking = true;
//            player.cur_AttackCount = 1;
//            player.last_ComboAttack_Time = Time.time;
//            player.last_Attack_Time = Time.time;
//        }
//        else if (player.isAttacking && player.cur_AttackCount < weapon_Data.max_Attack_Count
//            && Time.time - player.last_ComboAttack_Time <= player.comboTime)
//        {
//            player.animator.SetTrigger($"Attack_{player.cur_AttackCount + 1}");
//            player.cur_AttackCount++;
//            player.last_ComboAttack_Time = Time.time;
//            player.last_Attack_Time = Time.time;
//        }
//        else if (player.cur_AttackCount >= weapon_Data.max_Attack_Count)
//        {
//            player.isAttacking = false;
//            player.cur_AttackCount = 0;
//            player.last_Attack_Time = Time.time;
//        }
//    }

//    public void Skill()
//    {
//        if (Time.time >= player.last_Skill_Time + player.skill_Cooldown)
//        {
//            player.animator.SetTrigger(weapon_Data.skill_Trigger);
//            player.last_Skill_Time = Time.time;
//        }
//    }
//}
