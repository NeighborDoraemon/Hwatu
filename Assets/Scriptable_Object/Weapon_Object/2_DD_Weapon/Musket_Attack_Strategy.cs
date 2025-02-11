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

        is_Empty = false;
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;        
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    private enum WeaponState
    {
        Ready,
        Empty,
        Reloading
    }
    private WeaponState cur_WeaponState = WeaponState.Ready;

    private void Update_WeaponState(WeaponState newState)
    {
        cur_WeaponState = newState;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        switch (cur_WeaponState)
        {
            case WeaponState.Reloading:
                return;
            case WeaponState.Empty:
                Reload(player);
                return;
            case WeaponState.Ready:
                player.animator.SetTrigger("Attack");
                player.isAttacking = true;
                player.cur_AttackCount++;                
                return;
        }
    }

    public void Reload(PlayerCharacter_Controller player)
    {
        if (cur_WeaponState == WeaponState.Reloading)
        {
            Debug.Log("Reload is already in progress");
            return;
        }

        player.StartCoroutine(Reload_Coroutine(player));
    }

    private IEnumerator Reload_Coroutine(PlayerCharacter_Controller player)
    {
        Update_WeaponState(WeaponState.Reloading);        

        yield return new WaitForSeconds(player.attack_Cooldown);
        
        Update_WeaponState(WeaponState.Ready);
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        if (cur_WeaponState == WeaponState.Empty || cur_WeaponState == WeaponState.Reloading)
        {
            return;
        }

        GameObject projectile = Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        rb.velocity = shootDirection * projectile_Speed;

        Update_WeaponState(WeaponState.Empty);
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
