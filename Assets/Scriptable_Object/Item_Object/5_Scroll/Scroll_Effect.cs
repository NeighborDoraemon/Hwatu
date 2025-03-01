using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scroll_Effect", menuName = "ItemEffects/Scroll_Effect")]
public class Scroll_Effect : ItemEffect
{
    public GameObject clone_Prefab;
    private GameObject spawned_Clone;

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        if (clone_Prefab != null && spawned_Clone == null)
        {
            spawned_Clone = Instantiate(clone_Prefab, player.transform.position, Quaternion.identity);
            spawned_Clone.GetComponent<Player_Clone>().Initialize(player);
        }
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        if (spawned_Clone != null)
        {
            Destroy(spawned_Clone);
        }
    }
}
