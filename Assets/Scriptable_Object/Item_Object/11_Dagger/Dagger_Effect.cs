using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dagger_Effect", menuName = "ItemEffects/Dagger_Effect")]
public class Dagger_Effect : ItemEffect
{
    public float crit_Rate_Inc = 0.05f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.crit_Rate += crit_Rate_Inc;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.crit_Rate -= crit_Rate_Inc;
    }
}
