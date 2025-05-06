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

    private bool is_Channeling = false;
    private float forced_Cooltime_End_Time = 0.0f;

    public GameObject skill_Projectile_Prefab;
    public float skill_Projectile_Speed = 20.0f;
    public float channel_Duration = 2.0f;
    public float skill_Cancel_Cooltime = 2.0f;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();

        forced_Cooltime_End_Time = -Mathf.Infinity;
        is_Channeling = false;
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;        
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { }

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
        //Debug.Log($"[Musket.Skill] Time.time={Time.time:f2}, forcedEnd={forced_Cooltime_End_Time:f2}, isCh={is_Channeling}");
        if (Time.time < forced_Cooltime_End_Time) return;
        if (is_Channeling) return;

        player.StartCoroutine(Channel_And_Fire(player));
    }

    private IEnumerator Channel_And_Fire(PlayerCharacter_Controller player)
    {
        is_Channeling = true;
        Vector3 lastPos = player.transform.position;
        float elapsed = 0.0f;

        player.animator.SetBool("isHoldAtk", true);
        while (elapsed < channel_Duration)
        {
            if ((player.transform.position - lastPos).sqrMagnitude > Mathf.Epsilon)
            {
                forced_Cooltime_End_Time = Time.time + skill_Cancel_Cooltime;
                is_Channeling = false;
                player.animator.SetBool("isHoldAtk", false);
                yield break;
            }

            lastPos = player.transform.position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.animator.SetBool("isHoldAtk", false);
        is_Channeling = false;
        
        Vector3 spawn_Pos = player.weapon_Anchor.position;
        GameObject proj = Instantiate(skill_Projectile_Prefab, spawn_Pos, Quaternion.identity);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        rb.velocity = (player.is_Facing_Right ? Vector2.right : Vector2.left) * projectile_Speed;
        player.animator.SetTrigger("Skill");
        //Destroy(proj, 5.0f);

        player.Update_Skill_Timer();
    }
}
