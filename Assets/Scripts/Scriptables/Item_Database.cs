using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;  // 아이템 리스트

    // 모든 아이템 리스트를 반환하는 함수
    public List<Item> Get_All_Items()
    {
        return new List<Item>(items);  // 리스트 복사하여 반환
    }
}
