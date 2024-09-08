using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Enemy_Generator e_Generator; //Generator을 상속하려 했다가, 에디터에서 문제가 생겨서 생성시 받는 형태로 변경.

    public int health = 100; // 적 체력 변수


    private void Start()
    {
        e_Generator = FindObjectOfType<Enemy_Generator>();
    }

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
        e_Generator.i_Enemy_Count--; //남은 적의 수 계산용

        Debug.Log("적 사망");
        Destroy(gameObject);
    }
}
