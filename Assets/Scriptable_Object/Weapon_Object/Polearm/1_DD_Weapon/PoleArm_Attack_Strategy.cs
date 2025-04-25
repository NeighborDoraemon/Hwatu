using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoleArm_Attack", menuName = "Weapon/Attack Strategy/PoleArm")]
public class PoleArm_Attack_Strategy : ScriptableObject, IAttack_Strategy
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
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
    }
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player.is_Skill_Active) return;

        player.is_Skill_Active = true;
        float original_Cooldown = player.attack_Cooldown;

        player.attack_Cooldown *= 0.5f;
        Debug.Log("공격 속도 증가!");

        player.StartCoroutine(Reset_Attack_Speed(player, original_Cooldown, 3f));
    }

    private IEnumerator Reset_Attack_Speed(PlayerCharacter_Controller player, float original_Cooldown, float delay)
    {
        yield return new WaitForSeconds(delay);

        player.attack_Cooldown = original_Cooldown;
        player.is_Skill_Active = false;
        Debug.Log("공격 속도 복구..");
    }
}
