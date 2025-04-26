using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ddidon_Effect", menuName = "ItemEffects/Ddidon_Effect")]
public class Ddidon_Effect : ItemEffect
{
    public float movementSpeed_Reduce_Multiple = 0.1f;
    public float atkCooldown_Reduce_Multiple = 0.1f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed_Mul -= movementSpeed_Reduce_Multiple;
        player.attack_Cooltime_Mul -= atkCooldown_Reduce_Multiple;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed_Mul += movementSpeed_Reduce_Multiple;
        player.attack_Cooltime_Mul += atkCooldown_Reduce_Multiple;
    }
}
