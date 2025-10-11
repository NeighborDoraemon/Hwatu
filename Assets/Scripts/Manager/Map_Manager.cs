using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//맵 이동 총괄 매니저 #김윤혁
public class Map_Manager : MonoBehaviour, ISaveable
{
    [Header("Values")]
    [SerializeField] private int First_Using_Map_Count;
    [SerializeField] private int Second_Using_Map_Count;

    [Header("Lists")]
    [SerializeField] private List<Map_Value> Map_Data;
    [SerializeField] private Map_Value FB_Map_Data;
    [SerializeField] private List<Map_Value> Event_Map_Data;
    [SerializeField] private Map_Value Fmb_Map_Data; // First miniboss Map Data

    [Header("Second Stage")]
    [SerializeField] private List<Map_Value> Second_Stage_Map_Data;
    [SerializeField] private Map_Value SB_Map_Data; // Second Stage Boss Map Data


    [Header("others")]
    [SerializeField] private Vector3 FB_Boss_Point;
    [SerializeField] private Map_Value Map_Tutorial;
    [SerializeField] private Map_Value Map_Start;

    [Header("Market")]
    [SerializeField] private Vector3 Market_Point;
    [SerializeField] private Map_Value Market_Data;
    [SerializeField] private GameObject Market_Stall;
    private bool is_take_Market = false;
    private bool is_Market_Now = false;
    private bool is_Next_Event = false; // Event Map

    [Header("Boss Objects")]
    [SerializeField] private GameObject First_Boss;
    [SerializeField] private GameObject Obj_Fmb;


    [Header("Objects")]
    [SerializeField] private GameObject Obj_Player;
    [SerializeField] private Enemy_Generator Obj_e_Generator;
    [SerializeField] private New_Fade_Controller new_Fade;
    [SerializeField] private Object_Manager obj_manager;
    [SerializeField] private Start_Card_Npc start_Card_Npc;

    [SerializeField]
    private Camera_Manager camera_Manager;

    [HideInInspector]
    public bool IsOnPortal = false;

    private Vector3 v_Next_SpawnPoint;

    private Collider2D cur_Map_Boundary;

    // For Fade In & Out
    [SerializeField] private PlayerInput player_Input;


    // Map_Move Values
    private int map_Index = -1;
    private int Second_map_Index = -1;  // Second Stage Map Index

    [HideInInspector] public bool is_Boss_Stage = false;


    //Map Card Values
    private int map_Card_01;
    private int map_Card_02;
    private List<Card_Value> map_card_Values;

    private bool is_Event_Now = false;

    private bool is_Card_Set = true;    // map card boolean
    [SerializeField] private Match_Up_Manager match_manager;

    private Map_Value mv_Next_Map;
    private Map_Value mv_Current_Map;

    private List<int> Map_Index_List = new List<int>();
    private List<int> Second_Map_Index_List = new List<int>(); // Second Stage Map Index List

    //Stage Clear Inform
    private bool is_First_Cleared = false; // First Stage Cleared
    private bool is_Second_Cleared = false; // Second Stage Cleared

    private int i_player_token = 0;

    private bool is_Fmb_Cleared = false; // First Mini Boss Cleared
    private bool is_Fmb_Now = false; // First Mini Boss Now

    // New Values for New Logic
    private List<int> New_map_Index_List = new List<int>();
    private List<int> New_second_map_Index_List = new List<int>();
    private List<int> event_map_Index_List = new List<int>();


    // Create By JBJ
    [Header("BronzeBell Reroll Strategy")]
    [SerializeField] private BronzeBell_Attack_Strrategy BB_Strat;

