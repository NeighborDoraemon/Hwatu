using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Resolver : MonoBehaviour
{
    [SerializeField] private Player_SFX_Profile player_Profile;
    [SerializeField] private List<WeaponType_SFX_Profile> weapon_Type_Profiles;

    private Dictionary<WeaponType, WeaponType_SFX_Profile> type_Map = new();

    private void Awake()
    {
        type_Map.Clear();
        foreach (var p in weapon_Type_Profiles)
            if (p) type_Map[p.type] = p;
    }

    public Sound_Event Resolve(PlayerCharacter_Controller player, SFX_Tag tag)
    {
        if (!player) return null;

        switch (tag)
        {
            case SFX_Tag.Footstep:
                return player_Profile ? player_Profile.footstep : null;
            case SFX_Tag.Hurt:
                return player_Profile ? player_Profile.hurt : null;
            case SFX_Tag.Attack:
                {
                    var weapon_Data = player.cur_Weapon_Data;
                    var weapon_Type = (weapon_Data != null) ? weapon_Data.weapon_Type : WeaponType.None;
                    if (type_Map.TryGetValue(weapon_Type, out var prof) && prof != null)
                        return prof.attack;
                    return null;
                }

            default: return null;
        }
    }
}
