using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class Weapon_Data : ScriptableObject
{
    public string weapon_Name;
    public int attack_Damage;
    public float attack_Cooldown;
    public int max_Attack_Count;
    public int skill_Damage;
    public float skill_Cooldown;
    public string attack_Trigger;
    public string skill_Trigger;
    public GameObject weapon_Prefab;    
    public AnimatorOverrideController overrideController;    
    public List<Weapon_Animation_Position_Data> animation_Pos_Data_List;
    public Weapon_Effect_Data effect_Data;
    public ScriptableObject attack_Strategy;
}
