using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "No_Weapon", menuName = "Weapon/Attack Strategy/Null")]
public class No_Weapon_Strategy : ScriptableObject, IAttack_Strategy
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
        Debug.Log("맨손 상태입니다.");
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Debug.Log("맨손 상태입니다.");
    }
}
