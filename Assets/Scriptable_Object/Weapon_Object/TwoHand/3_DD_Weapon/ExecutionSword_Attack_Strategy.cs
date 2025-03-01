using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExecutionSword_Attack", menuName = "Weapon/Attack Strategy/ExecutionSword")]
public class ExecutionSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    private int kill_Stack = 0;
    public float move_Distance = 1.0f;
    private float move_Duration = 0.2f;
    public float move_Delay = 0.1f;
    private bool is_Moveing_Forward = false;
    private Queue<Vector3> moveQueue = new Queue<Vector3>();

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

        Vector3 target_Pos = player.transform.position +
            (player.is_Facing_Right ? Vector3.right : Vector3.left) * move_Distance;
        moveQueue.Enqueue(target_Pos);

        if (!is_Moveing_Forward)
        {
            player.StartCoroutine(MoveForward_While_Attacking(player));
        }

        Apply_Stack_Effects();
    }

    private IEnumerator MoveForward_While_Attacking(PlayerCharacter_Controller player)
    {
        is_Moveing_Forward = true;
        
        while (moveQueue.Count > 0) 
        {
            Vector3 target_Position = moveQueue.Dequeue();
            Vector3 initial_Position = player.transform.position;
            float elapsed_Time = 0f;

            while (elapsed_Time < move_Duration)
            {
                player.transform.position = Vector3.Lerp(initial_Position, target_Position, elapsed_Time / move_Duration);
                elapsed_Time += Time.deltaTime;
                yield return null;
            }

            player.transform.position = target_Position;

            if (moveQueue.Count > 0)
            {
                yield return new WaitForSeconds(move_Delay);
            }
        }
        
        is_Moveing_Forward = false;
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
