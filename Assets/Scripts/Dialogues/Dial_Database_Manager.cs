using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial_Database_Manager : MonoBehaviour
{
    public static Dial_Database_Manager instance;

    [SerializeField] string csv_filename;

    Dictionary<int, Dial_Script> dialogue_script_dictionary = new Dictionary<int, Dial_Script>();

    public static bool isFinished = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            Dialogue_Parser dialogue_parser = GetComponent<Dialogue_Parser>();
            Dial_Script[] dialogue_s = dialogue_parser.Parse(csv_filename);

            for (int i = 0; i < dialogue_s.Length; i++)
            {
                dialogue_script_dictionary.Add(i + 1, dialogue_s[i]);
            }
            isFinished = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Dial_Script[] GetDialogue_Sript(int _StartNum, int _EndNum)
    {
        List<Dial_Script> dialogue_list = new List<Dial_Script>();

        for (int i = 0; i <= _EndNum - _StartNum; i++)
        {
            dialogue_list.Add(dialogue_script_dictionary[_StartNum + 1 + i]);
            //Debug.Log(dialogue_list[i].name);
            //for (int j = 0; j < dialogue_list[i].contexts.Length; j++)
            //{
            //    Debug.Log(dialogue_list[i].contexts[j]);
            //}
        }

        return dialogue_list.ToArray();
    }
}
