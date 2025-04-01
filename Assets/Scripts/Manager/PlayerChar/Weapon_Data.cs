using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObjects/WeaponData")]
public class Weapon_Data : ScriptableObject
{
    public string weapon_Name;                                                  // 무기 이름
    public string comb_Name;                                                    // 카드 조합의 이름
    public int attack_Damage;                                                   // 무기 데미지
    public float attack_Cooldown;                                               // 공격 간 딜레이 타임
    public int max_Attack_Count;                                                // 최대 공격 가능 횟수
    public int skill_Damage;                                                    // 스킬 데미지
    public float skill_Cooldown;                                                // 스킬 쿨타임
    public GameObject weapon_Prefab;                                            // 무기 오브젝트 프리팹
    public AnimatorOverrideController overrideController;                       // 무기에 맞는 플레이어의 애니메이션 컨트롤러
    public AnimatorOverrideController weapon_overrideController;                // 무기의 애니메이션 컨트롤러
    public List<Weapon_Animation_Position_Data> animation_Pos_Data_List;        // 플레이어의 무기 앵커 위치값 리스트
    public Weapon_Effect_Data effect_Data;                                      // 무기 이펙트 데이터
    public ScriptableObject attack_Strategy;                                    // 공격 방식 스크립터블 오브젝트
    public bool is_HoldAttack_Enabled = false;                                  // 무기의 홀딩 조작 여부
}
