using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Prefab : MonoBehaviour
{
    public Item itemData;  // 아이템 데이터

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 아이템 초기화 메서드
    public void Initialize(Item newItemData)
    {
        itemData = newItemData;
        spriteRenderer.sprite = itemData.item_Icon;  // 아이템의 스프라이트 설정
    }
}
