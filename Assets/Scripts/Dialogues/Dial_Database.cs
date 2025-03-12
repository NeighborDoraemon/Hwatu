using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial_Database : MonoBehaviour
{
    [SerializeField] Dialogue_Event dialogues;

    private void Start()
    {
        btn_get_dialogue();
    }

    public void get_dialogue()
    {
        dialogues.Dialogues = Dial_Database_Manager.instance.GetDialogue_Sript((int)dialogues.Line.x, (int)dialogues.Line.y);
        for (int i = 0; i < dialogues.Dialogues.Length; i++)
        {
            Debug.Log(dialogues.Dialogues[i].contexts[i]);
        }
        //return dialogues.Dialogues;
    }


    public void btn_get_dialogue()
    {
        get_dialogue();
    }
}
