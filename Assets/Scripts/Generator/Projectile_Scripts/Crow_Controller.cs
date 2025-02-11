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
    private Vector3 og_Pos;

    public void Initialize(PlayerCharacter_Controller player, float attackRange, int attackDamage, float attack_Cooldown)
    {
        this.player = player;
        this.attack_Range = attackRange;
        this.attack_Damage = attackDamage;
        this.attack_Cooldown = attack_Cooldown;

        og_Pos = transform.localPosition;

        StartCoroutine(Patrol_Around_Player());
    }

    private IEnumerator Patrol_Around_Player()
    {        
        while (true)
        {
            if (player == null || isProtecting) yield break;

            float randomX = Random.Range(-patrol_Radius, patrol_Radius);
            patrol_Target = new Vector3(randomX, height_Offset, transform.localPosition.z);

            while (Mathf.Abs(transform.localPosition.x - patrol_Target.x) > 0.1f)
            {
                Vector3 targetPosition = new Vector3(patrol_Target.x, height_Offset, transform.localPosition.z);

                transform.localPosition = Vector3.SmoothDamp(
                    transform.localPosition,
                    targetPosition,
                    ref velocity,
                    0.2f,
                    patrol_Speed
                );

                yield return null;

                if (!isAttacking)
                {
                    Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attack_Range, LayerMask.GetMask("Enemy"));
                    if (enemies.Length > 0)
                    {
                        Transform closet_Enemy = Find_Closet_Enemy(enemies);
                        if (closet_Enemy != null)
                        {
                            StartCoroutine(Attack(closet_Enemy));
                            yield break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }
    }

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

    private IEnumerator Attack(Transform enemy)
    {
        if (enemy == null || isAttacking) yield break;

        isAttacking = true;

        Transform og_Parent = transform.parent;
        Vector3 start_Pos = transform.localPosition;
        Vector3 target_Pos = enemy.position;

        transform.SetParent(null);

        float attack_Duration = 0.5f;
        float elapsed_Time = 0.0f;

        while (elapsed_Time < attack_Duration)
        {
            if (enemy == null) break;

            transform.localPosition = Vector3.Lerp(start_Pos, target_Pos, elapsed_Time / attack_Duration);
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        if (enemy != null)
        {
            Enemy_Basic enemy_Controller = enemy.GetComponent<Enemy_Basic>();
            if (enemy_Controller != null)
            {
                enemy_Controller.TakeDamage(attack_Damage);
                Debug.Log($"Enemy {enemy_Controller.name} took {attack_Damage} damage from Crow!");
            }
        }

        elapsed_Time = 0.0f;
        while (elapsed_Time < attack_Duration)
        {
            transform.localPosition = Vector3.Lerp(target_Pos, start_Pos, elapsed_Time / attack_Duration);
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = start_Pos;

        transform.SetParent(og_Parent);

        yield return new WaitForSeconds(attack_Cooldown);
        isAttacking = false;

        StartCoroutine(Patrol_Around_Player());
    }

    public void Set_Protecting(bool protecting)
    {
        isProtecting = protecting;
    }

    public void Protect_Player(float protect_Duration)
    {
        if (isProtecting) return;
        Debug.Log("Starting Protect");

        isProtecting = true;
        Debug.Log("Crow_Controller: Starting Protect_Player");
        StopAllCoroutines();
        StartCoroutine(Protect_Routine(protect_Duration));
    }

    private IEnumerator Protect_Routine(float protect_Duration)
    {
        Vector3 start_Pos = transform.position;

        Vector3 protect_Position = player.transform.position + new Vector3(0.5f * (player.is_Facing_Right ? 1 : -1), -0.5f, 0);
        Debug.Log($"Crow flying to target position: {protect_Position}");

        float move_Duration = 0.5f;
        float elapsed_Time = 0.0f;
        
        while (elapsed_Time < move_Duration)
        {
            transform.position = Vector3.Lerp(start_Pos, protect_Position, elapsed_Time / move_Duration);
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        transform.position = protect_Position;

        elapsed_Time = 0.0f;
        while (elapsed_Time < protect_Duration)
        {
            elapsed_Time += Time.deltaTime;
            yield return null;
        }

        isProtecting = false;

        StartCoroutine(Patrol_Around_Player());
    }
}
