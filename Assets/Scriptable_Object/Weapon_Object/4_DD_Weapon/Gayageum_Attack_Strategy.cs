using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gayageum_Attack", menuName = "Weapon/Attack Strategy/Gayageum")]
public class Gayageum_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [SerializeField] private float attack_Range = 2.0f;
    [SerializeField] private float skill_Active_Time = 5.0f;
    public float hold_Atk_Interval = 3.0f;

    private float last_Hold_Attack_Time = -Mathf.Infinity;
    private bool is_Skill_Active = false;

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
        if (is_Skill_Active) return;

        is_Skill_Active = true;
        player.last_Skill_Time = Time.time;
        player.StartCoroutine(Skill_Effect());
    }
    
    private IEnumerator Skill_Effect()
    {
        float original_Range = attack_Range;
        attack_Range *= 2;
        Debug.Log($"Cur Attack Range = {attack_Range}");
        yield return new WaitForSeconds(skill_Active_Time);
        attack_Range = original_Range;
        is_Skill_Active = false;
        Debug.Log($"Cur Attack Range = {attack_Range}");
    }
}
