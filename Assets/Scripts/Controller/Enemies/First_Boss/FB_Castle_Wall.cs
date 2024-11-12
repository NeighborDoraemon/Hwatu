using MBT;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FB_Castle_Wall : MonoBehaviour
{
    ///*[HideInInspector] */public bool is_Attack_Start = false;

    [Header("Damage_Objects")]
    [SerializeField] private GameObject Obj_LandMine;
    [SerializeField] private GameObject Obj_ArrowRain_01;
    [SerializeField] private GameObject Obj_ArrowRain_02;
    [SerializeField] private GameObject Obj_Rock_Position;
    [SerializeField] private GameObject Obj_Sniper_Location;

    [Header("Others")]
    [SerializeField] private GameObject Prfb_Rock;
    [SerializeField] private GameObject Prfb_FB_Peasent;
    [SerializeField] private bool is_Started = false;
    [SerializeField] private bool is_Second_Phase = false;

    [Header("Value")]
    [SerializeField] private IntReference IR_Health;


    private float f_Repeat_Time = 0.0f;
    private float f_Do_Time = 3.0f;

    // LandMine Mechanism needs to be fixed
    //==== Value For LandMine
    private int Max_Health = 0;
    private int Quater_Health = 0;
    private bool is_Can_Use_LandMine = false;
    //==== Health section
    private int Health_Quater_01 = 0;
    private int Health_Quater_02 = 0;
    private int Health_Quater_03 = 0;
    //==== Bool value
    private bool is_Quater_01 = false;
    private bool is_Quater_02 = false;
    private bool is_Quater_03 = false;


    // Boolean for Attack Once
    private bool is_Once_Attacked = false;

    //====CoolDown Boolean
    private bool LandMine_CoolDown = false;
    private bool ArrowRain_CoolDown = false;
    private bool Rock_CoolDown = false;
    private bool Sniper_CoolDown = false;

    private enum Attack_State
    {
        LandMine,
        ArrowRain,
        Rock,
        Sniper,
        Nothing
    }

    private Attack_State Now_State = Attack_State.Nothing;

    // Start is called before the first frame update
    void Start()
    {
        Set_Health_Cal();
    }

    private void Set_Health_Cal()
    {
        Max_Health = IR_Health.Value; // Health reset
        Quater_Health = IR_Health.Value / 4;

        Health_Quater_01 = IR_Health.Value - Quater_Health;
        Health_Quater_02 = Health_Quater_01 - Quater_Health;
        Health_Quater_03 = Health_Quater_02 - Quater_Health;
    }

    private void Update()
    {
        if(is_Started)
        {
            f_Repeat_Time += Time.deltaTime;
            Health_Calculate();
            State_Setter();
        }
    }

    private void Health_Calculate()
    {
        if(IR_Health.Value <= Health_Quater_01 && IR_Health.Value > Health_Quater_02 && !is_Quater_01) // 51% - 75%
        {
            Debug.Log("LandMine 01 Called");

            is_Quater_01 = true;
            is_Can_Use_LandMine = true;
        }
        else if(IR_Health.Value <= Health_Quater_02 && IR_Health.Value > Health_Quater_03 && !is_Quater_02) // 26% - 50%
        {
            Debug.Log("LandMine 02 Called");

            is_Quater_02 = true;
            is_Can_Use_LandMine = true;
        }
        else if(IR_Health.Value <= Health_Quater_03 && !is_Quater_03) // 1% - 25%
        {
            Debug.Log("LandMine 03 Called");

            is_Quater_03 = true;
            is_Can_Use_LandMine = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (is_Started)
        {
            Attack_Call();
        }
    }

    private void State_Setter() // Dicide What to do
    {
        if (Now_State == Attack_State.Nothing)
        {
            if (!LandMine_CoolDown && is_Can_Use_LandMine)
            {
                Now_State = Attack_State.LandMine;
            }
            else if (!ArrowRain_CoolDown)
            {
                Now_State = Attack_State.ArrowRain;
            }
            else if (!Sniper_CoolDown)
            {
                Now_State = Attack_State.Sniper;
            }
            else if (!Rock_CoolDown)
            {
                Now_State = Attack_State.Rock;
            }
        }
    }

    private void Attack_Call()
    {
        if (!is_Once_Attacked)
        {
            switch (Now_State)
            {
                case Attack_State.LandMine:
                    {
                        Debug.Log("LandMine Called");
                        Obj_LandMine.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();

                        is_Once_Attacked = true;
                        LandMine_CoolDown = true;

                        is_Can_Use_LandMine = false;

                        break;
                    }
                case Attack_State.ArrowRain:
                    {
                        Debug.Log("ArrowRain Called");
                        Obj_ArrowRain_01.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();
                        Obj_ArrowRain_02.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();

                        is_Once_Attacked = true;
                        ArrowRain_CoolDown = true;

                        break;
                    }
                case Attack_State.Rock:
                    {
                        Instantiate(Prfb_Rock, Obj_Rock_Position.gameObject.transform.position, Obj_Rock_Position.gameObject.transform.rotation);

                        is_Once_Attacked = true;
                        Rock_CoolDown = true;

                        Invoke("Call_Coroutine", 2.0f);
                        break;
                    }
                case Attack_State.Sniper:
                    {
                        Obj_Sniper_Location.gameObject.GetComponent<FB_Sniping>().Call_Attack();

                        is_Once_Attacked = true;
                        Sniper_CoolDown = true;

                        Invoke("Call_Coroutine", 2.0f);
                        break;
                    }
                case Attack_State.Nothing:
                    {
                        break;
                    }
            }
        }
    }

    public void Call_Coroutine()
    {
        StartCoroutine(Attack_CoolDown(Now_State));

        Now_State = Attack_State.Nothing;
        is_Once_Attacked = false;
    }

    IEnumerator Attack_CoolDown(Attack_State attacks)
    {
        switch (attacks)
        {
            case Attack_State.LandMine:
                {
                    Debug.Log("LandMine_CoolDown");
                    yield return new WaitForSeconds(0.0f);

                    LandMine_CoolDown = false;
                    break;
                }
            case Attack_State.ArrowRain:
                {
                    Debug.Log("ArrowRain_CoolDown");
                    yield return new WaitForSeconds(15.0f);  //
                    ArrowRain_CoolDown = false;
                    break;
                }
            case Attack_State.Rock:
                {
                    Debug.Log("Rock_CoolDown");
                    yield return new WaitForSeconds(8.0f);  //8sec
                    Rock_CoolDown = false;
                    break;
                }
            case Attack_State.Sniper:
                {
                    Debug.Log("Sniper_CoolDown");
                    yield return new WaitForSeconds(12.0f);  //12sec
                    Sniper_CoolDown = false;
                    break;
                }
            default:
                {
                    Debug.Log("Now State Missing");
                    break;
                }
        }
    }
}
