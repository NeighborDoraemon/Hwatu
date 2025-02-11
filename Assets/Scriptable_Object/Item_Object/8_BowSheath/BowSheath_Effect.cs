using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowSheath_Effect", menuName = "ItemEffects/BowSheath_Effect")]
public class BowSheath_Effect : ItemEffect
{
    public float atkCooldown_Reduce_Multiple = 0.9f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.attack_Cooldown *= atkCooldown_Reduce_Multiple;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.attack_Cooldown /= atkCooldown_Reduce_Multiple;
    }
}
