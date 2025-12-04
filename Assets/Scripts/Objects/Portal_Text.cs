using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Portal_Text : MonoBehaviour
{
    [SerializeField] private TextMeshPro Txt_Warn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Print_Text()
    {
        Debug.Log("Portal Print Start");
        //Fade_Sprite(Txt_Warn, 1.0f, 1.5f, 0.0f);
        Txt_Warn.gameObject.SetActive(true);
        Invoke("Invoke_False", 2.0f);
    }

    private void Invoke_False()
    {
        Txt_Warn.gameObject.SetActive(false);
    }

    private IEnumerator Fade_Sprite(TextMeshPro sprite, float targetAlpha, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Color color = sprite.color;
        //float startAlpha = color.a;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0.0f, targetAlpha, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = targetAlpha;
        sprite.color = color;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(targetAlpha, 0.0f, t / duration);
            color.a = alpha;
            sprite.color = color;

            yield return null;
        }

        color.a = 0.0f;
        sprite.color = color;
    }
}
