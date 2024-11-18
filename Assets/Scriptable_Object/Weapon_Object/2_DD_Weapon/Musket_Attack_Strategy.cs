using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Musket_Attack", menuName = "Weapon/Attack Strategy/Musket")]
public class Musket_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public float projectile_Speed = 20.0f;

    private bool is_Reloading = false;
    private bool is_Empty = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();

        is_Reloading = false;
        is_Empty = false;
        //Debug.Log($"is_Reloading : {is_Reloading}, is_Empty : {is_Empty}");
        //Debug.Log($"Attack Cooldown : {player.attack_Cooldown}");
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
        //Debug.Log($"Attack called. is_Reloading : {is_Reloading}, is_Empty : {is_Empty}");

        if (is_Reloading)
        {
            Debug.Log("Currently Reloading");
            return;
        }
        
        if (is_Empty)
        {
            Debug.Log("Empty.");
            Reload(player);
            return;
        }

        if (!is_Empty && !is_Reloading)
        {
            Debug.Log("Start Attack");
            Start_Attack(player, weapon_Data);
        }        
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private void Start_Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount = 1;
        Update_Attack_Timers(player);

        is_Empty = true;
        //Debug.Log("Attack Completed. Ammo now empty");
    }

    public void Reload(PlayerCharacter_Controller player)
    {
        if (is_Reloading)
        {
            Debug.Log("Reload is already in progress");
            return;           
        }
        else
        {
            player.StartCoroutine(Reload_Coroutine(player));
        }        
    }

    private IEnumerator Reload_Coroutine(PlayerCharacter_Controller player)
    {
        is_Reloading = true;
        //Debug.Log($"Reloading Start : {Time.time} seconds");

        yield return new WaitForSeconds(player.attack_Cooldown);

        //Debug.Log($"Reload Complete : {Time.time}");
        is_Reloading = false;
        is_Empty = false;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_Attack_Time = Time.time;
        player.last_ComboAttack_Time = Time.time;
    }
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        if (is_Empty || is_Reloading)
        {
            //Debug.Log("Can't shoot while reloading or empty");
            return;
        }

        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        rb.velocity = shootDirection * projectile_Speed;

        is_Empty = true;
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
