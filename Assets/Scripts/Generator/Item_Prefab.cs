using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Prefab : MonoBehaviour
{
    public Item itemData;  // ������ ������

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ������ �ʱ�ȭ �޼���
    public void Initialize(Item newItemData)
    {
        itemData = newItemData;
        spriteRenderer.sprite = itemData.icon;  // �������� ��������Ʈ ����
    }
}
