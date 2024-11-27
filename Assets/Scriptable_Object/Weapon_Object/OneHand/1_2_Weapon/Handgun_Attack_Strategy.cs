using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "Handgun_Attack", menuName = "Weapon/Attack Strategy/Handgun")]
public class Handgun_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public float projectile_Speed = 20.0f;

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
        //Debug.Log("카드 공격 실행");
        if (Is_Cooldown_Complete(player))
        {
            Start_Attack(player, weapon_Data);
        }
        else if (Is_Combo_Complete(player, weapon_Data))
        {
            End_Attack(player);
        }
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);
    }

    private void End_Attack(PlayerCharacter_Controller player)
    {
        player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {        
        player.last_Attack_Time = Time.time;
        player.last_ComboAttack_Time = Time.time;
    }
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        Vector3 projectile_Scale = projectile.transform.localScale;
        projectile_Scale.x = (player.is_Facing_Right) ? Mathf.Abs(projectile_Scale.x) : -Mathf.Abs(projectile_Scale.x);
        projectile.transform.localScale = projectile_Scale;

        rb.velocity = shootDirection * projectile_Speed;
        projectile.transform.rotation = Quaternion.identity;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
