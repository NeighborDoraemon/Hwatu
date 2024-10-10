using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵 이동 총괄 매니저 #김윤혁
public class Map_Manager : MonoBehaviour
{
    [SerializeField]
    private Map_Value[] Map_Dataes;

    [SerializeField]
    private GameObject Obj_Player;

    [HideInInspector]
    public bool IsOnPortal = false;
    [HideInInspector]
    public GameObject Which_Portal;

    [HideInInspector]
    public Vector3 v_Now_Portal;

    private Vector3 v_Next_SpawnPoint;

    private List<Map_Value> ScObj_Not_Used_Map_Value = new List<Map_Value>();


    private int i_room_Num = 0;

    // Start is called before the first frame update
    void Start()
    {
        Reset_PortalList();
    }

    // Update is called once per frame
    void Update()
    {
        //Use_Portal();
    }

    public void Use_Portal()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsOnPortal && Enemy_Generator.Is_Room_Clear == true && ScObj_Not_Used_Map_Value.Count != 0) //맵 클리어시에만 이동 가능하도록 변경
            {
                Check_Portals();

                Obj_Player.transform.position = v_Next_SpawnPoint;

                //방 번호 전달
                Enemy_Generator.i_Room_Number = i_room_Num;
                //방 이동시 클리어 초기화
                Enemy_Generator.Is_Room_Clear = false;
                //스폰 시작 트리거
                Enemy_Generator.Is_Next_Spawn = true;
            }
        }
    }

    private void Check_Portals() //한 번의 루프 중, 이미 이동한 포탈인지 확인
    {
        int Randoms;

        //Reset_PortalList();


        if (v_Next_SpawnPoint == new Vector3(0.0f, 0.0f, 0.0f))
        {
            Randoms = Random.Range(0, ScObj_Not_Used_Map_Value.Count);
            v_Next_SpawnPoint = ScObj_Not_Used_Map_Value[Randoms].v_Map_Spawnpoint;

            //이동한 맵의 번호를 적 생성 스크립트에 전달하기 위함
            i_room_Num = ScObj_Not_Used_Map_Value[Randoms].i_Map_Counter;

            ScObj_Not_Used_Map_Value.RemoveAt(Randoms);

            //i_room_Num = ScObj_Not_Used_Map_Value[Randoms].i_Map_Counter;
        }

        if (v_Next_SpawnPoint == v_Now_Portal)
        {
            //while (Obj_NextPortal.name == Which_Portal.name /*|| Check_Use_Portal(Obj_NextPortal)*/)
            {
                Randoms = Random.Range(0, ScObj_Not_Used_Map_Value.Count);

                v_Next_SpawnPoint = ScObj_Not_Used_Map_Value[Randoms].v_Map_Spawnpoint;

                //이동한 맵의 번호를 적 생성 스크립트에 전달하기 위함
                i_room_Num = ScObj_Not_Used_Map_Value[Randoms].i_Map_Counter;

                ScObj_Not_Used_Map_Value.RemoveAt(Randoms);

                //Obj_NextPortal = Obj_Portals[Randoms];

                //i_room_Num = ScObj_Not_Used_Map_Value[Randoms].i_Map_Counter;
            }
        }
    }

    private void Reset_PortalList() // 처음 시작했을 때 and 한 번의 루프가 끝나서 모든 맵을 한번씩 이동했을 때, 리스트 초기화
    {
        if (ScObj_Not_Used_Map_Value.Count == 0)
        {
            for (int i = 0; i < Map_Dataes.Length; i++)
            {
                ScObj_Not_Used_Map_Value.Add(Map_Dataes[i]);
                //Debug.Log(ScObj_Not_Used_Map_Value[i]);
            }
            //Debug.Log("List Reset");
        }
        //ScObj_Not_Used_Map_Value.RemoveAt(0);
    }
}
