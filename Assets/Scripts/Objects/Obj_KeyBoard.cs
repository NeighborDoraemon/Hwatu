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
            textMesh.text = "←";
        }
        else if(textMesh.text == "Right Arrow")
        {
            textMesh.text = "→";
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
                textSize.x + padding,  // 텍스트 폭 + 여백
                spriteRenderer.size.y, // 기존 높이를 유지
                1f                     // 깊이는 기본값 유지
            );

            // SpriteRenderer의 크기를 변경
            spriteRenderer.size = newSize;
        }

        // 부모의 로컬 스케일 조정 (추가로 스케일 조정이 필요한 경우)
        //this.gameObject.transform.localScale = new Vector3(textSize.x + padding, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
    }
}
