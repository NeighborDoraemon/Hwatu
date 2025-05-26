using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healer : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Delay")]
    [SerializeField] private float f_Delay = 3.0f;
    [SerializeField] private float f_After_Delay = 0.0f;


    [SerializeField] private GameObject Obj_HealBox;

    [Header("Animator")]
    [SerializeField] private Animator Healer_Animator;
    [SerializeField] private Animator Effect_Animator;
    [SerializeField] private Animator Heal_Front;
    [SerializeField] private Animator Heal_Back;

    private bool is_First_End = false;
    //[SerializeField] private BoolReference BR_Stunned;

    private float f_Attack_Time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!BR_Stunned.Value)
        {
            f_Attack_Time += Time.deltaTime;

            Call_Heal();
        }
    }

    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    private void Call_Heal()
    {
        if(f_Attack_Time >= f_Delay && !is_First_End)
        {
            Debug.Log("Heal Called");
            Healer_Animator.SetBool("is_Attacking",true);
            Obj_HealBox.GetComponent<Heal_Box>().Heal();
            //f_Attack_Time = 0.0f;
            is_First_End = true;
            Effect_Animator.SetTrigger("Trigger_Healer");
            Heal_Front.SetTrigger("Healer_Front");
            Heal_Back.SetTrigger("Healer_Back");
        }

        if (f_Attack_Time >= f_After_Delay + f_Delay && is_First_End)
        {
            Healer_Animator.SetBool("is_Attacking", false);
            f_Attack_Time = 0.0f;
            is_First_End = false;
        }
    }

    public void Enemy_Stun(float Duration)
    {
        Healer_Animator.SetBool("is_Attacking", false);
        f_Attack_Time = 0.0f;
        Take_Stun(Duration);
        is_First_End = false;
    }
}
