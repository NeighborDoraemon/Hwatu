using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card_Effect : ScriptableObject
{
    [Header("공통 비주얼")]
    public Color tint = Color.red;

    public virtual void OnShoot(Card_Projectile proj, PlayerCharacter_Controller player) { }
    public virtual void OnHit(Card_Projectile proj, Collider2D target, Vector2 hit_Point) { }
    public virtual void OnExpire(Card_Projectile proj, Vector2 pos) { }
}
