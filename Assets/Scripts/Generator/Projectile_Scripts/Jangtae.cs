using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangtae : MonoBehaviour
{
    [SerializeField] private int base_Weapon_Damage;

    private PlayerCharacter_Controller player;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
        base_Weapon_Damage = player.cur_Weapon_Data.attack_Damage;
    }

    public int Calculate_Final_Damage()
    {
        int playerDamage = player != null ? player.attackDamage : 0;

        int totalDamage = base_Weapon_Damage + playerDamage;

        return totalDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            int finalDamage = Calculate_Final_Damage();
            other.GetComponent<Enemy_Basic>().TakeDamage(finalDamage);
        }
        else if (other.CompareTag("Walls"))
        {
            if (player.attack_Strategy is Jangtae_Attack_Startegy jt && jt.isRiding)
            {
                return;
            }
            Destroy(this.gameObject);
        }
    }
}
