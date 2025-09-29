using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VineAmulet_Effect", menuName = "ItemEffects/VineAmulet_Effect")]
public class VineAmulet_Effect : ItemEffect
{
    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.has_VineAmulet_Effect = true;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.has_VineAmulet_Effect = false;
    }
}
