using MBT;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zombie : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface, Enemy_Second_Phase
{       // Chase & Attack
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
    //[SerializeField] private BoolReference BR_Stunned;
    [SerializeField] private IntReference IR_Health;
    private int i_Max_Health;

    [Header("Others")]
    //[SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Enemy_Crash_Box;
    [SerializeField] private Animator Wide_Animator;
    [SerializeField] private Enemy_Basic e_Basic;
    [SerializeField] private float Revive_Time = 1.5f;

    private float Distance = 0.0f;

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지

    private bool is_First_Dead = false; // 첫번째 죽음 이후, 두번째 페이즈로 넘어가는지 확인

    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        i_Max_Health = IR_Health.Value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Distance = Mathf.Abs(this.gameObject.transform.position.x - Target_Player.transform.position.x);


        if (!BR_Stunned.Value)
        {
            if ((BR_Chasing.Value && Distance <= FR_Attack_Range.Value) || is_Attacking)
            {
                Wide_Animator.SetBool("is_Chasing", false);
                Attack_Call();
            }
            else if (BR_Chasing.Value && Distance > FR_Attack_Range.Value)
            {
                Wide_Animator.SetBool("is_Chasing", true);
                Chasing();
            }
        }
    }

    void Enemy_Second_Phase.Call_Second_Phase()
    {
        if(!is_First_Dead) // 두번째 페이즈로 넘어가는지 확인
        {
            Debug.Log("Do Second Phase");
            IR_Health.Value = i_Max_Health;
            is_First_Dead = true;

            e_Basic.Change_Immortal();

            Enemy_Stun(Revive_Time);
            StartCoroutine(Revive(Revive_Time));
        }
        else
        {
            e_Basic.Call_Custom_Die();
        }
    }

    private IEnumerator Revive(float time)
    {
        yield return new WaitForSeconds(time);
        e_Basic.Change_Immortal();
    }

    private void Chasing()
    {
        if (!is_Attacking)
        {
            TurnAround();

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
            Wide_Animator.SetBool("is_Delay_End", false);
        }

        Attack_Time += Time.deltaTime;

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            Wide_Animator.SetTrigger("is_Attacking");
            if (BR_Facing_Left.Value) //Attack Left
            {
                //Debug.Log(Attack_Time);
                WideSword_Attack(-1);
                is_Attack_Complete = true;
            }
            else //Attack Right
            {
                WideSword_Attack(1);
                is_Attack_Complete = true;
            }
        }

        //Call After Delay Method
        if (Attack_Time >= f_Before_Delay + f_After_Delay)
        {
            is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            Attack_Time = 0.0f;
            BR_Not_Attacking.Value = true;
            Wide_Animator.SetBool("is_Delay_End", true);
            TurnAround();
        }
    }

    private void WideSword_Attack(int Alpha) //Left = -1, Right = 1;
    {
        if (BR_Not_Attacking.Value)
        {
            Enemy_Crash_Box.GetComponent<Crash_Box>().Damage_Once = true;
            //Enemy_CB.Damage_Once = true;
            BR_Not_Attacking.Value = false;
        }
    }

    public void Enemy_Stun(float Duration)
    {
        Wide_Animator.SetBool("is_Chasing", false);
        Wide_Animator.SetBool("is_Delay_End", false);

        Wide_Animator.SetTrigger("Trigger_Stun");

        is_Attack_Turn = false;
        is_Attacking = false;
        is_Attack_Complete = false;

        //is_First_End = false;

        BR_Not_Attacking.Value = true;

        Attack_Time = 0.0f;

        //is_Attack_Once = false;

        Take_Stun(Duration);
    }
}