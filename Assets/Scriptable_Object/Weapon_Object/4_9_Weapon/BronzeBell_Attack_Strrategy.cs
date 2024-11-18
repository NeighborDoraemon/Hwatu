using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BronzeBell_Attack", menuName = "Weapon/Attack Strategy/BronzeBell")]
public class BronzeBell_Attack_Strrategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [SerializeField] private float attack_Range = 2.5f;            

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
        if (!player.isAttacking)
        {
            player.isAttacking = true;
        }

        LayerMask enemy_Layer = LayerMask.GetMask("Enemy");

        Collider2D[] hit_Enemies = Physics2D.OverlapCircleAll(player.transform.position, attack_Range, enemy_Layer);

        foreach (Collider2D enemy_Collider in hit_Enemies)
        {
            Enemy_Basic enemy = enemy_Collider.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.attackDamage);
                Debug.Log("Enemy hit by hold attack, Damage : " + player.attackDamage);
            }
            else
            {
                Debug.LogWarning($"Enemy_Basic component not found on : {enemy_Collider.name}");
            }
        }

        Update_Attack_Timers(player);
    }

    private bool Is_Cooldown_Complete(PlayerCharacter_Controller player)
    {
        return Time.time >= player.last_Attack_Time + player.attack_Cooldown;
    }

    private void Update_Attack_Timers(PlayerCharacter_Controller player)
    {
        player.last_ComboAttack_Time = Time.time;
        player.last_Attack_Time = Time.time;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    private bool Is_Skill_Cooldown_Complete()
    {
        return Time.time >= player.last_Skill_Time + player.skill_Cooldown;
    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        
    }
}
