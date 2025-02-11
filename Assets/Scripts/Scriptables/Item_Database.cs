using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> all_Items = new List<Item>();

    public List<Item> Get_All_Items()
    {
        return new List<Item>(all_Items);
    }

    public List<Item> Get_Items_By_Rarity(ItemRarity rarity)
    {
        return all_Items.Where(item => item.item_Rarity == rarity).ToList();
    }

    public List<Item> Get_Random_Items(int count)
    {
        List<Item> random_Items = new List<Item>();

        if (all_Items.Count == 0 || all_Items == null)
        {
            Debug.LogError("[ItemDatabase] 아이템 리스트가 비어 있습니다!");
            return random_Items;
        }

        Debug.Log($"[ItemDatabase] 요청된 아이템 개수: {count}, 현재 등록된 아이템 개수: {all_Items.Count}");

        for (int i = 0; i < count; i++)
        {
            Item random_Item = all_Items[Random.Range(0, all_Items.Count)];
            if (!random_Items.Contains(random_Item))
            {
                random_Items.Add(random_Item);
            }
        }

        Debug.Log($"[ItemDatabase] 실제 반환된 아이템 개수: {random_Items.Count}");
        return random_Items;
    }
}
