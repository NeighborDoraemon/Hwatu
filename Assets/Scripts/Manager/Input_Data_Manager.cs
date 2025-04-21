using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public enum Key_Enum 
{
    Left = 0, 
    Right = 1, 
    Jump = 2, 
    Teleport = 3, 
    Attack = 4, 
    Skill = 5, 
    Interaction = 6,
    Up = 7,
    Down = 8,
    Change_First = 9,
    Change_Second = 10,
    Escape = 11,
    Inventory = 12,
    KeyCount = 13
}


public class Input_Data_Manager : MonoBehaviour
{
    private Player_InputActions inputActions;

    [Header("Lists")]
    [SerializeField] private List<InputActionReference> Player_Input_List = new List<InputActionReference>();
    [SerializeField] private List<Text> Input_Text = new List<Text>();


    [Header("Key Change")]
    [SerializeField] private PlayerInput player_Input;
    [SerializeField] private PlayerCharacter_Controller player_Con;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private const string RebindsKey = "Rebinds";
    private bool is_Rebinding_Now = false;


    [Header("Canvas")]
    [SerializeField] private Canvas Main_Can;
    [SerializeField] private Canvas Option_Can;
    [SerializeField] private Canvas Pause_Can;

    private bool is_Option_Setting = false;

    [Header("Key Objects")]
    [SerializeField] private List<GameObject> Key_Objects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.HasKey(RebindsKey))
        {
            string rebinds = PlayerPrefs.GetString("Rebinds", string.Empty);

            if (string.IsNullOrEmpty(rebinds))
            {
                return;
            }

            player_Input.actions.LoadBindingOverridesFromJson(rebinds);

            Change_Key_Objects();
        }


        for (int i = 0; i < (int)Key_Enum.KeyCount; i++)
        {
            //Debug.Log("시작 키 호출");
            if (i == 0 || i == 1)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[1].action.bindings[i + 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else if(i == 7 || i == 8)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i - 7]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[7].action.bindings[i - 6].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[0]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape) && is_Option_Setting == true)
        //{
        //    Btn_Option_Quit();
        //}
    }

    public void Btn_Key_Change(int alpha)
    {
        if (!is_Rebinding_Now)
        {
            is_Rebinding_Now = true;
            StartRebinding(alpha);
        }
    }

    public void StartRebinding(int i_key_count)
    {
        if (i_key_count == 0 || i_key_count == 1)
        {
            int bindingIndex = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count]);
            int bindingIndex_02 = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count]);
            player_Input.SwitchCurrentActionMap("Menu");

            //Player_Input_List[i_key_count].action.Disable();

            rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
                  .WithControlsExcluding("Mouse")
                  .OnMatchWaitForAnother(0.1f)
                  .OnComplete(operation => {
                      string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

                      if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, bindingIndex_02))
                      {
                          Debug.Log("키 중복!");
                          StartRebinding(i_key_count);
                      }
                      else
                      {
                          Debug.Log("정상 등록");
                          RebinComplete(i_key_count);
                      }
                  })
                  .Start();
        }
        else if(i_key_count == 7 || i_key_count == 8)
        {
            int bindingIndex = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count - 7]);
            int bindingIndex_02 = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count - 7]);
            player_Input.SwitchCurrentActionMap("Menu");

            //Player_Input_List[i_key_count].action.Disable();

            rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
                  .WithControlsExcluding("Mouse")
                  .OnMatchWaitForAnother(0.1f)
                  .OnComplete(operation =>
                  {
                      string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

                      if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, bindingIndex_02))
                      {
                          Debug.Log("키 중복!");
                          StartRebinding(i_key_count);
                      }
                      else
                      {
                          Debug.Log("정상 등록");
                          RebinComplete(i_key_count);
                      }
                  })
                  .Start();
        }
        else
        {
            player_Input.SwitchCurrentActionMap("Menu");

            //Player_Input_List[i_key_count].action.Disable();

            rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding()
                  .WithControlsExcluding("Mouse")
                  .OnMatchWaitForAnother(0.1f)
                  .OnComplete(operation => {
                      string newBindingPath = Player_Input_List[i_key_count].action.bindings[0].effectivePath;

                      if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, 0))
                      {
                          Debug.Log("키 중복!");
                          StartRebinding(i_key_count);
                      }
                      else
                      {
                          Debug.Log("정상 등록");
                          RebinComplete(i_key_count);
                      }
                  })
                  .Start();
        }
    }

    private void RebinComplete(int alpha)
    {
        //Player_Input_List[alpha].action.ApplyBindingOverride("");

        for (int i = 0; i < (int)Key_Enum.KeyCount; i++)
        {
            if (i == 0 || i == 1)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[i + 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else if(i == 7 || i == 8)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i - 7]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[i - 6].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[0]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }
        //Player_Input_List[alpha].action.Enable();
        is_Rebinding_Now = false;
        rebindingOperation.Dispose();

        player_Input.SwitchCurrentActionMap("Player");
    }


    public void Save_Keys()
    {
        string rebinds = player_Input.actions.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("Rebinds", rebinds);

        Change_Key_Objects();
    }

    public void Btn_Option()
    {
        //player_Con.is_Key_Setting = true;

        if (Main_Can != null)
        {
            Main_Can.gameObject.SetActive(false);
        }
        Option_Can.gameObject.SetActive(true);
        is_Option_Setting = true;
    }

    public void Btn_Option_Quit()
    {
        if (is_Option_Setting == true)
        {
            Option_Can.gameObject.SetActive(false);
            if (Main_Can != null)
            {
                Main_Can.gameObject.SetActive(true);
            }
            is_Option_Setting = false;
            //player_Con.is_Key_Setting = true;
        }
    }

    public void Btn_Quit()
    {
        Application.Quit();
    }

    private void Change_Key_Objects()
    {
        foreach(GameObject g_Obj in Key_Objects)
        {
            g_Obj.GetComponent<Obj_KeyBoard>().Call_KeyObjects();
        }
    }

    private bool Check_Same(InputAction currentAction, string newPath, int currentBindingIndex)
    {
        for (int i = 0; i < Player_Input_List.Count; i++)
        {
            var action = Player_Input_List[i].action;

            for (int j = 0; j < action.bindings.Count; j++)
            {
                // 자기 자신의 바인딩은 건너뛴다 (같은 액션 + 같은 바인딩 인덱스)
                if (action == currentAction && j == currentBindingIndex)
                    continue;

                if (action.bindings[j].effectivePath == newPath)
                {
                    Debug.Log($"중복 발생! {newPath} 는 {action.name}의 {j}번 바인딩과 겹침");
                    return true;
                }
            }
        }

        return false;
    }
}
