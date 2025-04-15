using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenadier : Enemy_Parent, Enemy_Interface
{
    [Header("Float Values")]
    

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 2.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    //[SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;
    //[SerializeField] private BoolReference BR_Stunned;

    [Header("Others")]
    //[SerializeField] private GameObject Target_Player;
    [SerializeField] private Animator Grenadier_Animator;

    [Header("Grenader")]
    [SerializeField] private Transform player;       // 플레이어의 Transform
    [SerializeField] private Rigidbody2D grenade;   // 포탄의 Rigidbody2D
    [SerializeField] private float flightTime = 1f; // 포탄이 목표에 도달하는 시간
    [SerializeField] private float gravity = 9.8f;  // 중력 (Unity 기본값)

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
        if (!BR_Stunned.Value)
        {
            if (is_Attacking || BR_Chasing.Value)
            {
                Attack_Call();
            }
            else if (!BR_Chasing.Value)
            {
                Chasing();
            }
        }
    }

    private void Chasing()
    {
        //if (!is_Attacking)
        //{
        //    TurnAround();
        //}
    }

    private void Attack_Call()
    {
        Attack_Time += Time.deltaTime;
        TurnAround();
        if (!is_Attack_Turn) // 공격 시작 시, 플레이어 방향 보게하기
        {
            //TurnAround();
            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            //is_Attack_Turn = true;

            is_Attack_Complete = true;
            Grenadier_Animator.SetTrigger("is_Attacking");
            //Throw_Attack();
        }

        //Call After Delay Method
        if (Attack_Time >= f_Before_Delay + f_After_Delay)
        {
            is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            Attack_Time = 0.0f;
            BR_Not_Attacking.Value = true;
        }
    }

    public void Throw_Attack()
    {
        Vector2 startPosition = transform.position;          // 척탄병 위치
        Vector2 targetPosition = Target_Player.transform.position;            // 플레이어 위치
        Vector2 displacement = targetPosition - startPosition; // 거리 계산

        float t = flightTime;                                // 비행 시간
        float vx = displacement.x / t;                       // 수평 속도
        float vy = (displacement.y + 0.5f * gravity * t * t) / t; // 수직 속도

        Vector2 initialVelocity = new Vector2(vx, vy);       // 초기 속도
        Rigidbody2D grenadeInstance = Instantiate(grenade, startPosition, Quaternion.identity); // 포탄 생성
        grenadeInstance.velocity = initialVelocity;          // 속도 부여
    }

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
