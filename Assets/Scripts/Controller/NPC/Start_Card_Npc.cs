using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Start_Card_Npc : MonoBehaviour
{
    [SerializeField] private Transform[] Card_Spawn_Points;
    private int Spawn_Count = 2;

    private PlayerCharacter_Controller player;

    public bool give_Card = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
    }

    public void Request_Spawn_Cards()
    {
        if (Object_Manager.instance == null)
        {
            Debug.LogError("Card Box에서 Card_Spawner 인스턴스 실종");
            return;
        }

        int spawnCount = Mathf.Min(Spawn_Count, Card_Spawn_Points.Length);

        if (!give_Card)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                Vector2 spawnPos = Card_Spawn_Points[i].position;
                GameObject spawned_Card = Object_Manager.instance.Spawn_Cards(spawnPos);

                if (spawned_Card != null)
                {
                    player.AddCard(spawned_Card);
                }
            }
        }

        give_Card = true;
    }
}
