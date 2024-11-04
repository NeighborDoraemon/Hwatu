using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FB_DamageBox : MonoBehaviour
{
    [SerializeField] private int Trap_Damage = 5;
    [SerializeField] private float f_Attack_Delay = 2.0f;

    private GameObject Obj_Player;

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obj_Player = collision.gameObject;
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player") && f_Time_Count >= f_Start_Time + f_Attack_Delay && !is_Once_Act)
    //    {
    //        if (!is_Once_Act)
    //        {
    //            f_Start_Time = f_Time_Count;
    //            is_Once_Act = true;

    //        }
    //        Invoke("Give_Damage", f_Attack_Delay);
    //    }
    //}


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obj_Player = null;
        }
    }

    public void Call_Invoke()
    {
        Invoke("Gine_Damage", f_Attack_Delay);
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
    }
}
