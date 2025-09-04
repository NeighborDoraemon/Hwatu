using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "YellowCard", menuName = "Card_Effect/Yellow_Bounce")]
public class YellowCard_Effect : Card_Effect
{
    [Header("Bounce Settings")]
    public int max_Bounces = 3;
    public float search_Radius = 6.0f;
    public LayerMask enemyLayer;
    public float speed_Multiplier = 1.0f;
    public float damage_Falloff = 1.0f;

    private readonly Dictionary<Enemy_Basic, Transform> bt_Anchor_Cache = new();

    private readonly Dictionary<int, Bounce_State> states = new();

    private class Bounce_State
    {
        public HashSet<Enemy_Basic> hit_Enemies = new HashSet<Enemy_Basic>();
        public int bounces = 0;
        public int cur_Damage;
        public float cur_Speed;
    }

    public override void OnShoot(Card_Projectile proj, PlayerCharacter_Controller player)
    {
        var (_, speed) = proj.Get_Current_Flight();
        var key = proj.GetInstanceID();
        if (!states.ContainsKey(key))
        {
            states[key] = new Bounce_State
            {
                bounces = 0,
                cur_Damage = proj.Get_FinalDamage(),
                cur_Speed = speed
            };
        }
    }

    public override void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point)
    {
        var key = proj.GetInstanceID();
        if (!states.TryGetValue(key, out var st))
        {
            proj.Expire();
            return;
        }

        var enemy = target.GetComponentInParent<Enemy_Basic>();
        if (enemy != null)
            st.hit_Enemies.Add(enemy);

        st.bounces++;

        if (st.bounces >= max_Bounces)
        {
            Cleanup(key);
            proj.Expire();
            return;
        }

        Vector2 center = enemy != null ? Get_Target_Position(enemy) : (Vector2)proj.transform.position;

        var next = Find_Next_Target(proj.transform.position, st.hit_Enemies);
        if (next == null)
        {
            Cleanup(key);
            proj.Expire();
            return;
        }

        Vector2 aim_Pos = Get_Target_Position(next);
        Vector2 dir = (aim_Pos - center).normalized;

        st.cur_Speed = Mathf.Max(0.01f, st.cur_Speed * Mathf.Max(0.0001f, speed_Multiplier));

        if (damage_Falloff > 0.0f && damage_Falloff < 1.0f)
        {
            st.cur_Damage = Mathf.Max(1, Mathf.RoundToInt(st.cur_Damage * damage_Falloff));
            proj.Set_Override_Damage(st.cur_Damage);
        }

        proj.Retarget(dir, st.cur_Speed);
    }

    public override void OnExpire(Card_Projectile proj, Vector2 pos)
    {
        Cleanup(proj.GetInstanceID());
    }

    private void Cleanup(int key)
    {
        if (states.ContainsKey(key))
        {
            states.Remove(key);
        }
    }

    private Enemy_Basic Find_Next_Target(Vector2 from, HashSet<Enemy_Basic> excluded)
    {
        var hits = Physics2D.OverlapCircleAll(from, search_Radius, enemyLayer);
        Enemy_Basic best = null;
        float best_Dist = float.MaxValue;

        foreach (var h in hits)
        {
            var e = h.GetComponentInParent<Enemy_Basic>();
            if (e == null) continue;
            if (excluded != null && excluded.Contains(e)) continue;

            Vector2 aim_Pos = Get_Target_Position(e);
            float dSq = (aim_Pos - from).sqrMagnitude;
            if (dSq < best_Dist)
            {
                best_Dist = dSq;
                best = e;
            }
        }
        return best;
    }

    private Vector2 Get_Target_Position(Enemy_Basic e)
    {
        if (e == null) return Vector2.zero;

        if (!bt_Anchor_Cache.TryGetValue(e, out var anchor) || anchor == null)
        {
            var bt = e.GetComponentInChildren<MonoBehaviourTree>(true);
            anchor = bt != null ? bt.transform : null;
            bt_Anchor_Cache[e] = anchor;
        }

        if (anchor != null) return anchor.position;

        var col = e.GetComponentInChildren<Collider2D>();
        if (col != null) return col.bounds.center;

        return e.transform.position;
    }

}
