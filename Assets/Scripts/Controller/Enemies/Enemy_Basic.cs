using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Enemy_Basic : MonoBehaviour, Enemy_Interface
{
    [Header("BB Int")]
    [SerializeField] public IntReference IR_Health;

    [Header("Bool")]
    [SerializeField] private bool is_Boos_Object = false;

    [SerializeField] private Obj_ScareCrow scarecrow = null;

    [Header("Money")]
    [SerializeField] private int min_Money_Drop;
    [SerializeField] private int Max_Money_Drop;

    private PlayerCharacter_Controller player_Con;

    private GameObject Target_Player;
    private int i_Max_Health;


    [HideInInspector]
    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        player_Con = player;
    }

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
        Debug.Log($"Enmey took {damage} damage. Remaining health : {IR_Health.Value}");

        if(IR_Health.Value > i_Max_Health)
        {
            IR_Health.Value = i_Max_Health;
        }

        if (IR_Health.Value <= 0 && !is_Boos_Object)
        {
            Die();
        }

        if(scarecrow != null)
        {
            scarecrow.ShowDamage(damage);
        }
    }


    private void Die() 
    {
        Enemy_Generator.i_Enemy_Count--; 
        Debug.Log(Enemy_Generator.i_Enemy_Count);

        if(player_Con == null)
        {
            Debug.Log("Player null");
        }

        int DropMoney = Random.Range(min_Money_Drop, Max_Money_Drop);

        player_Con.Add_Player_Money(DropMoney);

        if (this.transform.parent != null)
        {
            Destroy(this.transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
