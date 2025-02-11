using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DoubleHandsSword_Attack", menuName = "Weapon/Attack Strategy/DoubleHandsSword")]
public class DoubleHandsSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject skill_Projectile_Prefab;
    public float projectile_Speed = 10.0f;

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

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (!player.isGrounded)
        {
            player.StartCoroutine(JumpAttack(player));
            return;
        }
        else
        {
            player.animator.SetTrigger("Attack");
            player.isAttacking = true;
            player.cur_AttackCount++;
        }
    }

    private IEnumerator JumpAttack(PlayerCharacter_Controller player)
    {
        while (!player.isGrounded)
        {
            player.is_Knock_Back = true;            

            player.animator.SetTrigger("Attack");
            player.rb.velocity = new Vector2(player.rb.velocity.x, -2f);
            yield return new WaitForSeconds(0.5f);
        }

        player.is_Knock_Back = false;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Vector3 spawn_Pos = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);

        player.animator.SetTrigger("Skill");
        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, spawn_Pos, player.transform.rotation);
        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
        Vector2 shoot_Direction = (player.weapon_Anchor.transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        rb.velocity = shoot_Direction * projectile_Speed;

        Destroy(skill_Projectile, 3.0f);
    }
}
