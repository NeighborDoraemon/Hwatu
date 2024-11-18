using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Obj_KeyBoard : MonoBehaviour
{
    [SerializeField] private InputActionReference Input_Ref;
    [SerializeField] private int Index;
    [SerializeField] private TextMeshPro textMesh;

    // Start is called before the first frame update
    void Start()
    {
        TextSetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TextSetting()
    {
        textMesh.text = InputControlPath.ToHumanReadableString(
                    Input_Ref.action.bindings[Index].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        if(textMesh.text == "Left Arrow")
        {
            textMesh.text = "¡ç";
        }
        else if(textMesh.text == "Right Arrow")
        {
            textMesh.text = "¡æ";
        }
    }
}
