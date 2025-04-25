using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExecutionSword_Attack", menuName = "Weapon/Attack Strategy/ExecutionSword")]
public class ExecutionSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float move_Distance = 1.0f;
    private float move_Duration = 0.2f;
    public float move_Delay = 0.1f;
    private bool is_Moveing_Forward = false;
    private Queue<Vector3> moveQueue = new Queue<Vector3>();

    public Sprite zeroStack_Weapon_Sprite;
    public Sprite halfStack_Weapon_Sprite;
    public Sprite fullStack_Weapon_Sprite;

    public Weapon_Effect_Data zeroStack_Effect_Data;
    public Weapon_Effect_Data halfStack_Effect_Data;
    public Weapon_Effect_Data fullStack_Effect_Data;

    public int inc_Attack_Dmg = 0;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();

        if (player.es_Stack == 0)
        {
            inc_Attack_Dmg = 0;
        }
        player.attackDamage += inc_Attack_Dmg;

        Update_By_Stack();
        Debug.Log($"Current Es Stack : {player.es_Stack}");

        player.On_Enemy_Killed += Handle_Enemy_Killed;
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;        
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats()
    {
        player.On_Enemy_Killed -= Handle_Enemy_Killed;
        player.attackDamage -= inc_Attack_Dmg;
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

    private void Handle_Enemy_Killed()
    {
        if (player.es_Stack < 100)
        {
            player.es_Stack++;
            if (player.es_Stack == 100)
            {
                player.player_Life++;
            }

            if (player.es_Stack % 10 == 0)
            {
                inc_Attack_Dmg++;
                player.attackDamage++;
            }

            Update_By_Stack();
            Debug.Log($"cur attack dmg{weapon_Data.attack_Damage}");
        }
    }

    private void Update_By_Stack()
    {
        if (player.cur_Weapon_Data != weapon_Data)
            return;

        SpriteRenderer sr = player.weapon_Prefab.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        if (sr != null)
        {
            if (player.es_Stack >= 100)
            {
                sr.sprite = fullStack_Weapon_Sprite;
                weapon_Data.effect_Data = fullStack_Effect_Data;
            }
            else if (player.es_Stack >= 50)
            {
                sr.sprite = halfStack_Weapon_Sprite;
                weapon_Data.effect_Data = halfStack_Effect_Data;
            }
            else
            {
                sr.sprite = zeroStack_Weapon_Sprite;
                weapon_Data.effect_Data = zeroStack_Effect_Data;
            }
        }
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point) { }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data) { }
    
    public void Reset_Dmg()
    {
        inc_Attack_Dmg = 0;
    }
}
