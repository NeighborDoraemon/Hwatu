using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Spawner : MonoBehaviour
{
    public static Card_Spawner instance { get; private set; } // �̱���

    public GameObject card_Prefab;
    public Card_Value[] card_Values;

    [SerializeField]
    private HashSet<Sprite> used_Card_Sprite = new HashSet<Sprite>(); // ���� ī���� ��������Ʈ ������ �ؽ���
    public List<GameObject> current_Spawned_Card = new List<GameObject>(); // ���� ���ڿ� ��ȯ�� ī�带 �����ϴ� ����Ʈ

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

    public GameObject Making_Card(Vector2 spawnPosition, Card_Value selected_Card, Sprite selected_Sprite)
    {
        GameObject spawnedCard = Instantiate(card_Prefab, spawnPosition, Quaternion.identity); // �������� �ʵ忡 ��ȯ

        // ��ũ���ͺ� ������Ʈ���� ī�� ���� �����ͼ� ī�忡 ����
        SpriteRenderer spriteRenderer = spawnedCard.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = selected_Sprite; // ���� ��������Ʈ

        Card cardComponent = spawnedCard.GetComponent<Card>();
        cardComponent.cardValue = selected_Card;
        cardComponent.selected_Sprite = selected_Sprite;

        spawnedCard.tag = "Card"; // ��ȯ ������Ʈ �±� ī��� ����

        current_Spawned_Card.Add(spawnedCard); // ���� ��ȯ�� ī�� ����Ʈ�� �߰�
        used_Card_Sprite.Add(selected_Sprite);

        return spawnedCard;
    }

    public void Spawn_Cards(Vector2 spawnPosition)
    {
        Card_Value selected_Card = null;
        Sprite selected_Sprite = null;
        bool found_Valid_Card = false;
        // ���ѷ��� ���� ����
        int attempts = 0;
        int maxAttempts = 100;

        while (!found_Valid_Card && attempts < maxAttempts)
        {
            int random_Card_Index = Random.Range(0, card_Values.Length); // ���� ��ũ���ͺ� ������Ʈ ����
            selected_Card = card_Values[random_Card_Index]; // ���õ� ī�忡 ����
            selected_Sprite = selected_Card.GetRandomSprite(); // ����� ī�忡 ���� ��������Ʈ ���� �� ���õ� ��������Ʈ�� ����

            // ���� ����� ��������Ʈ�� �ƴ� ��� Ż��
            if (!used_Card_Sprite.Contains(selected_Sprite))
            {
                found_Valid_Card = true;
            }

            attempts++;
        }

        if (!found_Valid_Card)
        {
            Debug.LogWarning("�� �̻� ī�� Ž�� �Ұ���");
            return;
        }

        used_Card_Sprite.Add(selected_Sprite); // ���� ��������Ʈ �ؽ��� ����

        Making_Card(spawnPosition, selected_Card, selected_Sprite); // ī�� ���� �Ϸ�
    }

    public void Destroy_All_Cards(GameObject card_To_Keep = null) // ���� ��ȯ�� ī�� ���� �Լ�
    {
        for (int i = current_Spawned_Card.Count -1; i >= 0; i--)
        {
            GameObject card = current_Spawned_Card[i];
            if (card != null && card != card_To_Keep)
            {
                Sprite cardSprite = card.GetComponent<SpriteRenderer>().sprite;
                Remove_Used_Sprite(cardSprite); // �����Ǵ� ī����� ��������Ʈ�� ���� ��������Ʈ �ؽ����� �����Ͽ� �ߺ� ��ȯ ����
                Destroy(card);
                current_Spawned_Card.RemoveAt(i); // ��ȯ�� ī�� ����Ʈ ���� �ڵ�
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

    public void Add_To_Used_Sprites(Sprite sprite) // ���� ��������Ʈ �ؽ��� �����ϴ� �Լ�
    {
        if (used_Card_Sprite.Add(sprite))
        {
            Debug.Log("���� ��������Ʈ �߰�" + sprite.name);
        }
    }

    public void Remove_Used_Sprite(Sprite sprite) // ���� ��������Ʈ �ؽ����� �����ϴ� �Լ�
    {
        if (used_Card_Sprite.Contains(sprite))
        {
            used_Card_Sprite.Remove(sprite);
        }
    }

    // ���� ī�� ����Ʈ �ʱ�ȭ �Լ�
    public void ResetUsedCards()
    {
        used_Card_Sprite.Clear();
    }

    public bool Is_Sprite_Used(Sprite sprite) // ��������Ʈ �˻� �Լ�
    {
        return used_Card_Sprite.Contains(sprite);
    }
}
