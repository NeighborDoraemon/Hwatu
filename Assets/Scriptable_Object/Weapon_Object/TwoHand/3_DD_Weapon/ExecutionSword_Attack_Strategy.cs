using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExecutionSword_Attack", menuName = "Weapon/Attack Strategy/ExecutionSword")]
public class ExecutionSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    private static Dictionary<int, int> weapon_Kill_Stacks = new Dictionary<int, int>();

    private int kill_Stack = 0;
    public float move_Distance = 1.0f;
    private float move_Duration = 0.2f;
    public float move_Delay = 0.1f;
    private bool is_Moveing_Forward = false;
    private Queue<Vector3> moveQueue = new Queue<Vector3>();

    private int weapon_ID;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;
        weapon_ID = weapon_Data.GetInstanceID();

        if (weapon_Kill_Stacks.ContainsKey(weapon_ID))
        {
            kill_Stack = weapon_Kill_Stacks[weapon_ID];
        }
        else
        {
            kill_Stack = 0;
            weapon_Kill_Stacks.Add(weapon_ID, kill_Stack);
        }

        Initialize_Weapon_Data();

        Manage_Stack_By_Card();
        Debug.Log($"Current Es Stack : {kill_Stack}");
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

        if (player.isGrounded)
        {
            Vector3 target_Pos = player.transform.position +
            (player.is_Facing_Right ? Vector3.right : Vector3.left) * move_Distance;
            moveQueue.Enqueue(target_Pos);

            if (!is_Moveing_Forward)
            {
                player.StartCoroutine(MoveForward_While_Attacking(player));
            }
        }

        Apply_Stack_Effects();
        Debug.Log($"Current Es Stack : {kill_Stack}");
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

            weapon_Kill_Stacks[weapon_ID] = kill_Stack;
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

    private void Manage_Stack_By_Card()
    {
        if (player == null) return;

        bool has_Three_Card = false;
        bool has_ThreeGwang_Card = false;

        for (int i = 0; i < player.card_Inventory.Length; i++)
        {
            GameObject card_Obj = player.card_Inventory[i];
            if (card_Obj != null)
            {
                int month = card_Obj.GetComponent<Card>().cardValue.Month;
                if (month == 3) has_Three_Card = true;
                if (month == 13) has_ThreeGwang_Card = true;
            }
        }

        if (!has_Three_Card && !has_ThreeGwang_Card)
        {
            Reset_Stack();
            Debug.Log("Es Stack is reset!");
        }
    }

    public void Reset_Stack()
    {
        kill_Stack = 0;
        if (weapon_Kill_Stacks.ContainsKey(weapon_ID))
        {
            weapon_Kill_Stacks[weapon_ID] = 0;
        }
    }
}
