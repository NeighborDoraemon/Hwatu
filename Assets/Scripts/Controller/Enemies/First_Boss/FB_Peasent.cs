using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class FB_Peasent : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject Mini_Homi;
    [SerializeField] private GameObject Big_Homi;
    [SerializeField] private GameObject Big_Homi_Reverse;
    [SerializeField] private GameObject Target_Player;
    [SerializeField] private GameObject FB_Heavy_01;
    [SerializeField] private GameObject FB_Heavy_02;


    [Header("Values")]
    //====Dash Value
    [SerializeField] private float f_Dash_Speed;
    [SerializeField] private float f_Dash_Distance;
    [SerializeField] private float f_Dash_Delay;

    [Header("Dash Value")]
    [SerializeField] private int i_Dash_Damage;
    [SerializeField] private float f_Knockback_Time;
    [SerializeField] private float f_Knockback_Power;
    //====Throw Value
    [SerializeField] private float f_Throw_Distance;
    [SerializeField] private float f_Throw_Speed;
    //====Start Value
    [SerializeField] private bool is_Started = false;
    //====Pattern Value
    [SerializeField] private float f_Pattern_Delay = 0.0f;
    

    private bool is_Facing_Left = true;
    private bool is_Once_Used = false;

    private bool is_Attack_Called = false;


    //=======CoolDown Boolean
    private bool Throw_Cooldown = false;
    private bool Heavy_Cooldown = false;
    private bool Dash_Cooldown = false;

    
    //private bool is_Acting = false;

    private Rigidbody2D enemy_Rigid;
    private GameObject attack_Point = null;

    //=======Big Homi Crash Boolean
    [HideInInspector] public bool is_Homi_Crashed = false;
    private bool is_Move_Complete = false;
    private bool is_Left_Point = false;
    //=======Mini Homi Crash Boolean
    [HideInInspector] public bool is_Homi_Back = false;
    //=======Dash Values
    private Vector3 V3_Start_Position;
    private float f_Dash_Time = 0.0f;
    

    private float Distance = 0.0f;

    private float f_Pattern_Time = 0.0f;

    private enum Attack_State
    {
        Throw,
        Heavy,
        Dash,
        Nothing
    }

    private Attack_State Now_State = Attack_State.Nothing;

    // Start is called before the first frame update
    void Start()
    {
        enemy_Rigid = this.gameObject.GetComponent<Rigidbody2D>();
        Now_State = Attack_State.Nothing;
    }

    private void Update()
    {
        if(is_Started)
        {
            Distance = Mathf.Abs(this.gameObject.transform.position.x - Target_Player.transform.position.x);

            State_Setter();

            if(Now_State == Attack_State.Dash)
            {
                f_Dash_Time += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        if (is_Started) // Start Condition
        {
            switch (Now_State)
            {
                case Attack_State.Throw:
                    {
                        Throw_Attack();
                        break;
                    }
                case Attack_State.Heavy:
                    {
                        Heavy_Attack();
                        break;
                    }
                case Attack_State.Dash:
                    {
                        Dash_Attack();
                        break;
                    }
                case Attack_State.Nothing:
                    {
                        break;
                    }
            }

        }
    }

    private void State_Setter() // Dicide What to do
    {
        if(Now_State == Attack_State.Nothing)
        {
            if (!Throw_Cooldown && Distance >= f_Throw_Distance)
            {
                Now_State = Attack_State.Throw;
            }
            else if (!Heavy_Cooldown)
            {
                Now_State = Attack_State.Heavy;
            }
            else if (!Dash_Cooldown)
            {
                Now_State = Attack_State.Dash;
            }
        }
    }

    private void Throw_Attack() // CoolDown - 5s / 
    {
        if(!is_Once_Used)
        {
            TurnAround();

            is_Once_Used = true;
        }

        if(!is_Attack_Called)
        {
            if(is_Facing_Left)
            {
                GameObject homi = Instantiate(Mini_Homi, this.gameObject.transform.position + new Vector3(-1.0f,0.0f,0.0f), Mini_Homi.gameObject.transform.rotation);
                //homi.GetComponent<Rigidbody2D>().velocity = new Vector2(-f_Throw_Speed, 0.0f);

                is_Attack_Called = true;
            }
            else
            {
                GameObject homi = Instantiate(Mini_Homi, this.gameObject.transform.position + new Vector3(1.0f, 0.0f, 0.0f), Quaternion.Euler(0.0f, 180.0f, -90.0f));
                //homi.GetComponent<Rigidbody2D>().velocity = new Vector2(f_Throw_Speed, 0.0f);

                is_Attack_Called = true;
            }
        }

        if(is_Homi_Back) // Mini Homi is Back & Attack Complete
        {
            Throw_Cooldown = true;

            is_Once_Used = false;
            is_Attack_Called = false;

            StartCoroutine(Attack_CoolDown(Now_State));

            Now_State = Attack_State.Nothing;
        }
    }

    private void Heavy_Attack() // CoolDown - 15s /
    {
        Quaternion quater = this.gameObject.transform.rotation;
        
        if (!is_Once_Used)
        {
            if(Target_Player.transform.position.x >= this.gameObject.transform.position.x) // Go Right
            {
                is_Left_Point = false;
            }
            else if (Target_Player.transform.position.x < this.gameObject.transform.position.x) // Go Left
            {
                is_Left_Point = true;
            }

            //Set_Attack_Point();
            TurnAround();
            //is_Homi_Crashed = false;
            is_Attack_Called = false;

            is_Move_Complete = false;

            is_Once_Used = true;
        }

        if (!is_Move_Complete)
        {
            if (!is_Left_Point) // Go Right
            {
                if (this.gameObject.transform.position.x < FB_Heavy_01.transform.position.x)
                {
                    enemy_Rigid.velocity = new Vector2(f_Dash_Speed, 0.0f);
                }
                else
                {
                    enemy_Rigid.velocity = Vector2.zero;

                    { // Turn Around
                        quater.y = 0.0f;

                        this.gameObject.transform.rotation = quater;
                        is_Facing_Left = true;
                    }
                    is_Move_Complete = true;
                    Debug.Log("Move _Complete");
                }
            }
            else if (is_Left_Point) // Go Left
            {
                if (this.gameObject.transform.position.x > FB_Heavy_02.transform.position.x)
                {
                    enemy_Rigid.velocity = new Vector2(-f_Dash_Speed, 0.0f);
                }
                else
                {
                    enemy_Rigid.velocity = Vector2.zero;

                    { // Turn Around
                        quater.y = 180.0f;

                        this.gameObject.transform.rotation = quater;
                        is_Facing_Left = false;
                    }
                    is_Move_Complete = true;
                    Debug.Log("Move _Complete");
                }
            }
        }


        if(!is_Left_Point && !is_Attack_Called && is_Move_Complete) //수정
        {
            GameObject homi = Instantiate(Big_Homi, FB_Heavy_01.transform.position, Quaternion.Euler(0.0f, 0.0f, -90.0f));
            is_Attack_Called = true;
        }
        else if(is_Left_Point && !is_Attack_Called && is_Move_Complete)
        {
            GameObject homi = Instantiate(Big_Homi_Reverse, FB_Heavy_02.transform.position, Quaternion.Euler(0.0f, 180.0f, -90.0f));
            is_Attack_Called = true;
        }

        if(is_Homi_Crashed) // Homi Attack Complete
        {
            Heavy_Cooldown = true;

            is_Once_Used = false;
            is_Attack_Called = false;
            is_Homi_Crashed = false;

            is_Move_Complete = false;

            StartCoroutine(Attack_CoolDown(Now_State));

            Now_State = Attack_State.Nothing;
        }
    }

    private void Dash_Attack(/*Vector3 Target_Position*/) // CoolDown - 7s /
    {
        if (!is_Once_Used)
        {
            TurnAround();

            V3_Start_Position = this.transform.position;
            is_Once_Used = true;
        }

        if (!is_Attack_Called && f_Dash_Time >= f_Dash_Delay)
        {
            if (is_Facing_Left)
            {
                enemy_Rigid.velocity = new Vector2(-f_Dash_Speed, 0.0f);
            }
            else
            {
                enemy_Rigid.velocity = new Vector2(f_Dash_Speed, 0.0f);
            }
        }


        if (Mathf.Abs(this.transform.position.x - V3_Start_Position.x) >= f_Dash_Distance || f_Dash_Time >= 4.0f) //Distance or Time Condition
        {
            enemy_Rigid.velocity = Vector2.zero;
            Dash_Cooldown = true;

            is_Attack_Called = true;
            is_Once_Used = false;

            f_Dash_Time = 0.0f;

            StartCoroutine(Attack_CoolDown(Now_State));

            Now_State = Attack_State.Nothing;
        }
    }
    


    IEnumerator Attack_CoolDown(Attack_State attacks)
    {
        switch(attacks)
        {
            case Attack_State.Throw:
                {
                    Debug.Log("Throw_Cool_Called");
                    yield return new WaitForSeconds(5.0f);  //5sec
                    Throw_Cooldown = false;
                    break;
                }
            case Attack_State.Heavy:
                {
                    Debug.Log("Heavy_Cool_Called");
                    yield return new WaitForSeconds(15.0f);  //15sec
                    Heavy_Cooldown = false;
                    break;
                }
            case Attack_State.Dash:
                {
                    Debug.Log("Dash_Cool_Called");
                    yield return new WaitForSeconds(7.0f);  //7sec
                    Dash_Cooldown = false;
                    break;
                }
            default:
                {
                    Debug.Log("Now State Missing");
                    break;
                }
        }
    }

    //private void Set_Attack_Point()
    //{
    //    if(this.gameObject.transform.position.x <= Target_Player.transform.position.x) // Player is On Right
    //    {
    //        attack_Point = FB_Heavy_01;
    //    }
    //    else
    //    {
    //        attack_Point = FB_Heavy_02;
    //    }
    //    attack_Point = null;
    //}

    private void TurnAround()
    {
        Quaternion quater = this.gameObject.transform.rotation;

        if (this.gameObject.transform.position.x <= Target_Player.transform.position.x && quater.y < 1.0f) // Player is On Right && Facing Left
        {
            quater.y = 180.0f;

            this.gameObject.transform.rotation = quater;
            is_Facing_Left = false;

        }
        else if (this.gameObject.transform.position.x > Target_Player.transform.position.x && quater.y >= 160.0f)
        {
            quater.y = 0.0f;

            this.gameObject.transform.rotation = quater;
            is_Facing_Left = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && Now_State == Attack_State.Dash)
        {
            collision.gameObject.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Dash_Damage);

            if (this.transform.position.x < collision.transform.position.x) // 적이 플레이어의 좌측에 있음
            {
                collision.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(1, f_Knockback_Time, f_Knockback_Power);
            }
            else
            {
                collision.GetComponent<PlayerCharacter_Controller>().Weak_Knock_Back(-1, f_Knockback_Time, f_Knockback_Power);
            }
        }
    }
}
