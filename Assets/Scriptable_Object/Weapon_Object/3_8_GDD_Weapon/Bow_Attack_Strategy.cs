using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bow_Attack", menuName = "Weapon/Attack Strategy/Bow")]
public class Bow_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Projectile Settings")]
    public float projectile_Speed = 10.0f;
    public float charge_Projectile_Speed = 20.0f;
    public float charge_Time_Threshold = 4.0f;

    [Header("Projectile Prefab")]
    public GameObject normal_Arrow_Prefab;
    public GameObject charged_Arrow_Prefab;

    [Header("Effect Prefab")]
    public GameObject skillTarget_Effect_Prefab;
    public GameObject skill_Effect_Prefab;
    public GameObject bow_Charging_Effect_Prefab;

    [Header("Base Attack Settings")]
    public int min_Damage = 5;
    public int max_Damage = 20;
    public float charge_Start_Time = 0.0f;
    public float charging_Effect_Duration = 1.0f;

    private GameObject cur_Charging_Effect;
    private bool is_Charging = false;

    [Header("Skill Settings")]
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

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (is_Charging) return;

        is_Charging = true;
        charge_Start_Time = Time.time;
        player.StartCoroutine(Check_Charge_Effect());
    }

    private IEnumerator Check_Charge_Effect()
    {
        while (is_Charging && (Time.time - charge_Start_Time) < charge_Time_Threshold)
        {
            yield return null;
        }

        if (is_Charging && bow_Charging_Effect_Prefab != null && cur_Charging_Effect == null)
        {
            Vector3 spawn_Pos = player.weapon_Anchor.position + Vector3.up * 0.5f;
            cur_Charging_Effect = Instantiate(bow_Charging_Effect_Prefab, spawn_Pos, Quaternion.identity, player.weapon_Anchor);
            Destroy(cur_Charging_Effect, charging_Effect_Duration);
        }
    }

    public void Release_Attack(PlayerCharacter_Controller player)
    {
        if (is_Charging)
        {
            is_Charging = false;

            float charge_Time = Time.time - charge_Start_Time;
            bool is_Charged_Shot = charge_Time >= charge_Time_Threshold;
            GameObject arrow_Prefab = is_Charged_Shot ? charged_Arrow_Prefab : normal_Arrow_Prefab;

            weapon_Data.attack_Damage = Get_Charge_Damage(charge_Time);

            Transform fire_Point = player.weapon_Anchor;
            Fire_Arrow(player, arrow_Prefab, fire_Point, is_Charged_Shot);

            if (player.has_BowSheath_Effect)
            {
                player.StartCoroutine(Fire_Arrow_With_Delay(player, arrow_Prefab, fire_Point, is_Charged_Shot));
            }

            player.animator.SetTrigger("Attack");
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

        Vector3 projectile_Scale = projectile.transform.localScale;
        projectile_Scale.x = (player.is_Facing_Right) ? Mathf.Abs(projectile_Scale.x) : -Mathf.Abs(projectile_Scale.x);
        projectile.transform.localScale = projectile_Scale;
    }

    private IEnumerator Fire_Arrow_With_Delay(PlayerCharacter_Controller player, GameObject projectile_Prefab, Transform fire_Point, bool is_Charged_Shot)
    {
        yield return new WaitForSeconds(0.1f);
        Fire_Arrow(player, projectile_Prefab, fire_Point, is_Charged_Shot);
    }

    private int Get_Charge_Damage(float charge_Time)
    {
        int calculated_Damage = min_Damage + ((int)charge_Time * 3);
        return Mathf.Clamp(calculated_Damage, min_Damage, max_Damage);
    }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return false;
        }

        Collider2D[] enemies = Physics2D.OverlapBoxAll(
            mainCamera.transform.position,
            new Vector2(mainCamera.orthographicSize * 2 * mainCamera.aspect, mainCamera.orthographicSize * 2),
            0,
            enemy_LayerMask
            );

        if (enemies.Length > 0)
        {
            foreach (Collider2D enemyCollider in enemies)
            {
                Enemy_Basic enemy = enemyCollider.GetComponent<Enemy_Basic>();
                if (enemy != null)
                {
                    player.StartCoroutine(Skill_Effect_Coroutine(enemy, player));
                }
            }
        }

        player.animator.SetTrigger("Skill");

        return true;
    }

    private IEnumerator Skill_Effect_Coroutine(Enemy_Basic enemy, PlayerCharacter_Controller player)
    {
        if (skillTarget_Effect_Prefab != null)
        {
            GameObject target_Effect = Instantiate(skillTarget_Effect_Prefab, enemy.transform.position, Quaternion.identity, enemy.transform);
            Destroy(target_Effect, 0.4f);
        }

        yield return new WaitForSeconds(0.3f);

        if (skill_Effect_Prefab != null)
        {
            GameObject skill_Effect = Instantiate(skill_Effect_Prefab, enemy.transform.position, Quaternion.identity, enemy.transform);
            Destroy(skill_Effect, 0.6f);
        }

        yield return new WaitForSeconds(0.2f);

        enemy.TakeDamage(player.Calculate_Skill_Damage());
    }
}
