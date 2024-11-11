using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Enemy_Basic : MonoBehaviour
{
    [Header("BB Int")]
    [SerializeField] public IntReference IR_Health;

    private GameObject Target_Player;
    private int i_Max_Health;

    public void Start()
    {
        i_Max_Health = IR_Health.Value;
    }

    public void TakeDamage(int damage)
    {
        if(damage < 0)
        {
            Debug.Log("Get Heal");
        }

        IR_Health.Value -= damage;

        if(IR_Health.Value > i_Max_Health)
        {
            IR_Health.Value = i_Max_Health;
        }        

        if (IR_Health.Value <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Enemy_Generator.i_Enemy_Count--;
        Debug.Log(Enemy_Generator.i_Enemy_Count);

        Destroy(gameObject);
    }
}
