using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyAttack_Attack", menuName = "Weapon/Attack Strategy/EnergyAttack")]
public class EnergyAttack_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float skill_Duration = 3.0f;
    public float pushBack_Force = 2.0f;
    public float energy_Grow_Rate = 5.0f;    
    public float damage_Interval = 0.5f;    

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

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");
        player.StartCoroutine(EnergyWave(player));

        return true;
    }

    private IEnumerator EnergyWave(PlayerCharacter_Controller player)
    {
        Debug.Log("Energy Wave Start");

        GameObject energy_Wave = new GameObject("EnergyWave");
        energy_Wave.transform.position = player.transform.position + new Vector3(player.is_Facing_Right ? 1 : -1, 0, 0);
        energy_Wave.transform.localScale = new Vector3(3, 1, 1);
        
        BoxCollider2D collider = energy_Wave.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(energy_Wave.transform.position, energy_Wave.transform.lossyScale, 0);
        
        foreach (Collider2D hit in hitEnemies)
        {
            Enemy_Basic enemy = hit.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.cur_Weapon_Data.skill_Damage);
            }
        }

        float elapsed_Time = 0.0f;
        while (elapsed_Time < 0.2f)
        {
            float push_Back = pushBack_Force * Time.deltaTime * (player.is_Facing_Right ? -1 : 1);
            player.transform.position += new Vector3(push_Back, 0, 0);
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        Destroy(energy_Wave);
        Debug.Log("EnergyWave end");
    }
}