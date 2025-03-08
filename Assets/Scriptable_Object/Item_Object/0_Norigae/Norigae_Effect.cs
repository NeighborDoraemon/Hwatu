using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Norigae_Effect", menuName = "ItemEffects/Norigae_Effect")]
public class Norigae_Effect : ItemEffect
{
    public float speedMultiplier = 1.1f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed *= speedMultiplier;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed /= speedMultiplier;
    }
}
