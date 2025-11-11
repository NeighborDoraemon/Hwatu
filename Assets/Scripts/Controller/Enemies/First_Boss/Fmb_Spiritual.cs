using MBT;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Fmb_Spiritual : MonoBehaviour, Npc_Interface, Enemy_Second_Phase
{
    public void Event_Attack(InputAction.CallbackContext ctx){return;}
    public void Event_Move(InputAction.CallbackContext ctx){return;}
    public void Event_Move_Direction(Vector2 dir){return;}

    public void Event_Start()
    {
        player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        StartCoroutine(Purify_With_Delay());
    }

    public void Npc_Interaction_End()
    {
        if (is_Ending_Text_Shown)
        {
            player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
            StopAllCoroutines();

            int DropMoney = Random.Range(min_Money_Drop, Max_Money_Drop);
            player.GetComponent<PlayerCharacter_Controller>().Add_Player_Money(DropMoney);
            player.Add_Player_Token(30); // Token Reward

            foreach (GameObject jail in Remain_Jails)
            {
                Destroy(jail);
            }
            obj_e_Generator.Custom_Room_Cleard();

            if (groggy_Wall != null)
            {
                groggy_Wall.GetComponent<Fmb_Groggy_Wall>().Force_Destroy();
            }


            if (is_Purified)
            {
                obj_Manager.Spawn_Item_From_MiniBoss_Purification(this.transform.position, this.transform.position - Vector3.left, player);
            }
            else
            {
                obj_Manager.Spawn_Item_From_MiniBoss_Extinction(this.transform.position, this.transform.position - Vector3.left, player);
            }


            //Txt_Purify.GetComponent<TextMeshProUGUI>().text = "수락";
            //Txt_Kill.GetComponent<TextMeshProUGUI>().text = "거절";

            Txt_Purify.GetComponent<Text>().text = "수락";
            Txt_Kill.GetComponent<Text>().text = "거절";

            
            Boss_Name_Text.GetComponent<Text>().text = "문지기 & 완득이";
            Boss_Health_Bar.GetComponent<Image>().fillAmount = 1.0f;
            Boss_Canvas.SetActive(false);

            Destroy(Fmb_Parent);
        }
        else
        {
            if (!is_Boss_Dead)
            {
                is_Started = true;  // Start Boss Fight
                player.State_Change(PlayerCharacter_Controller.Player_State.Normal);
                
                Boss_Canvas.SetActive(true);
                Boss_Health_Bar.GetComponent<Image>().fillAmount = 1.0f;
                Boss_Name_Text.GetComponent<Text>().text = "오염된 산신령";
            }
            else
            {
                player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
                StartCoroutine(Kill_With_Delay());

                map_manager.Token_Call();
            }
        }
    }

    public void Npc_Interaction_Start()
    {
        player.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        player.Player_Vector_Stop();

        if (is_Boss_Dead)
        {
            //Txt_Purify.GetComponent<TextMeshProUGUI>().text = "정화";
            //Txt_Kill.GetComponent<TextMeshProUGUI>().text = "소멸";

            Txt_Purify.GetComponent<Text>().text = "정화";
            Txt_Kill.GetComponent<Text>().text = "소멸";

            //StopAllCoroutines();

            //foreach (GameObject jail in Remain_Jails)
            //{
            //    Destroy(jail);
            //}
            //obj_e_Generator.Custom_Room_Cleard();

            //if (groggy_Wall != null)
            //{
            //    groggy_Wall.GetComponent<Fmb_Groggy_Wall>().Force_Destroy();
            //}
            StopAllCoroutines();
            Now_State = Attack_State.Nothing;
            Dialogue_Manager.instance.Start_Dialogue(After_Boss);
            return;
        }
        else
        {
            Dialogue_Manager.instance.Start_Dialogue(Interaction_Start);
        }
        return;
    }

    private IEnumerator Purify_With_Delay()
    {
        yield return new WaitForSeconds(0.1f);
        Dialogue_Manager.instance.Start_Dialogue(Purify);

        is_Purified = true;

        is_Ending_Text_Shown = true;
    }
    private IEnumerator Kill_With_Delay()
    {
        yield return new WaitForSeconds(0.1f);
        Dialogue_Manager.instance.Start_Dialogue(Kill);

        is_Ending_Text_Shown = true;
    }

    void Enemy_Second_Phase.Call_Second_Phase()
    {
        is_Boss_Dead = true;
        Npc_Interaction_Start();
    }


    [SerializeField] private PlayerCharacter_Controller player;

    [Header("Objects")]
    [SerializeField] private GameObject Obj_Root_Jail;
    [SerializeField] private GameObject Obj_Rock;
    [SerializeField] private GameObject Obj_Root_Wield;
    [SerializeField] private SpriteRenderer SR_Root_Wield;
    [SerializeField] private GameObject Obj_Warning;
    [SerializeField] private GameObject Obj_Groggy_Damage_Box;
    [SerializeField] private GameObject Obj_Groggy_Wall;
    [SerializeField] private GameObject Obj_Groggy_Position;
    [SerializeField] private List<GameObject> Rock_Position = new List<GameObject>();
    [SerializeField] private GameObject Fmb_Parent;
    [SerializeField] private Enemy_Generator obj_e_Generator;
    [SerializeField] private Object_Manager obj_Manager;

    [Header("Value")]
    [SerializeField] private IntReference IR_Health;
    [SerializeField] private float f_Pattern_Delay = 0.0f;
    private float f_Pattern_Time = 0.0f;
    [SerializeField] private int min_Money_Drop = 0;
    [SerializeField] private int Max_Money_Drop = 0;

    [Header("Others")]
    [SerializeField] private bool is_Started = false;
    [SerializeField] private GameObject Boss_Canvas;
    [SerializeField] private Map_Manager map_manager;

    [SerializeField] private GameObject Txt_Purify;
    [SerializeField] private GameObject Txt_Kill;

    [SerializeField] private GameObject Boss_Health_Bar;
    [SerializeField] private GameObject Boss_Name_Text;

    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_Start;
    [SerializeField] private int After_Boss;
    [SerializeField] private int Purify;
    [SerializeField] private int Kill;


    // Values about Dialogue
    private bool is_Boss_Dead = false;

    private enum Attack_State
    {
        Root_Jail,
        Rock_Rain,
        Root_Wield,
        Groggy_Wall,
        Nothing
    }

    private Attack_State Now_State = Attack_State.Nothing;


    private int Max_Health = 0;
    private int Target_Health = 0;
    // Boolean for Attack Once
    private bool is_Once_Attacked = false;
    private bool is_Groggy_Can_Use = false;
    private bool is_Groggy_Once = false;

    //--Cool Down Boolean
    private bool is_Root_Jail_Cool_Down = false;
    private bool is_Rock_Rain_Cool_Down = false;
    private bool is_Root_Wield_Cool_Down = false;

    private bool is_Ending_Text_Shown = false;

    private List<GameObject> Remain_Jails = new List<GameObject>();
    private GameObject groggy_Wall;

    private bool is_Purified = false;

    private Image health_bar;
    // Start is called before the first frame update
    void Start()
    {
        Set_Health_Cal();

        health_bar = Boss_Health_Bar.GetComponent<Image>();
        //obj_Manager.Spawn_Item_From_MiniBoss_Purification(this.transform.position, player);
        //obj_Manager.Spawn_Item_From_MiniBoss_Purification(this.transform.position + new Vector3(1.0f, 0.0f, 0.0f), player);
    }

    private void Set_Health_Cal()
    {
        Max_Health = IR_Health.Value; // Health reset
        Target_Health = (int)(Max_Health * 0.4f); // 40% of Max Health

        Debug.Log("Max Health : " + Max_Health);
        Debug.Log("Target Health : " + Target_Health);
    }

    // Update is called once per frame
    void Update()
    {
        if (is_Started)
        {
            health_bar.fillAmount = (float)IR_Health.Value / Max_Health;
            f_Pattern_Time += Time.deltaTime;

            Health_Calculate();
            State_Setter();
        }
    }

    void FixedUpdate()
    {
        if (is_Started)
        {
            Attack_Call();
        }
    }

    private void Health_Calculate()
    {
        if (IR_Health.Value <= Target_Health) // 40% of Max Health
        {
            is_Groggy_Can_Use = true;
        }
    }

    private void State_Setter() // Dicide What to do
    {
        if (Now_State == Attack_State.Nothing && f_Pattern_Time >= f_Pattern_Delay)
        {
            if(is_Groggy_Can_Use && !is_Groggy_Once)
            {
                Now_State = Attack_State.Groggy_Wall;
                is_Groggy_Once = true;
            }
            else if(!is_Root_Jail_Cool_Down)
            {
                Now_State = Attack_State.Root_Jail;
            }
            else if(!is_Rock_Rain_Cool_Down)
            {
                Now_State = Attack_State.Rock_Rain;
            }
            else if(!is_Root_Wield_Cool_Down)
            {
                Now_State = Attack_State.Root_Wield;
            }

            Debug.Log("Now State : " + Now_State.ToString());
        }
    }

    private void Attack_Call()
    {
        if (!is_Once_Attacked)
        {
            switch (Now_State)
            {
                case Attack_State.Root_Jail:
                    {
                        GameObject Warning = Instantiate(Obj_Warning, new Vector3(player.transform.position.x, gameObject.transform.position.y, 0.0f), Quaternion.identity);
                        Warning.transform.localScale = new Vector3(2.0f, 1.5f, 1.0f);
                        Warning.GetComponent<Warning_Object>().Initialize(1.0f, 0.8f, 0.5f);

                        StartCoroutine(Jail_Delay(1.6f, player.transform.position));

                        is_Once_Attacked = true;
                        is_Root_Jail_Cool_Down = true;

                        break;
                    }
                case Attack_State.Rock_Rain:
                    {
                        //Instantiate(Obj_Rock, new Vector3(player.transform.position.x, player.transform.position.y + 10.0f, 0.0f), Quaternion.identity);
                        StartCoroutine(Rock_Rain_Coroutine());

                        is_Once_Attacked = true;
                        is_Rock_Rain_Cool_Down = true;

                        break;
                    }
                case Attack_State.Root_Wield:
                    {
                        GameObject Warning = Instantiate(Obj_Warning, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y + 0.5f, 0.0f), Quaternion.identity);
                        Warning.transform.localScale = new Vector3(7.0f, 2.0f, 1.0f);
                        Warning.GetComponent<Warning_Object>().Initialize(1.0f, 0.8f, 0.5f);

                        StartCoroutine(Fade_Sprite(SR_Root_Wield, 1.0f, 0.6f, 0.0f));
                        Obj_Root_Wield.GetComponent<FB_DamageBox>().Call_Invoke();
                        
                        is_Once_Attacked = true;
                        is_Root_Wield_Cool_Down = true;

                        break;
                    }
                case Attack_State.Groggy_Wall:
                    {
                        groggy_Wall = Instantiate(Obj_Groggy_Wall, Obj_Groggy_Position.transform.position, Quaternion.identity);
                        groggy_Wall.GetComponent<Fmb_Groggy_Wall>().Initialize(10.0f, 8.0f, this.gameObject);

                        is_Once_Attacked = true;
                        is_Groggy_Once = true;

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
            case Attack_State.Root_Jail:
                {
                    yield return new WaitForSeconds(7.0f);

                    is_Root_Jail_Cool_Down = false;
                    break;
                }
            case Attack_State.Rock_Rain:
                {
                    yield return new WaitForSeconds(5.0f);  //
                    is_Rock_Rain_Cool_Down = false;
                    break;
                }
            case Attack_State.Root_Wield:
                {
                    yield return new WaitForSeconds(3.0f);  //8sec
                    is_Root_Wield_Cool_Down = false;
                    break;
                }
            case Attack_State.Groggy_Wall:
                {
                    break;
                }
            default:
                {
                    Debug.Log("Now State Missing");
                    break;
                }
        }
    }

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

    private IEnumerator Jail_Delay(float delay, Vector3 Player_Position)
    {
        yield return new WaitForSeconds(delay);
        GameObject new_Jail = Instantiate(Obj_Root_Jail, new Vector3(Player_Position.x, gameObject.transform.position.y, 0.0f), Quaternion.identity/*,this.transform*/);
        Remain_Jails.Add(new_Jail);

        Call_Coroutine();
    }

    public void End_Groggy_Wall()
    {
        Obj_Groggy_Damage_Box.GetComponent<FB_DamageBox>().Call_Invoke();
        Call_Coroutine();
    }

    public void Start_Groggy()
    {
        StartCoroutine(Groggy_Coroutine());
    }

    private IEnumerator Groggy_Coroutine()
    {
        Debug.Log("Groggy Start");
        yield return new WaitForSeconds(5.0f);
        Debug.Log("Groggy End");
        Call_Coroutine();
    }

    private IEnumerator Rock_Rain_Coroutine()
    {
        List<int> random_index = new List<int>();
        while (random_index.Count < 3)
        {
            int index = Random.Range(0, Rock_Position.Count);
            if (!random_index.Contains(index))
            {
                random_index.Add(index);
            }
        }


        for (int i = 0; i < random_index.Count; i++)
        {
            Instantiate(Obj_Rock, new Vector3(Rock_Position[random_index[i]].transform.position.x, Rock_Position[random_index[i]].transform.position.y, 0.0f), Quaternion.identity);
            yield return new WaitForSeconds(0.2f);
        }
        Call_Coroutine();

        yield return null;
    }

    public void Call_Start()
    {
        Dialogue_Manager.instance.Get_Npc_Data(gameObject);
        Npc_Interaction_Start();
    }
}
