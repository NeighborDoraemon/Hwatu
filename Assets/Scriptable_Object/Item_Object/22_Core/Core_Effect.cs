using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Core_Effect", menuName = "ItemEffects/Core_Effect")]
public class Core_Effect : ItemEffect
{
    [SerializeField] private float dmg_Inc_Value = 0.2f;
    [SerializeField] private float takenDmg_Inc_Value = 0.2f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.damage_Mul += dmg_Inc_Value;
        player.takenDamage_Mul += takenDmg_Inc_Value;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.damage_Mul -= dmg_Inc_Value;
        player.takenDamage_Mul -= takenDmg_Inc_Value;
    }
}
