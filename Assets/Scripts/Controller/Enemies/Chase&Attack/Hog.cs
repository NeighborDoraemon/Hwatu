using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hog : MonoBehaviour,Enemy_Interface
{
    [Header("Float Values")]
    [SerializeField] private float f_Chasing_Speed = 15.0f;

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 3.0f;
    [SerializeField] private float f_Dash_Distance = 15.0f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    [SerializeField] private BoolReference BR_Facing_Left;
    [SerializeField] private BoolReference BR_At_End;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [SerializeField] private IntReference IR_Attack_Damage;

    [Header("Others")]
    [SerializeField] private GameObject Target_Player;
    [SerializeField] private Animator Hog_Animator;
    [SerializeField] private Animator Hog_Effect_Animator;

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // ���� �� ������ ����� ��, �ٸ� �ൿ�� ���ϰ� ����
    private bool is_Attack_Complete = false; // ���Ӱ����� ����
    private bool is_First_End = false;


    private float f_Dash_StartPosition = 0.0f;

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
        if (BR_Chasing.Value || is_Attacking)
        {
            Attack_Call();
        }
    }

    private void Animation_Controll()
    {
        //Hog_Animator.SetFloat("X_Velocity", this.gameObject.)
    }


    private void Attack_Call()
    {
        Attack_Time += Time.deltaTime;
        BR_Not_Attacking.Value = false;

        if (!is_Attack_Turn && !is_Attack_Complete) // ���� ���� ��, �÷��̾� ���� �����ϱ�
        {
            TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;
            
            f_Dash_StartPosition = this.transform.position.x;
            Hog_Animator.SetBool("is_Attacking", true);
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            //BR_Not_Attacking.Value = false;
            Hog_Animator.SetBool("is_Delay_End", true);
            Hog_Effect_Animator.SetBool("is_Attacking", true);

            if (BR_Facing_Left.Value) //Attack Left
            {
                this.transform.Translate(Vector3.left * f_Chasing_Speed * Time.deltaTime);
            }
            else //Attack Right
            {
                this.transform.Translate(Vector3.right * -f_Chasing_Speed * Time.deltaTime);
            }

            if(Mathf.Abs(this.transform.position.x - f_Dash_StartPosition) >= f_Dash_Distance || Attack_Time >= 3.0f)
            {
                Attack_Time = 0.0f;
                is_First_End = true;
                is_Attack_Complete = true;

                Hog_Animator.SetBool("is_Delay_End", false);
                Hog_Animator.SetBool("is_Attacking", false);
                Hog_Effect_Animator.SetBool("is_Attacking", false);
            }
        }

        //Call After Delay Method
        if (Attack_Time >= f_After_Delay && is_First_End)
        {
            is_Attack_Turn = false;
            is_Attacking = false;
            is_Attack_Complete = false;

            is_First_End = false;

            BR_Not_Attacking.Value = true;

            Attack_Time = 0.0f;
        }
    }

    private void TurnAround()
    {
        Quaternion quater = this.gameObject.transform.rotation;

        if (this.gameObject.transform.position.x <= Target_Player.transform.position.x && BR_Facing_Left.Value) // ���� ������ & �÷��̾ ����
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
