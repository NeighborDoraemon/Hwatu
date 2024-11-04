using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Collision_Handler : MonoBehaviour
{
    [SerializeField] private int attack_Damage;
    [SerializeField] private Collider2D weapon_Collider;   

    private void Awake()
    {        
        if (weapon_Collider == null)
        {
            weapon_Collider = GetComponent<Collider2D>();            
        }
        weapon_Collider.enabled = false;
    }

    public void Set_Damage(int damage)
    {
        attack_Damage = damage;
    }

    public void Enable_Collider(float duration)
    {
        if (weapon_Collider != null)
        {
            weapon_Collider.enabled = true;
            if (weapon_Collider.enabled)
            {
                Debug.Log("���� �ݶ��̴� Ȱ��ȭ");
            }
            
            Invoke(nameof(Disable_Collider), duration);
        }                
    }

    private void Disable_Collider()
    {
        if (weapon_Collider != null)
        {
            weapon_Collider.enabled = false;
            if (!weapon_Collider.enabled)
            {
                Debug.Log("���� �ݶ��̴� ��Ȱ��ȭ");
            }            
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy_Basic>().TakeDamage(attack_Damage);
            Debug.Log("Enemy hit by weapon");
        }
    }
}