    // Start is called before the first frame update
    void Start()
    {
        Save_Manager.Instance.Register(this);

        if (!Save_Manager.Instance.Get<bool>(data => data.is_Map_Saved))
        {
            if (Save_Manager.Instance.Get<bool>(data => data.is_Tutorial_Cleared))
            {
                Obj_Player.transform.position = Map_Start.v_Map_Spawnpoint;
                mv_Current_Map = Map_Start;
            }
            else
            {
                Debug.Log("Tutorial Map Start");
                mv_Current_Map = Map_Tutorial;
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Save(SaveData data)
    { 
        data.is_Market_Now = is_Market_Now;
        data.is_take_Market = is_take_Market;
        data.is_Boss_Stage = is_Boss_Stage;
        data.is_Event_Now = is_Event_Now;

        data.player_token = i_player_token; // Player Token

        data.new_map_index_list = New_map_Index_List;
        data.new_event_map_list = event_map_Index_List;

        data.is_Fmb_Now = is_Fmb_Now;
    }

    private void Load_Saved_Data()
    {
        //Load Map Data

        is_Market_Now = Save_Manager.Instance.Get<bool>(data => data.is_Market_Now); //Market Now

        is_take_Market = Save_Manager.Instance.Get<bool>(data => data.is_take_Market); //Market Take

        is_Boss_Stage = Save_Manager.Instance.Get<bool>(data => data.is_Boss_Stage); //Boss Stage

        is_Event_Now = Save_Manager.Instance.Get<bool>(data => data.is_Event_Now); //Event Now

        i_player_token = Save_Manager.Instance.Get<int>(data => data.player_token); // Player Token

        New_map_Index_List = Save_Manager.Instance.Get<List<int>>(data => data.new_map_index_list);
        event_map_Index_List = Save_Manager.Instance.Get<List<int>>(data => data.new_event_map_list);

        is_Fmb_Now = Save_Manager.Instance.Get<bool>(data => data.is_Fmb_Now); // First Mini Boss Now
    }

    private void Load_Token()
    {
        i_player_token = Save_Manager.Instance.Get<int>(data => data.player_token); // Player Token
        if (i_player_token < 0)
        {
            i_player_token = 0; // Reset Token
        }

        Obj_Player.GetComponent<PlayerCharacter_Controller>().Add_Player_Token(i_player_token); // Add Player Token to Player
    }

    public void Token_Call()
    {
        i_player_token = Obj_Player.GetComponent<PlayerCharacter_Controller>().i_Token; // Player Token
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        map_Index = -1;

        Load_Token();

        if (Save_Manager.Instance.Get<bool>(data => data.is_Map_Saved))
        {
            Load_Saved_Data();  // have to Change***********************************
            New_New_Portal_Method(false);

            StartCoroutine(Wait_For_Enemy_Spawn());
        }
        else
        {
            //Shuffle_Maps();
            New_Shuffle_Maps();

            IsOnPortal = false;
        }

        Obj_e_Generator.Set_Use_Count(First_Using_Map_Count);
    }

    private IEnumerator Wait_For_Enemy_Spawn()
    {
        yield return null;
        Obj_e_Generator.Room_Clear_Setter();
    }

    public void Use_Portal(bool instrument)
    {
        {
            if (Obj_Player.GetComponent<PlayerCharacter_Card_Manager>().card_Inventory[1] != null)  //시작시 카드가 최소 2장이 있어야 이동 가능하게 변경
            {
                if (IsOnPortal && Enemy_Generator.Is_Room_Clear == true) //맵 클리어시에만 이동 가능하도록 변경
                {
                    if (!is_Card_Set)
                    {
                        Get_Random_Cards();

                        match_manager.Give_Map_Cards(map_Card_01, map_Card_02);
                        match_manager.Match_Reset();
                        //match_manager.Start_Match();
                    }

                    if (Obj_Player.GetComponent<PlayerCharacter_Card_Manager>().Has_Four_And_Nine())
                    {
                        BB_Strat.Reset_Reroll_Count();
                    }

                    player_Input.SwitchCurrentActionMap("Menu");
                    new_Fade.Fade_Out(() =>
                    {
                        //New_Portal_Method(true);
                        New_New_Portal_Method(true);

                        new_Fade.Fade_In(() =>
                        {
                            player_Input.SwitchCurrentActionMap("Player");
                            if (!is_Market_Now && !is_Next_Event)
                            {
                                match_manager.Start_Match();
                            }
                            is_Next_Event = false;
                        });
                    });
                    Object_Manager.instance.Destroy_All_Cards_And_Items();
                }
            }
        }
    }

    private void New_New_Portal_Method(bool b_index)
    {
        if (!b_index) // Saved Data is already exist  //Have to Load Last Map
        {
            if (is_Event_Now)
            {
                mv_Current_Map = Event_Map_Data[event_map_Index_List[0]];
            }
            else if (is_Market_Now)
            {
                mv_Current_Map = Market_Data; // Market Map
                Market_Stall.GetComponent<Obj_Market_Stall>().Market_Call();
            }
            else if (is_Boss_Stage)
            {
                if (is_Fmb_Now)
                {
                    mv_Current_Map = Fmb_Map_Data; // First Mini Boss Map
                    StartCoroutine(Wait_For_Fmb());
                }
                else
                {
                    mv_Current_Map = FB_Map_Data; // Boss Map
                    StartCoroutine(Wait_For_Boss());
                }
            }
            else
            {
                mv_Current_Map = Map_Data[New_map_Index_List[0]];
                //Debug.Log(Map_Data[New_map_Index_List[0]].name);
            }

            Obj_e_Generator.Set_Current(mv_Current_Map);

            i_player_token = Obj_Player.GetComponent<PlayerCharacter_Controller>().i_Token; // Player Token
            Obj_Player.transform.position = mv_Current_Map.v_Map_Spawnpoint;

            Save_Manager.Instance.Modify(data =>
            {
                data.is_Map_Saved = true;
                data.is_Inventory_Saved = true;
            });
            Save_Manager.Instance.SaveAll();

            if (!is_Market_Now && !is_Event_Now)
            {
                Debug.Log("Enemy Spawn Called");
                Obj_e_Generator.Set_Next();
                Obj_e_Generator.New_Enemy_Spawn(mv_Current_Map);
            }
        }
        else // Move by Portal
        {
            if (is_Event_Now)
            {
                is_Event_Now = false;
                event_map_Index_List.RemoveAt(0);
            }
            else if (is_Market_Now)
            {
                is_Market_Now = false;
                is_take_Market = true;
            }
            else if (is_Boss_Stage && is_Fmb_Now)
            {
                is_Fmb_Now = false;
                is_Boss_Stage = false;

                is_Fmb_Cleared = true;
            }
            else/* if (!is_Event_Now && !is_Market_Now && is_Boss_Stage)*/ // It was Normal Stage
            {
                Debug.Log("Remove Map Index List 0");
                New_map_Index_List.RemoveAt(0);
            }

            // Now Set Current Map
            if (New_map_Index_List.Count == 0) // Goto FB Boss
            {
                mv_Current_Map = FB_Map_Data; // Boss Map
                is_Boss_Stage = true;
                StartCoroutine(Wait_For_Boss());
            }
            else if (New_map_Index_List.Count <= 6 && New_map_Index_List.Count >= 4)
            {
                if (New_map_Index_List.Count == 4)
                {
                    if (!is_Fmb_Cleared)
                    {
                        mv_Current_Map = Fmb_Map_Data; // First Mini Boss Map
                        is_Boss_Stage = true;
                        is_Fmb_Now = true;
                        StartCoroutine(Wait_For_Fmb());
                    }
                    else if (!is_take_Market)
                    {
                        mv_Current_Map = Market_Data; // Market Map
                        is_Market_Now = true;
                        Market_Stall.GetComponent<Obj_Market_Stall>().Market_Call();
                    }
                    else
                    {
                        int rand = Random.Range(1, 11);
                        if (rand <= 4 && event_map_Index_List.Count > 0)
                        {
                            mv_Current_Map = Event_Map_Data[event_map_Index_List[0]];
                            is_Event_Now = true;
                        }
                    }
                }
                else
                {
                    int rand = Random.Range(1, 11);
                    if (rand <= 4 && event_map_Index_List.Count > 0)
                    {
                        mv_Current_Map = Event_Map_Data[event_map_Index_List[0]];
                        is_Event_Now = true;
                    }
                }
            }

            if (!is_Boss_Stage && !is_Market_Now && !is_Event_Now) // It was Normal Stage
            {
                //New_map_Index_List.RemoveAt(0);
                mv_Current_Map = Map_Data[New_map_Index_List[0]];
            }

            Obj_e_Generator.Set_Current(mv_Current_Map);

            i_player_token = Obj_Player.GetComponent<PlayerCharacter_Controller>().i_Token; // Player Token
            Obj_Player.transform.position = mv_Current_Map.v_Map_Spawnpoint;

            Save_Manager.Instance.Modify(data =>
            {
                data.is_Map_Saved = true;
                data.is_Inventory_Saved = true;
            });
            Save_Manager.Instance.SaveAll();

            if (!is_Market_Now && !is_Event_Now)
            {
                Debug.Log("Enemy Spawn Called");
                Obj_e_Generator.Set_Next();
                Obj_e_Generator.New_Enemy_Spawn(mv_Current_Map);
            }
        }
    }

    private IEnumerator Wait_For_Boss()
    {
        yield return null;
        First_Boss.GetComponent<FB_Castle_Wall>().Call_Start();
    }

    private IEnumerator Wait_For_Fmb()
    {
        yield return null;
        Obj_Fmb.GetComponent<Fmb_Spiritual>().Call_Start();
    }

    private void New_Shuffle_Maps()
    {
        New_map_Index_List.Clear();
        List<int> random_Queue = new List<int>();

        for (int i = 0; i < Map_Data.Count; i++)
        {
            random_Queue.Add(i);
        }
        for (int i = 0; i < First_Using_Map_Count; i++)
        {
            int rand = Random.Range(0, random_Queue.Count);

            New_map_Index_List.Add(random_Queue.ElementAt(rand));
            random_Queue.RemoveAt(rand);
        }

        random_Queue.Clear();
        for (int i = 0; i < Event_Map_Data.Count; i++)
        {
            random_Queue.Add(i);
        }
        for (int i = 0; i < Event_Map_Data.Count; i++)
        {
            int rand = Random.Range(0, random_Queue.Count);

            event_map_Index_List.Add(random_Queue.ElementAt(rand));
            random_Queue.RemoveAt(rand);
        }
        
        random_Queue.Clear();
        for (int i = 0; i < Map_Data.Count; i++)
        {
            random_Queue.Add(i);
        }
        // Second Stage Shuffle
        for (int i = 0; i < Second_Using_Map_Count; i++)
        {
            int rand = Random.Range(0, random_Queue.Count);

            Second_Map_Index_List.Add(random_Queue.ElementAt(rand));
            random_Queue.RemoveAt(rand);
        }
    }

    private void Get_Random_Cards()
    {
        int rand;

        GameObject player_card_01 = Obj_Player.GetComponent<PlayerCharacter_Controller>().card_Inventory[0];
        GameObject player_card_02 = Obj_Player.GetComponent<PlayerCharacter_Controller>().card_Inventory[1];
        int player_card_Value_01 = player_card_01.GetComponent<Card>().cardValue.Month;
        int player_card_Value_02 = player_card_02.GetComponent<Card>().cardValue.Month;

        map_card_Values = new List<Card_Value>(obj_manager.card_Values);

        do
        {
            rand = Random.Range(0, map_card_Values.Count);

            map_Card_01 = map_card_Values[rand].Month;
        } while (map_Card_01 == player_card_Value_01 || map_Card_01 == player_card_Value_02);

        if (map_Card_01 > 10)
        {
            map_card_Values.RemoveAt(rand); // 광 중복방지
        }


        do
        {
            rand = Random.Range(0, obj_manager.card_Values.Length);

            map_Card_02 = obj_manager.card_Values[rand].Month;
        } while (map_Card_02 == player_card_Value_01 || map_Card_02 == player_card_Value_02 || map_Card_02 == map_Card_01);
    }

    public void End_Stage(int stage_index)
    {
        switch (stage_index)
        {
            case 1:
                is_First_Cleared = true;
                break;
            case 2:
                is_Second_Cleared = true;
                break;
            default:
                Debug.LogError("Invalid Stage Index");
                break;
        }
    }

    public void Move_Tutorial()
    {
        if(mv_Current_Map != Map_Tutorial)
        {
            mv_Current_Map = Map_Tutorial; // Tutorial Map
        }
        else
        {
            mv_Current_Map = Map_Start; // Start Map

            Save_Manager.Instance.Modify(data =>
            {
                data.is_Tutorial_Cleared = true;
            });
            Save_Manager.Instance.SaveAll();

            start_Card_Npc.Reset_Bool();
        }

        Obj_e_Generator.Set_Current(mv_Current_Map);
        Obj_Player.transform.position = mv_Current_Map.v_Map_Spawnpoint;
    }

    public void Reset_Tutorial()
    {
        Save_Manager.Instance.Modify(data =>
        {
            data.is_Tutorial_Cleared = false;
        });
        Save_Manager.Instance.SaveAll();
    }
}
