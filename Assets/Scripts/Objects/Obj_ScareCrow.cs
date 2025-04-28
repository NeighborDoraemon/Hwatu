using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Obj_ScareCrow : MonoBehaviour
{
    [SerializeField] private TextMeshPro TextMesh;
    [SerializeField] private Animator Scarecrow_Animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDamage(int Damage)
    {
        TextMesh.text = Damage.ToString();
        TextMesh.alpha = 1f;
        StartCoroutine(FadeOut());

        Scarecrow_Animator.SetTrigger("Hit");
    }

    private IEnumerator FadeOut()
    {
        while(TextMesh.alpha > 0)
        {
            TextMesh.alpha -= Time.deltaTime * 0.5f;
            yield return null;
        }
    }
}
