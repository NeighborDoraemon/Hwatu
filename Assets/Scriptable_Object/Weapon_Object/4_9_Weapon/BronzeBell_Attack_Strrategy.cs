using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BronzeBell_Attack", menuName = "Weapon/Attack Strategy/BronzeBell")]
public class BronzeBell_Attack_Strrategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float attack_Range = 2.5f;
    public float hold_Atk_Interval = 3.0f;

    private float last_Hold_Attack_Time = -Mathf.Infinity;

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
        if (!player.isAttacking)
        {
            player.isAttacking = true;
        }

        if (Time.time < last_Hold_Attack_Time + hold_Atk_Interval)
        {
            return;
        }

        last_Hold_Attack_Time = Time.time;

        int mask = LayerMask.GetMask("Enemy", "Boss_Enemy");

        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attack_Range, mask);
        
        foreach (Collider2D enemy_Collider in hits)
        {
            Enemy_Basic enemy = enemy_Collider.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Damage());
                Debug.Log("Enemy hit by hold attack, Damage : " + player.attackDamage);
            }
            else
            {
                Debug.LogWarning($"Enemy_Basic component not found on : {enemy_Collider.name}");
            }
        }
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        
    }
}
