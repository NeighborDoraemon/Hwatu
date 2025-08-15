using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SnakeWine_Attack", menuName = "Weapon/Attack Strategy/Snake_Wine")]
public class SnakeWine_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public int skill_Count = 3;

    private bool prev_Has_OneFour = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();

        prev_Has_OneFour = this.player.Has_One_And_Four();
        Debug.Log(prev_Has_OneFour);
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { Reset_SkillCount(); }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.weapon_Animator.SetTrigger("Attack");
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }
    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (skill_Count > 0)
        {
            player.animator.SetTrigger("Skill");
            player.Player_Take_Heal(weapon_Data.skill_Damage);
            skill_Count--;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Reset_SkillCount()
    {
        bool has_OneFour = player.Has_One_And_Four();
        if (!has_OneFour)
        {
            skill_Count = 3;
        }
    }
}
