using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Save_Manager
{
    private static Save_Manager _instance;
    public static Save_Manager Instance => _instance ??= new Save_Manager();

    private SaveData currentData;
    private readonly List<ISaveable> saveables = new();
    private readonly string savePath;

    private Save_Manager()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save_data.json");
        currentData = LoadFromFile() ?? new SaveData();
    }

    public void Register(ISaveable saveable)
    {
        if (!saveables.Contains(saveable))
            saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable)
    {
        if (saveables.Contains(saveable))
            saveables.Remove(saveable);
    }

    public void SaveAll()
    {
        foreach (var saveable in saveables)
            saveable.Save(currentData);

        SaveToFile(currentData);

        Debug.Log("Saved");
    }

    //public void Save_

    public T Get<T>(Func<SaveData, T> selector)
    {
        return selector(currentData);
    }

    public void Modify(Action<SaveData> modifier)
    {
        modifier(currentData);
    }

    private void SaveToFile(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, Encrypt(json));
    }

    private SaveData LoadFromFile()
    {
        if (!File.Exists(savePath)) return null;
        string json = Decrypt(File.ReadAllText(savePath));
        return JsonUtility.FromJson<SaveData>(json);
    }

    private string Encrypt(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    private string Decrypt(string encoded) => Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
}

//How To Use
//public class PlayerStatus : MonoBehaviour, ISaveable
//{
//    public int hp;

//    private void Start()
//    {
//        Save_Manager.Instance.Register(this); // 생성이 아니라, 등록만
//    }

//    public void Save(SaveData data)
//    {
//        data.playerHp = this.hp;
//    }

//    private void OnDestroy()
//    {
//        Save_Manager.Instance.Unregister(this);
//    }

//    private How_To_Save_All()
//    {
//        Save_Manager.Instance.SaveAll();
//    }

//    public void IncreaseMaxHP(int amount)
//    {
//        // SaveData 내 값만 수정
//        Save_Manager.Instance.Modify(data =>
//        {
//            data.permanentStats.maxHP += amount;
//        });

//        // 변경 후 즉시 저장
//        Save_Manager.Instance.Save();
//    }
//}


public class SaveData
{
    //Maps
    public bool is_Map_Saved = false;

    public List<int> new_map_index_list = new List<int>();
    public List<int> new_event_map_list = new List<int>();
    public bool is_Fmb_Now = false;

    public bool is_Tutorial_Cleared = false;

    //Map-market
    public bool is_Market_Now = false;
    public bool is_take_Market = false;
    //Map-Event
    public bool is_Event_Now = false;
    public bool is_Hunting_Cleared = false;

    //Map_Boss
    public bool is_Boss_Stage = false;

    //Player Inventory
    public bool is_Inventory_Saved = false;
    public List<int> saved_Card_IDs = new List<int>();
    public List<int> saved_Item_IDs = new List<int>();

    //Player Money
    public int player_Money = 0;
    public int player_token = 0;

    //Player Stat
    public float health_Ratio;

    public int attack_Phase;
    public int health_Phase;
    public int atk_Cooltime_Phase;
    public int move_Phase;

    public int inc_AttackDamage;
    public int inc_Health;
    public int inc_Damage_Reduction;
    public float dec_AttackCooltime;
    public float inc_MoveSpeed;

    public bool dmg_Inc_To_Lost_Health;
    public bool card_Match_Dmg_Inc;
    public bool skill_Coomtime_Has_Dec;
    public bool money_Earned_Has_Inc;
    public bool invisible_Teleport;
}

public interface ISaveable
{
    void Save(SaveData data);
    //void Load(SaveData data);
}
