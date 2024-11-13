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

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();

        player.On_Player_Damaged += Decrease_Stack;
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
        //Debug.Log("카드 공격 실행");
        if (Is_Cooldown_Complete(player))
        {
            Start_Attack(player, weapon_Data);
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
    }

    private void End_Attack(PlayerCharacter_Controller player)
    {
        //player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_Attack_Time = Time.time;
    }
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        //Debug.Log("cur_Stack : " + cur_Stack);
        player.StartCoroutine(Shoot_With_Delay(player, fire_Point));
    }

    private IEnumerator Shoot_With_Delay(PlayerCharacter_Controller player, Transform fire_Point)
    {
        for (int i = 0; i < cur_Stack; i++)
        {
            GameObject projectile_Obj = Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);
            Horse_Projectile projectile = projectile_Obj.GetComponent<Horse_Projectile>();

            Vector2 shootDirection = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
            projectile.Initialized(this, shootDirection, projectile_Speed);

            yield return new WaitForSeconds(shoot_Delay);
        }
    }

    public void Increase_Stack()
    {
        if (cur_Stack < max_Stack)
        {
            cur_Stack++;
            Debug.Log("Stack increased to : " + cur_Stack);
        }
    }

    public void Decrease_Stack()
    {
        if (cur_Stack > 1)
        {
            cur_Stack--;
        }
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {

    }
}
