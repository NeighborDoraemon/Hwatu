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

    [Header("Second Stage")]
    [SerializeField] private List<Map_Value> Second_Stage_Map_Data;
    [SerializeField] private Map_Value SB_Map_Data; // Second Stage Boss Map Data


    //[HideInInspector] public static List<Map_Value> Map_Shuffled_List = new List<Map_Value>();
    //First Stage Queue
    private Queue<Map_Value> Map_Shuffled_Queue = new Queue<Map_Value>(); // new Shuffled
    private Queue<Map_Value> Event_Map_Shuffled_Queue = new Queue<Map_Value>(); // Event Shuffled

    //Second Stage Queue
    private Queue<Map_Value> Second_Map_Shuffled_Queue = new Queue<Map_Value>();

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


    [Header("Objects")]
    [SerializeField] private GameObject Obj_Player;
    [SerializeField] private Enemy_Generator Obj_e_Generator;
    [SerializeField] private New_Fade_Controller new_Fade;
    [SerializeField] private Object_Manager obj_manager;

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
    private bool is_Tutorial_Cleared = false;

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

    // Create By JBJ
    [Header("BronzeBell Reroll Strategy")]
    [SerializeField] private BronzeBell_Attack_Strrategy BB_Strat;

    // Start is called before the first frame update
    void Start()
    {
        //Shuffle_Maps();
        //Set_Next_Point();
        mv_Current_Map = Map_Start;
        Save_Manager.Instance.Register(this);
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
        data.Map_List_Index = Map_Index_List; // Original Index List
        data.Second_Map_Index_List = Second_Map_Index_List; // Second Stage Original Index List

        data.Map_Index = map_Index;
        data.Second_Map_Index = Second_map_Index; // Second Stage Map Index
        data.Current_Map = mv_Current_Map;
        data.Next_Map = mv_Next_Map;

        data.is_Market_Now = is_Market_Now;
        data.is_take_Market = is_take_Market;
        //data.is_Map_Saved = true;
        data.is_Boss_Stage = is_Boss_Stage;
        data.is_Tutorial_Cleared = is_Tutorial_Cleared; // Tutorial Cleared
        data.is_Event_Now = is_Event_Now;
    }

    private void Load_Saved_Data()
    {
        //Load Map Data
        mv_Next_Map = Save_Manager.Instance.Get<Map_Value>(data=>data.Next_Map);    //Next Map

        Map_Index_List = Save_Manager.Instance.Get<List<int>>(data => data.Map_List_Index); //Original Index List
        Second_Map_Index_List = Save_Manager.Instance.Get<List<int>>(data => data.Second_Map_Index_List); //Second Stage Original Index List

        map_Index = Save_Manager.Instance.Get<int>(data => data.Map_Index); //Map Index
        Second_map_Index = Save_Manager.Instance.Get<int>(data => data.Second_Map_Index); //Second Stage Map Index

        is_Market_Now = Save_Manager.Instance.Get<bool>(data => data.is_Market_Now); //Market Now

        is_take_Market = Save_Manager.Instance.Get<bool>(data => data.is_take_Market); //Market Take

        mv_Current_Map = Save_Manager.Instance.Get<Map_Value>(data => data.Current_Map); //Current Map

        is_Boss_Stage = Save_Manager.Instance.Get<bool>(data => data.is_Boss_Stage); //Boss Stage

        is_Tutorial_Cleared = Save_Manager.Instance.Get<bool>(data => data.is_Tutorial_Cleared); //Tutorial Cleared
        is_Event_Now = Save_Manager.Instance.Get<bool>(data => data.is_Event_Now); //Event Now
    }

    private void Make_Lists()
    {
        Map_Shuffled_Queue.Clear(); // Queue Clear

        //First Stage Lists
        for(int i = 0; i < Map_Index_List.Count; i++)
        {
            Map_Shuffled_Queue.Enqueue(Map_Data[Map_Index_List[i]]);
        }
        for(int i = 1; i < map_Index; i++)
        {
            Map_Shuffled_Queue.Dequeue();
        }

        //Second Stage Lists
        for (int i = 0; i < Second_Map_Index_List.Count; i++)
        {
            Second_Map_Shuffled_Queue.Enqueue(Second_Stage_Map_Data[Second_Map_Index_List[i]]);
        }
        if (Second_Map_Shuffled_Queue.Count > 0)
        {
            for (int i = 1; i < Second_map_Index; i++)
            {
                Second_Map_Shuffled_Queue.Dequeue();
            }
        }

        if (!is_Tutorial_Cleared)
        {
            mv_Current_Map = Map_Tutorial; // Current Map
        }
        else
        {
            if(is_Event_Now)
            {
                Set_Current_Event();
                return;
            }

            if (is_Market_Now)
            {
                mv_Current_Map = Market_Data; // Market Map
                return;
            }
            else if (is_Boss_Stage)
            {
                mv_Current_Map = FB_Map_Data; // Boss Map
                return;
            }
            else
            {
                mv_Current_Map = Map_Shuffled_Queue.Dequeue(); // Current Map
                return;
            }
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        map_Index = -1;
        //Map_Shuffled_List.Clear();

        Map_Shuffled_Queue.Clear(); // Queue Clear
        Event_Map_Shuffled_Queue.Clear();

        if (Save_Manager.Instance.Get<bool>(data => data.is_Map_Saved))
        {
            Load_Saved_Data();
            Make_Lists();
            New_Portal_Method(false);
            StartCoroutine(Wait_For_Enemy_Spawn());
        }
        else
        {
            Shuffle_Maps();
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
                        //Portal_Method(instrument);
                        New_Portal_Method(true);

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

    private void New_Portal_Method(bool b_index)
    {
        if (mv_Current_Map == Map_Tutorial && !is_Tutorial_Cleared)
        {
            is_Tutorial_Cleared = true;
        }

        if (b_index)
        {
            Set_New_Current(); // Set mv_Current Map
        }
        else
        {
            Obj_e_Generator.Set_Current(mv_Current_Map);
        }
        
        Obj_Player.transform.position = mv_Current_Map.v_Map_Spawnpoint;
        //Debug.Log("Current Map's Name :" + mv_Current_Map.name);

        Save_Manager.Instance.Modify(data =>
        {
            data.is_Map_Saved = true;
        });
        Save_Manager.Instance.SaveAll();

        if (is_Boss_Stage)
        {
            //First_Boss.GetComponent<FB_Castle_Wall>().Call_Start();
            StartCoroutine(Wait_For_Boss());
            return;
        }

        if (!is_Market_Now && mv_Current_Map != Map_Tutorial)
        {
            Debug.Log("Enemy Spawn Called");
            Obj_e_Generator.Set_Next();
            //Obj_e_Generator.New_Enemy_Spawn(); // First Spawn in map
            Obj_e_Generator.New_Enemy_Spawn(mv_Current_Map); // First Spawn in map
            if (!is_First_Cleared)
            {
                map_Index++;    // Plus map's Index when the map is Battle map
            }
            else if (is_First_Cleared && !is_Second_Cleared)
            {
                Second_map_Index++; // Plus Second Stage map's Index when the map is Battle map
            }
        }

        if (is_Market_Now)
        {
            Market_Stall.GetComponent<Obj_Market_Stall>().Market_Call();
        }
    }

    private IEnumerator Wait_For_Boss()
    {
        yield return null;
        First_Boss.GetComponent<FB_Castle_Wall>().Call_Start();
    }

    private void Set_New_Current()
    {
        if(!is_Tutorial_Cleared)
        {
            mv_Current_Map = Map_Tutorial; // Tutorial Map
            Obj_e_Generator.Set_Current(mv_Current_Map);
            return;
        }

        if (!is_First_Cleared)
        {
            if (Map_Shuffled_Queue.Count > 0)
            {
                if (Map_Shuffled_Queue.Count <= First_Using_Map_Count / 2 && !is_take_Market) // Goto Market
                {
                    mv_Current_Map = Market_Data; // Market Map
                    is_take_Market = true;
                    is_Market_Now = true;
                    Obj_e_Generator.Set_Current(mv_Current_Map);
                    return;
                }
                else
                {
                    int rand = 10;
                    if (map_Index >= 2 && map_Index <= 4)
                    {
                        rand = Random.Range(1, 11);
                        Debug.Log("Event Random Index : " + rand);
                    }

                    if (rand < 4 && Event_Map_Shuffled_Queue.Count != 0)
                    {
                        Set_Current_Event();
                        is_Market_Now = true;

                        is_Event_Now = true;

                        is_Card_Set = false;
                        Obj_e_Generator.Set_Current(mv_Current_Map);
                        return;
                    }
                    else
                    {
                        mv_Current_Map = Map_Shuffled_Queue.Dequeue();
                        is_Market_Now = false;

                        is_Card_Set = false;
                        Obj_e_Generator.Set_Current(mv_Current_Map);
                        return;
                    }
                }
            }
            else
            {
                mv_Current_Map = FB_Map_Data; // Boss Map
                is_Boss_Stage = true;
                Obj_e_Generator.Set_Current(mv_Current_Map);
                return;
            }
        }
        else if (is_First_Cleared && !is_Second_Cleared)
        {
            if (Second_Map_Shuffled_Queue.Count > 0)
            {
                if (Second_Map_Shuffled_Queue.Count <= First_Using_Map_Count / 2 && !is_take_Market) // Goto Market
                {
                    mv_Current_Map = Market_Data; // Market Map
                    is_take_Market = true;
                    is_Market_Now = true;
                    Obj_e_Generator.Set_Current(mv_Current_Map);
                    return;
                }
                else
                {
                    int rand = 10;
                    if (map_Index >= 2 && map_Index <= 4)
                    {
                        rand = Random.Range(1, 11);
                        Debug.Log("Event Random Index : " + rand);
                    }

                    if (rand < 4 && Event_Map_Shuffled_Queue.Count != 0)
                    {
                        Set_Current_Event();
                        is_Market_Now = true;

                        is_Event_Now = true;

                        is_Card_Set = false;
                        Obj_e_Generator.Set_Current(mv_Current_Map);
                        return;
                    }
                    else
                    {
                        if(Second_Map_Shuffled_Queue.Count == Second_Using_Map_Count)
                        {
                            is_Boss_Stage = false; // Reset Boss Stage
                        }
                        mv_Current_Map = Second_Map_Shuffled_Queue.Dequeue();
                        is_Market_Now = false;

                        is_Card_Set = false;
                        Obj_e_Generator.Set_Current(mv_Current_Map);
                        return;
                    }
                }
            }
            else
            {
                mv_Current_Map = FB_Map_Data; // Boss Map
                is_Boss_Stage = true;
                Obj_e_Generator.Set_Current(mv_Current_Map);
                return;
            }
        }
        else
        {
            Debug.LogError("Stage Bool Reference Error");
        }
    }

    private void Shuffle_Maps()
    {
        List<Map_Value> map_Data_Copy = new List<Map_Value>(Map_Data); // Copy Map Data
        List<Map_Value> Second_Data_Copy = new List<Map_Value>(Second_Stage_Map_Data); // Copy Second Stage Map Data
        Map_Index_List.Clear();

        for (int i = 0; i < First_Using_Map_Count && map_Data_Copy.Count > 0 /*Map_Data.Count*/; i++ )
        {
            int Index = Random.Range(0, map_Data_Copy.Count);
            Map_Value Selected_Map = map_Data_Copy[Index];

            //Map_Shuffled_List.Add(Selected_Map);
            Map_Shuffled_Queue.Enqueue(Selected_Map); // Queue Enqueue

            int Original_Index = Map_Data.IndexOf(Selected_Map);
            Map_Index_List.Add(Original_Index); // Original Index List

            map_Data_Copy.RemoveAt(Index);
        }

        List<Map_Value> event_map_data_copy = new List<Map_Value>(Event_Map_Data); // Copy Event Map Data
        while(event_map_data_copy.Count > 0)
        {
            int index = Random.Range(0, event_map_data_copy.Count);
            Event_Map_Shuffled_Queue.Enqueue(event_map_data_copy[index]);
            event_map_data_copy.RemoveAt(index);
        }

        // Second Stage Shuffle
        for (int i = 0; i < Second_Using_Map_Count && Second_Data_Copy.Count > 0; i++)
        {
            int Index = Random.Range(0, Second_Data_Copy.Count);
            Map_Value Selected_Map = Second_Data_Copy[Index];

            Second_Map_Shuffled_Queue.Enqueue(Selected_Map); // Queue Enqueue
            int Original_Index = Second_Stage_Map_Data.IndexOf(Selected_Map);

            Second_Map_Index_List.Add(Original_Index); // Original Index List
            Second_Data_Copy.RemoveAt(Index);
        }
    }

    public bool Check_Boss_Stage()
    {
        return true;
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

    private void Set_Event_Next()
    {
        mv_Next_Map = Event_Map_Shuffled_Queue.Dequeue();
        v_Next_SpawnPoint = mv_Next_Map.v_Map_Spawnpoint;
        is_Next_Event = true;

        Debug.Log("Event Map Next!");
    }

    private void Set_Current_Event()
    {
        mv_Current_Map = Event_Map_Shuffled_Queue.Dequeue();
        v_Next_SpawnPoint = mv_Current_Map.v_Map_Spawnpoint;
        is_Next_Event = false;
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
}
