using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public enum Key_Enum { Left = 0, Right = 1, Jump = 2, Teleport = 3, Attack = 4, Skill = 5, Interaction = 6, KeyCount = 7 }


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


    [Header("Canvas")]
    [SerializeField] private Canvas Main_Can;
    [SerializeField] private Canvas Option_Can;

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
        if (Input.GetKeyDown(KeyCode.Escape) && is_Option_Setting == true)
        {
            Btn_Option_Quit();
        }
    }

    public void Btn_Key_Change(int alpha)
    {
        StartRebinding(alpha);
    }

    public void StartRebinding(int i_key_count)
    {
        if (i_key_count == 0 || i_key_count == 1)
        {
            int bindingIndex = Player_Input_List[i_key_count].action.GetBindingIndexForControl(Player_Input_List[i_key_count].action.controls[i_key_count]);
            player_Input.SwitchCurrentActionMap("Menu");

            rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding(bindingIndex)
                  .WithControlsExcluding("Mouse")
                  .OnMatchWaitForAnother(0.1f)
                  .OnComplete(operation => RebinComplete(i_key_count))
                  .Start();
        }
        else
        {
            player_Input.SwitchCurrentActionMap("Menu");

            rebindingOperation = Player_Input_List[i_key_count].action.PerformInteractiveRebinding()
                  .WithControlsExcluding("Mouse")
                  .OnMatchWaitForAnother(0.1f)
                  .OnComplete(operation => RebinComplete(i_key_count))
                  .Start();
        }
    }

    private void RebinComplete(int alpha)
    {
        for (int i = 0; i < (int)Key_Enum.KeyCount; i++)
        {
            if (i == 0 || i == 1)
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[i]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[i + 1].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
            else
            {
                int bindingIndex = Player_Input_List[i].action.GetBindingIndexForControl(Player_Input_List[i].action.controls[0]);

                Input_Text[i].text = InputControlPath.ToHumanReadableString(
                    Player_Input_List[i].action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
            }
        }

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
        if(Main_Can != null)
        {
            Main_Can.gameObject.SetActive(false);
        }
        Option_Can.gameObject.SetActive(true);
        is_Option_Setting = true;
    }

    public void Btn_Option_Quit()
    {
        Option_Can.gameObject.SetActive(false);
        if (Main_Can != null)
        {
            Main_Can.gameObject.SetActive(true);
        }
        is_Option_Setting = false;
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
}
