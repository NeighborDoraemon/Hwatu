using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hunting_Npc : MonoBehaviour, Npc_Interface
{
    private bool is_Event_Acting = false;

    private PlayerCharacter_Controller p_control = null;
    private GameObject Player_Object = null;

    private Vector2 v_Horizontal = new Vector2(0.0f, 0.0f);
    private Vector2 v_Direction;


    [Header("Event Objects")]
    [SerializeField] private GameObject Bow_Point;
    [SerializeField] private GameObject Bow_Object;
    [SerializeField] private GameObject Event_Position;
    [SerializeField] private GameObject Event_Projectile;
    [SerializeField] private GameObject Result_Position;

    [Header("Event UI")]
    [SerializeField] private GameObject Event_Slider;
    [SerializeField] private SpriteRenderer Event_Time_Slider;
    [SerializeField] private TextMeshPro Event_Bullet_Text;
    

    [Header("Event Values")]
    [SerializeField] private float Max_Angle;
    [SerializeField] private float Rotation_Speed;
    [SerializeField] private float Event_Time;
    [SerializeField] private int Max_Bullet = 15;

    [Header("Dialogue Index")]
    [SerializeField] private int Interaction_start;
    [SerializeField] private int Choice_Yes;
    [SerializeField] private int Choice_No;
    [SerializeField] private int Event_Complete;
    [SerializeField] private int After_Event;

    [Header("Event Lists")]
    [SerializeField] private List<GameObject> Bird_Prefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> Spawn_Points = new List<GameObject>();
    [SerializeField] private List<GameObject> Box_Prefabs = new List<GameObject>();

    private float Input_Value = 0.0f;
    private float Current_Angle = 0.0f;

    private int Prf_Count = 0;  //index
    [HideInInspector] public int bird_Count = 0;

    private Coroutine coroutine;
    private bool is_Running = false;
    private bool is_Event_Once = false; //이벤트를 한번만 하기위한 Boolean
    private bool is_Result_Once = false;
    private int Used_bullet = 0;

    private List<GameObject> Birds_List = new List<GameObject>();

    private float currentTime;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float spriteHeight;
    private float totalTime = 60.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(is_Event_Acting)
        {
            Hunting_Move();
            UpdateTimeGauge();

            if ((bird_Count == 0 && is_Running) || (Used_bullet == Max_Bullet && is_Running))
            {
                StopCoroutine(coroutine);
                Event_Slider.SetActive(false);
                Event_Result();
            }
        }
    }

    public void Npc_Interaction_Start()
    {
        if (p_control != null && !is_Result_Once)
        {
            p_control.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
            Dialogue_Manager.instance.Start_Dialogue(Interaction_start);
            InitTimeGauge();
        }
        else if(p_control != null && is_Result_Once)
        {
            p_control.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
            Dialogue_Manager.instance.Start_Dialogue(After_Event);
        }
    }

    public void Event_Start()
    {
        if (p_control != null)
        {
            p_control.State_Change(PlayerCharacter_Controller.Player_State.Event_Doing);
            p_control.Event_State_Change(PlayerCharacter_Controller.Event_State.Bird_Hunting);
            is_Event_Acting = true;

            Bow_Object.SetActive(true);
            Bow_Point.SetActive(true);

            Player_Object.transform.position = Event_Position.transform.position;

            //Debug.Log("Event Start!");
            Hunting_Spawn();
            Event_Slider.SetActive(true);
            coroutine = StartCoroutine(Event_Coroutine());
        }
    }

    public void Npc_Interaction_End()
    {
        if (p_control != null)
        {
            p_control.State_Change(PlayerCharacter_Controller.Player_State.Normal);
            p_control.Event_State_Change(PlayerCharacter_Controller.Event_State.None);
            is_Event_Acting = false;

            Bow_Object.SetActive(false);
            Bow_Point.SetActive(false);

            Debug.Log("Event End!");

            if(is_Event_Once && !is_Result_Once)   //Spawn Result Box
            {
                if (bird_Count == 0)
                {
                    //Spawn Legendary
                    Instantiate(Box_Prefabs[2], Result_Position.transform.position, Quaternion.identity);
                }
                else if (bird_Count > 0 && bird_Count <= 3) //1~3
                {
                    //Spawn Epic
                    Instantiate(Box_Prefabs[1], Result_Position.transform.position, Quaternion.identity);
                }
                else if (bird_Count > 3 && bird_Count <= 9)    //4~9
                {
                    //Spawn Common
                    Instantiate(Box_Prefabs[0], Result_Position.transform.position, Quaternion.identity);
                }
                else
                {
                    Debug.Log("Out of Case!");
                }

                p_control.Add_Player_Token(10); // Token Reward

                for (int i = 0; i < Birds_List.Count; i++)
                {
                    if (Birds_List[i] != null)
                    {
                        Destroy(Birds_List[i].gameObject);
                    }
                }
                is_Result_Once = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player_Object = collision.gameObject;
            p_control = collision.GetComponent<PlayerCharacter_Controller>();
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{

    //}

    public void Event_Move(InputAction.CallbackContext ctx)
    {
        if (is_Event_Acting)
        {
            v_Horizontal = ctx.action.ReadValue<Vector2>();

            Vector2 input = ctx.ReadValue<Vector2>();
            Input_Value = input.x;

            //Debug.Log("Move Input Detected");
        }
    }

    public void Event_Move_Direction(Vector2 dir)
    {
        if (is_Event_Acting)
        {
            v_Direction = dir;
            v_Direction = new Vector2(dir.x, 0.0f);
            Input_Value = dir.x;
        }
    }

    public void Event_Attack(InputAction.CallbackContext ctx)
    {
        if (is_Event_Acting && ctx.phase == InputActionPhase.Started)
        {
            if (Used_bullet < Max_Bullet)
            {
                GameObject prf = Instantiate(Event_Projectile, Bow_Object.transform.position, Bow_Point.transform.rotation);
                prf.GetComponent<Hunting_Projectile>().Start_Rotate(Bow_Point.transform.eulerAngles.z);
                Used_bullet++;
                Set_Bullet_Text();
            }
            else
            {
                if (is_Running)
                {
                    StopCoroutine(coroutine);
                    Event_Result();
                }
            }
        }
    }

    private void Hunting_Move()
    {
        float Rotation_Amount = -Input_Value * Rotation_Speed * Time.deltaTime;
        Current_Angle += Rotation_Amount;

        Current_Angle = Mathf.Clamp(Current_Angle, -Max_Angle, Max_Angle);
        Bow_Point.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Current_Angle);
    }

    private void Hunting_Spawn()
    {
        Prf_Count = 0;
        bird_Count = 0;

        bool br_data = false;

        for (int i = 0; i < Spawn_Points.Count; i++)
        {
            if (i == 6 || i == 9)
            {
                Prf_Count++;
            }

            GameObject bird_pref = Instantiate(Bird_Prefabs[Prf_Count], Spawn_Points[i].transform.position, Quaternion.identity);
            Birds_List.Add(bird_pref);

            bird_pref.GetComponent<Hunting_Bird>().Set_Manager(this.gameObject);
            bird_pref.GetComponent<Hunting_Bird>().Set_Position(Spawn_Points[i].transform.position);
            bird_pref.GetComponent<Hunting_Bird>().is_Facing_Left = br_data;

            bird_Count++;
            br_data = !br_data;
        }
    }

    private IEnumerator Event_Coroutine()
    {
        is_Running = true;
        yield return new WaitForSeconds(Event_Time);
        Event_Result();
    }

    private void Event_Result()
    {
        Npc_Interaction_End();

        p_control.State_Change(PlayerCharacter_Controller.Player_State.Dialogue);
        Dialogue_Manager.instance.Start_Dialogue(Event_Complete);
        is_Running = false;
        is_Event_Once = true;

        //Npc_Interaction_End();
    }

    // 시간 게이지 초기화
    private void InitTimeGauge()
    {
        currentTime = totalTime;
        originalScale = Event_Time_Slider.transform.localScale;
        originalPosition = Event_Time_Slider.transform.localPosition;
        spriteHeight = Event_Time_Slider.bounds.size.y;
    }

    // 시간 게이지 업데이트
    private void UpdateTimeGauge()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, totalTime);

        float fillAmount = currentTime / totalTime;

        Event_Time_Slider.transform.localScale = new Vector3(originalScale.x, originalScale.y * fillAmount, originalScale.z);
        float offsetY = spriteHeight * (1 - fillAmount) * 0.5f;
        Event_Time_Slider.transform.localPosition = originalPosition - new Vector3(0f, offsetY, 0f);
    }

    private void Set_Bullet_Text()
    {
        Event_Bullet_Text.text = (Max_Bullet - Used_bullet).ToString();
    }
}
