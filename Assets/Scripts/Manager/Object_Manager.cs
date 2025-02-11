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
        used_Card_Sprite.Add(selected_Sprite);

        return spawnedCard;
    }

    public void Spawn_Cards(Vector2 spawnPosition)
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
            return;
        }

        used_Card_Sprite.Add(selected_Sprite);

        Making_Card(spawnPosition, selected_Card, selected_Sprite);
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

        ItemRarity selected_Rarity = Get_Random_Rarity(dropRates);
        Debug.Log($"선택된 아이템 등급 : {selected_Rarity}");

        Item random_Item = Get_Random_Item(selected_Rarity);

        if (random_Item == null) return;

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

    private Item Get_Random_Item(ItemRarity rarity)
    {
        List<Item> available_Items = item_Database.Get_Items_By_Rarity(rarity);
        
        if (available_Items.Count == 0)
        {
            Debug.LogWarning("해당 등급 아이템 없음. 다른 등급 아이템 반환");
            available_Items = item_Database.Get_All_Items();
        }        

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
}