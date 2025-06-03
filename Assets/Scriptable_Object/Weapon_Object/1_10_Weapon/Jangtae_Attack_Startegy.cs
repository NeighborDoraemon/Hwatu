using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jangtae_Attack", menuName = "Weapon/Attack Strategy/Jangtae")]
public class Jangtae_Attack_Startegy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Jangtae Settings")]
    public GameObject jangtae_Prefab;
    public float roll_Speed = 5.0f;
    public bool isRiding = false;
    public GameObject cur_Jangtae;
    private bool is_Mounting_In_Progress = false;

    private CapsuleCollider2D player_Col;
    private Vector2 og_Offset;
    private Vector2 og_Size;
    private bool og_Stored = false;

    [Header("Jangtae Collider Settings")]
    public float mount_Col_Offset;
    public float mount_Col_Height_Inc;

    [Header("Jangtae Skill Settings")]
    public GameObject explosion_Effect_Prefab;
    public float explosion_Radius = 3.0f;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        isRiding = false;

        player_Col = player.GetComponent<CapsuleCollider2D>();
        if (player_Col != null && !og_Stored)
        {
            og_Offset = player_Col.offset;
            og_Size = player_Col.size;
            og_Stored = true;
        }

        Initialize_Weapon_Data();
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Debug.Log("Attack called!");

        if (is_Mounting_In_Progress) return;

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
        else
        {
            if (cur_Jangtae != null)
            {
                Debug.Log("필드 상에 장태 존재하므로 리턴.");
                return;
            }
        }

        player.StartCoroutine(Mount_Jangtae(player, 0.3f));
    }
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {
        
    }
    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (cur_Jangtae == null || isRiding)
        {
            return false;
        }

        if (explosion_Effect_Prefab != null)
        {
            Instantiate(explosion_Effect_Prefab, cur_Jangtae.transform.position, Quaternion.identity);
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

        Destroy(cur_Jangtae);
        cur_Jangtae = null;
        isRiding = false;

        return true;
    }

    private IEnumerator Mount_Jangtae(PlayerCharacter_Controller player, float delay)
    {
        is_Mounting_In_Progress = true;

        player.can_Card_Change = false;
        player.rb.AddForce(new Vector2(0, player.jumpPower), ForceMode2D.Impulse);
        //if (player.isGrounded)
        //{
            
        //}

        yield return new WaitForSeconds(delay);

        if (jangtae_Prefab == null)
        {
            Debug.LogError("Jangtae Prefab is not assigned!");
            is_Mounting_In_Progress = false;
            yield break;
        }

        Vector3 spawn_Pos = player.transform.position + new Vector3(0, -0.5f, 0);
        cur_Jangtae = Instantiate(jangtae_Prefab, spawn_Pos, Quaternion.identity);
        cur_Jangtae.transform.SetParent(player.transform);
        cur_Jangtae.transform.localPosition = new Vector3(0, -0.6f, 0);

        Transform platform_Child = player.transform.Find("Platform_Collider");
        if (platform_Child != null)
        {
            Collider2D old_Collider = platform_Child.GetComponent<Collider2D>();
            if (old_Collider != null)
                old_Collider.enabled = false;
        }

        Collider2D jangtae_Collider = cur_Jangtae.GetComponent<Collider2D>();
        if (jangtae_Collider != null)
        {
            player.player_Platform_Collider = jangtae_Collider;
        }

        isRiding = true;

        if (player_Col != null)
        {
            player_Col.offset = og_Offset + new Vector2(0, mount_Col_Offset);
            player_Col.size = og_Size + new Vector2(0, mount_Col_Height_Inc);
        }

        is_Mounting_In_Progress = false;
    }

    private void Start_Rolling(PlayerCharacter_Controller player)
    {
        if (cur_Jangtae == null) return;

        if (player.isGrounded)
        {
            player.rb.AddForce(new Vector2(0, player.jumpPower), ForceMode2D.Impulse);
        }

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

        Transform platform_Child = player.transform.Find("Platform_Collider");
        if (platform_Child != null)
        {
            Collider2D old_Collider = platform_Child.GetComponent<Collider2D>();
            if (old_Collider != null)
            {
                old_Collider.enabled = true;
                player.player_Platform_Collider = old_Collider;
            }
        }

        if (player_Col != null)
        {
            player_Col.offset = og_Offset;
            player_Col.size = og_Size;
        }

        isRiding = false;
        player.can_Card_Change = true;
    }
}
