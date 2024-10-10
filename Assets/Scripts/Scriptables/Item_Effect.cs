using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void ApplyEffect(PlayerCharacter_Controller player);   // 아이템 효과 적용
    public abstract void RemoveEffect(PlayerCharacter_Controller player); // 아이템 효과 해제
}
