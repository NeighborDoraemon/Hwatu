using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Shield_Attack", menuName = "Weapon/Attack Strategy/Shield")]
public class Shield_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Parry Setting")]
    public bool isParrying;
    public float parry_Duration = 0.5f;
    public bool reset_Skill_OnParry = true;

    [Header("Shield Jump Attack Settings")]
    public LayerMask ground_Layer;
    public float fallSpeed = 1.0f;
    public float accel_Delay = 0.1f;
    public float accel_Rate = 20.0f;

    [Header("Shield Skill Settings")]
    public GameObject shield_Pj_Prefab;
    public float projectile_Speed = 15.0f;
    public float return_Speed = 20.0f;
    public float max_Distance = 8.0f;

    private bool shield_Thrown = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        shield_Thrown = false;

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
        if (shield_Thrown) return;

        if (player.isGrounded)
        {
            if (isParrying) return;
            Activate_Parry(player);
        }
        else if (!player.isGrounded)
        {
            player.animator.SetTrigger("Attack");
            player.StartCoroutine(Jump_Attack(player));
            player.StartCoroutine(Shield_Layer_Change());
        }
    }

    private IEnumerator Jump_Attack(PlayerCharacter_Controller player)
    {
        //player.animator.SetTrigger("Attack");
        player.jumpCount = player.maxJumpCount;

        float tp_Fall_Speed = fallSpeed;

        float elapsed = 0.0f;

        player.is_Knock_Back = true;
        while (!player.isGrounded)
        {
            elapsed += Time.deltaTime;

            if (elapsed > accel_Delay)
            {
                tp_Fall_Speed += accel_Rate * Time.deltaTime;
            }

            player.rb.velocity = new Vector2(player.rb.velocity.x, -tp_Fall_Speed);
            yield return null;
        }

        player.is_Knock_Back = false;
    }

    private IEnumerator Shield_Layer_Change()
    {
        player.weapon_Animator.SetBool("Is_Attacking", true);

        yield return new WaitForSeconds(0.5f);

        player.weapon_Animator.SetBool("Is_Attacking", false);
    }

    private void Activate_Parry(PlayerCharacter_Controller player)
    {
        isParrying = true;
        player.weapon_Animator.SetBool("Is_Attacking", true);
        player.StartCoroutine(Parry_Window(player));
    }

    private IEnumerator Parry_Window(PlayerCharacter_Controller player)
    {
        yield return new WaitForSeconds(parry_Duration);

        player.weapon_Animator.SetBool("Is_Attacking", false);
        isParrying = false;
    }

    public void Detect_EnemyAttack(PlayerCharacter_Controller player)
    {
        if (!isParrying) return;

        Debug.Log("Parry successful!");
        Debug.Log($"Cur player health : {player.health}");

        OnParry_Success(player);
    }

    public void Shield_Set_False()
    {
        player.weapon_Animator.SetBool("Is_Attacking", false);
        Debug.Log("Animation Event Called!");
    }

    private void OnParry_Success(PlayerCharacter_Controller player)
    {
        player.animator.SetTrigger("Skill");

        if (reset_Skill_OnParry && player != null)
        {
            player.Reset_Skill_Cooldown();
        }

        isParrying = false;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point) { }
    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        Debug.Log("[Shield_Strategy] Skill called");

        if (shield_Thrown)
        {
            Debug.Log("[Shield_Startegy] 이미 던져짐, 돌아오는 중");
            return false;
        }
        shield_Thrown = true;

        Vector3 spawn_Pos = player.weapon_Anchor.position;
        GameObject proj = Instantiate(shield_Pj_Prefab, spawn_Pos, shield_Pj_Prefab.transform.rotation);
        var shield_Proj = proj.GetComponent<Shield_Projectile>();

        Vector2 dir = player.is_Facing_Right ? Vector2.right : Vector2.left;

        shield_Proj.Initialize(
            dir,
            projectile_Speed,
            return_Speed,
            max_Distance,
            player.Calculate_Skill_Damage(),
            this
            );

        //player.animator.SetTrigger("Skill");

        return true;
    }

    public void On_Shield_Returned()
    {
        Debug.Log("[Shield_Strategy] On_Shield_Returned 호출됨");
        shield_Thrown = false;
    }
}