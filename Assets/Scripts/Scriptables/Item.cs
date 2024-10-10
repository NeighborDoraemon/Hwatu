using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;        // ������ �̸�
    public Sprite icon;            // ������ ������ (��������Ʈ)
    public ItemEffect itemEffect;  // ������ ȿ�� (��ũ���ͺ� ������Ʈ)
    public bool isConsumable;      // �Ҹ� ������ ���� 

    // �������� ȿ���� �����ϴ� �Լ�
    public void ApplyEffect(PlayerCharacter_Controller player)
    {
        itemEffect.ApplyEffect(player);
    }
}

