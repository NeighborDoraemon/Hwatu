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
    public bool Is_Next_Spawn = false;

    private int Wave_Count = 1;
    [HideInInspector]
    public int i_Enemy_Count = 0;


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


    private void Enemy_Spawning() //�� ��ȯ. Wave_Count�� ������ ���� ��ǥ ���� (������ ���� ����Ʈ�� �־ �����Ϸ� �ߴµ� �� �ȵǾ ������ ����)
    {
        if (Is_Next_Spawn)
        {
            switch (Wave_Count)
            {
                case 1:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[0].v_Enemy_Spawn_Points_01)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        Is_Next_Spawn = false;
                        break;
                    }
                case 2:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[0].v_Enemy_Spawn_Points_02)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        Is_Next_Spawn = false;
                        break;
                    }
                case 3:
                    {
                        foreach (Vector3 v_Spawns in ScObj_Map[0].v_Enemy_Spawn_Points_03)
                        {
                            Instantiate(Enemy_Prefab, v_Spawns, Quaternion.identity);
                            i_Enemy_Count++;
                        }
                        Wave_Count = 0;
                        Is_Next_Spawn = false;
                        break;
                    }
            }
        }
    }

    protected void Check_Need_Spawn() //�� ���� �ı� �� ����Wave�� �ø��� ������ True�� ����
    {
        if (i_Enemy_Count == 0)
        {
            //Debug.Log("Enemy All Died");
            Wave_Count++;
            Is_Next_Spawn = true;
        }
    }
}
