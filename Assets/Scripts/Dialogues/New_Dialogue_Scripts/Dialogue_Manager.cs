using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;

    private Dictionary<int, Dialogue_Data> Dialogue_Dict = new Dictionary<int, Dialogue_Data>();
    private Dictionary<int, Item_Data> Item_Dict = new Dictionary<int, Item_Data>();

    private Queue<string> called_Name = new Queue<string>();
    private Queue<string> called_Script = new Queue<string>();

    [Header("Objects")]
    [SerializeField] private Text Name_Text;
    [SerializeField] private Text Script_Text;
    [SerializeField] private GameObject Dialogue_Canvas;

    [Header("Choose Object")]
    [SerializeField] private GameObject Chose_Cursor_01;
    [SerializeField] private GameObject Chose_Cursor_02;
    [SerializeField] private GameObject Chose_Text_01;
    [SerializeField] private GameObject Chose_Text_02;

    private bool is_First_Chosing = true;   //커서의 위치 true = 1번, false = 2번
    private bool is_Chose_Waiting = false;

    private bool is_Dialogue = false;

    [Header("NPCs")]
    [SerializeField] private GameObject Event_NPC;

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
    }

    // Update is called once per frame
    void Update()
    {
        //if (is_Dialogue && Input.GetKeyDown(KeyCode.Space))
        //{
        //    Print_Next_Dialogue();
        //}
        //else if (!is_Dialogue/* && is_Chose_Waiting */&& Input.GetKeyDown(KeyCode.Space))
        //{
        //    Chose_Complete();
        //}
        //else if (!is_Dialogue && (Input.GetKeyDown(KeyCode.DownArrow) || (Input.GetKeyDown(KeyCode.UpArrow))))
        //{
        //    Chose_Cursor_Move();
        //}
    }
    public void Start_Dialogue(int Num)
    {
        Call_Dialogue(Num);
        Dialogue_Canvas.SetActive(true);

        Chose_Text_01.SetActive(false);
        Chose_Text_02.SetActive(false);
        Chose_Cursor_01.SetActive(false);
        Chose_Cursor_02.SetActive(false);
    }

    private void Dialogue_Pharsing()
    {
        int? Lastkey = null;
        string path = Path.Combine(Application.streamingAssetsPath, "Hwatu" + ".csv"); // 파일 이름 넣기


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
                    Dialogue_Dict[Keyvalue].Scripts.Add(data[2]);
                }
                else
                {
                    Debug.LogError("Dictionary Error!");
                    return;
                }
            }
            //else if(data.Length != 3)
            //{
            //    Debug.LogError("Dialogue Length Error!");
            //}
            else
            {
                if (!Dialogue_Dict.ContainsKey(Keyvalue))   //키가 없으니 마지막키에 키값넣고, 데이터넣기
                {
                    //string.IsNullOrWhiteSpace - 빈칸검사

                    Lastkey = Keyvalue;
                    Temp_Data.Event_Num = Keyvalue;
                    Temp_Data.Character_Name.Add(data[1]);
                    Temp_Data.Scripts.Add(data[2]);

                    Dialogue_Dict.Add(Keyvalue, Temp_Data);
                }
            }
        }
    }
    private void Call_Dialogue(int Key)
    {
        is_Dialogue = true;
        foreach (string alpha in Dialogue_Dict[Key].Character_Name)
        {
            called_Name.Enqueue(alpha);
        }

        foreach (string beta in Dialogue_Dict[Key].Scripts)
        {
            called_Script.Enqueue(beta);
        }
        Print_Next_Dialogue();
    }
    private void Print_Next_Dialogue()
    {
        if (called_Script.Count > 0)
        {

            Dialogue_Canvas.SetActive(true);
            Name_Text.text = called_Name.Dequeue();
            Script_Text.text = called_Script.Dequeue();
        }
        else    //대화 완료 이후 선택지 실행문 (선택지 있는지 검사 필요!!) else if로 실행하고 이거 밑으로 내려서 else로 연결
        {
            //is_Chose_Waiting = true;

            //Dialogue_Canvas.SetActive(false);
            Chose_Text_01.SetActive(true);
            Chose_Text_02.SetActive(true);
            Chose_Cursor_01.SetActive(true);

            is_Dialogue = false;
        }
    }
    private void Chose_Cursor_Move()
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

    private void Chose_Complete()
    {
        if (is_First_Chosing)
        {
            Debug.Log("First Choice Debug");
            //Event_NPC.GetComponent<Dialogue_Choose_Manager>().Event_Start();
        }
        else
        {
            Debug.Log("Second Choice Debug");
        }
        Dialogue_Canvas.SetActive(false);
    }
}

public class Dialogue_Data
{
    public int Event_Num;

    public List<string> Character_Name = new List<string>();
    public List<string> Scripts = new List<string>();
}

public class Item_Data
{
    public int Item_Num;

    public string Item_Name;
    public string Item_Dialogue;
    public string Item_Effect_Script;
}
