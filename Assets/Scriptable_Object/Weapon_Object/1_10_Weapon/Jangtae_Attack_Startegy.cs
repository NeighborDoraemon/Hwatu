using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jangtae_Attack", menuName = "Weapon/Attack Strategy/Jangtae")]
public class Jangtae_Attack_Startegy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject jangtae_Prefab;
    public float roll_Speed = 5.0f;
    public float explosion_Radius = 3.0f;

    private GameObject cur_Jangtae;
    public bool isRiding = false;
    //private bool isRolling = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        isRiding = false;

        Initialize_Weapon_Data();
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Debug.Log("Attack called!");
        player.rb.AddForce(new Vector2(0, player.jumpPower), ForceMode2D.Impulse);

        if (isRiding)
        {
            if (cur_Jangtae == null)
            {
                Debug.Log("Player is riding. Starting to roll Jangtae.");
                isRiding = false;
            }
            else
            {
                Start_Rolling(player);
                return;
            }
        }

        player.StartCoroutine(Mount_Jangtae(player, 0.3f));
    }
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        
    }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Jangtae[] all_Jangtae = FindObjectsOfType<Jangtae>();

        if (all_Jangtae == null || all_Jangtae.Length == 0)
        {
            return;
        }

        foreach (Jangtae jangtae in all_Jangtae)
        {
            if (jangtae == null)
            {
                Debug.LogWarning("Found a null Jangtae object. Skipping");
                continue;
            }

            Collider2D[] enemies = Physics2D.OverlapCircleAll(cur_Jangtae.transform.position, explosion_Radius, LayerMask.GetMask("Enemy"));

            foreach (Collider2D enemy in enemies)
            {
                Enemy_Basic enemyController = enemy.GetComponent<Enemy_Basic>();
                if (enemyController != null)
                {
                    enemyController.TakeDamage(weapon_Data.skill_Damage);
                }
            }

            Destroy(jangtae.gameObject);
        }

        //cur_Jangtae = null;
        //isRiding = false;
    }

    private IEnumerator Mount_Jangtae(PlayerCharacter_Controller player, float delay)
    {
        yield return new WaitForSeconds(delay);

        Debug.Log("Mount_Jangtae called!");

        if (jangtae_Prefab == null)
        {
            Debug.LogError("Jangtae Prefab is not assigned!");
            yield break;
        }

        Vector3 spawn_Pos = player.transform.position + new Vector3(0, -0.5f, 0);
        cur_Jangtae = Instantiate(jangtae_Prefab, spawn_Pos, Quaternion.identity);

        if (cur_Jangtae == null)
        {
            Debug.LogError("Failed to instantiate Jangtae prefab!");
            yield break;
        }

        Debug.Log("Jangtae instantiated successfully.");

        cur_Jangtae.transform.SetParent(player.transform);
        cur_Jangtae.transform.localPosition = new Vector3(0, -0.6f, 0);

        isRiding = true;
    }

    private void Start_Rolling(PlayerCharacter_Controller player)
    {
        if (cur_Jangtae == null) return;

        cur_Jangtae.transform.SetParent(null);

        Rigidbody2D jangtae_Rb = cur_Jangtae.AddComponent<Rigidbody2D>();
        jangtae_Rb.gravityScale = 0;
        jangtae_Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        jangtae_Rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (cur_Jangtae.GetComponent<Jangtae>() == null)
        {
            cur_Jangtae.AddComponent<Jangtae>();
        }

        Vector2 roll_Direction = player.is_Facing_Right ? Vector2.right : Vector2.left;
        jangtae_Rb.velocity = roll_Direction * roll_Speed;

        //isRolling = true;
        isRiding = false;
    }
}
