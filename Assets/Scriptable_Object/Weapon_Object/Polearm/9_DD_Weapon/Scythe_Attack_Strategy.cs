using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scythe_Attack", menuName = "Weapon/Attack Strategy/Scythe")]
public class Scythe_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float dash_Speed = 10.0f;
    public float dash_Duration = 0.5f;
    public int heal_Amount = 10;

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
        return player.cur_AttackCount >= weapon_Data.max_Attack_Count;
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
        //player.isAttacking = false;
        player.cur_AttackCount = 0;
        player.last_Attack_Time = Time.time;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }

    private bool Is_Skill_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Skill_Time + player.skill_Cooldown;
    }

    private void Update_Skill_Timer(PlayerCharacter_Controller player)
    {
        player.last_Skill_Time = Time.time;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (Is_Skill_Cooldown_Complete(player))
        {
            player.StartCoroutine(Dash_Skill(player));
            Update_Skill_Timer(player);
        }
        else
        {
            Debug.Log("Skill is on cooldown");
        }
    }

    private IEnumerator Dash_Skill(PlayerCharacter_Controller player)
    {
        Vector2 dash_Direction = (player.is_Facing_Right) ? Vector2.right : Vector2.left;              
        
        float elapsed = 0.0f;
        float check_Interval = 0.05f;
        float check_Radius = 0.5f;
        float dash_Step = dash_Speed * Time.deltaTime;
        LayerMask enemy_Layer = LayerMask.GetMask("Enemy");
        LayerMask wall_Layer = LayerMask.GetMask("Walls");

        while (elapsed < dash_Duration)
        {
            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, dash_Direction, dash_Step, wall_Layer);

            if (hit.collider != null)
            {
                Debug.Log("Dash blocked by wall");
                break;
            }

            player.transform.Translate(dash_Direction * dash_Speed * Time.deltaTime);

            if (elapsed % check_Interval < Time.deltaTime)
            {
                Collider2D[] hit_Enemies = Physics2D.OverlapCircleAll(player.transform.position, check_Radius, enemy_Layer);

                foreach (Collider2D enemyCollider in hit_Enemies)
                {
                    Enemy_Basic enemy = enemyCollider.GetComponent<Enemy_Basic>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(player.cur_Weapon_Data.skill_Damage);
                        Debug.Log("Enemy hit by Scythe skill attack, Damage : " + player.cur_Weapon_Data.skill_Damage);

                        if (enemy.IR_Health.Value <= 0)
                        {
                            player.Player_Take_Damage(-heal_Amount);

                            if (player.health > player.max_Health)
                            {
                                player.health = player.max_Health;
                            }
                        }
                    }
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.rb.velocity = Vector2.zero;
    }
}
