using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Manager : MonoBehaviour
{
    [SerializeField]
    private List<Weapon_Data> weaponList;

    public Weapon_Data Get_Weapon_Data(int index)
    {
        if (index >= 0 && index < weaponList.Count)
        {
            return weaponList[index];
        }
        Debug.LogError("무기 인덱스가 유효하지 않음");
        return null;
    }
}
