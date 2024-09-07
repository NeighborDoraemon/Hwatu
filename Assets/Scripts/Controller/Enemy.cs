using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100; // 적 체력 변수

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die() // 적 사망 처리 함수
    {
        Debug.Log("적 사망");
        Destroy(gameObject);
    }
}
