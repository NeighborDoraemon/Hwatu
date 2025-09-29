using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Data.Common;

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

    private static readonly HashSet<ItemRarity> Allowed_Chest = new() { ItemRarity.Common, ItemRarity.Epic, ItemRarity.Legendary };
    private static readonly HashSet<ItemRarity> Allowed_Purification  = new() { ItemRarity.Purification };
    private static readonly HashSet<ItemRarity> Allowed_Extinction  = new() { ItemRarity.Extinction };

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
        => Spawn_Item_From_Chest(spawnPos, dropRates, player);
    

    public void Spawn_Item_From_Chest(
        Vector2 spawnPos,
        Dictionary<ItemRarity, float> drop_Rates,
        PlayerCharacter_Controller player,
        string force_Item_Name = null)
    {
        if (!Try_Prapare_Spawn(player, out var player_Items)) return;

        Item item = Resolve_Item(
            drop_Rates: drop_Rates,
            allowed_Rarities: Allowed_Chest,
            force_Specific_Rarity: null,
            force_Item_Name: force_Item_Name,
            player_Items: player_Items
            );

        if (item == null)
        {
            Debug.LogError("[Object Manager] There are no droppable items within the allowed rarities. Please check the database.");
            return;
        }

        Spawn_Item_Instance(spawnPos, item);
    }

    public void Spawn_Item_From_MiniBoss_Purification(
        Vector2 spawnPos,
        PlayerCharacter_Controller player)
    {
        if (!Try_Prapare_Spawn(player, out var player_Items)) return;

        Item item = Resolve_Item(
            drop_Rates: null,
            allowed_Rarities: Allowed_Purification,
            force_Specific_Rarity: ItemRarity.Purification,
            force_Item_Name: null,
            player_Items: player_Items
            );

        if (item == null)
        {
            Debug.LogError("[Object Manager] Boss-Rarity items are not in the database.");
            return;
        }

        Spawn_Item_Instance(spawnPos, item);
    }

    public void Spawn_Item_From_MiniBoss_Extinction(
        Vector2 spawnPos,
        PlayerCharacter_Controller player)
    {
        if (!Try_Prapare_Spawn(player, out var player_Items)) return;

        Item item = Resolve_Item(
            drop_Rates: null,
            allowed_Rarities: Allowed_Extinction,
            force_Specific_Rarity: ItemRarity.Extinction,
            force_Item_Name: null,
            player_Items: player_Items
            );

        if (item == null)
        {
            Debug.LogError("[Object Manager] Boss-Rarity items are not in the database.");
            return;
        }

        Spawn_Item_Instance(spawnPos, item);
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

        Spawn_Item_Instance(spawn_Pos, item_To_Spawn);
    }

    private bool Try_Prapare_Spawn(PlayerCharacter_Controller player, out List<Item> player_Items)
    {
        player_Items = null;

        if (item_Database == null || itemPrefab == null)
        {
            Debug.LogError("[Object Manager] Item Database or Item Prefab is not assigned.");
            return false;
        }
        if (player == null)
        {
            Debug.LogError("[Object Manager] Player Object is null.");
            return false;
        }
        if (player.player_Inventory == null)
        {
            Debug.LogError("[Object Manager] Player Inventory is null.");
            return false;
        }

        player_Items = player.player_Inventory;
        Debug.Log($"[Object Manager] Player Inventory's Item Count: {player_Items.Count}");
        return true;
    }

    private Item Resolve_Item(
        Dictionary<ItemRarity, float> drop_Rates,
        HashSet<ItemRarity> allowed_Rarities,
        ItemRarity? force_Specific_Rarity,
        string force_Item_Name,
        List<Item> player_Items)
    {
        ItemRarity selected_Rarity = force_Specific_Rarity.HasValue
            ? force_Specific_Rarity.Value
            : Get_Random_Rarity_Filtered(drop_Rates, allowed_Rarities);

        if (!string.IsNullOrWhiteSpace(force_Item_Name))
        {
            var forced = item_Database.Get_All_Items()
                .FirstOrDefault(i =>
                    string.Equals(i.itemName, force_Item_Name, System.StringComparison.OrdinalIgnoreCase) &&
                    allowed_Rarities.Contains(i.item_Rarity) &&
                    !player_Items.Contains(i));

            if (forced != null)
                return forced;

            Debug.LogWarning($"[Object Manager] '{force_Item_Name}' not found within allowed rarities. Replaced randomly.");
        }

        var chosen = Get_Random_Item_Filtered(selected_Rarity, player_Items, allowed_Rarities);

        if (chosen == null)
        {
            Debug.LogWarning($"[Object Manager] Can't select in {selected_Rarity}. Re-search across all allowed rarities.");
            chosen = Get_Random_Item_From_All_Filtered(player_Items, allowed_Rarities);
        }

        return chosen;
    }

    private void Spawn_Item_Instance(Vector2 spawn_Pos, Item item)
    {
        GameObject instance = Instantiate(itemPrefab, spawn_Pos, Quaternion.identity);

        var prefab_Script = instance.GetComponent<Item_Prefab>();
        if (prefab_Script == null)
        {
            Debug.LogError("[Object Manager] Can't find Item_Prefab component.");
            Destroy(instance);
            return;
        }

        prefab_Script.Initialize(item);

        if (!item.isConsumable)
            spawnedItems.Add(item);

        spawned_Item_Instances.Add(instance);

        Debug.Log($"[Object Manager] Drop {item.itemName} / {item.item_Rarity}");
    }

    private ItemRarity Get_Random_Rarity_Filtered(
        Dictionary<ItemRarity, float> drop_Rates,
        HashSet<ItemRarity> allowed_Rarities)
    {
        if (allowed_Rarities == null || allowed_Rarities.Count == 0)
        {
            Debug.LogError("[Object Manager] allowed_Rarities is empty.");
            return ItemRarity.Common;
        }

        if (drop_Rates == null || drop_Rates.Count == 0)
        {
            return allowed_Rarities.First();
        }

        float total_Weight = 0.0f;
        foreach (var kv in drop_Rates)
        {
            if (allowed_Rarities.Contains(kv.Key) && kv.Value > 0.0f)
                total_Weight += kv.Value;
        }

        if (total_Weight <= 0.0f)
        {
            return allowed_Rarities.First();
        }

        float rand = Random.Range(0.0f, total_Weight);
        float cumulative = 0.0f;

        foreach (var kv in drop_Rates)
        {
            if (!allowed_Rarities.Contains(kv.Key) || kv.Value <= 0.0f) continue;
            cumulative += kv.Value;
            if (rand <= cumulative)
                return kv.Key;
        }

        return allowed_Rarities.First();
    }

    private Item Get_Random_Item_Filtered(
        ItemRarity rarity,
        List<Item> player_Items,
        HashSet<ItemRarity> allowed_Rarities)
    {
        if (!allowed_Rarities.Contains(rarity))
            return null;

        var pool = item_Database.Get_Items_By_Rarity(rarity)
                    .Where(i => !player_Items.Contains(i))
                    .ToList();

        if (pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }

    private Item Get_Random_Item_From_All_Filtered(
        List<Item> player_Items,
        HashSet<ItemRarity> allowed_Rarities)
    {
        var pool = item_Database.Get_All_Items()
                    .Where(i => allowed_Rarities.Contains(i.item_Rarity) && !player_Items.Contains(i))
                    .ToList();

        if (pool.Count == 0) return null;
        return pool[Random.Range(0, pool.Count)];
    }

    private ItemRarity Get_Random_Rarity(Dictionary<ItemRarity, float> dropRates)
        => Get_Random_Rarity_Filtered(dropRates, Allowed_Chest);


    private Item Get_Random_Item(ItemRarity rarity, List<Item> player_Items)
        => Get_Random_Item_Filtered(rarity, player_Items, Allowed_Chest);


    private Item Get_Random_Item_From_All(List<Item> player_Items)
        => Get_Random_Item_From_All_Filtered(player_Items, Allowed_Chest);
    

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