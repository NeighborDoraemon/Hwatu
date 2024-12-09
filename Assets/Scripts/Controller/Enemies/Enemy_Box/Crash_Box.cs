using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Crash_Box : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [Header("Knock_Back_Value")]
    [SerializeField] private float f_Knockback_Time;
    [SerializeField] private float f_Knockback_Power;

    [HideInInspector] public bool Damage_Once = false;

    private void Start()
    {
        Damage_Once = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log(Damage_Once);
            if(!BR_Not_Attacking.Value && Damage_Once)
            {
                GameObject Obj_Player = collision.gameObject;
                //플레이어에게 데미지&넉백 호출
                Debug.Log("Testing Called");

                Obj_Player.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(IR_Attack_Damage.Value);

                if (this.transform.position.x < Obj_Player.transform.position.x) // 적이 플레이어의 좌측에 있음
                {
                    Obj_Player.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(1, f_Knockback_Time, f_Knockback_Power);
                }
                else
                {
                    Obj_Player.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, f_Knockback_Time, f_Knockback_Power);
                }

                Damage_Once = false;
            }
        }
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        Damage_Once = true;
    //    }
    //}
}
