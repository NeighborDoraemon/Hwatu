using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Dialogue_Manager : MonoBehaviour
{
    public static Dialogue_Manager instance;

    private Dictionary<int, Dialogue_Data> Dialogue_Dict = new Dictionary<int, Dialogue_Data>();
    private Dictionary<int, Item_Data> Item_Dict = new Dictionary<int, Item_Data>();

    private Queue<string> called_Name = new Queue<string>();
    private Queue<string> called_Script = new Queue<string>();
    private bool called_is_Choice;

    private Queue<string> called_Speaker = new Queue<string>();
    private Queue<string> called_Portrait = new Queue<string>();


    [Header("Objects")]
    [SerializeField] private Text Name_Text;
    [SerializeField] private Text Script_Text;
    [SerializeField] private GameObject Dialogue_Canvas;
    [SerializeField] private GameObject UI_Canvas;
    [SerializeField] private PlayerCharacter_Controller p_Controller;
    [SerializeField] private RawImage Portrait_Image;
    [SerializeField] private Text Obj_Interaction_Announce;

    [Header("Choose Object")]
    [SerializeField] private GameObject Chose_Cursor_01;
    [SerializeField] private GameObject Chose_Cursor_02;
    [SerializeField] private GameObject Chose_Text_01;
    [SerializeField] private GameObject Chose_Text_02;
    [SerializeField] private GameObject Chose_Btn_01;
    [SerializeField] private GameObject Chose_Btn_02;

    [Header("Key Actions")]
    [SerializeField] private InputActionReference input_Interaction;


    private bool is_First_Chosing = true;   //커서의 위치 true = 1번, false = 2번

    private GameObject Event_NPC;

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
        //Dialogue_Pharsing();
        StartCoroutine(Dialogue_Pharsing());
        //Item_Pharsing();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Encoding DetectEncoding(string path)
    {
        byte[] bom = new byte[4];
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            fs.Read(bom, 0, 3);
        }

        if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
        {
            return Encoding.UTF8;
        }
        else
        {
            return Encoding.UTF8;
        }
    }

    private IEnumerator Dialogue_Pharsing()
    {
        int? Lastkey = null;
        string path = Path.Combine(Application.streamingAssetsPath, "Hwatu_Dialogue.csv");

        //string[] Lines;

#if UNITY_ANDROID
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(path);
        yield return www.SendWebRequest();

        if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to load dialogue CSV: " + www.error);
            yield break;
        }

        Lines = www.downloadHandler.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
#else
    if (!File.Exists(path))
    {
        Debug.LogError("Path File Missing!");
        yield break;
    }
        Encoding encoding = DetectEncoding(path);
        string[] Lines = File.ReadAllLines(path, encoding);
        //Lines = File.ReadAllLines(path);
