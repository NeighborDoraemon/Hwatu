using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector] public Animator animator;

    [Header("플레이어 기본 능력치")]
    public float base_MovementSpeed = 1.0f;     // 플레이어 기본 이동속도
    public float base_JumpPower = 5.0f;         // 플레이어 기본 점프력
    public int base_Max_Health = 100;           // 플레이어 기본 최대 체력

    [Header("플레이어 현재 능력치")]
    public float movementSpeed = 1.0f;          // 이동속도
    public float jumpPower = 5.0f;              // 점프력
    public int max_Health = 100;                // 최대 체력
    public int health = 100;                    // 체력
    public int attackDamage = 0;                // 플레이어 추가 공격력
    public int skill_Damage = 0;                // 추가 스킬 공격력
    public float crit_Rate = 0;                 // 치명타 확률
    public float crit_Dmg = 2;                  // 치명타 배율
    public int player_Life = 0;                 // 플레이어 현재 목숨

    [Header("능력치 증감 및 변화치")]
    public float damage_Mul = 1f;               // 주는 데미지 증감 배율
    public float takenDamage_Mul = 1f;          // 받는 데미지 증감 배율
    public float defend_Attack_Rate = 0f;       // 적의 공격 방어 확률
    public float movementSpeed_Mul = 1f;        // 이동속도 증감 배율
    public float health_Mul = 1f;               // 최대체력 증감 배율
    public int damage_Reduce_Min = 0;           // 데미지 감소 최소치
    public int damage_Reduce_Max = 0;           // 데미지 감소 최대치

    [Header("근접 공격 변수")]
    public float attackRange = 0.5f;
    public float comboTime = 0.5f;
    public float attack_Cooldown = 1.0f;
    [HideInInspector] public float last_Attack_Time = 0f;
    [HideInInspector] public float last_ComboAttack_Time = 0f;
    public bool isAttacking = false;
    public int cur_AttackCount = 0;
    public int max_AttackCount = 0;
    
    [Header("원거리 공격 변수")]
    public Transform firePoint;
    
    [Header("스킬 변수")]
    public float skill_Cooldown = 1.0f;
    public float last_Skill_Time = 0f;
    public bool is_Skill_Active = false;

    [Header("텔레포트")]
    public int max_Teleport_Count = 1;
    public float teleporting_Distance = 3.0f;
    public float teleporting_CoolTime = 3.0f;

    [Header("Money")]
    public int i_Money = 0;

    public Weapon_Data cur_Weapon_Data { get; private set; }

    public virtual void Set_Weapon(int weaponIndex)
    {
        Debug.Log($"무기 인덱스 : {weaponIndex}로 변경 시도");

        Weapon_Data new_Weapon = weapon_Manager.Get_Weapon_Data(weaponIndex);

        if (new_Weapon != null)
        {            
            cur_Weapon_Data = new_Weapon;
            //Debug.Log("무기 변경");            
        }
    }

    public void Increase_Health(int value)
    {
        max_Health += value;
        health += value;
        Debug.Log($"Max Health {value} Increase! And current health {value} heal.");
    }

    public void Increase_MoveSpeed(float value)
    {
        movementSpeed += value;
        Debug.Log($"Move Speed {value} Increased!");
    }

    public void Increase_AttackDamage(int value)
    {
        attackDamage += value;
        Debug.Log($"Attack Damage {value} Increased!");
    }

    public void Increase_CritRate(float value)
    {
        crit_Rate += value;
        Debug.Log($"Crit Rate {value} Increased!");
    }

    public void Increase_CritDamage(float value)
    {
        crit_Dmg += value;
        Debug.Log($"Crit Damage {value} Increased!");
    }
}
