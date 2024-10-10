using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack_Strategy
{
    void Attack();
    void Skill();
}
public class Base_Attack_Strategy : IAttack_Strategy // ¸ÁÅë ¹«±â
{
    private PlayerCharacter_Controller player;

    public Base_Attack_Strategy(PlayerCharacter_Controller player)
    {
        this.player = player;
        player.Set_AttackCooldown(1f);
        player.Set_Max_AttackCount(1);
        player.animator.SetLayerWeight(1, 1);
        player.animator.SetLayerWeight(2, 0);
    }

    public void Attack()
    {
        if (!player.isGrounded) return;

        if (Time.time >= player.last_Attack_Time + player.attack_Cooldown)
        {
            player.animator.SetTrigger("Attack_1");
            player.isAttacking = true;
            player.cur_AttackCount++;
            player.last_ComboAttck_Time = Time.time;
            player.last_Attack_Time = Time.time;
        }
    }

    public void Skill()
    {
        if (Time.time >= player.last_Skill_Time + player.skill_Cooldown)
        {
            player.animator.SetTrigger("Skill");

            player.last_Skill_Time = Time.time;
        }
    }
}
public class Base_Two_Attack_Strategy : IAttack_Strategy // ²ý ¹«±â
{
    private PlayerCharacter_Controller player;

    public Base_Two_Attack_Strategy(PlayerCharacter_Controller player)
    {
        this.player = player;
        player.Set_AttackCooldown(2f);
        player.Set_Max_AttackCount(2);
        player.animator.SetLayerWeight(1, 1);
        player.animator.SetLayerWeight(2, 0);
    }
    public void Attack()
    {
        if (!player.isGrounded) return;

        if (Time.time >= player.last_Attack_Time + player.attack_Cooldown)
        {
            player.animator.SetTrigger("Attack_1");
            player.isAttacking = true;
            player.cur_AttackCount++;
            player.last_ComboAttck_Time = Time.time;
            player.last_Attack_Time = Time.time;
        }
        else if (player.isAttacking && player.cur_AttackCount == 1 && Time.time - player.last_ComboAttck_Time <= player.comboTime)
        {
            player.animator.SetTrigger("Attack_2");
            player.isAttacking = true;
            player.cur_AttackCount++;
            player.last_ComboAttck_Time = Time.time;
        }
    }

    public void Skill()
    {
        if (Time.time >= player.last_Skill_Time + player.skill_Cooldown)
        {
            player.animator.SetTrigger("Skill");

            player.last_Skill_Time = Time.time;
        }

    }
}

public class Base_Three_Attack_Strategy : IAttack_Strategy // °©¿À ¹«±â
{
    private PlayerCharacter_Controller player;

    public Base_Three_Attack_Strategy(PlayerCharacter_Controller player)
    {
        this.player = player;
        player.Set_AttackCooldown(2.5f);
        player.Set_Max_AttackCount(3);
        player.animator.SetLayerWeight(1, 1);
        player.animator.SetLayerWeight(2, 0);
    }

    public void Attack()
    {
        if (!player.isGrounded) return;

        if (Time.time >= player.last_Attack_Time + player.attack_Cooldown)
        {
            player.animator.SetTrigger("Attack_1");
            player.isAttacking = true;
            player.cur_AttackCount = 1;
            player.last_ComboAttck_Time = Time.time;
            player.last_Attack_Time = Time.time;
        }
        else if (player.isAttacking && player.cur_AttackCount == 1 && Time.time - player.last_ComboAttck_Time <= player.comboTime)
        {
            player.animator.SetTrigger("Attack_2");
            player.cur_AttackCount++;
            player.last_ComboAttck_Time = Time.time;
        }
        else if (player.isAttacking && player.cur_AttackCount == 2 && Time.time - player.last_ComboAttck_Time <= player.comboTime)
        {
            player.animator.SetTrigger("Attack_3");
            player.cur_AttackCount++;
            player.last_ComboAttck_Time = Time.time;
        }
    }

    public void Skill()
    {
        if (Time.time >= player.last_Skill_Time + player.skill_Cooldown)
        {
            player.animator.SetTrigger("Skill");

            player.last_Skill_Time = Time.time;
        }

    }
}
public class Three_Eight_Attack_Strategy : IAttack_Strategy
{
    private PlayerCharacter_Controller player;

    public Three_Eight_Attack_Strategy(PlayerCharacter_Controller player)
    {
        this.player = player;
        player.Set_AttackCooldown(1f);
        player.Set_Max_AttackCount(1);
        player.animator.SetLayerWeight(1, 0);
        player.animator.SetLayerWeight(2, 1);
    }

    public void Attack()
    {
        if (!player.isGrounded) return;

        if (Time.time >= player.last_Attack_Time + player.attack_Cooldown)
        {
            player.animator.SetTrigger("Attack_1");

            player.last_Attack_Time = Time.time;
        }
    }
    public void Skill()
    {

    }
}
