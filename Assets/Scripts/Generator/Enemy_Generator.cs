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

    [Header("Array")]
    [SerializeField] private Map_Value[] ScObj_Map;
    [SerializeField] private GameObject[] Enemy_Prefabs;
    private GameObject new_Enemy;
    private int Enemy_Count = 0;


    [HideInInspector]
    public static bool Is_Next_Spawn = false;
    [HideInInspector]
    public static bool Is_Room_Clear = true; //방 전부 클리어 했는지 확인용

    [HideInInspector]
    public static int i_Room_Number = 0;

    private int Wave_Count = 1;
    [HideInInspector]
    public static int i_Enemy_Count = 0;


    private bool is_Now_Started = false;


    // Start is called before the first frame update
    void Start()
    {
        Is_Next_Spawn = false;
        i_Enemy_Count = 0;
        i_Room_Number = 0;

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
        Debug.Log("Enemy_Generator Reloaded");
        Is_Next_Spawn = false;
        Is_Room_Clear = true;
        i_Enemy_Count = 0;
        i_Room_Number = 0;

        Enemy_Count = 0;

        Is_Next_Spawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Enemy_Spawning();
        New_Enemy_Spawn();
        if (i_Room_Number >= 0)
        {
            Check_Need_Spawn();
        }
    }

    private void New_Enemy_Spawn()
    {
        if(i_Room_Number == -1)
        {

        }
        else if (Is_Next_Spawn == true && Is_Room_Clear == false && i_Room_Number >= 0)
        {
            foreach (Vector3 v_Spawn in ScObj_Map[i_Room_Number].v_New_Spawn_Points[Wave_Count - 1].v_Dataes)
            {
                new_Enemy = Enemy_Prefabs[ScObj_Map[i_Room_Number].i_Enemy_Index[Wave_Count - 1].i_enemy_Index[Enemy_Count]];
                Debug.Log("Enemy Spawned");
                Debug.Log(Wave_Count - 1);
                Debug.Log(Enemy_Count);
                Instantiate(new_Enemy, v_Spawn, Quaternion.identity);
                i_Enemy_Count++;
                Enemy_Count++;
            }
            Enemy_Count = 0;

            is_Now_Started = true;
            Is_Next_Spawn = false;

            //if (is_Now_Started) // 게임 시작 시 바로 스폰되는걸 방지
            //{
            //    Vector3 cardBox_SpawnPoint = ScObj_Map[i_Room_Number].v_CardBox_SpawnPoint;
            //    Instantiate(CardBox_Prefab, cardBox_SpawnPoint, Quaternion.identity);
            //}
        }
    }

    private void Enemy_Spawning() //적 소환. Wave_Count를 돌려서 생성 좌표 변경 (생성된 적을 리스트에 넣어서 관리하려 했는데 잘 안되어서 갯수로 변경)
    {
        if (Is_Next_Spawn == true && Is_Room_Clear == false && i_Room_Number >= 0)
        {
            Debug.Log("Switch Start");
            Debug.Log(Wave_Count);
            switch (Wave_Count)
            {
                case 1:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[i_Room_Number].v_Enemy_Spawn_Points_01)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        is_Now_Started = true;
                        Is_Next_Spawn = false;
                        break;
                    }
                case 2:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[i_Room_Number].v_Enemy_Spawn_Points_02)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        Is_Next_Spawn = false;
                        break;
                    }
                case 3:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[i_Room_Number].v_Enemy_Spawn_Points_03)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        //Wave_Count++;
                        Is_Next_Spawn = false;
                        //Is_Room_Clear = true;
                        break;
                    }
                case 4:
                    {
                        //클리어 후 대기상태

                        Wave_Count = 1;
                        Is_Room_Clear = true;
                        if(is_Now_Started) // 게임 시작 시 바로 스폰되는걸 방지
                        {
                            Vector3 cardBox_SpawnPoint = ScObj_Map[i_Room_Number].v_CardBox_SpawnPoint;
                            Instantiate(CardBox_Prefab, cardBox_SpawnPoint, Quaternion.identity);
                        }
                        break;
                    }
            }
        }
    }

    protected void Check_Need_Spawn() //적 전부 파괴 시 다음Wave로 올리고 스폰을 True로 변경
    {
        if (i_Enemy_Count <= 0 && Is_Room_Clear == false)
        {
            //Debug.Log("Enemy All Died");
            i_Enemy_Count = 0;
            Wave_Count++;

            if (ScObj_Map[i_Room_Number].i_How_Many_Wave + 1 <= Wave_Count)
            {
                Debug.Log("Clear");
                Is_Room_Clear = true;
                Wave_Count = 1;
            }

            Is_Next_Spawn = true;
        }
    }
}
