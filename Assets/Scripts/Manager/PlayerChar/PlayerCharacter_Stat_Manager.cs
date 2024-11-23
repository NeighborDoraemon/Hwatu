using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector]
    public Animator animator;

    [Header("Player Character Base Status")]
    public float base_MovementSpeed = 3.5f;
    public float base_jumpPower = 5.0f;
    public int base_Max_Health = 100;

    [Header("Player Character Status")]
    public float movementSpeed = 3.5f;
    public float jumpPower = 5.0f;
    public int max_Health = 100;
    public int health = 100;
    public int attackDamage = 0;
    public int player_Life = 0;

    [Header("Melee Attack")]
    public float attackRange = 0.5f;
    public float comboTime = 0.5f;
    public float attack_Cooldown = 1.0f;
    [HideInInspector]
    public float last_Attack_Time = 0f;
    [HideInInspector]
    public float last_ComboAttack_Time = 0f;
    public bool isAttacking = false;
    public int cur_AttackCount = 0;
    public int max_AttackCount = 0;        

    [Header("Range Attack")]
    public Transform firePoint;    
    
    [Header("Skill")]
    public float skill_Cooldown = 1.0f;
    public float last_Skill_Time = 0f;
    public bool is_Skill_Active = false;

    [Header("Teleport")]
    public float teleporting_Distance = 3.0f; // 순간이동 거리 변수
    public float teleporting_CoolTime = 3.0f; // 순간이동 쿨타임 변수

    [Header("Money")]
    public int i_Money = 0;

    public Weapon_Data cur_Weapon_Data { get; private set; }

    public virtual void Set_Weapon(int weaponIndex)
    {
        Debug.Log($"Weapon Index : Change to {weaponIndex}");

        Weapon_Data new_Weapon = weapon_Manager.Get_Weapon_Data(weaponIndex);

        if (new_Weapon != null)
        {            
            cur_Weapon_Data = new_Weapon;
            //Debug.Log("Weapon Changed");            
        }
    }
}
