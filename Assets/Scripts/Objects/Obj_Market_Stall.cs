using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Obj_Market_Stall : MonoBehaviour
{
    [Header("Lists")]
    [SerializeField] private List<GameObject> Market_Slot_List = new List<GameObject>();
    [SerializeField] private List<GameObject> Price_Slot_List = new List<GameObject>();

    private List<Item> Inventory_List = new List<Item>();
    private List<Item> Database_List = new List<Item>();

    private List<Item> Market_List = new List<Item>();

    private List<GameObject> Spawned_Items = new List<GameObject>();

    [Header("Others")]
    [SerializeField] private Object_Manager obj_manager;
    [SerializeField] private ItemDatabase item_data;
    [SerializeField] private PlayerChar_Inventory_Manager inventory_manager;


    private void OnEnable()
    {
        Market_Destroy_Manager.On_Item_Destroyed += Destroy_Handling;
    }

    private void OnDisable()
    {
        Market_Destroy_Manager.On_Item_Destroyed -= Destroy_Handling;
    }

    private void Destroy_Handling(GameObject item)
    {
        Debug.Log("Item Destroyed At Market");

        Price_Slot_Remove(item);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Market_Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Market_Call()
    {
        Market_On_Sale();
        Set_Item_Slots();
        Set_Price_Slot();
    }

    private void Market_Reset()
    {
        Market_Slot_List.Clear();
        Price_Slot_List.Clear();
    }

    public void Market_On_Sale()
    {
        Database_Setting();     //get Database list
        Get_Inventory_List();   //Get Inventory list

        Remove_Same();          //Remove same items

        Get_Random_Item();      //Get random 4 items
    }

    private void Database_Setting()
    {
        List<Item> database_list = item_data.Get_All_Items();

        for (int i = 0; i < database_list.Count; i++)
        {
            Database_List.Add(database_list[i]);
        }
    }
    
    private void Get_Inventory_List()
    {
        Item[] items = inventory_manager.Give_Inventory_Data();

        // Add Inventory Items to Queue
        for (int i = 0; i < items.Length; i++)
        {
            Inventory_List.Add(items[i]);
        }
    }

    private void Remove_Same()  // Remove same Item in Database
    {
        for(int i = 0; i < Inventory_List.Count; i++)
        {
            for(int j = 0; j < Database_List.Count; j++)
            {
                if (Database_List[j] == Inventory_List[i])
                {
                    Database_List.RemoveAt(j);
                }
            }
        }
    }

    private void Get_Random_Item()
    {
        int rand_index;
        for(int i = 0; i < 4; i++)
        {
            rand_index = Random.Range(0, Database_List.Count);

            Market_List.Add(Database_List[rand_index]);
            Debug.Log(Database_List[rand_index].name);

            Database_List.RemoveAt(rand_index);
        }
    }

    private void Set_Item_Slots()
    {
        for(int i = 0; i < Market_Slot_List.Count; i++)
        {
            if(Market_List.Count > 0)
            {
                GameObject spawned_object = obj_manager.Spawn_Market_Item(Market_Slot_List[i].transform.position, Market_List[i], Market_Slot_List[i]);
                Spawned_Items.Add(spawned_object);
            }
        }
    }

    private void Set_Price_Slot()
    {
        for(int i = 0; i < Price_Slot_List.Count; i++)
        {
            if(Market_List.Count > 0)
            {
                Price_Slot_List[i].GetComponent<TextMeshPro>().text = Market_List[i].item_Price.ToString();
            }
        }
    }

    private void Price_Slot_Remove(GameObject destroyed_item)
    {
        for(int i = 0; i < Spawned_Items.Count; i++) // 리스트에서 데이터 제거
        {
            if(destroyed_item == Spawned_Items[i])
            {
                Spawned_Items[i] = null;
            }
        }

        for (int i = 0; i < Spawned_Items.Count; i++)
        {
            if (Spawned_Items[i] == null)
            {
                Debug.Log("Do");
                Price_Slot_List[i].GetComponent<TextMeshPro>().text = " ";
            }
        }
    }

    public void Reroll_Market()
    {
        foreach (var go in Spawned_Items)
        {
            if (go != null)
            {
                Destroy(go);
            }
        }

        Spawned_Items.Clear();
        Market_List.Clear();
        Database_List.Clear();
        Inventory_List.Clear();

        Market_On_Sale();
        Set_Item_Slots();
        Set_Price_Slot();
    }
}
