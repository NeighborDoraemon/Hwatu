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

    [SerializeField] float padding = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        TextSetting();
        Update_Size();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Call_KeyObjects()
    {
        TextSetting();
        Update_Size();
    }

    private void TextSetting()
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

    private void Update_Size()
    {
        if (textMesh == null)
        {
            Debug.LogWarning("TextMeshPro null");
            return;
        }

        Vector2 textSize = textMesh.GetPreferredValues(textMesh.text);

        SpriteRenderer spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector3 newSize = new Vector3(
                textSize.x + padding,  
                spriteRenderer.size.y, 
                1f                     
            );
            spriteRenderer.size = newSize;
        }
    }
}
