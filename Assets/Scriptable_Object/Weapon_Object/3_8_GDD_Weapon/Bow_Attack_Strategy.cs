using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow_Attack", menuName = "Weapon/Attack Strategy/Bow")]
public class Bow_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float projectile_Speed = 10.0f;
    public float charge_Projectile_Speed = 20.0f;
    public float charge_Time_Threshold = 1.5f;

    public GameObject normal_Arrow_Prefab;
    public GameObject charged_Arrow_Prefab;
   
    private bool is_Charging = false;
    public float charge_Start_Time = 0.0f;

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
        if (is_Charging) return;

        is_Charging = true;        
        charge_Start_Time = Time.time;
        //player.animator.SetTrigger("Attack");
    }

    public void Release_Attack(PlayerCharacter_Controller player)
    {
        if (is_Charging)
        {
            is_Charging = false;

            float charge_Time = Time.time - charge_Start_Time;

            //Debug.Log($"Charge Start Time: {charge_Start_Time}, Current Time: {Time.time}, Charge Time: {charge_Time}");

            bool is_Charged_Shot = charge_Time >= charge_Time_Threshold;
            GameObject arrow_Prefab = is_Charged_Shot ? charged_Arrow_Prefab : normal_Arrow_Prefab;

            Transform fire_Point = player.weapon_Anchor;
            Fire_Arrow(player, arrow_Prefab, fire_Point, is_Charged_Shot);

            player.animator.SetTrigger("Attack");

            //Debug.Log($"Charge Time: {charge_Time}, Threshold: {charge_Time_Threshold}, Is Charged Shot: {is_Charged_Shot}");
        }
    }
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
            
    }

    private void Fire_Arrow(PlayerCharacter_Controller player, GameObject projectile_Prefab, Transform fire_Point, bool is_Charged_Shot)
    {
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent <Rigidbody2D>();

        Vector2 shoot_Direction = player.is_Facing_Right ? Vector2.right : Vector2.left;
        rb.velocity = shoot_Direction * (is_Charged_Shot ? charge_Projectile_Speed : projectile_Speed);

        //Debug.Log(is_Charged_Shot ? "Charged Shot Fired!" : "Normal Shot Fired!");
    }

    

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            mainCamera.transform.position,
            new Vector2(mainCamera.orthographicSize * 2 * mainCamera.aspect, mainCamera.orthographicSize * 2),
            0,
            LayerMask.GetMask("Enemy")
            );

        if (enemies.Length > 0)
        {
            foreach (Collider2D enemyCollider in enemies)
            {
                Enemy_Basic enemy = enemyCollider.GetComponent<Enemy_Basic>();
                if (enemy != null)
                {
                    enemy.TakeDamage(weapon_Data.skill_Damage);

                    Debug.Log($"Enemy {enemy.name} hit by Bow Skill!");
                }
            }
        }

        player.animator.SetTrigger("Skill");
    }
}
