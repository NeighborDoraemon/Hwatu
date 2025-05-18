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

    //for exchange
    private string tempOldBindingPath;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey(RebindsKey))
        {
            string rebinds = PlayerPrefs.GetString("Rebinds", string.Empty);

            if (string.IsNullOrEmpty(rebinds))
            {
                return;
            }

            player_Input.actions.LoadBindingOverridesFromJson(rebinds);

            Change_Key_Objects();
        }
        Set_Texts();
    }

    private void Set_Texts()
    {
        for (int i = 0; i < (int)Key_Enum.KeyCount; i++)
        {
            //Debug.Log("시작 키 호출");
            if (i == 0 || i == 1)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[1].action.bindings[i + 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else if (i == 7 || i == 8)
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

    }

    public void Btn_Key_Change(int Action_Index)
    {
        int Binding_Index = 0;

        if (Action_Index == 0 || Action_Index == 1)
        {
            Binding_Index = Action_Index + 1;
        }
        else if (Action_Index == 7 || Action_Index == 8)
        {
            Binding_Index = Action_Index - 6;
        }
        else
        {
            Binding_Index = 0;
        }

        if (!is_Rebinding_Now)
        {
            is_Rebinding_Now = true;
            Debug.Log("Start Rebinding");
            StartRebinding(Action_Index, Binding_Index);
        }
    }
    public void StartRebinding(int i_key_count, int bindingIndex)
    {
        var action = Player_Input_List[i_key_count].action;

        player_Input.SwitchCurrentActionMap("Menu");
        string oldPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

        //Player_Input_List[i_key_count].action.Disable();

        rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
              .WithControlsExcluding("Mouse")
              .OnMatchWaitForAnother(0.1f)
              .OnComplete(operation =>
              {
                  string newPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;
                  string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

                  var conflict = Check_Same(action, newBindingPath, bindingIndex);

                  RebinComplete(i_key_count, bindingIndex);

                  if (conflict != null)
                  {
                      Debug.Log("중복 키 발견 -> 기존 키로 2차 리바인딩 강제 적용");

                      // 충돌된 액션에 oldPath 적용
                      player_Input.SwitchCurrentActionMap("Menu");

                      ApplyForcedBinding(conflict.Value.conflictAction, conflict.Value.conflictBindingIndex, oldPath);

                      player_Input.SwitchCurrentActionMap("Player");

                      // UI 반영
                      UpdateUIFor(conflict.Value.conflictAction, conflict.Value.conflictBindingIndex);
                  }
              })
              .Start();
    }

    //public void StartRebinding(int i_key_count)
    //{
    //    if (i_key_count == 0 || i_key_count == 1)
    //    {
    //        int bindingIndex = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count]);
    //        int bindingIndex_02 = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count]);

    //        var action = Player_Input_List[i_key_count].action;

    //        player_Input.SwitchCurrentActionMap("Menu");

    //        //Player_Input_List[i_key_count].action.Disable();

    //        rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
    //              .WithControlsExcluding("Mouse")
    //              .OnMatchWaitForAnother(0.1f)
    //              .OnComplete(operation => {
    //                  string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

    //                  var conflict = Check_Same(action, newBindingPath, bindingIndex);

    //                  if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, bindingIndex_02))
    //                  {
    //                      Debug.Log("키 중복!");
    //                      StartRebinding(i_key_count);
    //                  }
    //                  else
    //                  {
    //                      Debug.Log("정상 등록");
    //                      RebinComplete(i_key_count);
    //                  }
    //              })
    //              .Start();
    //    }
    //    else if(i_key_count == 7 || i_key_count == 8)
    //    {
    //        int bindingIndex = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count - 7]);
    //        int bindingIndex_02 = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count - 7]);

    //        player_Input.SwitchCurrentActionMap("Menu");

    //        //Player_Input_List[i_key_count].action.Disable();

    //        rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
    //              .WithControlsExcluding("Mouse")
    //              .OnMatchWaitForAnother(0.1f)
    //              .OnComplete(operation =>
    //              {
    //                  string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

    //                  if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, bindingIndex_02))
    //                  {
    //                      Debug.Log("키 중복!");
    //                      StartRebinding(i_key_count);
    //                  }
    //                  else
    //                  {
    //                      Debug.Log("정상 등록");
    //                      RebinComplete(i_key_count);
    //                  }
    //              })
    //              .Start();
    //    }
    //    else
    //    {
    //        player_Input.SwitchCurrentActionMap("Menu");

    //        //Player_Input_List[i_key_count].action.Disable();

    //        rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding()
    //              .WithControlsExcluding("Mouse")
    //              .OnMatchWaitForAnother(0.1f)
    //              .OnComplete(operation => {
    //                  string newBindingPath = Player_Input_List[i_key_count].action.bindings[0].effectivePath;

    //                  if (Check_Same(Player_Input_List[i_key_count].action, newBindingPath, 0))
    //                  {
    //                      Debug.Log("키 중복!");
    //                      StartRebinding(i_key_count);
    //                  }
    //                  else
    //                  {
    //                      Debug.Log("정상 등록");
    //                      RebinComplete(i_key_count);
    //                  }
    //              })
    //              .Start();
    //    }
    //}

    private void RebinComplete(int actionIndex, int bindingIndex)
    {
        var path = Player_Input_List[actionIndex].action.bindings[bindingIndex].effectivePath;
        Input_Text[actionIndex].text = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);

        is_Rebinding_Now = false;
        rebindingOperation.Dispose();
        //rebindOperation.Dispose();

        player_Input.SwitchCurrentActionMap("Player");
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
            else if (i == 7 || i == 8)
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
        foreach (GameObject g_Obj in Key_Objects)
        {
            g_Obj.GetComponent<Obj_KeyBoard>().Call_KeyObjects();
        }
    }

    //private bool Check_Same(InputAction currentAction, string newPath, int currentBindingIndex)
    //{
    //    for (int i = 0; i < Player_Input_List.Count; i++)
    //    {
    //        var action = Player_Input_List[i].action;

    //        for (int j = 0; j < action.bindings.Count; j++)
    //        {
    //            // 자기 자신의 바인딩은 건너뛴다 (같은 액션 + 같은 바인딩 인덱스)
    //            if (action == currentAction && j == currentBindingIndex)
    //                continue;

    //            if (action.bindings[j].effectivePath == newPath)
    //            {
    //                Debug.Log($"중복 발생! {newPath} 는 {action.name}의 {j}번 바인딩과 겹침");
    //                //SwapBindings(currentAction, currentBindingIndex, action, j);
    //                return true;
    //            }
    //        }
    //    }

    //    return false;
    //}

    private (InputAction conflictAction, int conflictBindingIndex)? Check_Same(InputAction currentAction, string newPath, int currentBindingIndex)
    {
        for (int i = 0; i < Player_Input_List.Count; i++)
        {
            var action = Player_Input_List[i].action;

            for (int j = 0; j < action.bindings.Count; j++)
            {
                if (action == currentAction && j == currentBindingIndex)
                    continue;

                if (action.bindings[j].effectivePath == newPath)
                {
                    Debug.Log($"중복 발생! {newPath} 는 {action.name}의 {j}번 바인딩과 겹침");
                    return (action, j);
                }
            }
        }

        return null;
    }

    private void StartSecondaryRebinding(int actionIndex, InputAction conflictAction, int conflictBindingIndex)
    {
        conflictAction.Disable();
        // Player_Input_List에서 conflictAction에 해당하는 인덱스 찾기
        int conflictActionIndex = Player_Input_List.FindIndex(p => p.action == conflictAction);

        if (conflictActionIndex < 0)
        {
            Debug.LogError("2차 리바인딩 대상 액션을 찾지 못했습니다.");
            return;
        }

        int rebindingIndex = conflictBindingIndex;

        //// 0,1번 예외 처리: 바인딩 인덱스 조정 필요하면 여기서 하자
        //if (conflictActionIndex == 0 || conflictActionIndex == 1)
        //{
        //    // conflictBindingIndex가 액션 내부 composite 바인딩의 세부 인덱스라면,
        //    // 실제 바인딩 인덱스로 변환하거나 맞춰줘야 함 (예시)
        //    // rebindingIndex = conflictBindingIndex + 1; // 필요시 수정
        //}
        //else if (conflictActionIndex == 7 || conflictActionIndex == 8)
        //{
        //    // 7,8번도 마찬가지
        //    // rebindingIndex = conflictBindingIndex - 6; // 필요시 수정
        //}
        //else
        //{
        //    // 그 외 일반 액션들은 conflictBindingIndex 그대로 사용
        //}

        var action = Player_Input_List[conflictActionIndex].action;

        rebindingOperation = action.PerformInteractiveRebinding(rebindingIndex)
            .WithControlsExcluding("Mouse")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                Debug.Log($"2차 리바인딩 완료: {action.name}의 {rebindingIndex}번 바인딩");

                RebinComplete(actionIndex, conflictBindingIndex);
                //RebinComplete(conflictActionIndex);

                operation.Dispose();
                conflictAction.Enable();
            })
            .Start();

        is_Rebinding_Now = true;
    }

    private void ApplyForcedBinding(InputAction action, int bindingIndex, string newPath)
    {
        action.RemoveBindingOverride(bindingIndex);
        // 바인딩 변경
        action.ApplyBindingOverride(bindingIndex, newPath);

        // UI 업데이트 등 필요한 후처리
        int actionIndex = Player_Input_List.FindIndex(p => p.action == action);
        if (actionIndex >= 0)
        {
            RebinComplete(actionIndex, bindingIndex);
        }
    }

    void UpdateUIFor(InputAction action, int bindingIndex)
    {
        //for (int i = 0; i < Player_Input_List.Count; i++)
        //{
        //    if (Player_Input_List[i].action == action)
        //    {
        //        var path = action.bindings[bindingIndex].effectivePath;
        //        Input_Text[i].text = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);
        //        break;
        //    }
        //}
        Set_Texts();
    }

}
