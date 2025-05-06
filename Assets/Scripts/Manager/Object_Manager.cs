using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Object_Manager : MonoBehaviour
{
    public static Object_Manager instance { get; private set; }

    public GameObject card_Prefab;
    public Card_Value[] card_Values;

    [SerializeField]
    private HashSet<Sprite> used_Card_Sprite = new HashSet<Sprite>();
    public List<GameObject> current_Spawned_Card = new List<GameObject>();

    public ItemDatabase item_Database;
    public GameObject itemPrefab;

    private List<GameObject> spawned_Item_Instances = new List<GameObject>();
    private List<Item> spawnedItems = new List<Item>();


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Card
    public GameObject Making_Card(Vector2 spawnPosition, Card_Value selected_Card, Sprite selected_Sprite)
    {
        GameObject spawnedCard = Instantiate(card_Prefab, spawnPosition, Quaternion.identity);
        
        SpriteRenderer spriteRenderer = spawnedCard.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = selected_Sprite;

        Card cardComponent = spawnedCard.GetComponent<Card>();
        cardComponent.cardValue = selected_Card;
        cardComponent.selected_Sprite = selected_Sprite;

        spawnedCard.tag = "Card";

        current_Spawned_Card.Add(spawnedCard);
        //used_Card_Sprite.Add(selected_Sprite);

        return spawnedCard;
    }

    public GameObject Spawn_Cards(Vector2 spawnPosition)
    {
        Card_Value selected_Card = null;
        Sprite selected_Sprite = null;
        bool found_Valid_Card = false;
        
        int attempts = 0;
        int maxAttempts = 100;

        while (!found_Valid_Card && attempts < maxAttempts)
        {
            int random_Card_Index = Random.Range(0, card_Values.Length);
            selected_Card = card_Values[random_Card_Index];
            selected_Sprite = selected_Card.GetRandomSprite();
            
            if (!used_Card_Sprite.Contains(selected_Sprite))
            {
                found_Valid_Card = true;
            }

            attempts++;
        }

        if (!found_Valid_Card)
        {
            Debug.LogWarning("더 이상 카드 탐색 불가능");
            return null;
        }

        used_Card_Sprite.Add(selected_Sprite);

        GameObject new_Card = Making_Card(spawnPosition, selected_Card, selected_Sprite);
        return new_Card;
    }

    public void Destroy_All_Cards(GameObject card_To_Keep = null)
    {
        for (int i = current_Spawned_Card.Count -1; i >= 0; i--)
        {
            GameObject card = current_Spawned_Card[i];
            if (card != null && card != card_To_Keep)
            {
                Sprite cardSprite = card.GetComponent<SpriteRenderer>().sprite;
                Remove_Used_Sprite(cardSprite);
                Destroy(card);
                current_Spawned_Card.RemoveAt(i);
            }
        }
    }

    public void Remove_From_Spawned_Cards(GameObject card)
    {
        if (current_Spawned_Card.Contains(card))
        {
            current_Spawned_Card.Remove(card);
        }
    }

    public void Add_To_Used_Sprites(Sprite sprite)
    {
        if (used_Card_Sprite.Add(sprite))
        {
            Debug.Log("사용된 스프라이트 추가" + sprite.name);
        }
    }

    public void Remove_Used_Sprite(Sprite sprite)
    {
        if (used_Card_Sprite.Contains(sprite))
        {
            used_Card_Sprite.Remove(sprite);
        }
    }



    // Item
    public void Spawn_Item(Vector2 spawnPos, Dictionary<ItemRarity, float> dropRates, PlayerCharacter_Controller player)
    {
        if (item_Database == null || itemPrefab == null)
        {
            Debug.LogError("Item Database or Item Prefab is not assigned.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("[ERROR] Player 객체가 null입니다. Spawn_Box에서 올바르게 전달되었는지 확인하세요.");
            return;
        }

        if (player.player_Inventory == null)
        {
            Debug.LogError("[ERROR] 플레이어 인벤토리가 null입니다.");
            return;
        }

        List<Item> player_Items = player.player_Inventory;

        Debug.Log($"[DEBUG] 플레이어 인벤토리 아이템 개수: {player_Items.Count}");

        ItemRarity selected_Rarity = Get_Random_Rarity(dropRates);
        Debug.Log($"선택된 아이템 등급 : {selected_Rarity}");

        Item random_Item = Get_Random_Item(selected_Rarity, player_Items);

        if (random_Item == null)
        {
            Debug.LogWarning("[WARN] 선택된 등급에서 아이템을 찾을 수 없음. 전체 아이템에서 다시 검색.");
            random_Item = Get_Random_Item_From_All(player_Items);
        }

        Debug.Log($"드롭된 아이템 : {random_Item.itemName} (등급 : {random_Item.item_Rarity})");

        GameObject itemInstance = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        Item_Prefab itemPrefabScript = itemInstance.GetComponent<Item_Prefab>();
        itemPrefabScript.Initialize(random_Item);

        if (!random_Item.isConsumable)
        {
            spawnedItems.Add(random_Item);
        }
        spawned_Item_Instances.Add(itemInstance);
    }

    public void Spawn_Specific_Item(Vector2 spawn_Pos, Item item_To_Spawn)
    {
        if (item_To_Spawn == null)
        {
            Debug.LogError("[Object_Manager] Spawn Item is null");
            return;
        }

        if (itemPrefab == null)
        {
            Debug.LogError("[Object_Manager] ItemPrefab is null");
        }

        GameObject instance = Instantiate(itemPrefab, spawn_Pos, Quaternion.identity);

        Item_Prefab prefab_Script = instance.GetComponent<Item_Prefab>();
        if (prefab_Script == null)
        {
            Debug.LogError("[Object_Manager] Can't found Item_Prefab component");
            Destroy(instance);
            return;
        }
        prefab_Script.Initialize(item_To_Spawn);

        if (!item_To_Spawn.isConsumable)
            spawnedItems.Add(item_To_Spawn);

        spawned_Item_Instances.Add(instance);
    }

    private ItemRarity Get_Random_Rarity(Dictionary<ItemRarity, float> dropRates)
    {
        float total_Weight = 0f;
        foreach (var rate in dropRates.Values)
        {
            total_Weight += rate;
        }

        float random_Value = Random.Range(0f, total_Weight);
        float cumulative = 0f;

        foreach (var entry in dropRates)
        {
            cumulative += entry.Value;
            if (random_Value <= cumulative)
            {
                return entry.Key;
            }
        }

        return ItemRarity.Common;
    }

    private Item Get_Random_Item(ItemRarity rarity, List<Item> player_Items)
    {
        List<Item> available_Items = item_Database.Get_Items_By_Rarity(rarity)
            .Where(item => !player_Items.Contains(item))
            .ToList();

        //if (available_Items == null)
        //{
        //    Debug.LogError($"[ERROR] {rarity} 등급의 아이템을 가져오는 중 문제가 발생했습니다. 데이터베이스 확인 필요.");
        //    return null;
        //}

        //Debug.Log($"[DEBUG] {rarity} 등급의 아이템 개수: {available_Items.Count}");

        available_Items = available_Items.Where(item => !player_Items.Contains(item)).ToList();

        if (available_Items.Count == 0)
        {
            //Debug.LogWarning($"[WARN] {rarity} 등급에서 플레이어가 소지하지 않은 아이템이 없음. 전체 아이템에서 다시 검색.");
            available_Items = item_Database.Get_All_Items().Where(item => !player_Items.Contains(item)).ToList();
        }

        //Debug.Log($"[DEBUG] 최종 선택 가능한 아이템 개수: {available_Items.Count}");

        return available_Items.Count > 0 ? available_Items[Random.Range(0, available_Items.Count)] : null;
    }

    private Item Get_Random_Item_From_All(List<Item> player_Items)
    {
        List<Item> available_Items = item_Database.Get_All_Items()
            .Where(item => !player_Items.Contains(item))
            .ToList();

        return available_Items.Count > 0 ? available_Items[Random.Range(0, available_Items.Count)] : null;
    }

    public void Reset_Spawned_Items()
    {
        spawnedItems.Clear();
    }

    public void Destroy_All_Cards_And_Items()
    {
        foreach (GameObject card in FindObjectsOfType<GameObject>())
        {
            if (card.CompareTag("Card") && card.activeSelf)
            {
                Sprite cardSprite = card.GetComponent<SpriteRenderer>().sprite;
                Remove_Used_Sprite(cardSprite);
                Destroy(card);
            }
        }

        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            if (item.CompareTag("Item") && item.activeSelf) 
                Destroy(item);
        }
    }
    public void Destroy_All_Items()
    {
        foreach (GameObject item in FindObjectsOfType<GameObject>())
        {
            if (item.CompareTag("Item") && item.activeSelf)
                Destroy(item);
        }
    }

    //By KYH
    public GameObject Spawn_Market_Item(Vector3 spawnPos, Item spawn_item, GameObject obj_Parent)
    {
        if (item_Database == null || itemPrefab == null)
        {
            Debug.LogError("Item Database or Item Prefab is not assigned.");
            return null;
        }

        if (spawn_item == null) return null;

        Debug.Log($"드롭된 아이템 : {spawn_item.itemName}");

        GameObject itemInstance = Instantiate(itemPrefab, spawnPos, Quaternion.identity, obj_Parent.transform);

        itemInstance.tag = "Market_Item";

        Item_Prefab itemPrefabScript = itemInstance.GetComponent<Item_Prefab>();
        itemPrefabScript.Initialize(spawn_item);

        if (!spawn_item.isConsumable)
        {
            spawnedItems.Add(spawn_item);
        }
        spawned_Item_Instances.Add(itemInstance);

        return itemInstance;
    }
}