using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fan_Attack", menuName = "Weapon/Attack Strategy/Fan")]
public class Fan_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject first_Projectile_Prefab;
    public GameObject second_Projectile_Prefab;
    public GameObject skill_Projectile_Prefab;

    public float projectile_Speed = 10.0f;
    public float skill_Projectile_Speed = 10.0f;
    public float skill_Projectile_Scale = 1.0f;
    public float scale_Up_Duration = 1.5f;

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
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount++;
    }    
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject projectile_Prefab = Get_Pj_By_Atk_Animation(player);

        if (projectile_Prefab != null)
        {
            GameObject projectile = Instantiate(projectile_Prefab, player.transform.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 shoot_Direction = (player.is_Facing_Right) ? Vector2.right : Vector2.left;

            Vector3 projectile_Scale = projectile.transform.localScale;
            projectile_Scale.x = (player.is_Facing_Right) ? Mathf.Abs(projectile_Scale.x) : -Mathf.Abs(projectile_Scale.x);
            projectile.transform.localScale = projectile_Scale;

            rb.velocity = shoot_Direction * projectile_Speed;
        }
    }

    private GameObject Get_Pj_By_Atk_Animation(PlayerCharacter_Controller player)
    {
        AnimatorStateInfo state_Info = player.animator.GetCurrentAnimatorStateInfo(0);

        if (state_Info.IsTag("Attack_1"))
        {
            return first_Projectile_Prefab;
        }
        else if (state_Info.IsTag("Attack_2"))
        {
            return second_Projectile_Prefab;
        }

        return null;
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Vector3 spawn_Pos = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);

        player.animator.SetTrigger("Skill");

        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, spawn_Pos, Quaternion.identity);

        skill_Projectile.transform.localScale = Vector3.one;

        skill_Projectile.transform.DOScale(skill_Projectile_Scale, scale_Up_Duration).SetEase(Ease.OutQuad);

        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
        Vector2 shoot_Direction = (player.is_Facing_Right ? Vector2.right : Vector2.left);
        rb.velocity = shoot_Direction * projectile_Speed;

        Destroy(skill_Projectile, 3.0f);
    }
}
