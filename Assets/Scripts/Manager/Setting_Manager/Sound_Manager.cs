using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Sound_Manager : MonoBehaviour
{
    [Header("Sliders")]
    [SerializeField] private Slider Master_Slider;
    [SerializeField] private Slider Bgm_Slider;
    [SerializeField] private Slider Sfx_Slider;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private string master_Param = "MasterVol";
    [SerializeField] private string bgm_Param = "BgmVol";
    [SerializeField] private string sfx_Param = "SfxVol";

    [Header("Fallback")]
    [SerializeField] private AudioSource Bgm_AudioSource;
    [SerializeField] private AudioSource Sfx_AudioSource;

    const string KMaster = "SM_Master_Lin";
    const string KBgm = "SM_Bgm_Lin";
    const string KSfx = "SM_Sfx_Lin";

    bool initializing;

    private void Awake()
    {
        if (!mixer)
        {
            var am = FindObjectOfType<AudioManager>();
            if (am)
            {
                var g = typeof(AudioManager)
                        .GetField("default_Bgm_Group", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .GetValue(am) as AudioMixerGroup;
                if (g) mixer = g.audioMixer;
            }
        }
    }

    void Start()
    {
        initializing = true;

        if (mixer)
        {
            mixer.SetFloat(master_Param, 0.0f);
            mixer.SetFloat(bgm_Param, 0.0f);
            mixer.SetFloat(sfx_Param, 0.0f);
        }

        float m = PlayerPrefs.GetFloat(KMaster, 0.8f);
        float b = PlayerPrefs.GetFloat(KBgm, 0.8f);
        float s = PlayerPrefs.GetFloat(KSfx, 0.8f);

        if (Master_Slider) Master_Slider.SetValueWithoutNotify(m * 10.0f);
        if (Master_Slider) Bgm_Slider.SetValueWithoutNotify(b * 10.0f);
        if (Master_Slider) Sfx_Slider.SetValueWithoutNotify(s * 10.0f);

        Apply_All();

        if (Master_Slider) Master_Slider.onValueChanged.AddListener(_ => { if (!initializing) { Apply_All(); Save_Prefs(); } });
        if (Bgm_Slider) Bgm_Slider.onValueChanged.AddListener(_ => { if (!initializing) { Apply_All(); Save_Prefs(); } });
        if (Sfx_Slider) Sfx_Slider.onValueChanged.AddListener(_ => { if (!initializing) { Apply_All(); Save_Prefs(); } });

        initializing = false;
    }

    void Apply_All(bool log = false)
    {
        float m = Master_Slider ? Master_Slider.value / 10.0f : 0.8f;
        float b = Bgm_Slider ? Bgm_Slider.value / 10.0f : 0.8f;
        float s = Sfx_Slider ? Sfx_Slider.value / 10.0f : 0.8f;

        m = Mathf.Clamp(m, 0.0001f, 1.0f);
        b = Mathf.Clamp(b, 0.0001f, 1.0f);
        s = Mathf.Clamp(s, 0.0001f, 1.0f);

        if (mixer)
        {
            mixer.SetFloat(master_Param, Linear_To_Db(m));
            mixer.SetFloat(bgm_Param, Linear_To_Db(b));
            mixer.SetFloat(sfx_Param, Linear_To_Db(s));

            if (log)
            {
                mixer.GetFloat(master_Param, out var mdB);
                mixer.GetFloat(bgm_Param, out var bdB);
                mixer.GetFloat(sfx_Param, out var sdB);
                Debug.Log($"[Sound Manager] dB applied -> Master:{mdB} BGM:{bdB} SFX:{sdB}");
            }
        }
        else
        {
            if (Bgm_AudioSource) Bgm_AudioSource.volume = Mathf.Clamp01(m * b);
            if (Sfx_AudioSource) Sfx_AudioSource.volume = Mathf.Clamp01(m * s);
        }
    }

    private void Save_Prefs()
    {
        if (Master_Slider) PlayerPrefs.SetFloat(KMaster, Master_Slider.value / 10.0f);
        if (Bgm_Slider) PlayerPrefs.SetFloat(KBgm, Bgm_Slider.value / 10.0f);
        if (Sfx_Slider) PlayerPrefs.SetFloat(KSfx, Sfx_Slider.value / 10.0f);
        PlayerPrefs.Save();
    }

    static float Linear_To_Db(float x) => (x <= 0.0001f) ? -80.0f : Mathf.Log10(x) * 20.0f;

    public void Set_Master_Value() => Apply_All_And_Save();
    public void Set_Bgm_Value() => Apply_All_And_Save();
    public void Set_Sfx_Value() => Apply_All_And_Save();

    private void Apply_All_And_Save() { Apply_All(); Save_Prefs(); }
}
