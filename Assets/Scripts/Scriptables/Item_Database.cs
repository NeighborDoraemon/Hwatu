using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public List<Item> items;  // ������ ����Ʈ

    // ��� ������ ����Ʈ�� ��ȯ�ϴ� �Լ�
    public List<Item> Get_All_Items()
    {
        return new List<Item>(items);  // ����Ʈ �����Ͽ� ��ȯ
    }
}
