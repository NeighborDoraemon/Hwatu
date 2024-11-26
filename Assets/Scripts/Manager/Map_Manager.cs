using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//¸Ê ÀÌµ¿ ÃÑ°ý ¸Å´ÏÀú #±èÀ±Çõ
public class Map_Manager : MonoBehaviour
{
    [Header("Lists")]
    [SerializeField] private List<Map_Value> Map_Data;
    [SerializeField] private List<Map_Value> FB_Map_Data;

    [HideInInspector] public static List<Map_Value> Map_Shuffled_List = new List<Map_Value>();

    [SerializeField] private Vector3 FB_Boss_Point;
    [SerializeField] private Map_Value Map_Tutorial;

    [Header("Boss Objects")]
    [SerializeField] private GameObject First_Boss;


    [Header("Objects")]
    [SerializeField] private GameObject Obj_Player;
    [SerializeField] private Enemy_Generator Obj_e_Generator;
    [SerializeField] private New_Fade_Controller new_Fade;

    [SerializeField]
    private Camera_Manager camera_Manager;

    [HideInInspector]
    public bool IsOnPortal = false;

    private Vector3 v_Next_SpawnPoint;

    private Collider2D cur_Map_Boundary;

    // For Fade In & Out
    [SerializeField] private PlayerInput player_Input;


    // Map_Move Values
    private int map_Index = 0;
    private int Boss_map_Index = 0;
    private bool is_Tutorial_Cleared = false;

    [HideInInspector] public bool is_Boss_Stage = false;

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
        Shuffle_Maps();
        if (is_Tutorial_Cleared)
        {
            Set_Next_Point();
        }
        else
        {
            v_Next_SpawnPoint = Map_Tutorial.v_Map_Spawnpoint;
        }

        Update_Map_Boundary();
        IsOnPortal = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Use_Portal()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsOnPortal && Enemy_Generator.Is_Room_Clear == true) //¸Ê Å¬¸®¾î½Ã¿¡¸¸ ÀÌµ¿ °¡´ÉÇÏµµ·Ï º¯°æ
            {
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

                //Obj_Player.transform.position = v_Next_SpawnPoint;

                //if(is_Boss_Stage)
                //{
                //    First_Boss.GetComponent<FB_Castle_Wall>().Call_Start();
                //}

                //if (is_Tutorial_Cleared)
                //{
                //    Obj_e_Generator.Set_Next();
                //    Obj_e_Generator.New_Enemy_Spawn(); // First Spawn in map
                //}

                //Set_Next_Point();
                //is_Tutorial_Cleared = true;
                
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

        if (is_Tutorial_Cleared)
        {
            Obj_e_Generator.Set_Next();
            Obj_e_Generator.New_Enemy_Spawn(); // First Spawn in map
        }

        Set_Next_Point();
        is_Tutorial_Cleared = true;
    }

    private void Update_Map_Boundary()
    {
        GameObject boundary_Object = GameObject.FindWithTag("Boundary");
        if (boundary_Object != null)
        {
            cur_Map_Boundary = boundary_Object.GetComponent<Collider2D>();
            camera_Manager.Update_Confiner(cur_Map_Boundary);
        }
    }

    private void Shuffle_Maps()
    {
        for(int i = 0; i < Map_Data.Count;)
        {
            int Index = Random.Range(0, Map_Data.Count);
            Map_Shuffled_List.Add(Map_Data[Index]);
            Map_Data.RemoveAt(Index);

            Debug.Log(Map_Shuffled_List[Map_Shuffled_List.Count - 1].name);
        }
    }

    private void Set_Next_Point()
    {
        // Map Index Check
        if (Boss_map_Index == Map_Shuffled_List.Count) // Reset Another List Index
        {
            map_Index = 0;
            is_Boss_Stage = false;
        }

        if (map_Index == Map_Shuffled_List.Count)
        {
            Boss_map_Index = 0;
            is_Boss_Stage = true;
        }



        // Map Index Plus
        if (map_Index < Map_Shuffled_List.Count && !is_Boss_Stage)
        {
            v_Next_SpawnPoint = Map_Shuffled_List[map_Index].v_Map_Spawnpoint;

            map_Index++;

        }
        
        if (Boss_map_Index < Map_Shuffled_List.Count && is_Boss_Stage)
        {
            v_Next_SpawnPoint = FB_Map_Data[Boss_map_Index].v_Map_Spawnpoint;
            Boss_map_Index++;
        }
    }

    public bool Check_Boss_Stage()
    {
        return true;
    }
}
