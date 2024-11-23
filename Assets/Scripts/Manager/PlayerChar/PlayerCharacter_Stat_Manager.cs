using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector]
    public Animator animator;

    [Header("�÷��̾� ĳ���� �ɷ�ġ")]
    public float movementSpeed = 1.0f; // �̵� �ӵ� ���� ����
    public float jumpPower = 5.0f; // ������ ���� ����
    public int max_Health = 100;
    public int health = 100; // ���� ü��
    public int attackDamage = 25; // ���� ������
    public int player_Life = 0;

    [Header("���� ���� ����")]
    public float attackRange = 0.5f; // �÷��̾�� ���� ���� �� �Ÿ� ���� ����
    public float comboTime = 0.5f; // �޺� ���� ���ѽð�
    public float attack_Cooldown = 1.0f; // ���� ��Ÿ�� ����
    [HideInInspector]
    public float last_Attack_Time = 0f; // ������ ���� ���� ��� ����
    [HideInInspector]
    public float last_ComboAttack_Time = 0f; // ������ �޺����� ���� ��� ����
    public bool isAttacking = false; // ���� ���� üũ ����
    public int cur_AttackCount = 0;
    public int max_AttackCount = 0;
    // ���� ��Ʈ ���� ũ�� ����
    [HideInInspector]
    public float hitCollider_x = 0.2f;
    [HideInInspector]
    public float hitCollider_y = 0.4f;

    [Header("���Ÿ� ���� ����")]
    public Transform firePoint;
    public float arrowSpeed = 10.0f; // �Ѿ� �ӵ�
    
    [Header("��ų ����")]
    public float skill_Cooldown = 1.0f;
    public float last_Skill_Time = 0f;
    public bool is_Skill_Active = false;

    [Header("�ڷ���Ʈ")]
    public float teleporting_Distance = 3.0f; // �����̵� �Ÿ� ����
    public float teleporting_CoolTime = 3.0f; // �����̵� ��Ÿ�� ����

    [Header("Money")]
    public int i_Money = 0;

    public Weapon_Data cur_Weapon_Data { get; private set; }

    public virtual void Set_Weapon(int weaponIndex)
    {
        Debug.Log($"���� �ε��� : {weaponIndex}�� ���� �õ�");

        Weapon_Data new_Weapon = weapon_Manager.Get_Weapon_Data(weaponIndex);

        if (new_Weapon != null)
        {            
            cur_Weapon_Data = new_Weapon;
            //Debug.Log("���� ����");            
        }
    }
}
