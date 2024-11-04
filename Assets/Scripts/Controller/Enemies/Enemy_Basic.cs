using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MBT;

public class Enemy_Basic : MonoBehaviour
{
    [Header("BB Int")]
    [SerializeField] private IntReference IR_Health;

    private GameObject Target_Player;
    private int i_Max_Health;

    public void Start()
    {
        i_Max_Health = IR_Health.Value; //최대체력 저장
    }

    public void TakeDamage(int damage)
    {
        if(damage < 0)
        {
            Debug.Log("Get Heal");
        }

        IR_Health.Value -= damage;

        if(IR_Health.Value > i_Max_Health) //최대체력 초과시 최대체력으로 강제 변경
        {
            IR_Health.Value = i_Max_Health;
        }


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
