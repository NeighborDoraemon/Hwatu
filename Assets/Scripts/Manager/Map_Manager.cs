using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� �̵� �Ѱ� �Ŵ��� #������
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

    // Start is called before the first frame update
    void Start()
    {
        Reset_PortalList();
    }

    // Update is called once per frame
    void Update()
    {
        Use_Portal();
    }

    private void Use_Portal()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsOnPortal)
            {
                Check_Portals();

                Obj_Player.transform.position = v_Next_SpawnPoint;
                //E_Spawner.Is_Next_Spawn = true;
            }
        }
    }

    private void Check_Portals() //�� ���� ���� ��, �̹� �̵��� ��Ż���� Ȯ��
    {
        int Randoms;

        Reset_PortalList();


        if (v_Next_SpawnPoint == new Vector3(0.0f, 0.0f, 0.0f))
        {
            Randoms = Random.Range(0, ScObj_Not_Used_Map_Value.Count);
            v_Next_SpawnPoint = ScObj_Not_Used_Map_Value[Randoms].v_Map_Spawnpoint;

            ScObj_Not_Used_Map_Value.RemoveAt(Randoms);
        }

        if (v_Next_SpawnPoint == v_Now_Portal)
        {
            //while (Obj_NextPortal.name == Which_Portal.name /*|| Check_Use_Portal(Obj_NextPortal)*/)
            {
                Randoms = Random.Range(0, ScObj_Not_Used_Map_Value.Count);

                v_Next_SpawnPoint = ScObj_Not_Used_Map_Value[Randoms].v_Map_Spawnpoint;
                ScObj_Not_Used_Map_Value.RemoveAt(Randoms);

                //Obj_NextPortal = Obj_Portals[Randoms];

            }
        }
    }

    private void Reset_PortalList() // ó�� �������� �� and �� ���� ������ ������ ��� ���� �ѹ��� �̵����� ��, ����Ʈ �ʱ�ȭ
    {
        if (ScObj_Not_Used_Map_Value.Count == 0)
        {
            for (int i = 0; i < Map_Dataes.Length; i++)
            {
                ScObj_Not_Used_Map_Value.Add(Map_Dataes[i]);
                //Debug.Log(ScObj_Not_Used_Map_Value[i]);
            }
            Debug.Log("List Reset");
        }
    }
}
