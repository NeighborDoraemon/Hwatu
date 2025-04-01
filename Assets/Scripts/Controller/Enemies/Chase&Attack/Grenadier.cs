using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenadier : MonoBehaviour,Enemy_Interface
{
    [Header("Float Values")]
    

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 2.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Stunned;

    [Header("Others")]
    [SerializeField] private GameObject Target_Player;

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
        if (BR_Stunned.Value)
        {
            Stunned();
        }
        else
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

        if (!is_Attack_Turn) // 공격 시작 시, 플레이어 방향 보게하기
        {
            TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            if (BR_Facing_Left.Value) //Attack Left
            {
                is_Attack_Complete = true;
                Throw_Attack();
            }
            else //Attack Right
            {
                is_Attack_Complete = true;
                Throw_Attack();
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
        }
    }

    private void Throw_Attack()
    {
        Vector2 startPosition = transform.position;          // 척탄병 위치
        Vector2 targetPosition = player.position;            // 플레이어 위치
        Vector2 displacement = targetPosition - startPosition; // 거리 계산

        float t = flightTime;                                // 비행 시간
        float vx = displacement.x / t;                       // 수평 속도
        float vy = (displacement.y + 0.5f * gravity * t * t) / t; // 수직 속도

        Vector2 initialVelocity = new Vector2(vx, vy);       // 초기 속도
        Rigidbody2D grenadeInstance = Instantiate(grenade, startPosition, Quaternion.identity); // 포탄 생성
        grenadeInstance.velocity = initialVelocity;          // 속도 부여
    }

    private void TurnAround()
    {
        Quaternion quater = this.gameObject.transform.rotation;

        if (this.gameObject.transform.position.x <= Target_Player.transform.position.x && BR_Facing_Left.Value) // 좌측 보는중 & 플레이어가 우측
        {
            BR_Facing_Left.Value = false;
            quater.y = 180.0f;

            this.gameObject.transform.rotation = quater;
        }
        else if (this.gameObject.transform.position.x > Target_Player.transform.position.x && !BR_Facing_Left.Value)
        {
            BR_Facing_Left.Value = true;
            //Obj_Enemy.gameObject.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            quater.y = 0.0f;

            this.gameObject.transform.rotation = quater;
        }
    }

    private void Stunned()
    {

    }

    //private void OnDrawGizmos()
    //{
    //    if (player == null) return;

    //    Vector2 startPosition = transform.position;
    //    Vector2 targetPosition = player.position;
    //    Vector2 displacement = targetPosition - startPosition;

    //    float t = flightTime;
    //    float vx = displacement.x / t;
    //    float vy = (displacement.y + 0.5f * gravity * t * t) / t;

    //    Vector2 velocity = new Vector2(vx, vy);
    //    Vector2 currentPosition = startPosition;

    //    Gizmos.color = Color.red;
    //    for (float time = 0; time < t; time += 0.1f)
    //    {
    //        Vector2 nextPosition = startPosition + velocity * time + 0.5f * Physics2D.gravity * time * time;
    //        Gizmos.DrawLine(currentPosition, nextPosition);
    //        currentPosition = nextPosition;
    //    }
    //}
}
