using MBT;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FB_Castle_Wall : MonoBehaviour, Npc_Interface
{
    [SerializeField] private PlayerCharacter_Controller player;

    [Header("Damage_Objects")]
    [SerializeField] private GameObject Obj_LandMine;
    [SerializeField] private GameObject Obj_ArrowRain_01;
    [SerializeField] private GameObject Obj_ArrowRain_02;
    [SerializeField] private GameObject Obj_Rock_Position;
    [SerializeField] private GameObject Obj_Sniper_Location;
    [SerializeField] private GameObject Obj_FB_Peasent;

    [Header("Sprite Renders")]
    [SerializeField] private SpriteRenderer SR_LandMine;
    [SerializeField] private SpriteRenderer SR_Arrow_01;
    [SerializeField] private SpriteRenderer SR_Arrow_02;
    [SerializeField] private Pause_Manager pause_Manager;

    [Header("Others")]
    [SerializeField] private GameObject Prfb_Rock;
    [SerializeField] private GameObject Prfb_FB_Peasent;
    [SerializeField] private bool is_Started = false;
    [SerializeField] private bool is_Second_Phase = false;
    [SerializeField] private GameObject Boss_Canvas;
    [SerializeField] private Map_Manager map_manager;

    [Header("Value")]
    [SerializeField] private IntReference IR_Health;
    [SerializeField] private float f_Pattern_Delay = 0.0f;
    private float f_Pattern_Time = 0.0f;

    [Header("Boss_Boundary")]
    [SerializeField] private PolygonCollider2D boundary_01;
    [SerializeField] private PolygonCollider2D boundary_02;

    [Header("Boss Canvas")]
    [SerializeField] private Image Health_Image;

    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_Start;
    [SerializeField] private int Second_Phase_Start;
    [SerializeField] private int After_Boss;
    [SerializeField] private int Forgiving_Yes;
    [SerializeField] private int Forgiving_No;


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

    private bool is_Boss_Dead = false; // Boss is Dead
    private bool is_Forgiving = false; // Forgiving Yes or No


    // Boolean for Attack Once
    private bool is_Once_Attacked = false;

    //====CoolDown Boolean
    private bool LandMine_CoolDown = false;
    private bool ArrowRain_CoolDown = false;
    private bool Rock_CoolDown = false;
    private bool Sniper_CoolDown = false;

    private GameObject Rock_Prefab = null;

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
        Health_Image.fillAmount = (float)IR_Health.Value / 500;
        if (is_Started)
        {
            f_Pattern_Time += Time.deltaTime;

            if (IR_Health.Value <= 0 && !is_Second_Phase)
            {
                Fade_Stop();
                Do_Second_Phase();
            }
            else if (IR_Health.Value <= 0 && is_Second_Phase && !is_Boss_Dead)
            {
                Fade_Stop();
                is_Boss_Dead = true;
                is_Started = false;
                Obj_FB_Peasent.GetComponent<FB_Peasent>().Stop_Pattern();
                Npc_Interaction_Start();
            }

            Health_Calculate();
            State_Setter();
        }
    }

    private void Health_Calculate()
    {
        if (IR_Health.Value <= Health_Quater_01 && IR_Health.Value > Health_Quater_02 && !is_Quater_01) // 51% - 75%
        {
            Debug.Log("LandMine 01 Called");

            is_Quater_01 = true;
            is_Can_Use_LandMine = true;
        }
        else if (IR_Health.Value <= Health_Quater_02 && IR_Health.Value > Health_Quater_03 && !is_Quater_02) // 26% - 50%
        {
            Debug.Log("LandMine 02 Called");

            is_Quater_02 = true;
            is_Can_Use_LandMine = true;
        }
        else if (IR_Health.Value <= Health_Quater_03 && !is_Quater_03) // 1% - 25%
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
        if (Now_State == Attack_State.Nothing && f_Pattern_Time >= f_Pattern_Delay)
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
                        Obj_LandMine.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();
                        StartCoroutine(Fade_Sprite(SR_LandMine, 1.0f, 1.0f, 0.0f));

                        is_Once_Attacked = true;
                        LandMine_CoolDown = true;

                        is_Can_Use_LandMine = false;

                        break;
                    }
                case Attack_State.ArrowRain:
                    {
                        Obj_ArrowRain_01.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();
                        StartCoroutine(Fade_Sprite(SR_Arrow_01, 1.0f, 1.0f, 0.0f));
                        Obj_ArrowRain_02.gameObject.GetComponent<FB_DamageBox>().Call_Invoke();
                        StartCoroutine(Fade_Sprite(SR_Arrow_02, 1.0f, 1.0f, 3.0f));

                        is_Once_Attacked = true;
                        ArrowRain_CoolDown = true;

                        break;
                    }
                case Attack_State.Rock:
                    {
                        Rock_Prefab = Instantiate(Prfb_Rock, Obj_Rock_Position.gameObject.transform.position, Obj_Rock_Position.gameObject.transform.rotation);
                        Rock_Prefab.GetComponent<Rigidbody2D>().velocity = new Vector2(-3.0f, 0.0f);

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

        f_Pattern_Time = 0.0f;

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

    private void Do_Second_Phase()
    {
        IR_Health.Value = 500; // Second Phase Health
        is_Second_Phase = true;

        Obj_FB_Peasent.SetActive(true);
        is_Started = false;

        Npc_Interaction_Start();
        //Obj_FB_Peasent.GetComponent<FB_Peasent>().Start_Pattern();
    }

    public void Call_Start()
    {
        Dialogue_Manager.instance.Get_Npc_Data(gameObject);
        Npc_Interaction_Start();
    }

    private void Start_Pattern()
    {
        is_Started = true;
        Boss_Canvas.SetActive(true);
    }

    private void Fade_Stop()
    {
        StopAllCoroutines();

        Color alpha = SR_LandMine.color;
        alpha.a = 0.0f;
        SR_LandMine.color = alpha;
        SR_Arrow_01.color = alpha;
        SR_Arrow_02.color = alpha;

        Obj_LandMine.gameObject.GetComponent<FB_DamageBox>().Stop_All_Coroutine();
        Obj_ArrowRain_01.gameObject.GetComponent<FB_DamageBox>().Stop_All_Coroutine();
        Obj_ArrowRain_02.gameObject.GetComponent<FB_DamageBox>().Stop_All_Coroutine();
        Obj_Sniper_Location.gameObject.GetComponent<FB_Sniping>().Stop_Attack();

        if(Rock_Prefab != null)
        {
            Destroy(Rock_Prefab);
        }
    }
    // for Warning Fade
    private IEnumerator Fade_Sprite(SpriteRenderer sprite, float targetAlpha, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Color color = sprite.color;
        //float startAlpha = color.a;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0.0f, targetAlpha, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(targetAlpha, 0.0f, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = 0.0f;
        sprite.color = color;
    }

    public void Npc_Interaction_Start()
    {
        player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        player.Player_Vector_Stop();

        if (is_Boss_Dead)
        {
            Dialogue_Manager.instance.Start_Dialogue(After_Boss);
            return;
        }
        else
        {
            if (!is_Second_Phase)
            {
                Dialogue_Manager.instance.Start_Dialogue(Interaction_Start);
            }
            else
            {
                Dialogue_Manager.instance.Start_Dialogue(Second_Phase_Start);
            }
        }
    }
    public void Event_Start()   //방생을 위한 메서드
    {
        Debug.Log("Forgiving Start");
        is_Forgiving = true;
        player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        StartCoroutine(Forgiving_With_Delay());
    }
    public void Npc_Interaction_End()
    {
        if(!is_Boss_Dead)
        {
            Start_Pattern();
            if (is_Second_Phase)
            {
                Obj_FB_Peasent.GetComponent<FB_Peasent>().Start_Pattern();
            }
            is_Started = true;
            player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
        }
        else
        {
            if(!is_Forgiving)
            {
                is_Forgiving = true;
                Debug.Log("End_01");
                StartCoroutine(Forgiving_No_With_Delay());
                return;
            }
            Debug.Log("End_02");
            player.State_Change(PlayerCharacter_Controller.Player_State.Normal);

            Obj_FB_Peasent.SetActive(false);
            //pause_Manager.Show_Result(false);

            boundary_01.gameObject.SetActive(false);
            boundary_02.gameObject.SetActive(true);

            map_manager.End_Stage(1);

            Boss_Canvas.SetActive(false);
            Destroy(gameObject);    //Change it to Connect with Second Stage


            //Debug.Log("Boss Dead");
        }
    }

    private IEnumerator Forgiving_With_Delay()
    {
        yield return new WaitForSeconds(0.1f);
        Dialogue_Manager.instance.Start_Dialogue(Forgiving_Yes);
    }
    private IEnumerator Forgiving_No_With_Delay()
    {
        yield return new WaitForSeconds(0.1f);
        Dialogue_Manager.instance.Start_Dialogue(Forgiving_No);
    }

    public void Event_Move(InputAction.CallbackContext ctx) //Not used
    {}
    public void Event_Attack(InputAction.CallbackContext ctx)   //Not used
    {}
}