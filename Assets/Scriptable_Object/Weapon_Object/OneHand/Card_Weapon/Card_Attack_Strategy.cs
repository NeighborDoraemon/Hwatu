using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_Attack", menuName = "Weapon/Attack Strategy/Card")]
public class Card_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    private float projectile_Speed = 10.0f;
    
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
        Debug.Log("Attack function called");

        if (Is_Cooldown_Complete(player))
        {
            Debug.Log("Starting Attack - cur_Attack Count" + player.cur_AttackCount);
            Start_Attack(player, weapon_Data);
        }
        else if (Can_Combo_Attack(player, weapon_Data))
        {
            Debug.Log("Continuing Combo - cur_Attack Count" + player.cur_AttackCount);
            Continue_Combo(player);
        }
        else if (Is_Combo_Complete(player,weapon_Data))
        {
            Debug.Log("Combo complete, calling End_Attack");
            End_Attack(player);
        }
        else
        {
            Debug.Log("No conditions met for combo or attack completion");
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
        Debug.Log($"Is_Combo_Complete : {result} (cur_Attack Count : {player.cur_AttackCount}), max_Attack Count : {weapon_Data.max_Attack_Count})");
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
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
        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, fire_Point.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.weapon_Anchor.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shootDirection * projectile_Speed;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
