using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;
using Unity.Burst.Intrinsics;
using UnityEngine.InputSystem.XR;

public class Summoner : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 0.3f;
    [SerializeField] private float f_After_Delay = 0.3f;

    [SerializeField] private float f_Attack_Delay = 4.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    //[SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private IntReference IR_Attack_Damage;
    //[SerializeField] private BoolReference BR_Stunned;

    [Header("Others")]
    //[SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Summon_Position;
    [SerializeField] private Animator Summon_Animator;

    [Header("Enemies")]
    [SerializeField] private GameObject enemy_Hog;
    [SerializeField] private GameObject enemy_Bird;
    [SerializeField] private GameObject enemy_Magpie;
    [SerializeField] private GameObject enemy_Wolf;


    private Enemy_Generator Ene_Generator;

    private enum Enemies
    {
        HOG,
        BIRD,
        MAGPIE,
        WOLF
    }
    private List<Enemies> remain_Enemies = new List<Enemies>();

    
    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지
    private bool is_Attack_Delaying = false;

    private float Attack_Time = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        remain_Enemies.Add(Enemies.HOG);
        remain_Enemies.Add(Enemies.BIRD);
        remain_Enemies.Add(Enemies.MAGPIE);
        remain_Enemies.Add(Enemies.WOLF);
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
            if (BR_Not_Attacking.Value) // 공격 시작 시, 플레이어 방향 보게하기
            {
                TurnAround();
                Debug.Log("Not Attacking false");
                BR_Not_Attacking.Value = false;

                Attack_Time = 0.0f;

                is_Attacking = true;
            }
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            Summon_Animator.SetTrigger("is_Attacking");
            Summon();
            is_Attack_Complete = true;
        }

        //Call After Delay Method
        if (is_Attack_Complete)
        {
            //is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            BR_Not_Attacking.Value = true;

            Attack_Time = 0.0f;

            is_Attack_Delaying = true;
            StartCoroutine(Attack_Delay());
        }
    }

    private void Summon()
    {
        if(remain_Enemies.Count == 0)
        {
            Debug.Log("Spawned All Enemies!");
            return;
        }

        int rand_Index = Random.Range(0, remain_Enemies.Count);
        Enemies Selected_Enemy = remain_Enemies[rand_Index];

        GameObject spawned_Enemy = null;

        switch(remain_Enemies[rand_Index])
        {
            case Enemies.HOG:
                {
                    spawned_Enemy = Instantiate(enemy_Hog, Summon_Position.transform.position, Summon_Position.transform.rotation);
                    break;
                }
            case Enemies.BIRD:
                {
                    spawned_Enemy = Instantiate(enemy_Bird, Summon_Position.transform.position, Summon_Position.transform.rotation);
                    break;
                }
            case Enemies.MAGPIE:
                {
                    spawned_Enemy = Instantiate(enemy_Magpie, Summon_Position.transform.position, Summon_Position.transform.rotation);
                    break;
                }
            case Enemies.WOLF:
                {
                    spawned_Enemy = Instantiate(enemy_Wolf, Summon_Position.transform.position, Summon_Position.transform.rotation);
                    break;
                }
            default:
                {
                    Debug.Log("Summon Case Error!");
                    return;
                }
        }

        remain_Enemies.RemoveAt(rand_Index);
        Enemy_Generator.Instance.From_Other_Add_Enemy();

        foreach (Enemy_Interface enemy_interface in spawned_Enemy.GetComponentsInChildren<Enemy_Interface>(true)) //적에게 플레이어 전달
        {
            enemy_interface.Player_Initialize(Target_Player.GetComponent<PlayerCharacter_Controller>());
        }

        Ene_Generator = this.gameObject.GetComponent<Enemy_Basic>().Give_Enemy_Generator();

        foreach (Enemy_Basic enemy in spawned_Enemy.GetComponentsInChildren<Enemy_Basic>(true))
        {
            enemy.Get_Enemy_Generator(Ene_Generator);
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
