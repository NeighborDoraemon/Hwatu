using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideSword : MonoBehaviour
{       // Chase & Attack
    [Header("Float Values")]
    [SerializeField] private float f_Chasing_Speed = 3.0f;

    [Header("Attack Delay")]
    [SerializeField] private float f_Before_Delay = 1.5f;
    [SerializeField] private float f_After_Delay = 0.5f;

    private float Attack_Time = 0.0f;

    [Header("BB_Value")]
    [SerializeField] private BoolReference BR_Chasing;
    //[SerializeField] private GameObjectReference OR_Player;
    [SerializeField] private BoolReference BR_Facing_Left;

    [SerializeField] private FloatReference FR_Attack_Range;
    [SerializeField] private IntReference IR_Attack_Damage;
    [SerializeField] private BoolReference BR_Not_Attacking;

    [Header("Others")]
    [SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject Enemy_Crash_Box;

    private float Distance = 0.0f;

    private bool is_Attack_Turn = false;
    private bool is_Attacking = false; // ���� �� ������ ����� ��, �ٸ� �ൿ�� ���ϰ� ����
    private bool is_Attack_Complete = false; // ���Ӱ����� ����

    // Start is called before the first frame update
    void Start()
    {
        //Target_Player = OR_Player.Value;
        Target_Player = FindObjectOfType<PlayerCharacter_Controller>().gameObject;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Distance = Mathf.Abs(this.gameObject.transform.position.x - Target_Player.transform.position.x);

        if (Distance <= FR_Attack_Range.Value || is_Attacking)
        {
            Attack_Call();
        }
        else if (BR_Chasing.Value && Distance > FR_Attack_Range.Value)
        {
            Chasing();
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
        if (!is_Attack_Turn) // ���� ���� ��, �÷��̾� ���� �����ϱ�
        {
            TurnAround();

            Attack_Time = 0.0f;

            is_Attacking = true;
            is_Attack_Turn = true;

            is_Attack_Complete = false;
        }

        Attack_Time += Time.deltaTime;

        if (Attack_Time >= f_Before_Delay && !is_Attack_Complete) // Attack
        {
            if (BR_Facing_Left.Value) //Attack Left
            {
                //Debug.Log(Attack_Time);
                WideSword_Attack(-1);
                is_Attack_Complete = true;
            }
            else //Attack Right
            {
                WideSword_Attack(1);
                is_Attack_Complete = true;
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

    private void WideSword_Attack(int Alpha) //Left = -1, Right = 1;
    {
        //HashSet<PlayerCharacter_Controller> damaged_Plaer_Collider = new HashSet<PlayerCharacter_Controller>();

        //this.transform.Translate(Vector3.zero);

        //Vector2 boxSize = new Vector2(0.2f, 0.4f);
        //Vector2 boxCenter = (Vector2)transform.position + new Vector2(Alpha * FR_Attack_Range.Value, 0f);
        //LayerMask playerLayer = LayerMask.GetMask("Character");

        //Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, playerLayer);

        //foreach (Collider2D player in hitPlayer)
        //{
        //    PlayerCharacter_Controller P_Controller = player.GetComponent<PlayerCharacter_Controller>();

        //    if (player != null && !damaged_Plaer_Collider.Contains(P_Controller))
        //    {
        //        //Debug.Log("�÷��̾� ���� ����");
        //        player.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(IR_Attack_Damage.Value);

        //        damaged_Plaer_Collider.Add(P_Controller);
        //    }
        //    //enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        //}

        if (BR_Not_Attacking.Value)
        {
            Enemy_Crash_Box.GetComponent<Crash_Box>().Damage_Once = true;
            //Enemy_CB.Damage_Once = true;
            BR_Not_Attacking.Value = false;
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