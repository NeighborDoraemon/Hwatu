using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause_Manager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas Pause_Can;
    [SerializeField] private Canvas Setting_Can;
    [SerializeField] private Canvas Main_Can;
    [SerializeField] private Canvas Result_Can;
    [SerializeField] private Canvas JokBo_Can;

    [Header("Objects")]
    [SerializeField] private PlayerCharacter_Controller p_control;
    [SerializeField] private New_Fade_Controller new_Fade;
    [SerializeField] private Text Result_Text;
    [SerializeField] private Input_Data_Manager input_Data_Manager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show_Result(bool is_Died)
    {
        Main_Can.gameObject.SetActive(false);
        if(is_Died) // Player is Dead
        {
            Result_Text.text = "Game Over";
        }
        else
        {
            Result_Text.text = "Demo Clear!";
        }
        Result_Can.gameObject.SetActive(true);
    }

    public void Pause_Start()
    {
        Time.timeScale = 0.0f;
        Main_Can.gameObject.SetActive(false);
        Pause_Can.gameObject.SetActive(true);
    }

    public void Pause_Stop()
    {
        //if (!Setting_Can.gameObject.activeSelf)
        {
            Time.timeScale = 1.0f;
            Pause_Can.gameObject.SetActive(false);
            Main_Can.gameObject.SetActive(true);
            Setting_Can.gameObject.SetActive(false);
        }
    }

    public void Btn_Setting()
    {
        Pause_Can.gameObject.SetActive(false);
        Setting_Can.gameObject.SetActive(true);
    }

    public void Btn_Setting_Out()
    {
        //if (Setting_Can.gameObject.activeSelf)
        //{
            Setting_Can.gameObject.SetActive(false);
            Pause_Can.gameObject.SetActive(true);
        input_Data_Manager.Btn_Option_Quit();
        //}
    }


    public void Btn_Help()
    {
        JokBo_Can.gameObject.SetActive(true);
        Main_Can.gameObject.SetActive(false);
        Pause_Can.gameObject.SetActive(false);  
    }

    public void Btn_Help_Out()
    {
        JokBo_Can.gameObject.SetActive(false);
        Pause_Can.gameObject.SetActive(true);
    }

    public bool is_Help_Activated()
    {
        if(JokBo_Can.gameObject.activeSelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Btn_New_Game()
    {
        Pause_Stop();
        //fade_Con.Scene_Reload_Fade();
        Save_Manager.Instance.Modify(data =>
        {
            data.is_Map_Saved = false;
            data.is_Inventory_Saved = false;

            data.saved_Card_IDs.Clear();
            data.saved_Item_IDs.Clear();
        });
        Save_Manager.Instance.SaveAll();
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
