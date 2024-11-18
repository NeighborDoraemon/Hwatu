using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleHandsSword_Attack", menuName = "Weapon/Attack Strategy/DoubleHandsSword")]
public class DoubleHandsSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject skill_Projectile_Prefab;
    public float projectile_Speed = 10.0f;

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
        if (!player.isGrounded)
        {
            player.StartCoroutine(JumpAttack(player));
            return;
        }

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

    private IEnumerator JumpAttack(PlayerCharacter_Controller player)
    {
        while (!player.isGrounded)
        {
            player.animator.SetTrigger("Attack");
            player.rb.velocity = new Vector2(player.rb.velocity.x, -2f);
            yield return new WaitForSeconds(0.5f);
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

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        bool result = player.cur_AttackCount >= weapon_Data.max_Attack_Count;
        //Debug.Log($"Is_Combo_Complete : {result} (cur_Attack Count : {player.cur_AttackCount}), max_Attack Count : {weapon_Data.max_Attack_Count})");
        return result;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.attackDamage = weapon_Data.attack_Damage;
        player.isAttacking = true;
        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);

        Debug.Log($"Start Attack : {player.attackDamage}");
    }

    private void Continue_Combo(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger("Attack");
        player.cur_AttackCount++;
        player.isAttacking = true;
        Debug.Log("Continue_Combo - cur_Attack Count : " + player.cur_AttackCount);
        Update_Attack_Timers(player);
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
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (Is_Skill_Cooldown_Complete(player))
        {
            Start_Skill(player, weapon_Data);
        }
        else
        {
            Debug.Log("Skill is on cooldown");
        }
    }

    private bool Is_Skill_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Skill_Time + weapon_Data.skill_Cooldown;
    }

    private void Start_Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");
        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, player.transform.position, player.transform.rotation);                
        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
        Vector2 shoot_Direction = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shoot_Direction * projectile_Speed;

        Destroy(skill_Projectile, 3.0f);
    }

    private void Update_Skill_Timers(PlayerCharacter_Controller player)
    {
        player.last_Skill_Time = Time.time;
    }
}
