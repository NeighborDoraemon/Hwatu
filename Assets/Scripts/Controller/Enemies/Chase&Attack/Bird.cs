using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Bird : MonoBehaviour
{
    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 0.3f;
    //[SerializeField] private float f_After_Delay = 0.3f;
    //[SerializeField] private float f_Dash_Distance = 15.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private IntReference IR_Attack_Damage;

    [Header("Others")]
    [SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Obj_Attack_Box;


    private bool is_Attacking = false; // 공격 중 범위를 벗어났을 때, 다른 행동을 못하게 설정
    private bool is_Attack_Complete = false; // 연속공격의 방지

    private bool is_Down_Complete = false;


    [SerializeField] private float f_Attack_Move_Speed = 0.5f;
    [SerializeField] private float f_Attack_Down_Speed = 0.5f;

    private int i_For_UpDown = -1;

    // Start is called before the first frame update
    void Start()
    {
        //Target_Player = OR_Player.Value;
        Target_Player = FindObjectOfType<PlayerCharacter_Controller>().gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BR_Chasing.Value || is_Attacking)
        {
            Attack_Call();
        }
    }

    //private void Chasing()
    //{
    //    if (!is_Attacking)
    //    {
    //        TurnAround();

    //        if (BR_Facing_Left.Value)
    //        {
    //            this.transform.Translate(Vector3.left * f_Chasing_Speed * Time.deltaTime);
    //        }
    //        else
    //        {
    //            this.transform.Translate(Vector3.right * -f_Chasing_Speed * Time.deltaTime);
    //        }
    //    }
    //}

    private void Attack_Call()
    {
        Attack_Time += Time.deltaTime;
        
        if (BR_Not_Attacking.Value) // 공격 시작 시, 플레이어 방향 보게하기
        {
            Debug.Log("Attack_Called");
            BR_Not_Attacking.Value = false;
            //TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            //is_Attack_Turn = true;

            //f_Dash_StartPosition = this.transform.position.x;
        }

        if(Obj_Attack_Box.transform.localPosition.y <= 0.0f && i_For_UpDown == -1) //바닥을 찍었을 때, 다시 올라가게 설정
        {
            //Debug.Log("Down Complete");
            is_Down_Complete = true;
            i_For_UpDown = 1;
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            //BR_Not_Attacking.Value = false;

            if (BR_Facing_Left.Value) //Attack Left
            {
                //this.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f * f_Attack_Move_Speed, 0.0f);
                //Obj_Attack_Box.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f * f_Attack_Move_Speed, 0.0f);
                this.transform.Translate(new Vector3(-0.2f * f_Attack_Move_Speed, 0.0f));
                Obj_Attack_Box.transform.Translate(new Vector3(0.0f, 0.1f * f_Attack_Down_Speed * i_For_UpDown));
            }
            else //Attack Right
            {
                //this.transform.Translate(Vector3.right * -f_Down_Speed * Time.deltaTime);

                this.transform.Translate(new Vector3(-0.2f * f_Attack_Move_Speed, 0.0f));
                Obj_Attack_Box.transform.Translate(new Vector3(0.0f, 0.1f * f_Attack_Down_Speed * i_For_UpDown));

            }

            if (is_Down_Complete && Obj_Attack_Box.transform.localPosition.y >= 1.2f)
            {
                Debug.Log("First End");
                Attack_Time = 0.0f;

                is_Attack_Complete = true;
            }
        }

        //Call After Delay Method
        if (/*Attack_Time >= f_After_Delay && */is_Attack_Complete)
        {
            //is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;


            BR_Not_Attacking.Value = true;

            Attack_Time = 0.0f;

            i_For_UpDown = -1;
            is_Down_Complete = false;
            //Debug.Log("After Delay End");
        }
    }

    private void TurnAround()
    {
        Debug.Log("Turn Called");

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
}
