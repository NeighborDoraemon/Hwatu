using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_Attack", menuName = "Weapon/Attack Strategy/Card")]
public class Card_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Projectile")]
    public GameObject projectile_Prefab;

    [Header("Effect Table")]
    public List<Card_Effect> effects;
    [Range(0f, 1f)] public List<float> probabilities;

    private float projectile_Speed = 10.0f;
    
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
        player.isAttacking = true;
        player.animator.SetTrigger("Attack");
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        if (projectile_Prefab == null) return;

        Card_Effect selected = Select_Effect();

        GameObject go = Object.Instantiate(projectile_Prefab, fire_Point.position, Quaternion.identity);
        var proj = go.GetComponent<Card_Projectile>();

        Vector2 dir = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        proj.Initialized(player, selected, dir, projectile_Speed);
    }
    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        return false;
    }

    private Card_Effect Select_Effect()
    {
        if (effects == null || effects.Count == 0) return null;
        if (probabilities == null || probabilities.Count != effects.Count)
            return effects[0];

        float r = Random.value;
        float acc = 0.0f;
        for (int i = 0; i < effects.Count; i++)
        {
            acc += probabilities[i];
            if (r <= acc) return effects[i];
        }
        return effects[effects.Count - 1];
    }
}
