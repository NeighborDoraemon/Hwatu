using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector] public Animator animator;

    [Header("플레이어 기본 능력치")]
    public float base_MovementSpeed = 1.0f;       // 플레이어 기본 이동속도
    public float base_JumpPower = 5.0f;           // 플레이어 기본 점프력
    public int base_Max_Health = 100;             // 플레이어 기본 최대 체력

    [Header("플레이어 현재 능력치")]
    public float movementSpeed = 1.0f;            // 이동속도
    public float jumpPower = 5.0f;                // 점프력
    public int max_Health = 100;                  // 최대 체력
    public int health = 100;                      // 체력
    public int attackDamage = 0;                  // 플레이어 추가 공격력
    public int skill_Damage = 0;                  // 추가 스킬 공격력
    public float crit_Rate = 0;                   // 치명타 확률
    public float crit_Dmg = 2;                    // 치명타 배율
    public int player_Life = 0;                   // 플레이어 현재 목숨

    [Header("능력치 증감 및 변화치")]
    public float damage_Mul = 1.0f;               // 주는 데미지 증감 배율
    public float takenDamage_Mul = 1.0f;          // 받는 데미지 증감 배율
    public float defend_Attack_Rate = 0.0f;       // 적의 공격 방어 확률
    public float movementSpeed_Mul = 1.0f;        // 이동속도 증감 배율
    //public float health_Mul = 1.0f;             // 최대체력 증감 배율
    public float attack_Cooltime_Mul = 1.0f;      // 공격속도 쿨타임 증감 배율
    public float skill_Cooltime_Mul = 1.0f;       // 스킬 쿨타임 증감 배율
    public int damage_Reduce_Min = 0;             // 데미지 감소 최소치
    public int damage_Reduce_Max = 0;             // 데미지 감소 최대치
    public float heal_Amount_Mul = 1.0f;          // 회복량 증감 배율
    public float money_Earned_Mul = 1.0f;         // 돈 획득량 배율
    public float teleport_Cooltime_Mul = 1.0f;    // 텔레포트 쿨타임 증감 배율
    public float bleeding_Rate = 0.0f;            // 출혈 확률
    public int bleed_Damage = 2;                  // 출혈 데미지
    public int bleed_Count = 5;                   // 출혈 카운트
    public float bleed_Delay = 1.2f;              // 출혈 적용 딜레이
    public float stun_Rate = 0.0f;                // 기절 확률

    [Header("공격 관련 변수")]
    public float comboTime = 0.5f;
    public float attack_Cooldown = 1.0f;
    [HideInInspector] public float last_Attack_Time = 0f;
    [HideInInspector] public float last_ComboAttack_Time = 0f;
    public bool isAttacking = false;
    public int max_AttackCount = 0;
    public int es_Stack = 0;
    public bool has_Es_Extra_Life = false;

    [Header("스탯 강화 관련 변수")]
    public float inhance_Skillcooltime_Value = 0.1f;
    public float teleport_Invicible_Time = 0.1f;

    [HideInInspector] public int cur_AttackInc_Phase = 1;
    [HideInInspector] public int cur_HealthInc_Phase = 1;
    [HideInInspector] public int cur_AttackCoolTimeInc_Phase = 1;
    [HideInInspector] public int cur_MoveSpeedInc_Phase = 1;

    [HideInInspector] public int cur_Inc_AttackDamage = 0;
    [HideInInspector] public int cur_Inc_Health = 0;
    [HideInInspector] public int cur_Inc_DamageReduction = 0;
    [HideInInspector] public float cur_Dec_AttackCoolTime = 0;
    [HideInInspector] public float cur_Inc_MoveSpeed = 0;

    [HideInInspector] public bool dmg_Inc_To_Lost_Health = false;
    [HideInInspector] public bool card_Match_Dmg_Inc = false;
    [HideInInspector] public bool skill_Cooltime_Has_Dec = false;
    [HideInInspector] public bool money_Earned_Has_Inc = false;
    [HideInInspector] public bool invicible_Teleport = false; 
    
    [Header("스킬 변수")]
    public float skill_Cooldown = 1.0f;
    [HideInInspector]public float last_Skill_Time = 0f;
    public bool is_Skill_Active = false;

    [Header("텔레포트")]
    public int max_Teleport_Count = 1;
    public float teleporting_Distance = 3.0f;
    public float teleporting_CoolTime = 3.0f;

    [Header("Money")]
    public int i_Money = 0;
    public int i_Token = 0;

    [Header("UI_Text")]
    public TextMeshProUGUI money_Text;
    public TextMeshProUGUI token_Text;

    public Weapon_Data cur_Weapon_Data { get; private set; }

    public virtual void Set_Weapon(int weaponIndex)
    {
        Debug.Log($"무기 인덱스 : {weaponIndex}로 변경 시도");

        Weapon_Data new_Weapon = weapon_Manager.Get_Weapon_Data(weaponIndex);

        if (new_Weapon != null)
        {            
            cur_Weapon_Data = new_Weapon;
        }

        //comb_Text.text = new_Weapon.comb_Name;
    }

    public void Increase_AttackDamage()
    {
        if (cur_AttackInc_Phase == 1)
        {
            if (cur_Inc_AttackDamage < 10)
            {
                cur_Inc_AttackDamage++;
                attackDamage += 1;
                Debug.Log("Attack Damage Enhanced (Phase 1) : Current Attack Damage " + attackDamage);
                return;
            }
        }

        if (cur_AttackInc_Phase == 1 && cur_Inc_AttackDamage >= 10)
        {
            cur_AttackInc_Phase = 2;
            Debug.Log("Phase 1 Completed. Phase 2 Start.");
        }

        if (cur_AttackInc_Phase == 2)
        {
            if (!dmg_Inc_To_Lost_Health)
            {
                dmg_Inc_To_Lost_Health = true;
                return;
            }
        }

        if (cur_AttackInc_Phase == 2 && dmg_Inc_To_Lost_Health)
        {
            cur_AttackInc_Phase = 3;

            card_Match_Dmg_Inc = true;
            return;
        }
    }

    public void Increase_Health()
    {
        if (cur_HealthInc_Phase == 1)
        {
            if (cur_Inc_Health < 50)
            {
                cur_Inc_Health += 5;
                max_Health += 5;
                health += 5;
                Debug.Log("Health Enhanced (Phase 1) : Current Max Health " + max_Health);
                return;
            }
        }

        if (cur_HealthInc_Phase == 1 && cur_Inc_Health >= 50)
        {
            cur_HealthInc_Phase = 2;
            Debug.Log("Phase 1 Completed. Phase 2 Start.");
        }

        if (cur_HealthInc_Phase == 2)
        {
            if (cur_Inc_DamageReduction < 3)
            {
                cur_Inc_DamageReduction += 1;
                damage_Reduce_Min += 1;
                damage_Reduce_Max += 1;
                Debug.Log("Health Enhanced (Phase 2) : Current DamageReduction " + damage_Reduce_Min + "," + damage_Reduce_Max);
                return;
            }
        }

        if (cur_HealthInc_Phase == 2 && cur_Inc_DamageReduction >= 3)
        {
            cur_HealthInc_Phase = 3;
            Debug.Log("Phase 2 Completed. Phase 3 Start.");

            heal_Amount_Mul += 0.2f;
            heal_Amount_Mul = Mathf.Round(heal_Amount_Mul * 100f) / 100f;

            return;
        }
    }

    public void Increase_AttackCoolTime()
    {
        if (cur_AttackCoolTimeInc_Phase == 1)
        {
            if (cur_Dec_AttackCoolTime < 1.0f)
            {
                cur_Dec_AttackCoolTime += 0.1f;
                attack_Cooltime_Mul -= 0.01f;
                attack_Cooltime_Mul = Mathf.Round(attack_Cooltime_Mul * 100f) / 100f;
                Debug.Log("Attack CoolTime Enhanced (Phase 1) : Current Attack CoolTime " + attack_Cooldown);
                return;
            }
        }

        if (cur_AttackCoolTimeInc_Phase == 1 && cur_Dec_AttackCoolTime >= 1.0f)
        {
            cur_AttackCoolTimeInc_Phase = 2;
            Debug.Log("Phase 1 Completed. Phase 2 Start.");
        }

        if (cur_AttackCoolTimeInc_Phase == 2)
        {
            if (!skill_Cooltime_Has_Dec)
            {
                skill_Cooltime_Mul -= inhance_Skillcooltime_Value;
                skill_Cooltime_Has_Dec = true;
                return;
            }
        }
        
        if (cur_AttackCoolTimeInc_Phase == 2 && skill_Cooltime_Has_Dec)
        {
            cur_AttackCoolTimeInc_Phase = 3;

            max_Teleport_Count++;
            return;
        }
    }

    public void Increase_MoveSpeed()
    {
        if (cur_MoveSpeedInc_Phase == 1)
        {
            if (cur_Inc_MoveSpeed < 1.0f)
            {
                cur_Inc_MoveSpeed += 0.1f;
                movementSpeed += 0.1f;

                movementSpeed = (float)Math.Round(movementSpeed, 1);
                cur_Inc_MoveSpeed = (float)Math.Round(cur_Inc_MoveSpeed, 1);
                Debug.Log("Move Speed Enhanced (Phase 1) : Current Move Speed " + movementSpeed);
                return;
            }
        }

        if (cur_MoveSpeedInc_Phase == 1 && cur_Inc_MoveSpeed >= 1.0f)
        {
            cur_MoveSpeedInc_Phase = 2;
            Debug.Log("Phase 1 Completed. Phase 2 Start.");
        }

        if (cur_MoveSpeedInc_Phase == 2)
        {
            if (!money_Earned_Has_Inc)
            {
                money_Earned_Mul += 0.2f;
                money_Earned_Has_Inc = true;
                return;
            }
            
        }
        
        if (cur_MoveSpeedInc_Phase == 2 && money_Earned_Has_Inc)
        {
            cur_MoveSpeedInc_Phase = 3;
            invicible_Teleport = true;
            return;
        }
    }
}
