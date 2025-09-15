using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Horn_Effect", menuName = "ItemEffects/Horn_Effect")]
public class Horn_Effect : ItemEffect
{
    [Header("처치 카운트")]
    public int kills_Count = 5;

    [Header("버프 수치/시간")]
    [Range(0.0f, 0.95f)]
    public float atkSpeed_Dec_Value = 0.40f;
    public float buff_Duration = 5.0f;

    [Header("쿨타임")]
    public float cooldown_Duration = 10.0f;

    private float min_Cooldown_Mul = 0.05f;

    private readonly Dictionary<PlayerCharacter_Controller, int> _killCount = new();
    private readonly Dictionary<PlayerCharacter_Controller, bool> _isBuffing = new();
    private readonly Dictionary<PlayerCharacter_Controller, bool> _isColldown = new();
    private readonly Dictionary<PlayerCharacter_Controller, float> _appliedMul = new();
    private readonly Dictionary<PlayerCharacter_Controller, Coroutine> _running = new();
    private readonly Dictionary<PlayerCharacter_Controller, Action> _handlers = new();
    private readonly Dictionary<PlayerCharacter_Controller, int> _stacks = new();

    public override void ApplyEffect(PlayerCharacter_Controller player)
    {
        if (player == null) return;

        if (!_stacks.ContainsKey(player)) _stacks[player] = 0;
        _stacks[player]++;

        if (!_handlers.ContainsKey(player))
        {
            Action handler = () => On_Enemy_Killed(player);
            player.On_Enemy_Killed += handler;
            _handlers[player] = handler;

            _killCount[player] = 0;
            _isBuffing[player] = false;
            _isColldown[player] = false;
        }
    }

    public override void RemoveEffect(PlayerCharacter_Controller player)
    {
        if (player == null) return;

        if (_stacks.TryGetValue(player, out int s))
        {
            s--;
            if (s <= 0)
            {
                _stacks.Remove(player);

                if (_handlers.TryGetValue(player, out var h))
                {
                    player.On_Enemy_Killed -= h;
                    _handlers.Remove(player);
                }

                if (_running.TryGetValue(player, out var co) && co != null)
                {
                    player.StopCoroutine(co);
                    _running[player] = null;
                }

                if (_isBuffing.TryGetValue(player, out bool buff) && buff)
                    Remove_Buff(player);

                _killCount.Remove(player);
                _isBuffing.Remove(player);
                _isColldown.Remove(player);
                _appliedMul.Remove(player);
            }
            else
            {
                _stacks[player] = s;
            }
        }
    }

    private void On_Enemy_Killed(PlayerCharacter_Controller player)
    {
        if (_isBuffing.TryGetValue(player, out bool buffing) && buffing) return;
        if (_isColldown.TryGetValue(player, out bool cooldown) && cooldown) return;

        int cur = 0;
        _killCount.TryGetValue(player, out cur);
        cur++;
        _killCount[player] = cur;

        if (cur >= Mathf.Max(1, kills_Count))
        {
            _killCount[player] = 0;

            if (_running.TryGetValue(player, out var co) && co != null)
            {
                player.StopCoroutine(co);
                _running[player] = null;
            }

            _running[player] = player.StartCoroutine(Buff_Routine(player));
        }
    }

    private IEnumerator Buff_Routine(PlayerCharacter_Controller player)
    {
        _isBuffing[player] = true;
        Apply_Buff(player);

        yield return new WaitForSeconds(buff_Duration);

        Remove_Buff(player);
        _isBuffing[player] = false;

        _isColldown[player] = true;
        yield return new WaitForSeconds(cooldown_Duration);
        _isColldown[player] = false;

        _running[player] = null;
    }

    private void Apply_Buff(PlayerCharacter_Controller player)
    {
        float nominal_Delta = -atkSpeed_Dec_Value;

        float before = player.attack_Cooltime_Mul;
        float desired = before + nominal_Delta;
        float after = Mathf.Max(min_Cooldown_Mul, desired);

        float actually_Applied = after - before;
        if (_appliedMul.TryGetValue(player, out float prev_Applied)
            && Math.Abs(prev_Applied) > float.Epsilon)
        {
            player.attack_Cooltime_Mul = Mathf.Max(min_Cooldown_Mul, player.attack_Cooltime_Mul - prev_Applied);
        }

        _appliedMul[player] = actually_Applied;
        player.attack_Cooltime_Mul = after;
    }

    private void Remove_Buff(PlayerCharacter_Controller player)
    {
        if (_appliedMul.TryGetValue(player, out float applied))
        {
            player.attack_Cooltime_Mul = Mathf.Max(min_Cooldown_Mul, player.attack_Cooltime_Mul - applied);
            _appliedMul.Remove(player);
        }
    }
}
