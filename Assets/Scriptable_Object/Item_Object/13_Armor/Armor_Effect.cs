using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armor_Effect", menuName = "ItemEffects/Armor_Effect")]
public class Armor_Effect : ItemEffect
{
    public int damage_Reduce_Min = 3;
    public int damage_Reduce_Max = 5;

    public float dec_MoveSpeed_Mul = 0.2f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.damage_Reduce_Min += damage_Reduce_Min;
        player.damage_Reduce_Max += damage_Reduce_Max;

        player.movementSpeed_Mul -= dec_MoveSpeed_Mul;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.damage_Reduce_Min -= damage_Reduce_Min;
        player.damage_Reduce_Max -= damage_Reduce_Max;

        player.movementSpeed_Mul += dec_MoveSpeed_Mul;
    }
}
