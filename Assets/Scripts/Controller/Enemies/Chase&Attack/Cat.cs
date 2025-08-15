using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Cat : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 0.3f;
    [SerializeField] private float f_After_Delay = 0.3f;

    [SerializeField] private float f_Attack_Delay = 2.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    //[SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private IntReference IR_Attack_Damage;
    //[SerializeField] private BoolReference BR_Stunned;

    [Header("Others")]
    //[SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Obj_Attack_Box;
    [SerializeField] private GameObject Feather_Projectile;
    [SerializeField] float f_Projectile_Speed;


    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지
    private bool is_Attack_Delaying = false;

    private float Attack_Time = 0.0f;

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
            if (!is_Attack_Delaying)
            {
                if (BR_Chasing.Value || is_Attacking)
                {
                    Attack_Call();
                }
            }
        }
    }

    private void Attack_Call()
    {
        Attack_Time += Time.deltaTime;

        if (!is_Attack_Complete)
        {
            TurnAround();

            if (BR_Not_Attacking.Value) // 공격 시작 시, 플레이어 방향 보게하기
            {
                Debug.Log("Not Attacking false");
                BR_Not_Attacking.Value = false;

                Attack_Time = 0.0f;

                is_Attacking = true;
            }
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            Feather_Attack();

            is_Attack_Complete = true;
        }

        //Call After Delay Method
        if (is_Attack_Complete)
        {
            //is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            Debug.Log("Not Attacking true");
            BR_Not_Attacking.Value = true;

            Attack_Time = 0.0f;

            is_Attack_Delaying = true;
            StartCoroutine(Attack_Delay());
        }
    }

    private void Feather_Attack()
    {
        Vector2 direction = Target_Player.transform.position - Obj_Attack_Box.transform.position;
        direction.y = direction.y - 0.65f;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject Projectile = Instantiate(Feather_Projectile, Obj_Attack_Box.transform.position, Quaternion.Euler(0.0f, 0.0f, angle - 90f));

        Projectile.GetComponent<Enemy_Energy_Ball>().Get_Data(Target_Player, f_Projectile_Speed);
        Debug.Log("Give Data Complete");
    }

    public void Enemy_Stun(float Duration)
    {
        is_Attacking = false;
        is_Attack_Complete = false;

        BR_Not_Attacking.Value = true;

        Attack_Time = 0.0f;

        Take_Stun(Duration);
    }

    IEnumerator Attack_Delay()
    {
        yield return new WaitForSeconds(f_Attack_Delay);
        is_Attack_Delaying = false;
    }
}
