using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyAttack_Attack", menuName = "Weapon/Attack Strategy/EnergyAttack")]
public class EnergyAttack_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float skill_Duration = 3.0f;
    public float pushBack_Force = 2.0f;
    public float energy_Grow_Rate = 5.0f;    
    public float damage_Interval = 0.5f;    

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

    private bool Is_Combo_Complete(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        bool result = player.cur_AttackCount >= weapon_Data.max_Attack_Count;
        //Debug.Log($"Is_Combo_Complete : {result} (cur_Attack Count : {player.cur_AttackCount}), max_Attack Count : {weapon_Data.max_Attack_Count})");
        return result;
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
        if (!Is_Skill_Cooldown_Complete(player))
        {
            Debug.Log($"Skill is on cooldown. Remaining time : {player.last_Skill_Time + weapon_Data.skill_Cooldown - Time.time}");
            return;
        }

        Update_Skill_Timers(player);
        player.StartCoroutine(EnergyWave(player));
    }

    private bool Is_Skill_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Skill_Time + weapon_Data.skill_Cooldown;
    }   
    private void Update_Skill_Timers(PlayerCharacter_Controller player)
    {
        player.last_Skill_Time = Time.time;
    }

    private IEnumerator EnergyWave(PlayerCharacter_Controller player)
    {
        Debug.Log("Energy Wave Start");
        float elapsed_Time = 0.0f;

        GameObject energy_Wave = new GameObject("EnergyWave");
        energy_Wave.transform.position = player.transform.position + new Vector3(player.is_Facing_Right ? 1 : -1, 0, 0);
        energy_Wave.transform.localScale = new Vector3(3, 1, 1);

        SpriteRenderer renderer = energy_Wave.AddComponent<SpriteRenderer>();
        renderer.color = Color.red;
        renderer.sortingOrder = 10;

        BoxCollider2D collider = energy_Wave.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        Dictionary<Collider2D, float> last_Damage_Time = new Dictionary<Collider2D, float>();        

        while (elapsed_Time < skill_Duration)
        {
            float push_Back = pushBack_Force * Time.deltaTime * (player.is_Facing_Right ? -1 : 1);
            player.transform.position += new Vector3(push_Back, 0, 0);

            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(energy_Wave.transform.position, energy_Wave.transform.lossyScale, 0);

            foreach(Collider2D hit in hitEnemies)
            {
                Enemy_Basic enemy = hit.GetComponent<Enemy_Basic>();
                if (enemy != null)
                {
                    if (!last_Damage_Time.ContainsKey(hit) || Time.time >= last_Damage_Time[hit] + damage_Interval)
                    {
                        enemy.TakeDamage(player.cur_Weapon_Data.skill_Damage);
                        last_Damage_Time[hit] = Time.time;
                    }
                }
            }

            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        Destroy(energy_Wave);
        Debug.Log("EnergyWave end");
    }
}