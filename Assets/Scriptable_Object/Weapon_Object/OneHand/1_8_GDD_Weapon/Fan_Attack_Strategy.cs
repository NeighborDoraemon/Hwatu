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
        player.attackDamage = weapon_Data.attack_Damage;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (Is_Cooldown_Complete(player))
        {
            Start_Attack(player, weapon_Data);
        }
        else if (Can_Combo_Attack(player, weapon_Data))
        {
            Continue_Combo(player);
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
    private bool Can_Combo_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.isAttacking &&
               player.cur_AttackCount < weapon_Data.max_Attack_Count &&
               Time.time - player.last_ComboAttack_Time <= player.comboTime;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount = 1;        
        Update_Attack_Timers(player);
    }
    
    private void Continue_Combo(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger("Attack");
        player.cur_AttackCount = 2;
        player.isAttacking = true;
        Update_Attack_Timers(player);
    }

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.cur_AttackCount >= player.max_AttackCount;
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

    private bool Is_Skill_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Skill_Time + player.skill_Cooldown;
    }

    private void Update_Skill_Timer(PlayerCharacter_Controller player)
    {
        player.last_Skill_Time = Time.time;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        GameObject projectile_Prefab = null;

        if (player.cur_AttackCount == 1)
        {
            projectile_Prefab = first_Projectile_Prefab;
        }
        else if (player.cur_AttackCount == 2)
        {
            projectile_Prefab = second_Projectile_Prefab;
        }

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
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (Is_Skill_Cooldown_Complete(player))
        {
            GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, player.transform.position, player.transform.rotation);

            skill_Projectile.transform.localScale = Vector3.one * 0.05f;

            skill_Projectile.transform.DOScale(skill_Projectile_Scale, scale_Up_Duration).SetEase(Ease.OutQuad);

            Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
            Vector2 shoot_Direction = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
            rb.velocity = shoot_Direction * projectile_Speed;

            Destroy(skill_Projectile, 3.0f);
            Update_Skill_Timer(player);
        }
        else
        {
            Debug.Log("Skill is on cooldown");
        }
    }
}
