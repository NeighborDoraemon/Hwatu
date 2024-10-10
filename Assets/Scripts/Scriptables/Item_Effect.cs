using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void ApplyEffect(PlayerCharacter_Controller player);   // ������ ȿ�� ����
    public abstract void RemoveEffect(PlayerCharacter_Controller player); // ������ ȿ�� ����
}
