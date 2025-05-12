using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BronzeBell_Attack", menuName = "Weapon/Attack Strategy/BronzeBell")]
public class BronzeBell_Attack_Strrategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    public float attack_Range = 2.5f;
    public float hold_Atk_Interval = 3.0f;

    private float last_Hold_Attack_Time = -Mathf.Infinity;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        this.player = player;
        this.weapon_Data = weapon_Data;

        Initialize_Weapon_Data();
    }

    private void Initialize_Weapon_Data()
    {
        player.animator.runtimeAnimatorController = weapon_Data.overrideController;
        player.attack_Cooldown = weapon_Data.attack_Cooldown;
        player.max_AttackCount = weapon_Data.max_Attack_Count;
        player.skill_Cooldown = weapon_Data.skill_Cooldown;
    }

    public void Reset_Stats() { }

    public void Attack(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (!player.isAttacking)
        {
            player.isAttacking = true;
        }

        if (Time.time < last_Hold_Attack_Time + hold_Atk_Interval)
        {
            return;
        }

        last_Hold_Attack_Time = Time.time;

        int mask = LayerMask.GetMask("Enemy", "Boss_Enemy");

        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attack_Range, mask);
        
        foreach (Collider2D enemy_Collider in hits)
        {
            Enemy_Basic enemy = enemy_Collider.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Damage());
                Debug.Log("Enemy hit by hold attack, Damage : " + player.attackDamage);
            }
            else
            {
                Debug.LogWarning($"Enemy_Basic component not found on : {enemy_Collider.name}");
            }
        }
    }

    public void Shoot(PlayerCharacter_Controller player, Transform fire_Point)
    {

    }

    public void Skill(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        var Obj_Manager = Object_Manager.instance;

        var old_Cards = Obj_Manager.current_Spawned_Card.ToList();
        var card_Positions = new List<Vector2>();
        foreach (var card in old_Cards)
        {
            card_Positions.Add(card.transform.position);
            var sprite = card.GetComponent<SpriteRenderer>().sprite;
            Obj_Manager.Remove_Used_Sprite(sprite);
            Obj_Manager.Remove_From_Spawned_Cards(card);
            Destroy(card);
        }
        foreach (var pos in card_Positions)
        {
            Obj_Manager.Spawn_Cards(pos);
        }

        var item_Objects = GameObject.FindGameObjectsWithTag("Item");
        foreach (var item_Obj in item_Objects)
        {
            var pos = (Vector2)item_Obj.transform.position;
            var prefab_Script = item_Obj.GetComponent<Item_Prefab>();
            var old_Item = prefab_Script.GetItem();
            var rarity = old_Item.item_Rarity;

            var candidates = Obj_Manager.item_Database.
                Get_Items_By_Rarity(rarity).
                Where(i => i != old_Item).
                ToList();

            if (candidates.Count == 0)
            {
                candidates = Obj_Manager.item_Database.Get_All_Items().
                    Where(i => i != old_Item).
                    ToList();
            }

            var new_Item = candidates[Random.Range(0, candidates.Count)];

            Destroy(item_Obj);
            Obj_Manager.Spawn_Specific_Item(pos, new_Item);
        }
    }
}
