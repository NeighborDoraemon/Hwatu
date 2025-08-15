using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Money_Effect", menuName = "ItemEffects/Money_Effect")]
public class Money_Effect : ItemEffect
{
    public int money_Value;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.Add_Player_Money(money_Value);
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        
    }
}
