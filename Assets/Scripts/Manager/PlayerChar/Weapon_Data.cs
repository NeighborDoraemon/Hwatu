using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class Weapon_Data : ScriptableObject
{
    public string weapon_Name;
    public int attack_Power;
    public float attackCooldown;
    public int max_Attack_Count;
    public float skillCooldown;
    public string attack_Trigger;
    public string skill_Trigger;
    public AnimatorOverrideController overrideController;
    public ScriptableObject attack_Strategy;
}
