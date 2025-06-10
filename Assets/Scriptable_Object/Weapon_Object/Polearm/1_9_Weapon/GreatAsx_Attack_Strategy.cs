using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "GreatAxe_Attack", menuName = "Weapon/Attack Strategy/GreatAxe")]
public class GreatAsx_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public Vector2 spin_Box_Size = new Vector2(4.0f, 2.0f);
    public Vector2 spin_Box_Offset = Vector2.zero;

    public float smash_Delay = 0.3f;
    public float smash_Offset = 1.0f;
    public Vector2 smash_Box_Size = new Vector2(1.0f, 1.0f);

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

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.StartCoroutine(Skill_Coroutine(player));

        return true;
    }

    private IEnumerator Skill_Coroutine(PlayerCharacter_Controller player)
    {
        int enemy_Layers = LayerMask.GetMask("Enemy", "Boss_Enemy");
        int first_Hit_Damage = Calc_Axe_SkillDamage(player.cur_Weapon_Data.skill_Damage);
        int second_Hit_Damage = Calc_Axe_SkillDamage(player.cur_Weapon_Data.skill_Damage * 3);

        Vector2 box_Center = (Vector2)player.transform.position + spin_Box_Offset;
        Collider2D[] hits_First = Physics2D.OverlapBoxAll(box_Center, spin_Box_Size, 0.0f, enemy_Layers);
        foreach (var hit in hits_First)
        {
            if (hit.TryGetComponent<Enemy_Basic>(out var enemy))
            {
                enemy.TakeDamage(first_Hit_Damage);
            }
        }

        yield return new WaitForSeconds(smash_Delay);

        Vector2 origin = (Vector2)player.transform.position
                         + Vector2.right * (player.is_Facing_Right ? 1 : -1) * smash_Offset;
        Collider2D[] hits_Second = Physics2D.OverlapBoxAll(origin, smash_Box_Size, 0.0f, enemy_Layers);
        foreach (var hit in hits_Second)
        {
            if (hit.TryGetComponent<Enemy_Basic>(out var enemy))
            {
                enemy.TakeDamage(second_Hit_Damage);
            }
        }
    }

    private int Calc_Axe_SkillDamage(int raw_SkillDamage)
    {
        int base_Dmg = raw_SkillDamage + player.skill_Damage;
        int total_Dmg = Mathf.RoundToInt(base_Dmg * player.damage_Mul);
        if (Random.value <= player.crit_Rate)
            total_Dmg = Mathf.RoundToInt(total_Dmg * player.crit_Dmg);
        return total_Dmg;
    }
}
