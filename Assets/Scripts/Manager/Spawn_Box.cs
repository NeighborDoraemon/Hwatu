using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn_Box : MonoBehaviour
{
    public Transform[] spawnPoints;            // 카드 소환 위치
    public int number_Of_Objects_To_Spawn = 2; // 카드 소환 갯수

    public string itemName_To_Spawn;
    PlayerCharacter_Controller player;

    public float common_DropRates = 80;
    public float epic_DropRates = 17;
    public float legendary_DropRates = 3;

    private Dictionary<ItemRarity, float> dropRates;

    private void Awake()
    {
        dropRates = new Dictionary<ItemRarity, float>()
        {
            { ItemRarity.Common, common_DropRates },
            { ItemRarity.Epic, epic_DropRates },
            { ItemRarity.Legendary, legendary_DropRates }
        };
    }

    public void Request_Spawn_Cards()
    {
        if (Object_Manager.instance == null)
        {
            Debug.LogError("Card Box에서 Card_Spawner 인스턴스 실종");
            return;
        }

        int spawnCount = Mathf.Min(number_Of_Objects_To_Spawn, spawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 spawnPos = spawnPoints[i].position;

            if (i == 0)
            {
                Object_Manager.instance.Spawn_Cards(spawnPos);
            }
            else if (i == 1)
            {
                Object_Manager.instance.Spawn_Item(spawnPos, dropRates, player);
            }
        }

        Destroy(gameObject);
    }
}
