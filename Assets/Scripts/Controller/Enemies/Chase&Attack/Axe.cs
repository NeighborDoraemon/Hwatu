using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Float Values")]
    [SerializeField] private float f_Chasing_Speed = 3.0f;

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 0.5f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;

    [SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [Header("Others")]
    [SerializeField] private GameObject Enemy_Crash_Box;
    [SerializeField] private Animator Axe_Animator;
    [SerializeField] private Animator Axe_Effect_Animator;
    [SerializeField] private GameObject Obj_Effect;

    private float Distance = 0.0f;

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지

    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Distance = Mathf.Abs(this.gameObject.transform.position.x - Target_Player.transform.position.x);


        if (!BR_Stunned.Value)
        {
            if ((BR_Chasing.Value && Distance <= FR_Attack_Range.Value) || is_Attacking)
            {
                Axe_Animator.SetBool("is_Chasing", false);
                Attack_Call();
            }
            else if ((BR_Chasing.Value && Distance > FR_Attack_Range.Value) && !is_Attacking)
            {
                Axe_Animator.SetBool("is_Chasing", true);
                Chasing();
            }
        }
    }

    private void Chasing()
    {
        if (!is_Attacking)
        {
            TurnAround();
            Debug.Log("Chasing");
            if (BR_Facing_Left.Value)
            {
                this.transform.Translate(Vector3.left * f_Chasing_Speed * Time.deltaTime);
            }
            else
            {
                this.transform.Translate(Vector3.right * -f_Chasing_Speed * Time.deltaTime);
            }
        }
    }

    private void Attack_Call()
    {
        if (!is_Attack_Turn) // 공격 시작 시, 플레이어 방향 보게하기
        {
            //TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;

            is_Attack_Complete = false;
            Axe_Animator.SetBool("is_Delay_End", false);
        }

        Attack_Time += Time.deltaTime;

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            Axe_Animator.SetTrigger("is_Attacking");

            is_Attack_Complete = true;
        }

        //Call After Delay Method
        if (Attack_Time >= f_Before_Delay + f_After_Delay)
        {
            is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            Attack_Time = 0.0f;
            BR_Not_Attacking.Value = true;
            Axe_Animator.SetBool("is_Delay_End", true);
        }
    }

    public void Axe_Attack() //Left = -1, Right = 1;
    {
        if (BR_Not_Attacking.Value)
        {
            Enemy_Crash_Box.GetComponent<Crash_Box>().Damage_Once = true;
            //Enemy_CB.Damage_Once = true;
            BR_Not_Attacking.Value = false;
            Attack_Time = 0.0f;
        }
    }

    public void Axe_Attack_End()
    {
        BR_Not_Attacking.Value = true;
    }

    public void Enemy_Stun(float Duration)
    {
        Axe_Animator.SetBool("is_Chasing", false);
        Axe_Animator.SetBool("is_Delay_End", false);

        is_Attack_Turn = false;
        is_Attacking = false;
        is_Attack_Complete = false;

        //is_First_End = false;

        BR_Not_Attacking.Value = true;

        Attack_Time = 0.0f;

        //is_Attack_Once = false;

        Take_Stun(Duration);
    }

    public void Effect_Start()
    {
        Obj_Effect.SetActive(true);
        Axe_Effect_Animator.SetTrigger("is_Attacking");
    }

    public void Effect_End()
    {
        Obj_Effect.SetActive(false);
    }
}
