using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warning_Object : MonoBehaviour
{
    //[SerializeField] private float Target_Alpha = 1.0f;
    //[SerializeField] private float Duration= 0.5f;
    //[SerializeField] private float Delay = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(float alpha, float dur, float del)
    {
        StartCoroutine(Fade_Sprite(GetComponent<SpriteRenderer>(), alpha, dur, del));
    }

    private IEnumerator Fade_Sprite(SpriteRenderer sprite, float targetAlpha, float duration, float delay)
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

        Destroy(gameObject);
    }
}
