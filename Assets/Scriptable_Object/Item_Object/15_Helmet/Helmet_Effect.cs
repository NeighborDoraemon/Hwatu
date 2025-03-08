using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Helmet_Effect", menuName = "ItemEffects/Helmet_Effect")]
public class Helmet_Effect : ItemEffect
{
    public float inc_Defend_Rate = 0.03f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.defend_Attack_Rate += inc_Defend_Rate;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.defend_Attack_Rate -= inc_Defend_Rate;
    }
}
