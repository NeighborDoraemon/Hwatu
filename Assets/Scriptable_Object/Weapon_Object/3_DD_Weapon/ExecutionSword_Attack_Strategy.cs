using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExecutionSword_Attack", menuName = "Weapon/Attack Strategy/ExecutionSword")]
public class ExecutionSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    private int kill_Stack = 0;
    private float move_Distance = 1.0f;
    private float move_Duration = 0.2f;
    private bool is_Moveing_Forward = false;

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

        if (!is_Moveing_Forward)
        {
            player.StartCoroutine(MoveForward_While_Attacking(player));
        }
        
        Apply_Stack_Effects();
    }

    private bool Can_Combo_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return player.isAttacking &&
               player.cur_AttackCount < weapon_Data.max_Attack_Count &&
               Time.time - player.last_ComboAttack_Time <= player.comboTime;
    }

    private void Continue_Combo(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger("Attack");
        player.cur_AttackCount++;
        player.isAttacking = true;
        Update_Attack_Timers(player);

        if (!is_Moveing_Forward)
        {
            player.StartCoroutine(MoveForward_While_Attacking(player));
        }

        Apply_Stack_Effects();
    }

    private IEnumerator MoveForward_While_Attacking(PlayerCharacter_Controller player)
    {
        is_Moveing_Forward = true;
        float elapsed_Time = 0f;
        Vector3 initialPosition = player.transform.position;
        Vector3 target_Position = initialPosition + (player.is_Facing_Right ? Vector3.right : Vector3.left) * move_Distance;

        while (elapsed_Time < move_Duration) 
        {
            player.transform.position = Vector3.Lerp(initialPosition, target_Position, elapsed_Time / move_Duration);
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        player.transform.position = target_Position;
        is_Moveing_Forward = false;
    }

    private void End_Attack(PlayerCharacter_Controller player)
    {
        player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }

    public void Add_Kill_Stack()
    {
        if (kill_Stack < 100)
        {
            kill_Stack++;
            if (kill_Stack % 10 == 0)
            {
                player.attackDamage += 1;

            }

            if (kill_Stack == 100)
            {
                player.player_Life++;
            }
        }
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }

    private void Apply_Stack_Effects()
    {
        player.attackDamage = weapon_Data.attack_Damage + (kill_Stack / 10);        
    }
}
