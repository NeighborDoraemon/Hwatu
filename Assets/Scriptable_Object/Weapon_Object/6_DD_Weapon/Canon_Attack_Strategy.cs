using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Canon_Attack", menuName = "Weapon/Attack Strategy/Canon")]
public class Canon_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public float projectile_Speed = 5.0f;
    public float shoot_Angle = 45.0f;

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
    }
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, fire_Point.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.gravityScale = 1.0f;

        float angle = player.is_Facing_Right ? shoot_Angle : 180 - shoot_Angle;

        Vector2 launch_Velocity = Quaternion.Euler(0, 0, angle) * Vector2.right * projectile_Speed;
        rb.velocity = launch_Velocity;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
