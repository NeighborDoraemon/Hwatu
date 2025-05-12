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

    [Header("Normal Bullet")]
    public GameObject projectile_Prefab;
    public float projectile_Speed = 20.0f;

    [Header("Skill Bullet")]
    public int skill_Bullet_Count = 3;
    public GameObject skill_Projectile_Prefab;

    private int remaining_Bullet = 0;

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

    public void Reset_Stats() { remaining_Bullet = 0; }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject prefab_To_Use;

        if (remaining_Bullet > 0)
        {
            prefab_To_Use = skill_Projectile_Prefab;
            remaining_Bullet--;
        }
        else
        {
            prefab_To_Use = projectile_Prefab;
        }

        GameObject projectile = Instantiate(prefab_To_Use, fire_Point.position, Quaternion.identity);
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
        player.animator.SetTrigger("Skill");
        remaining_Bullet = skill_Bullet_Count;
        //player.StartCoroutine(Skill_Coroutine(player));
    }

    private IEnumerator Skill_Coroutine(PlayerCharacter_Controller player)
    {
        yield return new WaitForSeconds(0.3f);

        GameObject projectile = Instantiate(skill_Projectile_Prefab, player.weapon_Anchor.position, Quaternion.identity);

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        Vector3 projectile_Scale = projectile.transform.localScale;
        projectile_Scale.x = (player.is_Facing_Right) ? Mathf.Abs(projectile_Scale.x) : -Mathf.Abs(projectile_Scale.x);
        projectile.transform.localScale = projectile_Scale;

        rb.velocity = shootDirection * projectile_Speed;
        projectile.transform.rotation = Quaternion.identity;
    }
}
