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
        player.attack_Cooldown = weapon_Data.skill_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (Is_Cooldown_Complete(player))
        {
            Debug.Log("Starting Attack - cur_AttackCount: " + player.cur_AttackCount);
            Debug.Log("isAttacking state: " + player.isAttacking);
            Start_Attack(player, weapon_Data);
        }
        else if (Can_Combo_Attack(player, weapon_Data))
        {
            Debug.Log("Continuing Combo - cur_AttackCount: " + player.cur_AttackCount);
            Continue_Combo(player);
        }
        else if (Is_Combo_Complete(player, weapon_Data))
        {
            Debug.Log("Ending Attack Combo");
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
        Debug.Log("Start Attack - cur Attack Count : " + player.cur_AttackCount);
        Debug.Log("isAttacking state: " + player.isAttacking);
        Update_Attack_Timers(player);
    }
    
    private void Continue_Combo(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger("Attack");
        player.cur_AttackCount++;
        player.isAttacking = true;
        Debug.Log("Continue_Combo - cur_AttackCount increased to: " + player.cur_AttackCount);
        Update_Attack_Timers(player);
    }

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
    }    

    private void End_Attack(PlayerCharacter_Controller player)
    {
        player.isAttacking = false;
        player.cur_AttackCount = 0;
        Debug.Log("End_Attack - cur_AttackCount reset to: " + player.cur_AttackCount);
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_Attack_Time = Time.time;
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
            GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, fire_Point.rotation);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            Vector2 shoot_Direction = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
            rb.velocity = shoot_Direction * projectile_Speed;
        }
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, player.transform.position, player.transform.rotation);

        skill_Projectile.transform.localScale = Vector3.one * 0.05f;

        skill_Projectile.transform.DOScale(skill_Projectile_Scale, scale_Up_Duration).SetEase(Ease.OutQuad);

        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
        Vector2 shoot_Direction = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shoot_Direction * projectile_Speed;

        Destroy(skill_Projectile, 3.0f);
    }
}
