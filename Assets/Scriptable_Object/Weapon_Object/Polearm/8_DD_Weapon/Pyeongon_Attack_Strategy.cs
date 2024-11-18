using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pyeongon_Attack", menuName = "Weapon/Attack Strategy/Pyeongon")]
public class Pyeongon_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    private float original_Y_Value = 0;

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
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        //player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        //player.cur_AttackCount = 1;
        Update_Attack_Timers(player);
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
        float duration = 1.0f;
        float move_Speed = 4.0f;
        player.StartCoroutine(Air_Move(player, duration, move_Speed));
    }

    private IEnumerator Air_Move(PlayerCharacter_Controller player, float duration, float move_Speed)
    {
        Lock_Y_Position(player);
        player.is_Knock_Back = true;

        Vector2 original_Velocity = player.rb.velocity;
        player.rb.velocity = Vector2.zero;

        float diretion = player.is_Facing_Right ? 1 : -1;
        float elapsed_Time = 0.0f;

        while(elapsed_Time < duration)
        {
            player.transform.position += new Vector3(move_Speed * diretion * Time.deltaTime, 0, 0);

            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        player.rb.velocity = original_Velocity;

        Unlock_Y_Position(player);
        player.is_Knock_Back = false;
    }

    private void Lock_Y_Position(PlayerCharacter_Controller player)
    {
        original_Y_Value = player.transform.position.y;

        player.rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    private void Unlock_Y_Position(PlayerCharacter_Controller player)
    {
        player.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
