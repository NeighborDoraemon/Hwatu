using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar_Inventory_Manager : PlayerCharacter_Card_Manager
{
    public List<Item> player_Inventory = new List<Item>();

    public void AddItem(Item newItem)
    {
        if (!newItem.isConsumable)
        {
            player_Inventory.Add(newItem);  // ������ �߰�
            PlayerCharacter_Controller player = this.GetComponent<PlayerCharacter_Controller>();
            if (player != null) 
            {
                newItem.ApplyEffect(this.GetComponent<PlayerCharacter_Controller>());
            }
        }
    }

    // ������ ȹ�� �� �÷��̾ ��ȣ�ۿ��� �������� �����ϴ� �Լ�
    public void RemoveItem(Item item)
    {
        player_Inventory.Remove(item);  // �κ��丮���� ������ ����
        //Debug.Log($"{item.itemName} �κ��丮���� ���ŵ�.");
    }
}
