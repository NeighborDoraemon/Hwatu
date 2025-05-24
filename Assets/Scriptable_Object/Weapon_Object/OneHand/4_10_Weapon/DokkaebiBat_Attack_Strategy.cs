using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[CreateAssetMenu(fileName = "DokkaebiBat_Attack", menuName = "Weapon/Attack Strategy/DokkaebiBat")]
public class DokkaebiBat_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Skill Settings")]
    public float skill_Range = 1.5f;
    public Vector2 skill_Offset = new Vector2(1.5f, 0.0f);
    public Vector2 skill_BoxSize = new Vector2(1.0f, 2.0f);
    public LayerMask enemy_LayerMask;

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
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }    

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");

        player.StartCoroutine(Skill_Routine(player));
    }

    private IEnumerator Skill_Routine(PlayerCharacter_Controller player)
    {
        yield return new WaitForSeconds(0.4f);

        Vector2 origin = player.weapon_Anchor.position;

        Vector2 box_Center = origin + Vector2.right * (player.is_Facing_Right ? 1 : -1) * skill_Offset.x + Vector2.up * skill_Offset.y;

        Collider2D[] hits = Physics2D.OverlapBoxAll(box_Center, skill_BoxSize, 0.0f, enemy_LayerMask);
        //Debug.Log($"[Dokkabie Bat] hits count = {hits.Length}");
        //for (int i = 0; i < hits.Length; i++)
        //{
        //    Debug.Log($"[Dokkabie Bat] hit[{i}] ¡æ {hits[i].gameObject.name} (Layer: {LayerMask.LayerToName(hits[i].gameObject.layer)})");
        //}

        foreach (var hit in hits)
        {
            var enemy = hit.GetComponent<Enemy_Basic>()
                ?? hit.GetComponentInParent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Skill_Damage());
                Debug.Log($"[Dokkabie Bat] Applied damage to {enemy.name}");
            }
        }
    }
}
