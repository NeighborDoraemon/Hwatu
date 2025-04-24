using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Enemy_Parent, Enemy_Interface, Enemy_Stun_Interface
{
    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 1.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [Header("Others")]
    [SerializeField] private GameObject Bullet_Prefab;
    [SerializeField] private CapsuleCollider2D Lie_Down_Collider;
    [SerializeField] private CapsuleCollider2D Stand_Collider;
    [SerializeField] private Siege_Chase_Box chase_Box;

    private float arrowSpeed = 15.0f;

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지

    private bool is_Look_Once = false; // 플레이어 감지 여부
    
    public void Player_Initialize(PlayerCharacter_Controller player)
    {
        Target_Player = player.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (chase_Box != null)
        {
            chase_Box.Player_Detect += Player_Detect_Method;
        }
    }

    private void OnDestroy()
    {
        chase_Box.Player_Detect -= Player_Detect_Method;
    }

    // Update is called once per frame
    void Update()
    {
        if (!BR_Stunned.Value)
        {
            if (BR_Chasing.Value || is_Attacking)
            {
                Attack_Call();
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
            BR_Not_Attacking.Value = false;
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            if (BR_Facing_Left.Value) //Attack Left
            {
                Acher_Attack(-1);
                is_Attack_Complete = true;
            }
            else //Attack Right
            {
                Acher_Attack(1);
                is_Attack_Complete = true;
            }
        }

        //Call After Delay Method
        if (Attack_Time >= f_Before_Delay + f_After_Delay)
        {
            Debug.Log("Archer Turn");

            is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;
            BR_Not_Attacking.Value = true;

            if(is_Look_Once)
            {
                Change_Stand();
            }

            Attack_Time = 0.0f;
        }
    }

    private void Acher_Attack(int Alpha) //Left = -1, Right = 1;
    {
        GameObject projectile = MonoBehaviour.Instantiate(Bullet_Prefab, this.gameObject.transform.position, this.gameObject.transform.rotation);

        Rigidbody2D projectile_Rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = new Vector2(Alpha, 0.0f);
        projectile_Rb.velocity = shootDirection * arrowSpeed;
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

    private void Player_Detect_Method()
    {
        is_Look_Once = true;
    }

    private void Change_Stand()
    {
        Stand_Collider.enabled = true;
        Lie_Down_Collider.enabled = false;
    }
}