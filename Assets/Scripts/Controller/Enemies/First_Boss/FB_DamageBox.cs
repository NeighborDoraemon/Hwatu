using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FB_DamageBox : MonoBehaviour
{
    [SerializeField] private int Trap_Damage = 5;
    [SerializeField] private float f_Attack_Delay = 2.0f;
    [SerializeField] private bool is_Call_CoolDown;

    [SerializeField] private Animator anim_Attack;

    [SerializeField] private List<Animator> animators = new List<Animator>();

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


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obj_Player = null;
        }
    }

    public void Call_Invoke()
    {
        StartCoroutine(Give_Damage());
    }

    public void Stop_All_Coroutine()
    {
        StopAllCoroutines();
    }

    IEnumerator Give_Damage()
    {
        yield return new WaitForSeconds(f_Attack_Delay);
        Call_Animation();
        if (Obj_Player != null)
        {
            //Call_Animation();
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

        if(is_Call_CoolDown)
        {
            transform.parent.SendMessage("Call_Coroutine");
        }
    }

    private void Call_Animation()
    {
        if(anim_Attack != null)
        {
            anim_Attack.SetTrigger("Attack");
        }
        else
        {
            if(animators.Count > 0)
            {
                foreach(Animator anim in animators)
                {
                    anim.SetTrigger("Attack");
                }
            }
        }
    }
}
