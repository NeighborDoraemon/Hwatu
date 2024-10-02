using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Generator : MonoBehaviour
{
    [SerializeField]
    private GameObject Enemy_Prefab;
    [SerializeField]
    private Map_Value[] ScObj_Map;


    [HideInInspector]
    public static bool Is_Next_Spawn = false;
    [HideInInspector]
    public static bool Is_Room_Clear = false; //방 전부 클리어 했는지 확인용

    [HideInInspector]
    public static int i_Room_Number = 0;

    private int Wave_Count = 1;
    [HideInInspector]
    public static int i_Enemy_Count = 0;


    // Start is called before the first frame update
    void Start()
    {
        Is_Next_Spawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        Enemy_Spawning();
        Check_Need_Spawn();
    }


    private void Enemy_Spawning() //적 소환. Wave_Count를 돌려서 생성 좌표 변경 (생성된 적을 리스트에 넣어서 관리하려 했는데 잘 안되어서 갯수로 변경)
    {
        if (Is_Next_Spawn == true && Is_Room_Clear == false)
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
                        break;
                    }
            }
        }
    }

    protected void Check_Need_Spawn() //적 전부 파괴 시 다음Wave로 올리고 스폰을 True로 변경
    { 
        if (i_Enemy_Count == 0 && Is_Room_Clear == false)
        {
            //Debug.Log("Enemy All Died");
            Wave_Count++;
            Is_Next_Spawn = true;
        }
    }
}
