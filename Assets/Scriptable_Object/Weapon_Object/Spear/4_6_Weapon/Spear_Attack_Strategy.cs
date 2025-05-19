using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spear_Attack", menuName = "Weapon/Attack Strategy/Spear")]
public class Spear_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Dash Skill Settings")]
    public float dash_Speed = 15.0f;
    public float dash_Distance = 5.0f;
    public float dash_Hit_Radius = 0.5f;
    public LayerMask enemy_Layer;

    private bool is_Dashing = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        is_Dashing = false;

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
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (is_Dashing) return;

        Vector2 input_Dir = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            );

        if (input_Dir == Vector2.zero)
        {
            input_Dir = player.is_Facing_Right
                ? Vector2.right
                : Vector2.left;
        }

        input_Dir.Normalize();

        player.StartCoroutine(Dash_Coroutine(input_Dir));
    }

    private IEnumerator Dash_Coroutine(Vector2 dir)
    {
        is_Dashing = true;

        float duration = dash_Distance / dash_Speed;
        float elapsed = 0.0f;

        float og_Gravity = player.rb.gravityScale;
        player.rb.gravityScale = 0.0f;

        //player.animator.SetTrigger("Skill");

        HashSet<Collider2D> damaged = new HashSet<Collider2D>();

        while(elapsed < duration)
        {
            player.rb.velocity = dir * dash_Speed;

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                player.transform.position,
                dash_Hit_Radius,
                enemy_Layer
                );
            foreach(var col in hits)
            {
                if (!damaged.Contains(col))
                {
                    damaged.Add(col);
                    var enemy = col.GetComponent<Enemy_Basic>();
                    if (enemy != null)
                        enemy.TakeDamage(player.Calculate_Skill_Damage());
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        player.rb.velocity = Vector2.zero;
        player.rb.gravityScale = og_Gravity;
        is_Dashing = false;
    }
}
