using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EarRing_Effect", menuName = "ItemEffects/EarRing_Effect")]
public class EarRing_Effect : ItemEffect
{
    public GameObject explosion_Prefab;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.has_EarRing_Effect = true;
        player.earRing_Explosion_Prefab = explosion_Prefab;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.has_EarRing_Effect = false;
        player.earRing_Explosion_Prefab = null;
    }
}
