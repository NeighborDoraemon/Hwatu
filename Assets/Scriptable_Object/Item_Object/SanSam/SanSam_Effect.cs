using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SanSam_Effect", menuName = "ItemEffects/SanSam_Effect")]
public class SanSam_Effect : ItemEffect
{
    public int heal_Amount = 50;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.health += heal_Amount;
        if (player.health > player.max_Health)
        {
            player.health = player.max_Health;
        }
        //Debug.Log("체력이 회복되었습니다.");
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        
    }
}
