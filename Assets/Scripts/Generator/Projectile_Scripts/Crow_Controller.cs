using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow_Controller : MonoBehaviour
{
    private PlayerCharacter_Controller player;
    private float attack_Range;
    private int attack_Damage;
    private float attack_Cooldown;
    private bool isAttacking = false;
    public bool isProtecting = false;

    public float patrol_Radius = 1.0f;
    public float patrol_Speed = 2.0f;
    public float height_Offset = 1.0f;

    private Vector3 patrol_Target;
    private Vector3 velocity = Vector3.zero;
    private Vector3 pre_Attack_Pos;

    private enum Crow_State { Patrol, Attack, Protect }
    private Crow_State cur_State;

    // 정찰 대기 변수
    private float patrol_Wait_Timer = 0.0f;
    private bool is_Waiting = false;

    // 공격 상태 변수
    private Transform attack_Target;
    private float attack_Timer = 0.0f;
    private float attack_Duration = 0.5f;
    public float attack_Speed = 10.0f;
    private Vector3 attack_End_Pos;
    private bool attacking_Forward = true;
    private bool can_Attack = true;

    // 보호 상태 변수
    private float protect_Duration;
    private float protect_Timer = 0.0f;

    public float teleport_Distance_Threshold = 15.0f;

    public GameObject shield_Effect;
    private SpriteRenderer sprite_Renderer;
    private Animator animator;

    private void Awake()
    {
        sprite_Renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    public void Initialize(PlayerCharacter_Controller player, float attackRange, int attackDamage, float attack_Cooldown)
    {
        this.player = player;
        this.attack_Range = attackRange;
        this.attack_Damage = attackDamage;
        this.attack_Cooldown = attack_Cooldown;

        Set_State(Crow_State.Patrol);
    }

    private void Set_State(Crow_State new_State)
    {
        switch (cur_State)
        {
            case Crow_State.Patrol:
                Exit_Patrol_State();
                break;
            case Crow_State.Attack:
                Exit_Attack_State();
                break;
            case Crow_State.Protect:
                Exit_Protect_State();
                break;
        }

        cur_State = new_State;

        switch (cur_State)
        {
            case Crow_State.Patrol:
                Enter_Patrol_State();
                break;
            case Crow_State.Attack:
                Enter_Attack_State();
                break;
            case Crow_State.Protect:
                Enter_Protect_State();
                break;
        }
    }

    #region Patrol State
    private void Enter_Patrol_State()
    {
        is_Waiting = false;
        patrol_Wait_Timer = 0.0f;
        Choose_New_Patrol_Target();
    }
    
    private void Update_Patrol_State()
    {
        if (player == null || isProtecting)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, patrol_Target, ref velocity, 0.2f, patrol_Speed);

        if (Mathf.Abs(transform.position.x - patrol_Target.x) < 0.1f)
        {
            if (!is_Waiting)
            {
                is_Waiting = true;
                patrol_Wait_Timer = 1.0f;
            }
        }
        if (is_Waiting)
        {
            patrol_Wait_Timer -= Time.deltaTime;
            if (patrol_Wait_Timer <= 0.0f)
            {
                is_Waiting = false;
                Choose_New_Patrol_Target();
            }
        }

        if (!isAttacking && can_Attack)
        {
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attack_Range, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                Transform closet_Enemy = Find_Closet_Enemy(enemies);
                if (closet_Enemy != null)
                {
                    attack_Target = closet_Enemy;
                    can_Attack = false;
                    Set_State(Crow_State.Attack);
                }
            }
        }
    }

    private void Exit_Patrol_State()
    {

    }

    private void Choose_New_Patrol_Target()
    {
        float random_X = Random.Range(-patrol_Radius, patrol_Radius);
        patrol_Target = player.transform.position + new Vector3(random_X, height_Offset, 0);
    }
    #endregion

    #region Attack State
    private void Enter_Attack_State()
    {
        if (attack_Target == null)
        {
            Set_State(Crow_State.Patrol);
            return;
        }

        isAttacking = true;
        attacking_Forward = true;
        pre_Attack_Pos = transform.position;
    }

    private void Update_Attack_State()
    {
        if (attacking_Forward)
        {
            if (attack_Target == null)
            {
                attacking_Forward = false;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, attack_Target.position, attack_Speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, attack_Target.position) < 0.01f)
                {
                    Enemy_Basic enemy_Controller = attack_Target.GetComponent<Enemy_Basic>();
                    if (enemy_Controller != null)
                    {
                        enemy_Controller.TakeDamage(player.Calculate_Damage());
                    }
                    attacking_Forward = false;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, pre_Attack_Pos, attack_Speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, pre_Attack_Pos) < 0.01f)
            {
                Finish_Attack();
            }
        }
    }

    private void Finish_Attack()
    {
        transform.position = pre_Attack_Pos;
        attack_Target = null;
        isAttacking = false;
        Set_State(Crow_State.Patrol);
    }

    private IEnumerator Attack_Cooldown_Coroutine()
    {
        yield return new WaitForSeconds(attack_Cooldown);
        can_Attack = true;
    }

    private void Exit_Attack_State()
    {
        StartCoroutine(Attack_Cooldown_Coroutine());
    }
    #endregion

    #region Protect State
    private void Enter_Protect_State()
    {
        if (shield_Effect != null)
            shield_Effect.SetActive(true);

        protect_Timer = 0.0f;
    }

    private void Update_Protect_State()
    {
        Vector3 disired_Pos = player.transform.position + new Vector3(1.0f * (player.is_Facing_Right ? 1 : -1), -0.5f, 0);
        float move_Speed = 5.0f;
        transform.position = Vector3.Lerp(transform.position, disired_Pos, Time.deltaTime * move_Speed);

        protect_Timer += Time.deltaTime;
        if (protect_Timer >= protect_Duration)
        {
            isProtecting = false;
            Set_State(Crow_State.Patrol);
        }
    }

    private void Exit_Protect_State()
    {
        if (shield_Effect != null)
            shield_Effect.SetActive(false);
    }
    #endregion

    private Transform Find_Closet_Enemy(Collider2D[] enemies)
    {
        Transform closet_Enemy = null;
        float closet_Distance = Mathf.Infinity;

        foreach (Collider2D enemy_Collider in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy_Collider.transform.position);
            if (distance < closet_Distance)
            {
                closet_Distance = distance;
                closet_Enemy = enemy_Collider.transform;
            }
        }

        return closet_Enemy;
    }

    private void Check_And_Teleport_To_Player()
    {
        if (player != null && Vector3.Distance(transform.position, player.transform.position) > teleport_Distance_Threshold)
        {
            Debug.Log("Crow teleporting to player due to distance threshold.");
            transform.position = player.transform.position;
            Set_State(Crow_State.Patrol);
        }
    }

    private void Update()
    {
        Check_And_Teleport_To_Player();

        switch (cur_State)
        {
            case Crow_State.Patrol:
                Update_Patrol_State();
                break;
            case Crow_State.Attack:
                Update_Attack_State();
                break;
            case Crow_State.Protect:
                Update_Protect_State();
                break;
        }

        Update_Sprite_And_Animator();
    }

    private void Update_Sprite_And_Animator()
    {
        animator.SetBool("is_Attacking", cur_State == Crow_State.Attack);
        animator.SetBool("is_Protecting", cur_State == Crow_State.Protect);

        float x_Diff = 0.0f;
        switch (cur_State)
        {
            case Crow_State.Patrol:
                x_Diff = patrol_Target.x - transform.position.x;
                break;
            case Crow_State.Attack:
                if (attack_Target != null) x_Diff = attack_Target.position.x - transform.position.x;
                break;
            case Crow_State.Protect:
                x_Diff = transform.position.x - player.transform.position.x;
                break;
        }

        if (Mathf.Abs(x_Diff) > 0.01f)
        {
            sprite_Renderer.flipX = (x_Diff > 0);
        }
    }

    public void Set_Protecting(bool protecting)
    {
        isProtecting = protecting;
        if (protecting && cur_State != Crow_State.Protect)
        {
            Set_State(Crow_State.Protect);
        }
    }

    public void Protect_Player(float protect_Duration)
    {
        if (isProtecting) return;
        isProtecting = true;
        this.protect_Duration = protect_Duration;
        Set_State(Crow_State.Protect);
    }
}
