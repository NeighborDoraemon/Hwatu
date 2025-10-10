using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Box : MonoBehaviour
{
    [SerializeField] private int Trap_Damage = 5;
    private bool is_Once_Act = false;

    [SerializeField] private float f_Attack_Delay;

    [SerializeField]private GameObject Obj_Player;
    [SerializeField] private List<Animator> Animators;

    private bool is_Called_Once = false;

    private void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Obj_Player = collision.gameObject;
        if (collision.gameObject.CompareTag("Player") && !is_Once_Act)
        {
            is_Once_Act = true;
            StartCoroutine(Delay(f_Attack_Delay));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && !is_Once_Act)
        {
            is_Once_Act = true;
            StartCoroutine(Delay(f_Attack_Delay));
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Obj_Player = null;
        }
    }

    IEnumerator Damage_Coroutine()
    {
        yield return new WaitForSeconds(f_Attack_Delay);

        if (Obj_Player != null)
        {
            Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Player_Take_Damage(Trap_Damage);

            if (this.transform.position.x < Obj_Player.transform.position.x) // 적이 플레이어의 좌측에 있음
            {
                Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(1, 0.2f, 3.0f);
            }
            else
            {
                Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, 0.2f, 3.0f);
            }
        }
        is_Once_Act = false;
    }

    IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);

        foreach (Animator anim in Animators)
        {
            anim.SetTrigger("Trigger_Spike");
        }
    }

    public void Once_End()
    {
        is_Once_Act = false;
        is_Called_Once = false;
    }

    public void On_Damage()
    {
        if (Obj_Player != null && !is_Called_Once)
        {
            Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Player_Take_Damage(Trap_Damage);

            if (this.transform.position.x < Obj_Player.transform.position.x) // 적이 플레이어의 좌측에 있음
            {
                Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(1, 0.2f, 3.0f);
            }
            else
            {
                Obj_Player.GetComponentInParent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, 0.2f, 3.0f);
            }
            is_Called_Once = true;
        }
    }
}
