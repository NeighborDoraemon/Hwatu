using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Box : MonoBehaviour
{
    [SerializeField] private int Trap_Damage = 5;
    private bool is_Once_Act = false;

    private float f_Time_Count = 0.0f;
    private float f_Start_Time = 0.0f;
    [SerializeField] private float f_Attack_Delay;

    private GameObject Obj_Player;

    private void Update()
    {
        f_Time_Count += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!is_Once_Act)
            {
                f_Start_Time = f_Time_Count;
                is_Once_Act = true;

                Obj_Player = collision.gameObject;


                Invoke("Give_Damage", f_Attack_Delay);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && f_Time_Count >= f_Start_Time + f_Attack_Delay && !is_Once_Act)
        {
            if (!is_Once_Act)
            {
                f_Start_Time = f_Time_Count;
                is_Once_Act = true;

            }
            Invoke("Give_Damage", f_Attack_Delay);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obj_Player = null;
        }
    }

    private void Give_Damage()
    {
        if (Obj_Player != null)
        {
            Obj_Player.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(Trap_Damage);

            if (this.transform.position.x < Obj_Player.transform.position.x) // 적이 플레이어의 좌측에 있음
            {
                Obj_Player.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(1, 0.2f, 3.0f);
            }
            else
            {
                Obj_Player.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, 0.2f, 3.0f);
            }
        }
        is_Once_Act = false;
    }
}
