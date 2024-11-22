using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class Fade_Controller : MonoBehaviour
{
    private float Fade_Time = 1.0f;

    [Header("Fade_Objects")]
    [SerializeField] private Image Fade_Image;
    [SerializeField] private Canvas Fade_Canvas;

    [HideInInspector] public static bool is_Fading = false;


    // Start is called before the first frame update
    void Start()
    {
        Fade_In();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Fade_In()
    {
        StartCoroutine(Fade(true));
    }

    public void Fade_Out()
    {
        StartCoroutine(Fade(false));
        Invoke("Fade_In", Fade_Time);
    }

    public void Scene_Fade_Out()
    {
        Fade_Canvas.gameObject.SetActive(true);
        StartCoroutine(Fade(false));
        Invoke("Scene_Move", Fade_Time);
    }

    public void Scene_Reload_Fade()
    {
        Fade_Canvas.gameObject.SetActive(true);
        StartCoroutine(Fade(false));
        Invoke("Scene_Reload", Fade_Time);
    }

    private IEnumerator Fade(bool In_or_Out)
    {
        Fade_Image.gameObject.SetActive(true);
        is_Fading = true;
        Color imageColor = Fade_Image.color;

        if (In_or_Out)//Fade In
        {
            //while (imageColor.a > 0.0f)
            //{
            //    imageColor.a -= Time.deltaTime / Fade_Time;

            //    if (imageColor.a <= 0.0f)
            //    {
            //        imageColor.a = 0.0f;
            //    }

            //    Fade_Image.color = imageColor;
            //    yield return null;
            //}
            Fade_Image.DOFade(0.0f, 2.0f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Debug.Log("Fade In Complete"));

            Fade_Image.gameObject.SetActive(false);
            is_Fading = false;

            Fade_Canvas.gameObject.SetActive(false);

            yield return null;
        }
        else // Fade Out
        {
            //imageColor.a = 0.0f;
            //Fade_Image.color = imageColor;

            //while (imageColor.a < 1.0f)
            //{
            //    imageColor.a += Time.deltaTime / Fade_Time;

            //    if (imageColor.a >= 1.0f)
            //    {
            //        imageColor.a = 1.0f;
            //    }

            //    Fade_Image.color = imageColor;
            //    yield return null;
            //}

            Fade_Image.DOFade(1.0f, 2.0f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => Debug.Log("Fade Out Complete"));

            yield return null;
        }
    }

    private void Scene_Move()
    {
        if (SceneManager.GetActiveScene().name == "Start_Scene")
        {
            SceneManager.LoadScene("MainScene");
        }
        else if (SceneManager.GetActiveScene().name == "MainScene")
        {
            SceneManager.LoadScene("Start_Scene");
        }
    }

    private void Scene_Reload()
    {
        if (SceneManager.GetActiveScene().name == "Start_Scene")
        {
            SceneManager.LoadScene("Start_Scene");
        }
        else if (SceneManager.GetActiveScene().name == "MainScene")
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    public void Btn_Start()
    {
        if (is_Fading == false)
        {
            Scene_Fade_Out();
        }
    }
}
