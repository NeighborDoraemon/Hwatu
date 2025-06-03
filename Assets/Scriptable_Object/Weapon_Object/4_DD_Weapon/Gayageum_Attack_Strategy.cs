using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gayageum_Attack", menuName = "Weapon/Attack Strategy/Gayageum")]
public class Gayageum_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Base Attack Settings")]
    [SerializeField] private float attack_Range = 2.0f;
    public float hold_Atk_Interval = 3.0f;
    public LayerMask enemy_Layer;

    private float last_Hold_Attack_Time = -Mathf.Infinity;
    private bool is_Skill_Active = false;

    [Header("Skill Settings")]
    [SerializeField] private float skill_Active_Time = 5.0f;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        last_Hold_Attack_Time = -Mathf.Infinity;

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attack_Range, enemy_Layer);

        foreach (Collider2D enemy_Collider in hits)
        {
            Enemy_Basic enemy = enemy_Collider.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Damage());
                Debug.Log("Enemy hit by hold attack, Damage : " + player.attackDamage);

                if (UnityEngine.Random.value <= player.stun_Rate)
                {
                    Enemy_Stun_Interface enemy_Interface = enemy.GetComponent<Enemy_Stun_Interface>()
                                ?? enemy.GetComponentInParent<Enemy_Stun_Interface>()
                                ?? enemy.GetComponentInChildren<Enemy_Stun_Interface>();

                    enemy_Interface.Enemy_Stun(2.0f);
                }

                if (UnityEngine.Random.value <= player.bleeding_Rate)
                {
                    enemy.GetComponent<Enemy_Basic>().Bleeding_Attack(player.Calculate_Damage(), 5, 1.1f);
                }

                player.Trigger_Enemy_Hit();
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

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (is_Skill_Active) return false;

        is_Skill_Active = true;
        player.last_Skill_Time = Time.time;
        player.StartCoroutine(Skill_Effect(player));

        return true;
    }
    
    private IEnumerator Skill_Effect(PlayerCharacter_Controller player)
    {
        float original_Range = attack_Range;
        attack_Range *= 2;
        Debug.Log($"Cur Attack Range = {attack_Range}");

        Vector3 og_Scale = player.effect_Render.transform.localScale;
        player.effect_Render.transform.localScale *= 2;

        yield return new WaitForSeconds(skill_Active_Time);

        attack_Range = original_Range;
        is_Skill_Active = false;

        player.effect_Render.transform.localScale = og_Scale;

        Debug.Log($"Cur Attack Range = {attack_Range}");
    }
}
