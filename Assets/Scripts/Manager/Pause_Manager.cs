using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas Pause_Can;
    [SerializeField] private Canvas Setting_Can;
    [SerializeField] private Canvas Main_Can;
    [SerializeField] private Canvas Result_Can;

    [Header("Objects")]
    [SerializeField] private PlayerCharacter_Controller p_control;
    [SerializeField] private New_Fade_Controller new_Fade;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show_Result()
    {
        Main_Can.gameObject.SetActive(false);
        Result_Can.gameObject.SetActive(true);
    }

    public void Pause_Start()
    {
        Main_Can.gameObject.SetActive(false);
        Pause_Can.gameObject.SetActive(true);
    }

    public void Pause_Stop()
    {
        Time.timeScale = 1.0f;
        Pause_Can.gameObject.SetActive(false);
        Main_Can.gameObject.SetActive(true);
        Setting_Can.gameObject.SetActive(false);
    }

    public void Btn_Setting()
    {
        Pause_Can.gameObject.SetActive(false);
        Setting_Can.gameObject.SetActive(true);
    }

    public void Btn_Setting_Out()
    {
        Setting_Can.gameObject.SetActive(false);
        Pause_Can.gameObject.SetActive(true);
    }

    public void Btn_Help()
    {

    }

    public void Btn_New_Game()
    {
        Pause_Stop();
        //fade_Con.Scene_Reload_Fade();
        new_Fade.Scene_Fade_Out("MainScene");
    }

    public void Btn_Game_Quit()
    {
        Pause_Stop();
        //fade_Con.Scene_Fade_Out();
        new_Fade.Scene_Fade_Out("Start_Scene");
    }

    public void Btn_Result_Newgame()
    {
        //fade_Con.Scene_Reload_Fade();
        new_Fade.Scene_Fade_Out("MainScene");
    }

    public void Btn_Result_Quit()
    {
        //fade_Con.Scene_Fade_Out();
        new_Fade.Scene_Fade_Out("Start_Scene");
    }
}
