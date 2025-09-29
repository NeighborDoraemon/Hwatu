using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet_Effect", menuName = "ItemEffects/Bullet_Effect")]
public class Bullet_Effect : ItemEffect
{
    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.has_CorrodedBullet_Effect = true;
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.has_CorrodedBullet_Effect = false;
    }
}
