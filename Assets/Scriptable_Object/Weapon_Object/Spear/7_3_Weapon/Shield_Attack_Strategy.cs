using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "Shield_Attack", menuName = "Weapon/Attack Strategy/Shield")]
public class Shield_Attack_Strategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public bool isParrying;
    public float parry_Duration = 0.5f;    

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

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player.isGrounded)
        {
            if (isParrying) return;

            Activate_Parry(player);
        }
        else if (!player.isGrounded)
        {
            player.StartCoroutine(Jump_Attack(player));
            player.StartCoroutine(Shield_Layer_Change());
        }
    }

    private IEnumerator Jump_Attack(PlayerCharacter_Controller player)
    {
        while (!player.isGrounded)
        {
            player.is_Knock_Back = true;

            player.animator.SetTrigger("Attack");
            player.rb.velocity = new Vector2(player.rb.velocity.x, -2f);
            yield return new WaitForSeconds(0.5f);
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

        isParrying = false;
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point) { }
    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data) { }    
}