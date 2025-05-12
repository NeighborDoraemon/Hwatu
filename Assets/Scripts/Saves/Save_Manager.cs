using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Save_Manager : MonoBehaviour
{
    public static Save_Manager Instance { get; private set; }

    private SaveData currentData;

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;

        savePath = Path.Combine(Application.persistentDataPath, "save_data.json");
        currentData = LoadFromFile() ?? new SaveData();
    }

    // 직접적인 SaveData 접근은 막음
    public T Get<T>(Func<SaveData, T> selector)
    {
        return selector(currentData);
    }

    public void Modify(Action<SaveData> modifier)
    {
        modifier(currentData);
    }

    public void Save() => SaveToFile(currentData);

    // 내부 저장/불러오기
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

public class SaveData
{
    //Maps
    public List<Map_Value> Map_List = new List<Map_Value>();
    public int Map_Index;

    //Player

    //Event

    //Market
    public bool is_Market_Now = false;
    public bool is_take_Market = false;
}
