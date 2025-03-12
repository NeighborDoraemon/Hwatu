using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dialogue_Parser : MonoBehaviour
{
    public Dial_Script[] Parse(string _CSVFileName)
    {
        List<Dial_Script> dialogue_List = new List<Dial_Script>();
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName);

        string[] Data = csvData.text.Split(new char[] { '\n' });

        for (int i = 1; i < Data.Length;)
        {
            string[] row = Data[i].Split(new char[] { ',' });

            Dial_Script dialogue_Script = new Dial_Script(); //대사 리스트 생성
            dialogue_Script.name = row[1]; //이름부분 저장

            Debug.Log(row[1]);

            List<string> context_list = new List<string>();

            do
            {
                context_list.Add(row[2]);

                Debug.Log(row[2]);

                if (++i < Data.Length)
                {
                    row = Data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == "");

            dialogue_Script.contexts = context_list.ToArray();

            dialogue_List.Add(dialogue_Script);
        }

        return dialogue_List.ToArray();
    }

    //public void Start()
    //{
    //    Parse("CSV테스트5");
    //}
}