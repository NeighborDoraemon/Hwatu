using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Enemy_Basic : MonoBehaviour
{
    [Header("BB Int")]
    [SerializeField] private IntReference IR_Health;

    [Header("BB Bool")]
    [SerializeField] private BoolReference BR_Facing_Left;


    private GameObject Target_Player;

    public void TakeDamage(int damage)
    {
        IR_Health.Value -= damage;
        Debug.Log("몬스터 데미지 입음 : " + damage);
        if (IR_Health.Value <= 0)
        {
            Die();
        }
    }

    private void Die() // 적 사망 처리 함수
    {
        Enemy_Generator.i_Enemy_Count--; //남은 적의 수 계산용
        Debug.Log(Enemy_Generator.i_Enemy_Count);

        Debug.Log("적 사망");
        Destroy(gameObject);
    }
}
