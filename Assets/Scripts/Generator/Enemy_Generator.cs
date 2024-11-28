using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy_Generator : MonoBehaviour
{
    [SerializeField]
    private GameObject Enemy_Prefab;
    [SerializeField]
    private GameObject CardBox_Prefab;
    [SerializeField] private PlayerCharacter_Controller p_Controller;

    [Header("Array")]
    [SerializeField] private GameObject[] Enemy_Prefabs;
    private GameObject new_Enemy;
    private int Enemy_Count = 0;


    [HideInInspector]
    public static bool Is_Next_Spawn = false;
    [HideInInspector]
    public static bool Is_Room_Clear = true; //�� ���� Ŭ���� �ߴ��� Ȯ�ο�

    private int Wave_Count = 1;
    [HideInInspector]
    public static int i_Enemy_Count = 0;


    private bool is_Now_Started = false;


    // New Values
    private int i_Map_Count = 0;
    private bool is_Do_Once = false;
    private bool b_boss_Stage = false;


    // Start is called before the first frame update
    void Start()
    {
        Is_Next_Spawn = false;
        i_Enemy_Count = 0;

        Is_Next_Spawn = true;
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
        Is_Next_Spawn = false;
        Is_Room_Clear = true;
        i_Enemy_Count = 0;

        Enemy_Count = 0;

        Is_Next_Spawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (i_Enemy_Count <= 0 && !Is_Room_Clear)
        {
            Check_Need_Spawn();
        }
    }

    public void New_Enemy_Spawn()
    {
        if (Is_Next_Spawn && !Is_Room_Clear && !b_boss_Stage)
        {
            foreach (Vector3 v_Spawn in Map_Manager.Map_Shuffled_List[i_Map_Count].v_New_Spawn_Points[Wave_Count - 1].v_Dataes)
            {
                new_Enemy = Enemy_Prefabs[Map_Manager.Map_Shuffled_List[i_Map_Count].i_Enemy_Index[Wave_Count - 1].i_enemy_Index[Enemy_Count]];

                GameObject spawned_Enemy = Instantiate(new_Enemy, v_Spawn, Quaternion.identity);

                foreach(Enemy_Interface enemy_interface in spawned_Enemy.GetComponentsInChildren<Enemy_Interface>(true)) //������ �÷��̾� ����
                {
                    enemy_interface.Player_Initialize(p_Controller);
                }

                i_Enemy_Count++;
                Enemy_Count++;
            }
            Enemy_Count = 0;

            is_Do_Once = false;

            is_Now_Started = true;
            Is_Next_Spawn = false;

            //Box Spawn

            //if (is_Now_Started) // ���� ���� �� �ٷ� �����Ǵ°� ����
            //{
            //    Vector3 cardBox_SpawnPoint = ScObj_Map[i_Room_Number].v_CardBox_SpawnPoint;
            //    Instantiate(CardBox_Prefab, cardBox_SpawnPoint, Quaternion.identity);
            //}
        }
    }

    protected void Check_Need_Spawn() //�� ���� �ı� �� ����Wave�� �ø��� ������ True�� ����
    {
        if (i_Enemy_Count <= 0 && Is_Room_Clear == false && !is_Do_Once)
        {
            is_Do_Once = true;

            Debug.Log("Enemy All Died");

            i_Enemy_Count = 0;
            Wave_Count++;

            if (Map_Manager.Map_Shuffled_List[i_Map_Count].i_How_Many_Wave + 1 <= Wave_Count)
            {
                Debug.Log("Clear");
                if (is_Now_Started) // ���� ���� �� �ٷ� �����Ǵ°� ����
                {
                    Vector3 cardBox_SpawnPoint = Map_Manager.Map_Shuffled_List[i_Map_Count].v_CardBox_SpawnPoint;
                    Instantiate(CardBox_Prefab, cardBox_SpawnPoint, Quaternion.identity);
                }

                Is_Room_Clear = true;
                Wave_Count = 1;

                if(i_Map_Count < Map_Manager.Map_Shuffled_List.Count - 1)
                {
                    i_Map_Count++;
                }
                else
                {
                    b_boss_Stage = true;
                }
            }

            Is_Next_Spawn = true;
            New_Enemy_Spawn();
        }
    }

    public void Set_Next()
    {
        Is_Next_Spawn = true;
        Is_Room_Clear = false;
    }
}
