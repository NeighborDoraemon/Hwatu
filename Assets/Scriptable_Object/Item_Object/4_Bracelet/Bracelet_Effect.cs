using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bracelet_Effect", menuName = "ItemEffects/Bracelet_Effect")]
public class Bracelet_Effect : ItemEffect
{
    public int tp_Count_Inc = 1;
    public float tp_Cooldown_Inc_Multiple = 0.5f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.max_Teleport_Count += tp_Count_Inc;
        player.teleport_Cooltime_Mul += tp_Cooldown_Inc_Multiple;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.max_Teleport_Count -= tp_Count_Inc;
        player.teleport_Cooltime_Mul -= tp_Cooldown_Inc_Multiple;
    }
}
