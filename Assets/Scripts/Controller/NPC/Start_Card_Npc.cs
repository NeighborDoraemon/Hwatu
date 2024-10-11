using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Card_Npc : MonoBehaviour
{
    [SerializeField] private Transform[] Card_Spawn_Points;
    private int Spawn_Count = 2;

    private bool is_Gave = false;

    public void Request_Spawn_Cards() // ī�� ��ȯ ��û �Լ�
    {
        if (!is_Gave)
        {
            if (Object_Manager.instance == null)
            {
                Debug.LogError("Card Box���� Card_Spawner �ν��Ͻ� ����");
                return;
            }

            int spawnCount = Mathf.Min(Spawn_Count, Card_Spawn_Points.Length); // �� �� �� ���� ���� ����ī��Ʈ ��ȯ

            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPos = Card_Spawn_Points[i].position;

                if (i == 0)
                {
                    Object_Manager.instance.Spawn_Cards(spawnPos);
                }
                else if (i == 1)
                {
                    Object_Manager.instance.Spawn_Cards(spawnPos);
                }
            }
            is_Gave = true;
        }
    }
}
