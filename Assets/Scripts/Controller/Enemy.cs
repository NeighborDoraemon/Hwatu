using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Enemy_Generator e_Generator; //Generator�� ����Ϸ� �ߴٰ�, �����Ϳ��� ������ ���ܼ� ������ �޴� ���·� ����.

    public int health = 100; // �� ü�� ����


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

    void Die() // �� ��� ó�� �Լ�
    {
        e_Generator.i_Enemy_Count--; //���� ���� �� ����

        Debug.Log("�� ���");
        Destroy(gameObject);
    }
}
