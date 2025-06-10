using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Screen_Manager : MonoBehaviour
{
    [Header("Dropdown")]
    [SerializeField] private Dropdown Dropdown_Screen;
    [SerializeField] private Dropdown Dropdown_Resolution;

    private FullScreenMode Pre_Screen;

    // Start is called before the first frame update
    void Start()
    {
        //Dropdown_Screen.onValueChanged.AddListener(ChangeScreenMode);

        Pre_Screen = Screen.fullScreenMode;

        UpdateResolutionDropdown();
        UpdateScreenModeDropdown();
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.fullScreenMode != Pre_Screen)
        {
            Pre_Screen = Screen.fullScreenMode;
            UpdateScreenModeDropdown();
            UpdateResolutionDropdown();
        }
    }

    public void OnScreenModeChanged(int index)
    {
        switch (index)
        {
            case 0: // 테두리 없음
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1: // 창모드
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2: // 전체화면
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }
    }

    public void OnResolutionChanged(int index)
    {
        // 선택한 해상도 인덱스에 맞는 해상도로 변경
        string resolution = Dropdown_Resolution.options[index].text;
        string[] dimensions = resolution.Split('×'); // "1920x1080"에서 x로 분리

        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        // 해상도 변경
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }

    private void UpdateScreenModeDropdown()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                Dropdown_Screen.value = 0;
                break;
            case FullScreenMode.Windowed:
                Dropdown_Screen.value = 1;
                break;
            case FullScreenMode.ExclusiveFullScreen:
                Dropdown_Screen.value = 2;
                break;
            default:
                Dropdown_Screen.value = 1;
                break;
        }

        Dropdown_Screen.RefreshShownValue();
    }

    private void UpdateResolutionDropdown()
    {
        for (int i = 0; i < Dropdown_Resolution.options.Count; i++)
        {
            string resolution = Dropdown_Resolution.options[i].text;
            string[] dimensions = resolution.Split('×'); // "1920x1080"에서 x로 분리

            int width = int.Parse(dimensions[0]);
            int height = int.Parse(dimensions[1]);

            if (Screen.width == width && Screen.height == height)
            {
                Dropdown_Resolution.value = i;
                break;
            }
        }

        Dropdown_Resolution.RefreshShownValue();
    }
}
