using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Canon_Attack", menuName = "Weapon/Attack Strategy/Canon")]
public class Canon_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public GameObject skill_Projectile_Prefab;
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
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.isAttacking = true;
        player.animator.SetTrigger("Attack");
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        float y_Offset = 0.6f;

        Vector3 spawn_Position = fire_Point.position;
        spawn_Position.y -= y_Offset;

        GameObject projectile = Instantiate(projectile_Prefab, spawn_Position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        rb.gravityScale = 1.0f;

        float angle = player.is_Facing_Right ? shoot_Angle : 180 - shoot_Angle;

        Vector2 launch_Velocity = Quaternion.Euler(0, 0, angle) * Vector2.right * projectile_Speed;
        rb.velocity = launch_Velocity;
    }
    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");

        Cannon_Animation_Controller cac = player.weapon_Prefab.GetComponent<Cannon_Animation_Controller>();
        cac.Trigger_Skill();

        float y_Offset = 0.6f;

        Vector3 spawn_Position = player.weapon_Anchor.position;
        spawn_Position.y -= y_Offset;

        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, spawn_Position, Quaternion.identity);
        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();

        rb.gravityScale = 1.0f;

        float angle = player.is_Facing_Right ? shoot_Angle : 180 - shoot_Angle;

        Vector2 launch_Velocity = Quaternion.Euler(0, 0, angle) * Vector2.right * projectile_Speed;
        rb.velocity = launch_Velocity;

        return true;
    }
}
