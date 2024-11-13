using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hog : MonoBehaviour
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

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // ���� �� ������ ����� ��, �ٸ� �ൿ�� ���ϰ� ����
    private bool is_Attack_Complete = false; // ���Ӱ����� ����
    private bool is_First_End = false;


    private float f_Dash_StartPosition = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //Target_Player = OR_Player.Value;
        Target_Player = FindObjectOfType<PlayerCharacter_Controller>().gameObject;
    }

    // Update is called once per frame
    void Update()
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

        if (!is_Attack_Turn && !is_Attack_Complete) // ���� ���� ��, �÷��̾� ���� �����ϱ�
        {
            TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;
            
            f_Dash_StartPosition = this.transform.position.x; 
        }

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            BR_Not_Attacking.Value = false;

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
            Debug.Log("After Delay End");
        }
    }

    private void TurnAround()
    {
        Debug.Log("Turn Called");

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
