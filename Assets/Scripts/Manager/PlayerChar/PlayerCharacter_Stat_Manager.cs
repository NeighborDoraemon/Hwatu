using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector]
    public Animator animator;

    [Header("플레이어 캐릭터 능력치")]
    public float movementSpeed = 1.0f; // 이동 속도 조절 변수
    public float jumpPower = 5.0f; // 점프력 조절 변수
    public int max_Health = 100;
    public int health = 100; // 현재 체력
    public int attackDamage = 25; // 공격 데미지

    [Header("근접 공격 변수")]
    public float attackRange = 0.5f; // 플레이어와 공격 범위 간 거리 설정 변수
    public float comboTime = 0.5f; // 콤보 공격 제한시간
    public float attack_Cooldown = 1.0f; // 공격 쿨타임 변수
    [HideInInspector]
    public float last_Attack_Time = 0f; // 마지막 공격 시점 기록 변수
    [HideInInspector]
    public float last_ComboAttack_Time = 0f; // 마지막 콤보어택 시점 기록 변수
    public bool isAttacking = false; // 공격 상태 체크 변수
    public int cur_AttackCount = 0;
    public int max_AttackCount = 0;
    // 공격 히트 범위 크기 변수
    [HideInInspector]
    public float hitCollider_x = 0.2f;
    [HideInInspector]
    public float hitCollider_y = 0.4f;

    [Header("원거리 공격 변수")]
    public Transform firePoint;
    public float arrowSpeed = 10.0f; // 총알 속도
    
    [Header("스킬 변수")]
    public float skill_Cooldown = 1.0f;
    public float last_Skill_Time = 0f;
    public bool is_Skill_Active = false;

    [Header("텔레포트")]
    public float teleporting_Distance = 3.0f; // 순간이동 거리 변수
    public float teleporting_CoolTime = 3.0f; // 순간이동 쿨타임 변수

    public Weapon_Data cur_Weapon_Data { get; private set; }

    public void Change_Attack_Strategy(IAttack_Strategy new_Strategy)
    {
        attack_Strategy = new_Strategy;
    }
    public void ShootPrefab(GameObject prefab)
    {
        GameObject projectile = MonoBehaviour.Instantiate(prefab, firePoint.position, firePoint.rotation);

        Rigidbody2D projectile_Rb = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = (transform.localScale.x < 0) ? Vector2.left : Vector2.right;
        projectile_Rb.velocity = shootDirection * arrowSpeed;        
    }

    public virtual void Set_Weapon(int weaponIndex)
    {
        Debug.Log($"무기 인덱스 : {weaponIndex}로 변경 시도");

        Weapon_Data new_Weapon = weapon_Manager.Get_Weapon_Data(weaponIndex);

        if (new_Weapon != null)
        {            
            cur_Weapon_Data = new_Weapon;
            attackDamage = new_Weapon.attack_Damage;
            attack_Cooldown = new_Weapon.attack_Cooldown;
            max_AttackCount = new_Weapon.max_Attack_Count;

            //Debug.Log("무기 변경");            
        }

    }
}
