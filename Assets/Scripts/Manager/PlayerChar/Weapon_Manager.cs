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
        Debug.LogError("Weapon Index is invalid");
        return null;
    }
}
