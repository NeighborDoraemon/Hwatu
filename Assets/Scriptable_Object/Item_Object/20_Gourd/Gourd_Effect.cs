using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gourd_Effect", menuName = "ItemEffects/Gourd_Effect")]
public class Gourd_Effect : ItemEffect
{
    [Header("처치 시 회복 설정")]
    public int heal_Onkill = 5;

    private readonly Dictionary<PlayerCharacter_Controller, int> _stacks = new();
    private readonly Dictionary<PlayerCharacter_Controller, Action> _handlers = new();

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        if (!_stacks.ContainsKey(player)) _stacks[player] = 0;
        _stacks[player]++;

        if (!_handlers.ContainsKey(player))
        {
            Action handler = () => OnEnemyKilled_Heal(player);
            player.On_Enemy_Killed += handler;
            _handlers[player] = handler;
        }
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        if (_stacks.TryGetValue(player, out int count))
        {
            count--;
            if (count <= 0)
            {
                _stacks.Remove(player);
                if (_handlers.TryGetValue(player, out var handler))
                {
                    player.On_Enemy_Killed -= handler;
                    _handlers.Remove(player);
                }
            }
            else
            {
                _stacks[player] = count;
            }
        }
    }

    private void OnEnemyKilled_Heal(PlayerCharacter_Controller player)
    {
        if (!_stacks.TryGetValue(player, out int stack) || stack <= 0) return;

        int total_Heal = Mathf.Max(0, heal_Onkill) * stack;
        if (total_Heal <= 0) return;

        player.Player_Take_Heal(total_Heal);
    }
}
