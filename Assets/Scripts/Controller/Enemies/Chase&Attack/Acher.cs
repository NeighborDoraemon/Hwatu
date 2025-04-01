using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acher : MonoBehaviour, Enemy_Interface
{
    //[Header("Float Values")]
    //[SerializeField] private float f_Chasing_Speed = 4.0f;

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 1.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private BoolReference BR_Facing_Left;
    //[SerializeField] private GameObjectReference OR_Player;

    //[SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Stunned;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [Header("Others")]
    [SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Bullet_Prefab;

    private float arrowSpeed = 15.0f;

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
    void Update()
    {
        if (BR_Stunned.Value)
        {
            Stunned();
        }
        else
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

            Attack_Time = 0.0f;
        }
    }

    private void Acher_Attack(int Alpha) //Left = -1, Right = 1;
    {
        GameObject projectile = MonoBehaviour.Instantiate(Bullet_Prefab, this.gameObject.transform.position, this.gameObject.transform.rotation);

        projectile.GetComponent<Enemy_Porjectile>().i_Projectile_Damage = IR_Attack_Damage.Value;

        Rigidbody2D projectile_Rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = new Vector2 (Alpha, 0.0f);
        projectile_Rb.velocity = shootDirection * arrowSpeed;

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
}
