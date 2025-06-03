using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pyeongon_Attack", menuName = "Weapon/Attack Strategy/Pyeongon")]
public class Pyeongon_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public GameObject cloud_Prefab;
    
    public float duration = 1.0f;
    public float move_Speed = 1.0f;
    private float original_Y_Value = 0;

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
        player.isAttacking = true;        
    }
    
    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public bool Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        player.animator.SetTrigger("Skill");
        player.StartCoroutine(Air_Move(player, duration, move_Speed));

        return true;
    }

    private IEnumerator Air_Move(PlayerCharacter_Controller player, float duration, float move_Speed)
    {
        GameObject cloud = Create_Cloud(player);

        Lock_Y_Position(player);
        player.is_Knock_Back = true;

        Vector2 original_Velocity = player.rb.velocity;
        player.rb.velocity = Vector2.zero;

        float diretion = player.is_Facing_Right ? 1 : -1;
        float elapsed_Time = 0.0f;

        while(elapsed_Time < duration)
        {
            Vector2 target_Position = player.rb.position + new Vector2(move_Speed * diretion * Time.fixedDeltaTime, 0);
            player.rb.MovePosition(target_Position);

            elapsed_Time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        player.rb.velocity = original_Velocity;

        Unlock_Y_Position(player);
        player.is_Knock_Back = false;

        if (cloud != null)
        {
            Destroy(cloud);
        }
    }

    private GameObject Create_Cloud(PlayerCharacter_Controller player)
    {
        if (cloud_Prefab == null)
        {
            Debug.LogError("Cloud Prefab is not assigned!");
            return null;
        }

        GameObject cloud = Instantiate(cloud_Prefab, player.transform.position, Quaternion.identity);
        cloud.transform.SetParent(player.transform);
        cloud.transform.localPosition = new Vector3(0.02f, -0.48f, 0);

        Vector3 cloud_Scale = cloud.transform.localScale;
        cloud_Scale.x = player.is_Facing_Right ? -Mathf.Abs(cloud_Scale.x) : Mathf.Abs(cloud_Scale.x);
        cloud.transform.localScale = cloud_Scale;

        return cloud;
    }

    private void Lock_Y_Position(PlayerCharacter_Controller player)
    {
        original_Y_Value = player.transform.position.y;

        player.rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    }

    private void Unlock_Y_Position(PlayerCharacter_Controller player)
    {
        player.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
