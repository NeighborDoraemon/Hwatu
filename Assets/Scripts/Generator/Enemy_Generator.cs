using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy_Generator : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Box_Prefabs;
    [SerializeField] private PlayerCharacter_Controller p_Controller;
    [SerializeField] private TextMeshPro Wave_Text;

    [Header("Array")]
    [SerializeField] private GameObject[] Enemy_Prefabs;
    private GameObject new_Enemy;
    private int Enemy_Count = 0;


    [HideInInspector]
    public static bool Is_Next_Spawn = false;
    [HideInInspector]
    public static bool Is_Room_Clear = true; //방 전부 클리어 했는지 확인용

    private int Wave_Count = 1;
    


    private bool is_Now_Started = false;


    // New Values
    private int i_Map_Count = 0;
    private bool is_Do_Once = false;
    private bool b_boss_Stage = false;


    public static Enemy_Generator Instance { get; private set; }

    [HideInInspector]public static int i_Enemy_Count = 0;

    private Map_Value Current_Map;
    

    //Box Enum
    private enum Box_Rate
    {
        Cummon_Box = 70,
        Rare_Box = 25,
        Legendary_Box = 5,
        Max_Rate = 101
    }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Is_Next_Spawn = false;
        //i_Enemy_Count = 0;

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
        //i_Enemy_Count = 0;

        Enemy_Count = 0;

        Is_Next_Spawn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Room Clear : " + Is_Room_Clear);
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

                foreach(Enemy_Interface enemy_interface in spawned_Enemy.GetComponentsInChildren<Enemy_Interface>(true)) //적에게 플레이어 전달
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

            if(Wave_Count != 1)
            {
                Wave_Print(0);
            }

            //Box Spawn

            //if (is_Now_Started) // 게임 시작 시 바로 스폰되는걸 방지
            //{
            //    Vector3 cardBox_SpawnPoint = ScObj_Map[i_Room_Number].v_CardBox_SpawnPoint;
            //    Instantiate(CardBox_Prefab, cardBox_SpawnPoint, Quaternion.identity);
            //}
        }
    }

    public void New_Enemy_Spawn(Map_Value Current)
    {
        Current_Map = Current;
        if(Current_Map.i_How_Many_Wave == 0)
        {
            return;
        }

        Debug.Log("Wave : " + Wave_Count);
        Debug.Log("is_Room_Clear : " + Is_Room_Clear);

        if (Is_Next_Spawn && !Is_Room_Clear && !b_boss_Stage)
        {
            foreach (Vector3 v_Spawn in Current.v_New_Spawn_Points[Wave_Count - 1].v_Dataes)
            {
                new_Enemy = Enemy_Prefabs[Current.i_Enemy_Index[Wave_Count - 1].i_enemy_Index[Enemy_Count]];

                GameObject spawned_Enemy = Instantiate(new_Enemy, v_Spawn, Quaternion.identity);

                foreach (Enemy_Interface enemy_interface in spawned_Enemy.GetComponentsInChildren<Enemy_Interface>(true)) //적에게 플레이어 전달
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

            if (Wave_Count != 1)
            {
                Wave_Print(0);
            }
        }
    }

    protected void Check_Need_Spawn() //적 전부 파괴 시 다음Wave로 올리고 스폰을 True로 변경
    {
        if (/*i_Enemy_Count <= 0 && !Is_Room_Clear && */!is_Do_Once)
        {
            is_Do_Once = true;

            Debug.Log("Enemy All Died");

            i_Enemy_Count = 0;
            Wave_Count++;

            if (Current_Map.i_How_Many_Wave + 1 <= Wave_Count)
            {
                Debug.Log("Clear");
                if (is_Now_Started) // 게임 시작 시 바로 스폰되는걸 방지
                {
                    Vector3 cardBox_SpawnPoint = Current_Map.v_CardBox_SpawnPoint;
                    Instantiate(Box_Random(), cardBox_SpawnPoint, Quaternion.identity);

                    Wave_Print(1);
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
            //New_Enemy_Spawn();
            New_Enemy_Spawn(Current_Map);
        }
    }

    public void Set_Next()
    {
        Is_Next_Spawn = true;
        Is_Room_Clear = false;
        Wave_Count = 1;
    }

    public void Room_Clear_Setter()
    {
        Is_Room_Clear = false;
    }

    public void From_Other_Add_Enemy()
    {
        i_Enemy_Count++;
    }

    private GameObject Box_Random()
    {
        int index = 0;
        int rand = UnityEngine.Random.Range(1, (int)Box_Rate.Max_Rate);

        if (rand <= (int)Box_Rate.Cummon_Box)
        {
            index = 0;
        }
        else if (rand <= (int)Box_Rate.Rare_Box)
        {
            index = 1;
        }
        else
        {
            index = 2;
        }

        return Box_Prefabs[index];
    }

    private void Wave_Print(int case_index)
    {
        switch(case_index)
        {
            case 0:
                Wave_Text.text = "아직 끝나지 않았다...";
                Wave_Text.color = new Color(255.0f, 0.0f, 0.0f);
                break;
            case 1:
                Wave_Text.text = "이제 고요하다...";
                Wave_Text.color = new Color(255.0f, 255.0f, 255.0f);
                break;
        }
        StartCoroutine(Fade_Sprite(Wave_Text, 1.0f, 1.0f, 0.0f));
    }

    private IEnumerator Fade_Sprite(TextMeshPro sprite, float targetAlpha, float duration, float delay)
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

    public void Set_Current(Map_Value map)
    {
        Current_Map = map;
    }
}
