using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Book_Effect", menuName = "ItemEffects/Book_Effect")]
public class Book_Effect : ItemEffect
{
    public float skill_Cooldown_Reduce_Multiple = 0.9f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.skill_Cooldown *= skill_Cooldown_Reduce_Multiple;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.skill_Cooldown /= skill_Cooldown_Reduce_Multiple;
    }
}