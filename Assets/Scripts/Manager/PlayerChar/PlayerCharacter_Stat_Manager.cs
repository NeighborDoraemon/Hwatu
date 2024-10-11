using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter_Stat_Manager : MonoBehaviour
{
    public IAttack_Strategy attack_Strategy;
    public Weapon_Manager weapon_Manager;
    [HideInInspector]
    public Animator animator;
    private bool comboPressed = false;

    [Header("�÷��̾� ĳ���� �ɷ�ġ")]
    public float movementSpeed = 1.0f; // �̵� �ӵ� ���� ����
    public float jumpPower = 5.0f; // ������ ���� ����
    public int max_Health = 100;
    public int health = 100; // ���� ü��
    public int attackDamage = 25; // ���� ������

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

    [Header("�ڷ���Ʈ")]
    public float teleporting_Distance = 3.0f; // �����̵� �Ÿ� ����
    public float teleporting_CoolTime = 3.0f; // �����̵� ��Ÿ�� ����

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

    public void Set_Weapon(int weaponIndex)
    {
        PlayerCharacter_Controller player = GetComponent<PlayerCharacter_Controller>();

        Weapon_Data newWeapon = weapon_Manager.Get_Weapon_Data(weaponIndex);
        if (newWeapon != null)
        {
            attack_Strategy = new Weapon_Attack_Strategy(player, newWeapon);
            cur_Weapon_Data = newWeapon;
            
            isAttacking = false;
            cur_AttackCount = 0;
        }
    }

    // 1�� ��ų ���� �κ� ------------------------------------------------------------------------------------------------------------
    public float reduced_Attack_Cooldown;
    private bool isCooldown_Reduced = false;
    public void One_DD_Skill()
    {
        if (!isCooldown_Reduced)
        {
            reduced_Attack_Cooldown = attack_Cooldown * 0.5f;
            attack_Cooldown = reduced_Attack_Cooldown;

            isCooldown_Reduced = true;

            StartCoroutine(Restore_Cooldown_After_Delay());
        }
    }

    private IEnumerator Restore_Cooldown_After_Delay()
    {
        yield return new WaitForSeconds(3f);

        attack_Cooldown = reduced_Attack_Cooldown / 0.5f;
        isCooldown_Reduced = false;
    }
    //--------------------------------------------------------------------------------------------------------------------------------
}