#endif

        for (int i = 1; i < Lines.Length; i++)
        {
            Dialogue_Data Temp_Data = new Dialogue_Data();

            string[] data = Lines[i].Trim().Split(',');

            if (!int.TryParse(data[0].Trim(), out int Keyvalue))
            {
                Debug.LogError($"[Row {i}] Event Number Conversion Error: {data[0]}");
                continue;
            }

            if (Lastkey == Keyvalue)
            {
                if (Dialogue_Dict.ContainsKey(Keyvalue))
                {
                    Dialogue_Dict[Keyvalue].Character_Name.Add(data[1].Trim());
                    Dialogue_Dict[Keyvalue].Scripts.Add(ReplaceSpecialChars(data[2]));

                    Dialogue_Dict[Keyvalue].Speaker.Add(data[4].Trim());
                    Dialogue_Dict[Keyvalue].Portrait.Add(data[5].Trim());
                }
                else
                {
                    Debug.LogError($"[Row {i}] Key {Keyvalue} not found in dictionary.");
                    continue;
                }
            }
            else
            {
                if (!Dialogue_Dict.ContainsKey(Keyvalue))
                {
                    Lastkey = Keyvalue;
                    Temp_Data.Event_Num = Keyvalue;
                    Temp_Data.Character_Name.Add(data[1].Trim());
                    Temp_Data.Scripts.Add(ReplaceSpecialChars(data[2]));

                    bool.TryParse(data[3], out Temp_Data.is_Choice);

                    Temp_Data.Speaker.Add(data[4].Trim());
                    Temp_Data.Portrait.Add(data[5].Trim());

                    Dialogue_Dict.Add(Keyvalue, Temp_Data);
                }
            }
        }

        Debug.Log("Dialogue CSV Parsing Complete. Loaded Keys: " + Dialogue_Dict.Count);
    }
    //private void Item_Pharsing()
    //{
    //    int? Lastkey = null;
    //    string path = Path.Combine(Application.streamingAssetsPath, "Hwatu_Item_Table" + ".csv"); // 파일 이름 넣기


    //    if (!File.Exists(path))
    //    {
    //        Debug.LogError("Path File Missing!");
    //        return;
    //    }
    //    string[] Lines = File.ReadAllLines(path);

    //    for (int i = 1; i < Lines.Length; i++)
    //    {
    //        Item_Data Temp_Data = new Item_Data();

    //        string[] data = Lines[i].Split(',');

    //        if (!int.TryParse(data[0], out int Keyvalue))
    //        {
    //            Debug.LogError("Item Index Error!");
    //            return;
    //        }

    //        if (Lastkey == Keyvalue) //키가 존재하니 추가하지 않음 (아이템은 중복 없음)
    //        {
    //            //if (Item_Dict.ContainsKey(Keyvalue))
    //            //{
    //            //    Item_Dict[Keyvalue].Item.Add(data[1]);
    //            //    Item_Dict[Keyvalue].Scripts.Add(ReplaceSpecialChars(data[2]));
    //            //}
    //            //else
    //            {
    //                Debug.LogError("Dictionary Error!");
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            if (!Dialogue_Dict.ContainsKey(Keyvalue))   //키가 없으니 마지막키에 키값넣고, 데이터넣기
    //            {
    //                Lastkey = Keyvalue;
    //                Temp_Data.Item_Num = Keyvalue;
    //                Temp_Data.Item_Name = data[1];
    //                Temp_Data.Item_Dialogue = ReplaceSpecialChars(data[2]);
    //                Temp_Data.Item_Effect_Script = ReplaceSpecialChars(data[3]);

    //                //bool.TryParse(data[3], out Temp_Data.is_Choice);

    //                Item_Dict.Add(Keyvalue, Temp_Data);
    //            }
    //        }
    //    }
    //}

    //public Item_Data Get_Item_Data(int Index)
    //{
    //    return Item_Dict[Index];
    //}

    private void Call_Dialogue(int Key)
    {
        called_Name.Clear();
        called_Script.Clear();
        called_Speaker.Clear();
        called_Portrait.Clear();

        foreach (string alpha in Dialogue_Dict[Key].Character_Name)
        {
            called_Name.Enqueue(alpha);
        }

        foreach (string beta in Dialogue_Dict[Key].Scripts)
        {
            called_Script.Enqueue(beta);
        }
        called_is_Choice = Dialogue_Dict[Key].is_Choice;

        foreach (string gamma in Dialogue_Dict[Key].Speaker)
        {
            called_Speaker.Enqueue(gamma);
        }

        foreach (string delta in Dialogue_Dict[Key].Portrait)
        {
            called_Portrait.Enqueue(delta);
        }

        if(Obj_Interaction_Announce != null)
        {
            string interactionText = input_Interaction.action.bindings[0].ToDisplayString();
            Obj_Interaction_Announce.text = "[" + interactionText + "] " + "계속";
        }


        //Debug.Log("Speaker Count : " + called_Speaker.Count);
        //Debug.Log("Portrait Count : " + called_Portrait.Count);

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

            if (called_Portrait.Count > 0 && !string.IsNullOrEmpty(called_Portrait.Peek()))
            {
                Portrait_Image.gameObject.SetActive(true);
                string path = $"Portraits/{called_Speaker.Dequeue()}/{called_Portrait.Dequeue()}";
                Sprite portraitSprite = Resources.Load<Sprite>(path);

                if (portraitSprite != null)
                {
                    Portrait_Image.texture = portraitSprite.texture;
                }
                else
                {
                    Debug.LogError($"Portrait not found at path: {path}");
                    Portrait_Image.gameObject.SetActive(false);
                }
            }
            else
            {
                Portrait_Image.gameObject.SetActive(false);
                Debug.Log("There's no portrait for this dialogue or the portrait is empty.");
            }
            //Portrait_Image.texture = Resources.Load<Texture2D>("Portraits/" + called_Portrait.Dequeue());
        }
        else    //대화 완료 이후 선택지 실행문 (선택지 있는지 검사 필요!!) else if로 실행하고 이거 밑으로 내려서 else로 연결
        {
            if (called_is_Choice)   //선택지 실행
            {
                //선택지 pc버전
                Chose_Text_01.SetActive(true);
                Chose_Text_02.SetActive(true);
                Chose_Cursor_01.SetActive(true);

                // 선택지 모바일 버전
                //Chose_Btn_01.SetActive(true);
                //Chose_Btn_02.SetActive(true);

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

        Chose_Btn_01.SetActive(false);
        Chose_Btn_02.SetActive(false);
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

    public void Btn_Choice_Accept()
    {
        is_First_Chosing = true;
        Chose_Btn_01.SetActive(false);
        Chose_Btn_02.SetActive(false);
        Chose_Complete();
    }

    public void Btn_Choice_Cancel()
    {
        is_First_Chosing = false;
        Chose_Btn_01.SetActive(false);
        Chose_Btn_02.SetActive(false);
        Chose_Complete();
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

    public List<string> Speaker = new List<string>();
    public List<string> Portrait = new List<string>();
}

public class Item_Data
{
    public int Item_Num;

    public string Item_Name;
    public string Item_Dialogue;
    public string Item_Effect_Script;
}