using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring_Effect", menuName = "ItemEffects/Ring_Effect")]

public class Ring_Effect : ItemEffect
{
    public float inc_Stun_Rate = 0.03f;
    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.stun_Rate += inc_Stun_Rate;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.stun_Rate -= inc_Stun_Rate;
    }
}
