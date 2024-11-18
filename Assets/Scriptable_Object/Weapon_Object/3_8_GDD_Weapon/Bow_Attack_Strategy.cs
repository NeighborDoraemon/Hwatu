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

    private bool is_Ready_To_Shoot = false;
    private bool is_Charging = false;
    private float charge_Time = 0.0f;

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
        player.attackDamage = weapon_Data.attack_Damage;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        is_Charging = true;
        is_Ready_To_Shoot = false;
        charge_Time = 0;
        player.animator.SetTrigger("Attack");
    }

    public void Release_Attack(PlayerCharacter_Controller player)
    {
        if (is_Charging)
        {
            is_Charging = false;
            is_Ready_To_Shoot = true;

            player.animator.SetTrigger("Attack");           
        }
    }

    public void Update_Charge_Time()
    {
        if (is_Charging)
        {
            charge_Time += Time.deltaTime;
        }
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        if (is_Ready_To_Shoot)
        {
            bool is_Charged_Shot = charge_Time >= charge_Time_Threshold;
            GameObject arrow_Prefab = is_Charged_Shot ? charged_Arrow_Prefab : normal_Arrow_Prefab;
            Fire_Arrow(player, arrow_Prefab, fire_Point, is_Charged_Shot);

            is_Ready_To_Shoot = false;
        }        
    }

    private void Fire_Arrow(PlayerCharacter_Controller player, GameObject projectile_Prefab, Transform fire_Point, bool is_Charged_Shot)
    {
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, fire_Point.rotation);
        Rigidbody2D rb = projectile.GetComponent <Rigidbody2D>();

        Vector2 shoot_Direction = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shoot_Direction * (is_Charged_Shot ? charge_Projectile_Speed : projectile_Speed);
    }

    public void OnUpdate()
    {
        Update_Charge_Time();
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
