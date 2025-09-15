using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon_Effect_Provider
{
    Weapon_Effect_Data Get_Normal_Attack_EffectData();
    Weapon_Effect_Data Get_Normal_Attack_EffectData_WhileSkill();
}
