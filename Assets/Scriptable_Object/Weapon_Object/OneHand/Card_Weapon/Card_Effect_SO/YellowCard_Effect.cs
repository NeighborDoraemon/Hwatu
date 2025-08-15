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

        var enemy = target.GetComponent<Enemy_Basic>();
        if (enemy != null)
            st.hit_Enemies.Add(enemy);

        st.bounces++;

        if (st.bounces >= max_Bounces)
        {
            Cleanup(key);
            proj.Expire();
            return;
        }

        var next = Find_Next_Target(proj.transform.position, st.hit_Enemies);
        if (next == null)
        {
            Cleanup(key);
            proj.Expire();
            return;
        }

        Vector2 dir = ((Vector2)next.transform.position - (Vector2)proj.transform.position).normalized;

        st.cur_Speed = Mathf.Max(0.01f, st.cur_Speed * speed_Multiplier);

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
            var e = h.GetComponent<Enemy_Basic>();
            if (e == null) continue;
            if (excluded != null && excluded.Contains(e)) continue;

            float d = Vector2.SqrMagnitude((Vector2)e.transform.position - from);
            if (d < best_Dist)
            {
                best_Dist = d;
                best = e;
            }
        }
        return best;
    }
}
