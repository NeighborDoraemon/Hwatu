using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TteolJam_Effect", menuName = "ItemEffects/TteolJam_Effect")]
public class TteolJamp_Effect : ItemEffect
{
    public float critDmg_Inc_Value = 0.1f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.crit_Dmg += critDmg_Inc_Value;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.crit_Dmg -= critDmg_Inc_Value;
    }
}
