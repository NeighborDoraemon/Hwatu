using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crow_Card_Attack", menuName = "Weapon/Attack Strategy/Crow_Card")]
public class Crow_Card_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;
    private GameObject crow;
    private Crow_Controller crow_Controller;

    public GameObject crow_Prefab;
    public float protect_Duration = 3.0f;

    public GameObject projectile_Prefab;
    private float projectile_Speed = 10.0f;

    [Header("Effect Table")]
    public List<Card_Effect> effects;
    [Range(0f, 1f)] public List<float> probabilities;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        if (crow_Prefab != null)
        {
            crow = Instantiate(crow_Prefab, player.transform.position, Quaternion.identity);
            crow_Controller = crow.GetComponent<Crow_Controller>();

            if (crow_Controller != null)
            {
                crow_Controller.Initialize(player, weapon_Data.attack_Damage, weapon_Data.attack_Cooldown);
            }
        }

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
        if (crow != null)
        {
            Destroy(crow);
            crow = null;
            crow_Controller = null;
        }
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Attack");
        player.isAttacking = true;
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

    private Card_Effect Select_Effect()
    {
        if (effects == null || effects.Count == 0) return null;
        if (probabilities == null || probabilities.Count != effects.Count)
            return effects[0];

        float total = 0.0f;
        for (int i = 0; i < probabilities.Count; i++)
            total += Mathf.Max(0.0f, probabilities[i]);

        if (total <= 0.0f)
            return effects[0];

        float pick = Random.value * total;

        float acc = 0.0f;
        for (int i = 0; i < effects.Count; i++)
        {
            acc += Mathf.Max(0.0f, probabilities[i]);
            if (pick <= acc) return effects[i];
        }
        return effects[effects.Count - 1];
    }

    public bool Crow_Is_Protecting()
    {
        return crow_Controller.isProtecting;
    }

    public void Start_Protection()
    {
        if (crow_Controller != null && !crow_Controller.isProtecting)
        {
            Debug.Log("Crow protection start.");
            crow_Controller.Protect_Player(protect_Duration);
        }
    }

    public void End_Protection()
    {
        if (crow_Controller != null && crow_Controller.isProtecting)
        {
            Debug.Log("Crow protection ended.");
            crow_Controller.Set_Protecting(false);
        }
    }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Debug.Log("Crow_Card_Attack_Strategy.Skill called");

        if (crow_Controller != null)
        {
            Debug.Log($"Crow_Controller found: {crow_Controller.name}");
            Start_Protection();

            return true;
        }
        else
        {
            Debug.LogWarning("Crow_Controller is null.");

            return false;
        }
    }
}
