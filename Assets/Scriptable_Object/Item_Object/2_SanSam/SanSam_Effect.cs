using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "SanSam_Effect", menuName = "ItemEffects/SanSam_Effect")]
public class SanSam_Effect : ItemEffect
{
    public int heal_Amount = 50;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.Player_Take_Heal(heal_Amount);
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        
    }
}
