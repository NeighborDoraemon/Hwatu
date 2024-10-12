using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Enemy_Generator e_Generator; //Generator�� ����Ϸ� �ߴٰ�, �����Ϳ��� ������ ���ܼ� ������ �޴� ���·� ����.

    public int health = 100; // �� ü�� ����

    private bool is_Chasing = false;

    private Vector2 Move_Vector = new Vector2(1.0f, 0.0f);

    private Rigidbody2D Enemy_Rigid2D;

    public float Enemy_MoveSpeed = 0.8f;

    private GameObject player_Object;

    [Header("Enemy_Short_Range_Attack")]
    [SerializeField] private float attackRange = 0.8f; // �÷��̾�� ���� ���� �� �Ÿ� ���� ����
    [SerializeField] private float attackCooldown = 1.5f; // ���� ���� ��Ÿ�� ����
    private float last_Attack_Time = 0f; // ������ ���� ���� ��� ����
    private int i_Attack_Damage = 5;

    [Header("Enemy Time set")]
    [SerializeField] private float Spawn_Delay;


    [Header("Ray Cast")] //�÷������� - ���� �÷����϶� �߰�
    [SerializeField] private LayerMask platform_Layer;
    [SerializeField] private float raycastDistance = 3.0f;
    private bool is_Same_Platform = false;


    private bool is_Attacking = false;
    private bool is_Facing_Left = true;

    private bool is_Changed_Direction = false;

    //���� �̵��� ���� ����
    private float f_Move_Time = 0.0f;
    private float f_Wander_Rand = 0.0f;
    private float f_Rest_Rand = 0.0f;

    private bool is_Resting = false;

    private bool is_Dead = false;



    private void Start()
    {
        e_Generator = FindObjectOfType<Enemy_Generator>();

        Enemy_Rigid2D = this.gameObject.GetComponent<Rigidbody2D>();
        player_Object = FindObjectOfType<PlayerCharacter_Controller>().gameObject;

        f_Wander_Rand = Random.Range(2.0f, 15.0f);
        f_Rest_Rand = Random.Range(1.0f, 3.0f);
    }

    private void Update()
    {
        Check_Platform();

        if (Mathf.Abs(player_Object.transform.position.x - this.transform.position.x) <= attackRange && is_Chasing)
        {
            Check_Attack_Range();
        }
        else if (is_Chasing && is_Same_Platform)
        {
            Chasing_Player();
        }
        else
        {
            Wandering();
        }
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
        if(!is_Dead)
        {
            Enemy_Generator.i_Enemy_Count--; //���� ���� �� ����
            //Debug.Log(Enemy_Generator.i_Enemy_Count);

            Debug.Log("�� ���");
            is_Dead = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player_Object = collision.gameObject;

            is_Chasing = true;
            Debug.Log("�÷��̾� ����");

            if (collision.transform.position.x < this.transform.position.x && !is_Facing_Left)//Player is on Enmemis left
            {
                Change_Direction();
            }
            else if (collision.transform.position.x > this.transform.position.x && is_Facing_Left)//Player is on Enemies Right
            {
                Change_Direction();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Walls")
        {
            if (!is_Changed_Direction)
            {
                is_Changed_Direction = true;
                Change_Direction();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            is_Chasing = false;
            Debug.Log("�÷��̾� ��ħ");
        }
        else if (collision.gameObject.tag == "Platform" || collision.gameObject.tag == "OneWayPlatform")
        {
            if (!is_Changed_Direction)
            {
                is_Changed_Direction = true;
                Change_Direction();
            }
        }
    }

    private void Check_Platform()
    {
        // �� �Ʒ��� ����ĳ��Ʈ ���
        RaycastHit2D enemyRay = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, platform_Layer);

        // �÷��̾� �Ʒ��� ����ĳ��Ʈ ���
        RaycastHit2D playerRay = Physics2D.Raycast(player_Object.transform.position, Vector2.down, raycastDistance, platform_Layer);

        // ���� �÷��̾ ���� �÷����� ���� ���
        if (enemyRay.collider != null && playerRay.collider != null && enemyRay.collider == playerRay.collider)
        {
            is_Same_Platform = true;
        }
        else
        {
            is_Same_Platform = false;
        }
    }

    private void Check_Attack_Range()
    {
        if (Time.time > last_Attack_Time + attackCooldown && !is_Attacking)
        {
            //Debug.Log("Melee Attack Call");
            is_Attacking = true;
            //Invoke("Temp", 2.5f);
            //Melee_Attack();
            StartCoroutine(DelayedAttack(2.5f));
        }
        //    Debug.Log("Melee Attack Call");
        //is_Attacking = true;
        ////Invoke("Temp", 2.5f);
        ////Melee_Attack();
        //StartCoroutine(DelayedAttack(2.5f));
    }

    private void Wandering()
    {
        f_Move_Time += Time.deltaTime;

        if (f_Move_Time > f_Wander_Rand)
        {
            is_Resting = true;

            StartCoroutine(TimeRest(f_Rest_Rand));
            //Debug.Log("Rest");

            f_Move_Time = 0.0f;
            f_Wander_Rand = Random.Range(2.0f, 15.0f);
            f_Rest_Rand = Random.Range(1.0f, 3.0f);

            int i_Direction_Change = Random.Range(1, 11);
            if (i_Direction_Change <= 4)
            {
                Change_Direction();
            }
        }

        if (!is_Resting)
        {
            if (Check_Direction())
            {
                Enemy_Rigid2D.velocity = new Vector2(-Enemy_MoveSpeed, 0.0f);
            }
            else
            {
                Enemy_Rigid2D.velocity = new Vector2(Enemy_MoveSpeed, 0.0f);
            }
        }
    }

    private bool Check_Direction()
    {
        return transform.rotation.y == 0.0f;
    }

    private void Change_Direction()
    {
        Vector3 scale = transform.localScale;

        Quaternion quater = transform.rotation;

        if (/*scale.x > Mathf.Epsilon*/quater.y == 0.0f)
        {//Right
            is_Facing_Left = false;
            quater.y = 180.0f;
            //scale.x = -Mathf.Abs(scale.x);
        }
        else
        {//Left
            is_Facing_Left = true;
            quater.y = 0.0f;
            //scale.x = Mathf.Abs(scale.x);
        }
        //scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        //transform.localScale = scale;
        transform.rotation = quater;

        is_Changed_Direction = false;
    }

    private void Chasing_Player()
    {
        if (!is_Attacking && Mathf.Abs(player_Object.transform.position.x - this.transform.position.x) > attackRange)
        {
            if (player_Object.transform.position.x < this.transform.position.x)
            {//�÷��̾ ���� ������ ��ġ�� ���
                if (is_Facing_Left == false)
                {
                    Debug.Log("����������ȯ");
                    Change_Direction();
                }
                Enemy_Rigid2D.velocity = new Vector2(-Enemy_MoveSpeed, 0.0f);
            }
            else
            {
                if (is_Facing_Left != false)
                {
                    Debug.Log("����������ȯ");
                    Change_Direction();
                }
                Enemy_Rigid2D.velocity = new Vector2(Enemy_MoveSpeed, 0.0f);
            }
        }
    }

    private void OnDrawGizmosSelected() // ����׿� ���� ���� �׸���
    {
        Vector2 boxSize = new Vector2(0.2f, 0.4f);
        int direction_Sign = 1;

        if (is_Facing_Left)
        {
            direction_Sign = 1;
        }
        else
        {
            direction_Sign = -1;
        }

        Vector2 boxCenter = (Vector2)transform.position - new Vector2(direction_Sign * (Mathf.Abs(transform.localScale.x) + attackRange), 0f);


        //Vector2 boxCenter = (Vector2)transform.position - new Vector2(transform.localScale.x + attackRange, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }

    private void Melee_Attack()
    {
        //if (Time.time > last_Attack_Time + attackCooldown/* && !is_Attacking*/)
        //{
        //is_Attacking = true;
        //Debug.Log("Attack_Called");
        //Debug.Log(is_Attacking);

        HashSet<PlayerCharacter_Controller> damaged_Plaer_Collider = new HashSet<PlayerCharacter_Controller>();

        Enemy_Rigid2D.velocity = Vector2.zero;

        // ���� ���� �ڽ� �ݶ��̴� ����
        Vector2 boxSize = new Vector2(0.2f, 0.4f);
        Vector2 boxCenter = (Vector2)transform.position + new Vector2(transform.localScale.x * attackRange, 0f);
        LayerMask playerLayer = LayerMask.GetMask("Character");

        Collider2D[] hitPlayer = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0, playerLayer); // �� ���̾� ����

        // ������ ������ ������ ó��
        foreach (Collider2D player in hitPlayer)
        {
            PlayerCharacter_Controller P_Controller = player.GetComponent<PlayerCharacter_Controller>();

            if (player != null && !damaged_Plaer_Collider.Contains(P_Controller))
            {
                Debug.Log("�÷��̾� ���� ����");
                player.GetComponent<PlayerCharacter_Controller>().Player_Take_Damage(i_Attack_Damage);

                damaged_Plaer_Collider.Add(P_Controller);
            }
            //enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }

        last_Attack_Time = Time.time; // ������ ���� �ð� ���� �ð����� ����
        //}
    }

    private IEnumerator TimeRest(float RestTime)
    {
        Enemy_Rigid2D.velocity = Vector2.zero;
        yield return new WaitForSeconds(RestTime);
        is_Resting = false;
        //Debug.Log("Rest End");
    }

    private IEnumerator DelayedAttack(float delayTime)
    {
        is_Attacking = true;
        Debug.Log("Attack Started");

        // ���� ����
        Melee_Attack();

        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delayTime);

        // ���� ���� �� ���� ����
        is_Attacking = false;
        Debug.Log("Attack Finished");
    }
}
