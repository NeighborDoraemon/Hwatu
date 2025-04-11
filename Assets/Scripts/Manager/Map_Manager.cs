using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//¸Ê ÀÌµ¿ ÃÑ°ý ¸Å´ÏÀú #±èÀ±Çõ
public class Map_Manager : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private int i_Using_Map_Count;

    [Header("Lists")]
    [SerializeField] private List<Map_Value> Map_Data;
    [SerializeField] private List<Map_Value> FB_Map_Data;
    [SerializeField] private List<Map_Value> Event_Map_Data;

    [HideInInspector] public static List<Map_Value> Map_Shuffled_List = new List<Map_Value>();
    [HideInInspector] public static Queue<Map_Value> Map_Shuffled_Queue = new Queue<Map_Value>(); // new Shuffled
    [HideInInspector] public static Queue<Map_Value> Event_Map_Shuffled_Queue = new Queue<Map_Value>(); // Event Shuffled

    [SerializeField] private Vector3 FB_Boss_Point;
    [SerializeField] private Map_Value Map_Tutorial;

    [Header("Market")]
    [SerializeField] private Vector3 Market_Point;
    [SerializeField] private Map_Value Market_Data;
    private bool is_take_Market = false;
    private bool is_Market_Now = false;

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
    private int Boss_map_Index = 0;
    private bool is_Tutorial_Cleared = false;

    [HideInInspector] public bool is_Boss_Stage = false;


    //Map Card Values
    private int map_Card_01;
    private int map_Card_02;
    private List<Card_Value> map_card_Values;

    private bool is_Card_Set = true;    // map card boolean
    [SerializeField] private Match_Up_Manager match_manager;

    private Map_Value mv_Next_Map;

    // Start is called before the first frame update
    void Start()
    {
        //Shuffle_Maps();
        //Set_Next_Point();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        map_Index = 0;
        Boss_map_Index = 0;
        Map_Shuffled_List.Clear();

        Map_Shuffled_Queue.Clear(); // Queue Clear
        Event_Map_Shuffled_Queue.Clear();

        Shuffle_Maps();

        if (is_Tutorial_Cleared)
        {
            Set_Next_Point();
        }
        else
        {
            v_Next_SpawnPoint = Map_Tutorial.v_Map_Spawnpoint;
        }

        //Update_Map_Boundary();
        IsOnPortal = false;
        Debug.Log(i_Using_Map_Count / 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Use_Portal()
    {
        {
            if (IsOnPortal && Enemy_Generator.Is_Room_Clear == true) //¸Ê Å¬¸®¾î½Ã¿¡¸¸ ÀÌµ¿ °¡´ÉÇÏµµ·Ï º¯°æ
            {
                if(!is_Card_Set)
                {
                    Get_Random_Cards();

                    match_manager.Give_Map_Cards(map_Card_01, map_Card_02);
                    match_manager.Start_Match();
                }

                player_Input.SwitchCurrentActionMap("Menu");
                new_Fade.Fade_Out(() =>
                {
                    Portal_Method();
                    Debug.Log("Fade Out Complete");


                    new_Fade.Fade_In(() =>
                    {
                        player_Input.SwitchCurrentActionMap("Player");
                        Debug.Log("Fade In Complete");
                    });
                });
            }
        }
    }

    private void Portal_Method()
    {
        Obj_Player.transform.position = v_Next_SpawnPoint;

        if (is_Boss_Stage)
        {
            First_Boss.GetComponent<FB_Castle_Wall>().Call_Start();
        }

        if (is_Tutorial_Cleared && !is_Market_Now) 
        {
            Obj_e_Generator.Set_Next();
            Obj_e_Generator.New_Enemy_Spawn(); // First Spawn in map
            map_Index++;    // Plus map's Index when the map is Battle map
        }

        Set_Next_Point();
        is_Tutorial_Cleared = true;
    }

    //private void Update_Map_Boundary()
    //{
    //    GameObject boundary_Object = GameObject.FindWithTag("Boundary");
    //    if (boundary_Object != null)
    //    {
    //        cur_Map_Boundary = boundary_Object.GetComponent<Collider2D>();
    //        camera_Manager.Update_Confiner(cur_Map_Boundary);
    //    }
    //}

    private void Shuffle_Maps()
    {
        for(int i = 0; i < i_Using_Map_Count /*Map_Data.Count*/; i++ )
        {
            int Index = Random.Range(0, Map_Data.Count);
            Map_Shuffled_List.Add(Map_Data[Index]);

            Map_Shuffled_Queue.Enqueue(Map_Data[Index]); // Queue Enqueue

            Map_Data.RemoveAt(Index);
        }

        for(int i = 0; i < Event_Map_Data.Count; i++)   //Event Map Shuffle
        {
            int Index = Random.Range(0, Event_Map_Data.Count);
            Event_Map_Shuffled_Queue.Enqueue(Event_Map_Data[Index]);
        }
        Debug.Log(string.Join(", ", Map_Shuffled_Queue)); // Queue Debug
    }

    private void Set_Next_Point()
    {
        //½Å±Ô ¸Ê ½Ã½ºÅÛ ºÎºÐ 02.18

        if (Map_Shuffled_Queue.Count <= 0) // Goto Boss Stage
        {
            mv_Next_Map = FB_Map_Data[Boss_map_Index];
            v_Next_SpawnPoint = mv_Next_Map.v_Map_Spawnpoint;
            is_Boss_Stage = true;
        }
        else
        {
            if (Map_Shuffled_Queue.Count <= i_Using_Map_Count / 2 && !is_take_Market) // Goto Market
            {
                mv_Next_Map = Market_Data;
                v_Next_SpawnPoint = mv_Next_Map.v_Map_Spawnpoint;
                is_take_Market = true;
                is_Market_Now = true;
            }
            else // Goto Next Map
            {
                int rand = 10;
                if (map_Index >= 3 && map_Index <= 5)
                {
                    rand = Random.Range(1, 11);
                    Debug.Log("Event Random Index : " + rand);
                }

                if (rand < 4 && Event_Map_Shuffled_Queue.Count != 0)
                {
                    Set_Event_Next();
                    is_Market_Now = true;

                    is_Card_Set = false;
                }
                else
                {
                    mv_Next_Map = Map_Shuffled_Queue.Dequeue();
                    v_Next_SpawnPoint = mv_Next_Map.v_Map_Spawnpoint;
                    is_Market_Now = false;

                    is_Card_Set = false;
                }
            }
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
            map_card_Values.RemoveAt(rand); // ±¤ Áßº¹¹æÁö
        }


        do
        {
            rand = Random.Range(0, obj_manager.card_Values.Length);

            map_Card_02 = obj_manager.card_Values[rand].Month;
        } while (map_Card_02 == player_card_Value_01 || map_Card_02 == player_card_Value_02 || map_Card_02 == map_Card_01);

        //Debug.Log(map_Card_01);
        //Debug.Log(map_Card_02);
    }

    private void Set_Event_Next()
    {
        mv_Next_Map = Event_Map_Shuffled_Queue.Dequeue();
        v_Next_SpawnPoint = mv_Next_Map.v_Map_Spawnpoint;
        Debug.Log("Event Map Next!");
    }
}
