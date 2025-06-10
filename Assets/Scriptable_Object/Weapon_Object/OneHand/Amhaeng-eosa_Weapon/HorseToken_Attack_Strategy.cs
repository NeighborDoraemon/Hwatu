using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HorseToken_Attack", menuName = "Weapon/Attack Strategy/HorseToken")]
public class HorseToken_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject projectile_Prefab;
    public float projectile_Speed = 20.0f;
    public float shoot_Delay = 0.2f;

    public int max_Stack = 5;
    private int cur_Stack = 1;

    public bool is_Skill_On = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        cur_Stack = 1;
        is_Skill_On = false;

        Initialize_Weapon_Data();

        player.On_Player_Damaged += Decrease_Stack;
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
        player.On_Player_Damaged -= Decrease_Stack;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        Debug.Log("cur_Stack : " + cur_Stack);
        player.StartCoroutine(Shoot_With_Delay(player, fire_Point, cur_Stack));
    }

    private IEnumerator Shoot_With_Delay(PlayerCharacter_Controller player, Transform fire_Point, int fire_Stack)
    {
        int shots_To_Fire = fire_Stack;

        for (int i = 0; i < shots_To_Fire; i++)
        {
            Vector3 spawn_Position = fire_Point.position + new Vector3(0, 0.5f, 0);
            GameObject projectile_Obj = Instantiate(projectile_Prefab, spawn_Position, Quaternion.identity);
            Horse_Projectile projectile = projectile_Obj.GetComponent<Horse_Projectile>();

            Vector2 shootDirection = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
            projectile.transform.localScale = new Vector3(
                Mathf.Abs(projectile.transform.localScale.x) * (player.is_Facing_Right ? 1 : -1),
                projectile.transform.localScale.y,
                projectile.transform.localScale.z);

            int final_Damage = Calc_Horse_Damage(weapon_Data.attack_Damage);
            projectile.Initialized(this, shootDirection, projectile_Speed, final_Damage);

            yield return new WaitForSeconds(shoot_Delay);
        }
    }

    private int Calc_Horse_Damage(int Damage)
    {
        int raw_WeaponDmg = is_Skill_On ? Damage * 4 : Damage;
        int base_Dmg = raw_WeaponDmg + player.attackDamage;

        int total_Dmg = Mathf.RoundToInt(base_Dmg * player.damage_Mul);

        if (Random.value <= player.crit_Rate)
            total_Dmg = Mathf.RoundToInt(total_Dmg * player.crit_Dmg);

        return total_Dmg;
    }

    public void Increase_Stack()
    {
        if (is_Skill_On) return;

        if (cur_Stack < max_Stack)
        {
            cur_Stack++;
            Debug.Log("Stack increased to : " + cur_Stack);
        }
    }
    public void Decrease_Stack()
    {
        if (is_Skill_On) return;

        if (cur_Stack > 1)
        {
            cur_Stack--;
        }
    }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        is_Skill_On = !is_Skill_On;

        if (is_Skill_On)
        {
            cur_Stack = 1;
            Debug.Log("마패 스킬 ON");
        }
        else
        {
            Debug.Log("마패 스킬 OFF");
        }

        return true;
    }
}
