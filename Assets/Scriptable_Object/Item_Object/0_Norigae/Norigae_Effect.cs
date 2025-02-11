using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Norigae_Effect", menuName = "ItemEffects/Norigae_Effect")]
public class Norigae_Effect : ItemEffect
{
    public float speedMultiplier = 1.1f;  // 속도 증가 배율

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed *= speedMultiplier;  // 플레이어의 이동 속도를 증가시킴
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        player.movementSpeed /= speedMultiplier;  // 효과 해제 시 속도 복원
    }
}
