using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon_Animation_Controller : MonoBehaviour
{
    PlayerCharacter_Controller player;
    public Animator cannon_Animator;

    private void Awake()
    {
        player = FindObjectOfType<PlayerCharacter_Controller>();
    }

    private void Update()
    {
        Update_Animation();
    }

    void Update_Animation()
    {
        cannon_Animator.SetBool("is_PC_Move", player.isMoving);

        if (player.isAttacking)
        {
            cannon_Animator.SetTrigger("Attack");
        }
    }
    
    public void Trigger_Skill()
    {
        cannon_Animator.SetTrigger("Skill");
    }
}
