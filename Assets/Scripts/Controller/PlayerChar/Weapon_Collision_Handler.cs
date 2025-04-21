using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Collision_Handler : MonoBehaviour
{
    [SerializeField] private int base_Weapon_Damage;
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
            int finalDamage = player.Calculate_Damage();
            other.GetComponent<Enemy_Basic>().TakeDamage(finalDamage);

            if (Random.value <= player.stun_Rate)
            {
                Enemy_Stun_Interface enemy = other.GetComponent<Enemy_Stun_Interface>()
                            ?? other.GetComponentInParent<Enemy_Stun_Interface>()
                            ?? other.GetComponentInChildren<Enemy_Stun_Interface>();

                enemy.Enemy_Stun(2.0f);
                //other.GetComponentInChildren<Enemy_Stun_Interface>().Enemy_Stun(2.0f);
            }

            if (Random.value <= player.bleeding_Rate)
            {
                other.GetComponent<Enemy_Basic>().Bleeding_Attack(finalDamage, 5, 1.1f);
            }

            player.Trigger_Enemy_Hit();
        }
    }
}
