using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BronzeBell_Attack", menuName = "Weapon/Attack Strategy/BronzeBell")]
public class BronzeBell_Attack_Strrategy : ScriptableObject, IAttack_Strategy
{
    private PlayerCharacter_Controller player;
    private Weapon_Data weapon_Data;

    [Header("Base Attack Settings")]
    public float attack_Range = 2.5f;
    public float hold_Atk_Interval = 3.0f;
    public LayerMask enemy_Layer;

    private float last_Hold_Attack_Time = -Mathf.Infinity;

    [Header("Reroll Skill Settings")]
    public bool has_Rerolled = false;

    public void Initialize(PlayerCharacter_Controller player, Weapon_Data weapon_Data)
    {
        if (player == null || weapon_Data == null)
        {
            throw new System.ArgumentNullException("Player or Weapon Data is null");
        }

        last_Hold_Attack_Time = -Mathf.Infinity;

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
            Debug.Log($"[BronzeBell] Has Returned");
            return;
        }

        last_Hold_Attack_Time = Time.time;

        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, attack_Range, enemy_Layer);
        Debug.Log($"[BronzeBell] hits Count = {hits.Length}");
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log($"[BronzeBell] hit #{i} -> {hits[i].name} (Layer : {LayerMask.LayerToName(hits[i].gameObject.layer)}");
        }

        foreach (Collider2D enemy_Collider in hits)
        {
            Enemy_Basic enemy = enemy_Collider.GetComponent<Enemy_Basic>();
            if (enemy != null)
            {
                enemy.TakeDamage(player.Calculate_Damage());
                Debug.Log("Enemy hit by hold attack, Damage : " + player.attackDamage);

                if (Random.value <= player.stun_Rate)
                {
                    Enemy_Stun_Interface enemy_Interface = enemy.GetComponent<Enemy_Stun_Interface>()
                                ?? enemy.GetComponentInParent<Enemy_Stun_Interface>()
                                ?? enemy.GetComponentInChildren<Enemy_Stun_Interface>();

                    enemy_Interface.Enemy_Stun(2.0f);
                }

                if (Random.value <= player.bleeding_Rate)
                {
                    enemy.GetComponent<Enemy_Basic>().Bleeding_Attack(player.Calculate_Damage(), 5, 1.1f);
                }

                player.Trigger_Enemy_Hit();
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
        if (has_Rerolled)
            return;

        var Obj_Manager = Object_Manager.instance;

        // 리롤할 카드 수집
        var old_Cards = Obj_Manager.current_Spawned_Card
        .Where(c => c != null)
        .Select(c => new
        {
            gameObject = c,
            position = (Vector2)c.transform.position,
            sprite = c.GetComponent<SpriteRenderer>().sprite
        })
        .ToList();

        // 리롤할 필드 아이템 수집
        var old_Items_Data = GameObject.FindGameObjectsWithTag("Item")
        .Where(obj => obj != null)
        .Select(obj => new {
            position = (Vector2)obj.transform.position,
            itemData = obj.GetComponent<Item_Prefab>().GetItem(),
            gameObject = obj
        })
        .ToList();

        // 상점 아이템 수집
        var old_Market_Items = GameObject.FindGameObjectsWithTag("Market_Item")
            .Where(obj => obj != null)
            .Select(obj => new
            {
                gameObject = obj,
                position = (Vector2)obj.transform.position,
                itemData = obj.GetComponent<Item_Prefab>().GetItem()
            })
            .ToList();

        // 리롤 대상이 없을 시 리턴
        if (old_Cards.Count == 0 && old_Items_Data.Count == 0 && old_Market_Items.Count == 0)
            return;

        // 카드 리롤
        foreach (var card in old_Cards)
        {
            Obj_Manager.Remove_Used_Sprite(card.sprite);
            Obj_Manager.Remove_From_Spawned_Cards(card.gameObject);
            Destroy(card.gameObject);
        }
        foreach (var card in old_Cards)
        {
            Obj_Manager.Spawn_Cards(card.position);
        }

        // 아이템 리롤
        foreach(var item in old_Items_Data)
        {
            Destroy(item.gameObject);
        }
        foreach (var item_Obj in old_Items_Data)
        {
            var candidates = Obj_Manager.item_Database.
                Get_Items_By_Rarity(item_Obj.itemData.item_Rarity).
                Where(i => i != item_Obj.itemData).
                ToList();

            if (candidates.Count == 0)
            {
                candidates = Obj_Manager.item_Database
                    .Get_All_Items()
                    .Where(i => i != item_Obj.itemData)
                    .ToList();
            }

            var new_Item = candidates[Random.Range(0, candidates.Count)];
            Obj_Manager.Spawn_Specific_Item(item_Obj.position, new_Item);
        }

        // 상점 아이템 리롤
        foreach(var mk_Item in old_Market_Items)
        {
            Destroy(mk_Item.gameObject);
        }
        foreach(var mk_Item in old_Market_Items)
        {
            var candidates = Obj_Manager.item_Database
                .Get_Items_By_Rarity(mk_Item.itemData.item_Rarity)
                .Where(i => i != mk_Item.itemData)
                .ToList();

            if (candidates.Count == 0)
            {
                candidates = Obj_Manager.item_Database
                    .Get_All_Items()
                    .Where(i => i != mk_Item.itemData)
                    .ToList();
            }

            var new_MarketItem = candidates[Random.Range(0, candidates.Count)];
            Obj_Manager.Spawn_Market_Item(mk_Item.position, new_MarketItem, Obj_Manager.gameObject);
        }

        // 리롤 완료 처리
        has_Rerolled = true;
    }

    public void Reset_Reroll_Count()
    {
        has_Rerolled = false;
    }
}
