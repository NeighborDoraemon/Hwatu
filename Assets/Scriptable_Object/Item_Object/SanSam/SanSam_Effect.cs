using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

[CreateAssetMenu(fileName = "SanSam_Effect", menuName = "ItemEffects/SanSam_Effect")]
public class SanSam_Effect : ItemEffect
{
    public int heal_Amount = -50;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        Debug.Log("산삼 효과 적용");
        player.Player_Take_Damage(heal_Amount);
        if (player.health > player.max_Health)
        {
            player.health = player.max_Health;
        }
        Debug.Log($"플레이어 체력 : {player.health}");
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        
    }
}
