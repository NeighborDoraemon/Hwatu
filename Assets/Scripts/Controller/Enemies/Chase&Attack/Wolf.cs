using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Float Values")]
    [SerializeField] private float f_Chasing_Speed = 4.0f;

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 2.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    //[SerializeField] private BoolReference BR_Facing_Left;
    //[SerializeField] private GameObjectReference OR_Player;
    [SerializeField] private BoolReference BR_Not_Attacking;
    //[SerializeField] private BoolReference BR_Stunned;

    [SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;

    [Header("Others")]
    //[SerializeField] private GameObject Target_Player;
    [SerializeField] private Crash_Box Enemy_CB;
    [SerializeField] private Animator ShortSword_Animator;
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
        //Target_Player = OR_Player.Value;
        //Target_Player = FindObjectOfType<PlayerCharacter_Controller>().gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Distance = Mathf.Abs(this.gameObject.transform.position.x - Target_Player.transform.position.x);

        if (!BR_Stunned.Value)
        {
            if ((BR_Chasing.Value && Distance <= FR_Attack_Range.Value) || is_Attacking)
            {
                //ShortSword_Animator.SetBool("is_Chasing", false);
                //ShortSword_Animator.SetBool("is_Attacking", true);
                Attack_Call();
            }
            else if (BR_Chasing.Value && Distance > FR_Attack_Range.Value)
            {
                //ShortSword_Animator.SetBool("is_Chasing", true);
                Chasing();
            }
        }
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
        Attack_Time += Time.deltaTime;

        if (!is_Attack_Turn) // 공격 시작 시, 플레이어 방향 보게하기
        {
            TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;

            //ShortSword_Animator.SetBool("is_Delay_End", true);
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            //ShortSword_Animator.SetTrigger("Trigger_Attack");
            if (BR_Facing_Left.Value) //Attack Left
            {
                ShortSword_Attack(-1);
                is_Attack_Complete = true;
                //ShortSword_Animator.SetBool("is_Delay_End", false);
            }
            else //Attack Right
            {
                ShortSword_Attack(1);
                is_Attack_Complete = true;
                //ShortSword_Animator.SetBool("is_Delay_End", false);
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

            //ShortSword_Animator.SetBool("is_Attacking", false);

        }
    }

    private void ShortSword_Attack(int Alpha) //Left = -1, Right = 1;
    {
        if (BR_Not_Attacking.Value)
        {
            Enemy_CB.Damage_Once = true;
            BR_Not_Attacking.Value = false;
        }
    }

    //private void TurnAround()
    //{
    //    Quaternion quater = this.gameObject.transform.rotation;

    //    if (this.gameObject.transform.position.x <= Target_Player.transform.position.x && BR_Facing_Left.Value) // 좌측 보는중 & 플레이어가 우측
    //    {
    //        BR_Facing_Left.Value = false;
    //        quater.y = 180.0f;

    //        this.gameObject.transform.rotation = quater;
    //    }
    //    else if (this.gameObject.transform.position.x > Target_Player.transform.position.x && !BR_Facing_Left.Value)
    //    {
    //        BR_Facing_Left.Value = true;
    //        //Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
    //        quater.y = 0.0f;

    //        this.gameObject.transform.rotation = quater;
    //    }
    //}

    public void Enemy_Stun(float Duration)
    {
        is_Attack_Turn = false;
        is_Attacking = false;
        is_Attack_Complete = false;

        BR_Not_Attacking.Value = true;

        Attack_Time = 0.0f;

        Take_Stun(Duration);
    }
}
