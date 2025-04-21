using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hairpin_Effect", menuName = "ItemEffects/Hairpin_Effect")]
public class Hairpin_Effect : ItemEffect
{
    public float inc_Bleeding_Rate = 0.05f;
    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.bleeding_Rate += inc_Bleeding_Rate;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.bleeding_Rate -= inc_Bleeding_Rate;
    }
}
