using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[CreateAssetMenu(fileName = "DokkaebiBat_Attack", menuName = "Weapon/Attack Strategy/DokkaebiBat")]
public class DokkaebiBat_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float skill_Range = 1.5f;
    public float skill_Offset = 0.5f;
    public LayerMask enemy_LayerMask;

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

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount++;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }    

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");

        Vector2 origin = (Vector2)player.transform.position + Vector2.right * (player.is_Facing_Right ? 1 : -1) * skill_Offset;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, skill_Range, enemy_LayerMask);

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Skill_Damage());
            }
        }
    }
}
