using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Musket_Attack", menuName = "Weapon/Attack Strategy/Musket")]
public class Musket_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public float projectile_Speed = 20.0f;

    private bool is_Reloading = false;

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
        player.attack_Cooldown = weapon_Data.skill_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (is_Reloading)
        {
            Debug.Log("Empty..Press Attack Button to reload.");
            Reload();
        }

        if (Is_Cooldown_Complete(player))
        {
            Start_Attack(player, weapon_Data);
        }
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);

        is_Reloading = true;
    }

    public void Reload()
    {
        if (is_Reloading)
        {
            Debug.Log("Reloading!");
            is_Reloading = false;
            //player.animator.SetTrigger("Reload");
        }
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_Attack_Time = Time.time;
    }
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, fire_Point.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shootDirection * projectile_Speed;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
