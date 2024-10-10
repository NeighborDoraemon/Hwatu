using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Item> player_Inventory = new List<Item>();

    public void AddItem(Item newItem)
    {
        player_Inventory.Add(newItem);  // ������ �߰�
        Debug.Log($"{newItem.itemName} �κ��丮�� �߰���.");

        // ȿ�� ����
        newItem.ApplyEffect(this.GetComponent<PlayerCharacter_Controller>());
    }

    // ������ ȹ�� �� �÷��̾ ��ȣ�ۿ��� �������� �����ϴ� �Լ�
    public void RemoveItem(Item item)
    {
        player_Inventory.Remove(item);  // �κ��丮���� ������ ����
        Debug.Log($"{item.itemName} �κ��丮���� ���ŵ�.");
    }
}
