using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Resolver : MonoBehaviour
{
    [SerializeField] private Player_SFX_Profile player_Profile;
    [SerializeField] private List<WeaponType_SFX_Profile> type_Profiles;

    private Dictionary<WeaponType, WeaponType_SFX_Profile> type_Map;

    private void Awake()
    {
        type_Map = new();
        foreach (var p in type_Profiles) if (p) type_Map[p.type] = p;
    }

    public Sound_Event Resolve(PlayerCharacter_Controller player, SFX_Tag tag)
    {
        var weapon_Data = player.cur_Weapon_Data;
        var type = weapon_Data != null ? weapon_Data.weapon_Type : WeaponType.Blade;

        if (weapon_Data != null)
        {
            if (tag == SFX_Tag.Skill && weapon_Data.skill_SE) return weapon_Data.skill_SE;
        }

        if (type_Map.TryGetValue(type, out var tp) && tp != null)
        {
            if (tag == SFX_Tag.Attack) return tp.attack;
        }

        if (tag == SFX_Tag.Footstep) return player_Profile.footstep;
        if (tag == SFX_Tag.Hurt) return player_Profile.hurt;

        return null;
    }
}
