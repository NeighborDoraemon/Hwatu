using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    [Header("Player_Stats")]
    public float movementSpeed = 1.0f; // 이동 속도 조절 변수
    public float jumpPower = 5.0f; // 점프력 조절 변수
    public int health = 100; // 체력
    public int attackDamage = 25; // 공격 데미지
}
