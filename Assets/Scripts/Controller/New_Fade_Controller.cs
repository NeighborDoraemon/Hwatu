using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class New_Fade_Controller : MonoBehaviour
{
    [Header("Fade Target")]
    [SerializeField] private Image fade_Image;
    private float fade_Time = 1.0f;
    private Ease fadeEase = Ease.Linear;

    private bool is_Fading = false;


    private void Awake()
    {
        Set_Black_Screen();
    }

    // Start is called before the first frame update
    void Start()
    {
        Fade_In();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fade_Out(System.Action OnComplete = null)
    {
        if(is_Fading)
        {
            return;
        }

        is_Fading = true;

        fade_Image.DOFade(1.0f, fade_Time)
               .SetEase(Ease.InOutQuad)
               .OnComplete(() =>
               {
                   is_Fading = false;
                   OnComplete?.Invoke();
                   });
    }

    public void Fade_In(System.Action OnComplete = null)
    {
        if (is_Fading)
        {
            return;
        }

        is_Fading = true;

        fade_Image.DOFade(0.0f, fade_Time)
               .SetEase(Ease.InOutQuad)
               .OnComplete(() =>
               {
                   is_Fading = false;
                   OnComplete?.Invoke();
               });
    }

    public void Scene_Fade_Out(string sceneName)
    {
        Fade_Out(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    private void Set_Black_Screen()
    {
        Color color = fade_Image.color;
        color.a = 1.0f;
        fade_Image.color = color;
    }

    public void New_Fade_Start_Button()
    {
        if(!is_Fading)
        {
            Scene_Fade_Out("MainScene");
        }
    }
}
