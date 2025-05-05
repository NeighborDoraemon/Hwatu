using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.InputSystem.XR;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;

    private Dictionary<int, Dialogue_Data> Dialogue_Dict = new Dictionary<int, Dialogue_Data>();
    private Dictionary<int, Item_Data> Item_Dict = new Dictionary<int, Item_Data>();

    private Queue<string> called_Name = new Queue<string>();
    private Queue<string> called_Script = new Queue<string>();
    private bool called_is_Choice;

    [Header("Objects")]
    [SerializeField] private Text Name_Text;
    [SerializeField] private Text Script_Text;
    [SerializeField] private GameObject Dialogue_Canvas;
    [SerializeField] private GameObject UI_Canvas;
    [SerializeField] private PlayerCharacter_Controller p_Controller;

    [Header("Choose Object")]
    [SerializeField] private GameObject Chose_Cursor_01;
    [SerializeField] private GameObject Chose_Cursor_02;
    [SerializeField] private GameObject Chose_Text_01;
    [SerializeField] private GameObject Chose_Text_02;

    private bool is_First_Chosing = true;   //커서의 위치 true = 1번, false = 2번

    private GameObject Event_NPC;

    private int Temp = 5001;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Dialogue_Pharsing();
        Item_Pharsing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Dialogue_Pharsing()
    {
        int? Lastkey = null;
        string path = Path.Combine(Application.streamingAssetsPath, "Hwatu_Dialogue" + ".csv"); // 파일 이름 넣기


        if (!File.Exists(path))
        {
            Debug.LogError("Path File Missing!");
            return;
        }
        string[] Lines = File.ReadAllLines(path);

        for (int i = 1; i < Lines.Length; i++)
        {
            Dialogue_Data Temp_Data = new Dialogue_Data();

            string[] data = Lines[i].Split(',');

            if (!int.TryParse(data[0], out int Keyvalue))
            {
                Debug.LogError("Event Number Conversion Error!");
                return;
            }

            if (Lastkey == Keyvalue) //키가 존재하니 바로 문장 추가
            {
                if (Dialogue_Dict.ContainsKey(Keyvalue))
                {
                    Dialogue_Dict[Keyvalue].Character_Name.Add(data[1]);
                    Dialogue_Dict[Keyvalue].Scripts.Add(ReplaceSpecialChars(data[2]));
                }
                else
                {
                    Debug.LogError("Dictionary Error!");
                    return;
                }
            }
            else
            {
                if (!Dialogue_Dict.ContainsKey(Keyvalue))   //키가 없으니 마지막키에 키값넣고, 데이터넣기
                {
                    Lastkey = Keyvalue;
                    Temp_Data.Event_Num = Keyvalue;
                    Temp_Data.Character_Name.Add(data[1]);
                    Temp_Data.Scripts.Add(ReplaceSpecialChars(data[2]));

                    bool.TryParse(data[3], out Temp_Data.is_Choice);    //bool 데이터 파싱 성공시 데이터 입력. 실패시 false 입력

                    Dialogue_Dict.Add(Keyvalue, Temp_Data);
                }
            }
        }
    }

    private void Item_Pharsing()
    {
        int? Lastkey = null;
        string path = Path.Combine(Application.streamingAssetsPath, "Hwatu_Item_Table" + ".csv"); // 파일 이름 넣기


        if (!File.Exists(path))
        {
            Debug.LogError("Path File Missing!");
            return;
        }
        string[] Lines = File.ReadAllLines(path);

        for (int i = 1; i < Lines.Length; i++)
        {
            Item_Data Temp_Data = new Item_Data();

            string[] data = Lines[i].Split(',');

            if (!int.TryParse(data[0], out int Keyvalue))
            {
                Debug.LogError("Item Index Error!");
                return;
            }

            if (Lastkey == Keyvalue) //키가 존재하니 추가하지 않음 (아이템은 중복 없음)
            {
                //if (Item_Dict.ContainsKey(Keyvalue))
                //{
                //    Item_Dict[Keyvalue].Item.Add(data[1]);
                //    Item_Dict[Keyvalue].Scripts.Add(ReplaceSpecialChars(data[2]));
                //}
                //else
                {
                    Debug.LogError("Dictionary Error!");
                    return;
                }
            }
            else
            {
                if (!Dialogue_Dict.ContainsKey(Keyvalue))   //키가 없으니 마지막키에 키값넣고, 데이터넣기
                {
                    Lastkey = Keyvalue;
                    Temp_Data.Item_Num = Keyvalue;
                    Temp_Data.Item_Name = data[1];
                    Temp_Data.Item_Dialogue = ReplaceSpecialChars(data[2]);
                    Temp_Data.Item_Effect_Script = ReplaceSpecialChars(data[3]);

                    //bool.TryParse(data[3], out Temp_Data.is_Choice);

                    Item_Dict.Add(Keyvalue, Temp_Data);
                }
            }
        }
    }

    public Item_Data Get_Item_Data(int Index)
    {
        return Item_Dict[Index];
    }

    private void Call_Dialogue(int Key)
    {
        called_Name.Clear();
        called_Script.Clear();

        foreach (string alpha in Dialogue_Dict[Key].Character_Name)
        {
            called_Name.Enqueue(alpha);
        }

        foreach (string beta in Dialogue_Dict[Key].Scripts)
        {
            called_Script.Enqueue(beta);
        }
        called_is_Choice = Dialogue_Dict[Key].is_Choice;

        Print_Next_Dialogue();
    }

    public void Print_Next_Dialogue()
    {
        if (called_Script.Count > 0)
        {
            Dialogue_Canvas.SetActive(true);
            UI_Canvas.SetActive(false);
            Name_Text.text = called_Name.Dequeue();
            Script_Text.text = called_Script.Dequeue();
        }
        else    //대화 완료 이후 선택지 실행문 (선택지 있는지 검사 필요!!) else if로 실행하고 이거 밑으로 내려서 else로 연결
        {
            if (called_is_Choice)   //선택지 실행
            {
                Chose_Text_01.SetActive(true);
                Chose_Text_02.SetActive(true);
                Chose_Cursor_01.SetActive(true);

                p_Controller.State_Change(PlayerCharacter_Controller.Player_State.Dialogue_Choice);

                is_First_Chosing = true;
            }
            else
            {
                Dialogue_Canvas.SetActive(false);
                UI_Canvas.SetActive(true);
                Event_NPC.GetComponent<Npc_Interface>().Npc_Interaction_End();
            }
        }
    }
    public void Chose_Cursor_Move()
    {
        if (is_First_Chosing)
        {
            Chose_Cursor_01.SetActive(false);
            Chose_Cursor_02.SetActive(true);
            is_First_Chosing = false;
        }
        else
        {
            Chose_Cursor_01.SetActive(true);
            Chose_Cursor_02.SetActive(false);
            is_First_Chosing = true;
        }
    }

    public void Start_Dialogue(int Num)
    {
        Call_Dialogue(Num);
        Dialogue_Canvas.SetActive(true);
        UI_Canvas.SetActive(false);

        Chose_Text_01.SetActive(false);
        Chose_Text_02.SetActive(false);
        Chose_Cursor_01.SetActive(false);
        Chose_Cursor_02.SetActive(false);
    }
    public void Chose_Complete()
    {
        if (is_First_Chosing)
        {
            Debug.Log("First Choice Debug");
            Event_NPC.GetComponent<Npc_Interface>().Event_Start();
        }
        else
        {
            Debug.Log("Second Choice Debug");
            Event_NPC.GetComponent<Npc_Interface>().Npc_Interaction_End();
        }
        Dialogue_Canvas.SetActive(false);
        UI_Canvas.SetActive(true);
    }

    public void Get_Npc_Data(GameObject npc)
    {
        Event_NPC = npc;
    }

    // 기호를 콤마로 변환하는 메서드 추가
    private string ReplaceSpecialChars(string input)
    {
        return input.Replace("^", ",");
                    //.Replace("※", ",")
                    //.Replace("//", ",");
    }
}

public class Dialogue_Data
{
    public int Event_Num;

    public List<string> Character_Name = new List<string>();
    public List<string> Scripts = new List<string>();

    public bool is_Choice;
}

public class Item_Data
{
    public int Item_Num;

    public string Item_Name;
    public string Item_Dialogue;
    public string Item_Effect_Script;
}
