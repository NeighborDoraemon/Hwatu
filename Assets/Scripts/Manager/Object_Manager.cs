using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void Spawn_Item(string itemName, Vector2 spawnPos, PlayerCharacter_Controller player)
    {
        if (item_Database == null || itemPrefab == null)
        {
            Debug.LogError("Item Database or Item Prefab is not assigned.");
            return;
        }

        Item randomItem = Get_Random_Item();
        if (randomItem == null) return;

        GameObject itemInstance = Instantiate(itemPrefab, spawnPos, Quaternion.identity);
        Item_Prefab itemPrefabScript = itemInstance.GetComponent<Item_Prefab>();
        itemPrefabScript.Initialize(randomItem);

        if (!randomItem.isConsumable)
        {
            spawnedItems.Add(randomItem);
        }
        spawned_Item_Instances.Add(itemInstance);
    }

    private Item Get_Random_Item()
    {
        List<Item> available_Items = new List<Item>(item_Database.Get_All_Items());
        
        available_Items.RemoveAll(item => spawnedItems.Contains(item));

        if (available_Items.Count == 0)
        {
            Debug.LogWarning("모든 아이템 소환 완료");
            return null;
        }        
        return available_Items[Random.Range(0, available_Items.Count)];
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