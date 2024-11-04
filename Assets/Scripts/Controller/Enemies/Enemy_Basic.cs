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
        Debug.Log("���� ������ ���� : " + damage);
        if (IR_Health.Value <= 0)
        {
            Die();
        }
    }

    private void Die() // �� ��� ó�� �Լ�
    {
        Enemy_Generator.i_Enemy_Count--; //���� ���� �� ����
        Debug.Log(Enemy_Generator.i_Enemy_Count);

        Debug.Log("�� ���");
        Destroy(gameObject);
    }
}
