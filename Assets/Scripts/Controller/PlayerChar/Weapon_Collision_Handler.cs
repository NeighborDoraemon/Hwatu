using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Collision_Handler : MonoBehaviour
{
    [SerializeField] private int base_Weapon_Damage;
    [SerializeField] private float crit_Chance = 0.0f;
    [SerializeField] private float crit_Multiple = 2.0f;

    [SerializeField] private Collider2D weapon_Collider;

    private PlayerCharacter_Controller player;
    
    private void Awake()
    {
        if (weapon_Collider == null)
        {
            weapon_Collider = GetComponent<Collider2D>();            
        }
        weapon_Collider.enabled = false;

        player = FindObjectOfType<PlayerCharacter_Controller>();
        base_Weapon_Damage = player.cur_Weapon_Data.attack_Damage;
    }

    public int Calculate_Final_Damage()
    {
        int playerDamage = player != null ? player.attackDamage : 0;

        int totalDamage = base_Weapon_Damage + playerDamage;
        
        bool isCritical = Random.value <= crit_Chance;
        if (isCritical)
        {
            totalDamage = Mathf.RoundToInt(totalDamage * crit_Multiple);
            Debug.Log($"Critical Hit! Total Damage: {totalDamage}");
        }
        else
        {
            Debug.Log($"Normal Hit! Total Damage: {totalDamage}");
        }

        return totalDamage;
    }

    public void Enable_Collider(float duration)
    {
        if (weapon_Collider != null)
        {
            weapon_Collider.enabled = true;                        
            Invoke(nameof(Disable_Collider), duration);
        }                
    }

    private void Disable_Collider()
    {
        if (weapon_Collider != null)
        {
            weapon_Collider.enabled = false;                      
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            int finalDamage = Calculate_Final_Damage();
            other.GetComponent<Enemy_Basic>().TakeDamage(finalDamage);
        }
    }
}
