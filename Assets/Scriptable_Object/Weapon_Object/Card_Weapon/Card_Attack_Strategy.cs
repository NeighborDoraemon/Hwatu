using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_Attack", menuName = "Weapon/Attack Strategy/Card")]
public class Card_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;
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
        player.attack_Cooldown = weapon_Data.skill_Cooldown;
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
        else if (Can_Combo_Attack(player, weapon_Data))
        {
            Continue_Combo(player);
        }
        else if (Is_Combo_Complete(player,weapon_Data))
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

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger(weapon_Data.attack_Trigger);
        player.isAttacking = true;
        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);
    }

    private void Continue_Combo(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger($"Attack_{player.cur_AttackCount + 1}");
        player.cur_AttackCount++;
        player.isAttacking = true;
        Update_Attack_Timers(player);
    }

    private void End_Attack(PlayerCharacter_Controller player)
    {
        //player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }
    public void Shoot(PlayerCharacter_Controller player, GameObject prefab, Transform fire_Point)
    {
        GameObject projectile = Instantiate(prefab, fire_Point.position, fire_Point.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.weapon_Anchor.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shootDirection * projectile_Speed;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
