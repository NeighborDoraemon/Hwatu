using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scythe_Attack", menuName = "Weapon/Attack Strategy/Scythe")]
public class Scythe_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Skill Settings")]
    public float dash_Speed = 10.0f;
    public float dash_Duration = 0.5f;
    public int heal_Amount = 10;

    [Header("LayerMask Settings")]
    public LayerMask enemy_LayerMask;
    public LayerMask wall_LayerMask;

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

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        //player.cur_AttackCount++;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        
    }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");
        player.StartCoroutine(Dash_Skill(player));        

        return true;
    }

    private IEnumerator Dash_Skill(PlayerCharacter_Controller player)
    {
        Vector2 dash_Direction = (player.is_Facing_Right) ? Vector2.right : Vector2.left;              
        
        float elapsed = 0.0f;
        float check_Interval = 0.05f;
        float check_Radius = 0.5f;

        LayerMask boss_Layer = LayerMask.GetMask("Boss_Enemy");

        player.rb.velocity = dash_Direction * dash_Speed;
        player.is_Knock_Back = true;

        while (elapsed < dash_Duration)
        {
            player.is_Knock_Back = true;

            RaycastHit2D hit = Physics2D.Raycast(player.transform.position, dash_Direction, 0.1f, wall_LayerMask);
            RaycastHit2D hit_boss = Physics2D.Raycast(player.transform.position, dash_Direction, 0.1f, boss_Layer);

            if (hit.collider != null || hit_boss.collider != null)
            {
                Debug.Log("Dash blocked by wall");
                break;
            }

            if (elapsed % check_Interval < Time.deltaTime)
            {
                Collider2D[] hit_Enemies = Physics2D.OverlapCircleAll(player.transform.position, check_Radius, enemy_LayerMask);

                foreach (Collider2D enemyCollider in hit_Enemies)
                {
                    Enemy_Basic enemy = enemyCollider.GetComponent<Enemy_Basic>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(player.Calculate_Skill_Damage());
                        Debug.Log("Enemy hit by Scythe skill attack, Damage : " + player.Calculate_Skill_Damage());

                        if (enemy.IR_Health.Value <= 0)
                        {
                            player.Player_Take_Heal(heal_Amount);
                        }
                    }
                }
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        player.rb.velocity = Vector2.zero;
        player.is_Knock_Back = false;
    }
}
