using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "DoubleHandsSword_Attack", menuName = "Weapon/Attack Strategy/DoubleHandsSword")]
public class DoubleHandsSword_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject skill_Projectile_Prefab;
    public float projectile_Speed = 10.0f;

    [Header("Jump Attack Settings")]
    public LayerMask ground_Layer;
    public float fallSpeed = 5.0f;
    public float accel_Delay = 0.3f;
    public float accel_Rate = 5.0f;

    [Header("Ground Proximity Check")]
    public float ground_Check_Distance = 0.08f;
    public float ground_Box_Shrink = 0.98f;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

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
        if (!player.isGrounded)
        {
            if (!player.can_JumpAtk)
                return;
            
            if (Is_Touching_Or_TooClose_ToGround(player))
            {
                player.animator.SetTrigger("Attack");
                player.isAttacking = true;
                return;
            }

            if (!player.can_JumpAtk)
                return;

            player.isAttacking = true;
            player.StartCoroutine(JumpAttack(player));
            return;
        }
        else
        {
            player.animator.SetTrigger("Attack");
            player.isAttacking = true;
        }
    }

    private IEnumerator JumpAttack(PlayerCharacter_Controller player)
    {
        player.jumpCount = player.maxJumpCount;

        player.animator.SetTrigger("Attack");

        float tp_Fall_Speed = fallSpeed;
        float elapsed = 0.0f;

        //player.is_Knock_Back = true;
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

        //player.is_Knock_Back = false;
        player.can_JumpAtk = true;
    }

    private bool Is_Touching_Or_TooClose_ToGround(PlayerCharacter_Controller player)
    {
        var col = player.GetComponent<Collider2D>();
        if (col == null) return false;
        
        Bounds b = col.bounds;

        Vector2 boxSize = new Vector2(b.size.x * ground_Box_Shrink, b.size.y * ground_Box_Shrink);
        RaycastHit2D hit = Physics2D.BoxCast(b.center, boxSize, 0.0f, Vector2.down, ground_Check_Distance, ground_Layer);
        if (hit.collider != null)
            return true;

        Vector2 overlap_Center = new Vector2(b.center.x, b.min.y - 0.0005f);
        Vector2 overlap_Size = new Vector2(b.size.x * ground_Box_Shrink, 0.02f);
        Collider2D overlap = Physics2D.OverlapBox(overlap_Center, overlap_Size, 0.0f, ground_Layer);
        return overlap != null;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point) { }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");
        player.StartCoroutine(Skill_Coroutine(player));

        return true;
    }

    private IEnumerator Skill_Coroutine(PlayerCharacter_Controller player)
    {
        yield return new WaitForSeconds(0.2f);

        Vector3 spawn_Pos = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
        GameObject skill_Projectile = Instantiate(skill_Projectile_Prefab, spawn_Pos, player.transform.rotation);
        Rigidbody2D rb = skill_Projectile.GetComponent<Rigidbody2D>();
        Vector2 shoot_Direction = (player.is_Facing_Right) ? Vector2.right : Vector2.left;
        
        Vector3 projectile_Scale = skill_Projectile.transform.localScale;
        projectile_Scale.x = (player.is_Facing_Right) ? Mathf.Abs(projectile_Scale.x) : -Mathf.Abs(projectile_Scale.x);
        skill_Projectile.transform.localScale = projectile_Scale;

        rb.velocity = shoot_Direction * projectile_Speed;

        Destroy(skill_Projectile, 3.0f);
    }
}
