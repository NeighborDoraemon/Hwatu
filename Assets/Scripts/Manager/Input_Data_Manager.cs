using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;


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
    [SerializeField] private List<GameObject> Key_Warning_Objects = new List<GameObject>();


    [Header("Key Change")]
    [SerializeField] private PlayerInput player_Input;
    [SerializeField] private PlayerCharacter_Controller player_Con;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private const string RebindsKey = "Rebinds";
    private bool is_Rebinding_Now = false;


    [Header("Canvas")]
    [SerializeField] private Canvas Main_Can;
    [SerializeField] private Canvas Option_Can;
    //[SerializeField] private Canvas Pause_Can;

    [Header("Panel")]
    [SerializeField] private GameObject Panel_Warning;

    private bool is_Option_Setting = false;

    [Header("Key Objects")]
    [SerializeField] private List<GameObject> Key_Objects = new List<GameObject>();

    [Header("Scroll Objects")]
    [SerializeField] private RectTransform ViewPort;
    [SerializeField] private GameObject Warning_Up;
    [SerializeField] private GameObject Warning_Down;

    [Header("Audio Manager")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Slider master_Slider;
    [SerializeField] private Slider bgm_Slider;
    [SerializeField] private Slider sfx_Slider;
    [SerializeField] private bool slider_Use_0to10 = true;

    private int Samed_Index = 0; // for warning

    //for exchange
    private string tempOldBindingPath;

    private bool is_Change_Exist = false;   //변경점이 있다 = true
    private bool is_Warning_Print = false; //경고문구 출력 여부



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


    public void CheckWarningVisibility()
    {
        Rect viewportWorldRect = GetWorldRect(ViewPort);

        int Above_Count = 0;
        int Below_Count = 0;

        foreach (GameObject icon in Key_Warning_Objects)
        {
            if (icon == null || !icon.activeInHierarchy)
                continue;

            RectTransform iconRect = icon.GetComponent<RectTransform>();
            Rect iconWorldRect = GetWorldRect(iconRect);

            // 화면에 안 보이는 아이콘만 따짐
            if (!viewportWorldRect.Overlaps(iconWorldRect, true))
            {
                float iconY = iconWorldRect.center.y;
                float viewMinY = viewportWorldRect.yMin;
                float viewMaxY = viewportWorldRect.yMax;

                if (iconY > viewMaxY)
                {
                    Above_Count++;
                }
                else if (iconY < viewMinY)
                {
                    Below_Count++;
                }
            }
        }

        // 결과 처리
        if (Above_Count > 0)
        {
            //Debug.Log("▲ 위쪽에 안 보이는 경고 있음");
            Warning_Up.SetActive(true);
        }
        else
        {
            Warning_Up.SetActive(false);
        }

        if (Below_Count > 0)
        {
            //Debug.Log("▼ 아래쪽에 안 보이는 경고 있음");
            Warning_Down.SetActive(true);
        }
        else
        {
            Warning_Down.SetActive(false);
        }
    }

    private Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        Vector3 bottomLeft = corners[0]; // corners[0] = BottomLeft
        Vector3 topRight = corners[2];   // corners[2] = TopRight

        return new Rect(bottomLeft, topRight - bottomLeft);
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
              .WithCancelingThrough("") // 취소 키 비활성화
              .OnComplete(operation =>
              {
                  string newPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;
                  string newBindingPath = Player_Input_List[i_key_count].action.bindings[bindingIndex].effectivePath;

                  if (newPath == oldPath)
                  {
                      Debug.Log("동일한 키 입력됨 - 아무것도 안 바뀜");
                      is_Change_Exist = false;
                  }
                  else
                  {
                      is_Change_Exist = true;
                  }

                      var conflict = Check_Same(action, newBindingPath, bindingIndex, i_key_count);

                  RebinComplete(i_key_count, bindingIndex);

                  if (conflict != null)
                  {
                      Debug.Log("중복 키 발견 -> 기존 키로 2차 리바인딩 강제 적용");

                      // 충돌된 액션에 oldPath 적용
                      player_Input.SwitchCurrentActionMap("Menu");

                      ApplyForcedBinding(conflict.Value.conflictAction, conflict.Value.conflictBindingIndex, oldPath);

                      player_Input.SwitchCurrentActionMap("Player");

                      // UI 반영
                      Set_Texts();
                  }
              })
              .Start();
    }

    private void RebinComplete(int actionIndex, int bindingIndex)
    {
        var path = Player_Input_List[actionIndex].action.bindings[bindingIndex].effectivePath;
        Input_Text[actionIndex].text = InputControlPath.ToHumanReadableString(path, InputControlPath.HumanReadableStringOptions.OmitDevice);

        if (is_Change_Exist)
        {
            Key_Warning_Objects[actionIndex].SetActive(true);
            CheckWarningVisibility();
        }

        is_Rebinding_Now = false;
        rebindingOperation.Dispose();

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

        foreach(GameObject Warnings in Key_Warning_Objects)
        {
            Warnings.SetActive(false);
        }

        if (audioManager && master_Slider && bgm_Slider && sfx_Slider)
        {
            float k = slider_Use_0to10 ? 0.1f : 1.0f;
            float m = Mathf.Clamp01(master_Slider.value * k);
            float b = Mathf.Clamp01(bgm_Slider.value * k);
            float s = Mathf.Clamp01(sfx_Slider.value * k);

            audioManager.Apply_Volumes01(m, b, s, save: true);
        }
        else
        {
            float k = slider_Use_0to10 ? 0.1f : 1.0f;
            if (master_Slider) PlayerPrefs.SetFloat("AM_Master_Lin", Mathf.Clamp01(master_Slider.value * k));
            if (bgm_Slider) PlayerPrefs.SetFloat("AM_Bgm_Lin", Mathf.Clamp01(bgm_Slider.value * k));
            if (sfx_Slider) PlayerPrefs.SetFloat("AM_Sfx_Lin", Mathf.Clamp01(sfx_Slider.value * k));
            PlayerPrefs.Save();
        }
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

        Load_Audio_Sliders_FromSaved();
    }

    public void Btn_Option_Quit()
    {
        //if (is_Option_Setting == true)
        //{
        //    Option_Can.gameObject.SetActive(false);
        //    if (Main_Can != null)
        //    {
        //        Main_Can.gameObject.SetActive(true);
        //    }
        //    is_Option_Setting = false;
        //    //player_Con.is_Key_Setting = true;
        //}
        if (PlayerPrefs.HasKey(RebindsKey))
        {
            string rebinds = PlayerPrefs.GetString("Rebinds", string.Empty);

            if (string.IsNullOrEmpty(rebinds))
            {
                return;
            }

            player_Input.actions.LoadBindingOverridesFromJson(rebinds);

            Change_Key_Objects();

            foreach (GameObject Warnings in Key_Warning_Objects)
            {
                Warnings.SetActive(false);
            }
        }
        Set_Texts();

        if (audioManager)
        {
            float m = PlayerPrefs.GetFloat("AM_Master_Lin", audioManager.GetMaster01());
            float b = PlayerPrefs.GetFloat("AM_Bgm_Lin", audioManager.GetBgm01());
            float s = PlayerPrefs.GetFloat("AM_Sfx_Lin", audioManager.GetSfx01());

            audioManager.Apply_Volumes01(m, b, s, save: false);
        }
        Load_Audio_Sliders_FromSaved();
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

    private (InputAction conflictAction, int conflictBindingIndex)? Check_Same(InputAction currentAction, string newPath, int currentBindingIndex, int Key_Index)
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
                    Samed_Index = i;
                    if(Key_Index == 0 && Samed_Index == 0)
                    {
                        Samed_Index = 1;
                    }
                    else if(Key_Index == 7 && Samed_Index == 7)
                    {
                        Samed_Index = 8;
                    }
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

        Key_Warning_Objects[Samed_Index].SetActive(true);
        CheckWarningVisibility();

        // UI 업데이트 등 필요한 후처리
        int actionIndex = Player_Input_List.FindIndex(p => p.action == action);
        if (actionIndex >= 0)
        {
            RebinComplete(actionIndex, bindingIndex);
        }
    }

    private void Load_Audio_Sliders_FromSaved()
    {
        if (!master_Slider || !bgm_Slider || !sfx_Slider) return;

        float m = audioManager ? audioManager.GetMaster01() : PlayerPrefs.GetFloat("AM_Master_Lin", 0.8f);
        float b = audioManager ? audioManager.GetBgm01() : PlayerPrefs.GetFloat("AM_Bgm_Lin", 0.8f);
        float s = audioManager ? audioManager.GetSfx01() : PlayerPrefs.GetFloat("AM_Sfx_Lin", 0.8f);

        float k = slider_Use_0to10 ? 10.0f : 1.0f;
        master_Slider.value = m * k;
        bgm_Slider.value = b * k;
        sfx_Slider.value = s * k;
    }
}
