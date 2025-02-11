using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Tiger_Attack", menuName = "Weapon/Attack Strategy/Tiger")]
public class Tiger_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public Vector2 roar_Size = new Vector2(4.0f, 2.0f);
    public Vector2 roar_Offset = new Vector2(2.0f, 0.0f);

    private CapsuleCollider2D og_Collider;
    private BoxCollider2D tiger_Collider;
    private int og_Maxhealth;
    private float og_Movespeed;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        og_Collider = player.GetComponent<CapsuleCollider2D>();
        if (og_Collider != null)
        {
            og_Collider.enabled = false;
        }

        tiger_Collider = player.gameObject.AddComponent<BoxCollider2D>();
        tiger_Collider.size = new Vector2(0.6f, 0.35f);
        tiger_Collider.offset = new Vector2(0, -0.3f);
        tiger_Collider.isTrigger = true;

        og_Maxhealth = player.max_Health;
        og_Movespeed = player.movementSpeed;

        player.max_Health = Mathf.RoundToInt(og_Maxhealth * 1.3f);
        player.movementSpeed = og_Movespeed * 1.1f;

        Initialize_Weapon_Data();
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
        if (tiger_Collider != null)
        {
            Destroy(tiger_Collider);
        }

        if (og_Collider != null)
        {
            og_Collider.enabled = true;
        }
        
        player.max_Health = og_Maxhealth;
        player.movementSpeed = og_Movespeed;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
        player.cur_AttackCount++;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Vector2 roar_Center = player.transform.position;
        roar_Center += (player.is_Facing_Right ? roar_Offset : new Vector2(-roar_Offset.x, roar_Offset.y));

        Collider2D[] enemies = Physics2D.OverlapBoxAll(roar_Center, roar_Size, 0, LayerMask.GetMask("Enemy"));

        foreach (Collider2D enemy in enemies)
        {
            Enemy_Basic enemy_Controller = enemy.GetComponent<Enemy_Basic>();
            if (enemy_Controller != null)
            {
                enemy_Controller.TakeDamage(weapon_Data.skill_Damage);
            }
        }
    }
}
