using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bracelet_Effect", menuName = "ItemEffects/Bracelet_Effect")]
public class Bracelet_Effect : ItemEffect
{
    public float skill_Cooldown_Reduce_Multiple = 0.9f;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.skill_Cooldown *= skill_Cooldown_Reduce_Multiple;
        Debug.Log("Skill Cooldown has been reduced");
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.skill_Cooldown /= skill_Cooldown_Reduce_Multiple;
        Debug.Log("Skill cooldown has been restored");
    }
}