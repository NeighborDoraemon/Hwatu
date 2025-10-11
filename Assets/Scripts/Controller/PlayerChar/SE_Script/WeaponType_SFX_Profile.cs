using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/WeaponType_SFX_Profile")]
public class WeaponType_SFX_Profile : ScriptableObject
{
    public WeaponType type;
    public Sound_Event attack;
}
