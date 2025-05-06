using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sound_Manager : MonoBehaviour
{
    [SerializeField] private Slider Master_Slider;
    [SerializeField] private Slider Bgm_Slider;
    [SerializeField] private Slider Sfx_Slider;
    [SerializeField] private AudioSource Bgm_AudioSource;
    [SerializeField] private AudioSource Sfx_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        Set_Sounds();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Set_Sounds()
    {
        Master_Slider.value = PlayerPrefs.GetFloat("Master_Slider", 0.5f) * 10;
        Bgm_Slider.value = PlayerPrefs.GetFloat("Bgm_Slider", 0.5f) * 10;
        Sfx_Slider.value = PlayerPrefs.GetFloat("Sfx_Slider", 0.5f) * 10;

        Bgm_AudioSource.volume = PlayerPrefs.GetFloat("BGM_Volume", 0.5f);
        Sfx_AudioSource.volume = PlayerPrefs.GetFloat("Sfx_Volume", 0.5f);
    }

    public void Set_Master_Value()
    {
        Bgm_AudioSource.volume = (Master_Slider.value * Bgm_Slider.value) / 100;
        Sfx_AudioSource.volume = (Master_Slider.value * Sfx_Slider.value) / 100;

        Save_Prefs();
    }

    public void Set_Bgm_Volume()
    {
        Bgm_AudioSource.volume = (Master_Slider.value * Bgm_Slider.value) / 100;
        Save_Prefs();
    }

    public void Set_Sfx_Volume()
    {
        Sfx_AudioSource.volume = (Master_Slider.value * Sfx_Slider.value) / 100;
        Save_Prefs();
    }

    private void Save_Prefs()
    {
        PlayerPrefs.SetFloat("BGM_Volume", Bgm_AudioSource.volume);
        PlayerPrefs.SetFloat("Sfx_Volume", Sfx_AudioSource.volume);

        PlayerPrefs.SetFloat("Master_Slider", Master_Slider.value / 10);
        PlayerPrefs.SetFloat("Bgm_Slider", Bgm_Slider.value / 10);
        PlayerPrefs.SetFloat("Sfx_Slider", Sfx_Slider.value / 10);

        PlayerPrefs.Save();
    }
}
