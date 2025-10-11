using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum VolumeKind { Master, BGM, SFX }

[RequireComponent(typeof(Slider))]
public class VolumeSlider_Binding : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private VolumeKind kind;
    [SerializeField] private bool use_0to10 = true;

    private Slider slider;

    private void Reset() => slider = GetComponent<Slider>();
    private void Awake()
    { 
        if (!slider) slider = GetComponent<Slider>();
        if (use_0to10)
        {
            slider.wholeNumbers = true;
            slider.minValue = 0;
            slider.maxValue = 10;
        }
        else
        {
            slider.wholeNumbers = false;
            slider.minValue = 0;
            slider.maxValue = 1;
        }
    }

    private void OnEnable()
    {
        Sync_From_Store();
        slider.onValueChanged.AddListener(OnSlider_Changed);
        AudioSettings_Store.OnChanged += Handle_Store_Changed;
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSlider_Changed);
        AudioSettings_Store.OnChanged -= Handle_Store_Changed;
    }

    private void Handle_Store_Changed(float m, float b, float s)
    {
        float v01 = kind switch
        {
            VolumeKind.Master => m,
            VolumeKind.BGM    => b,
            _                 => s
        };
        float v = use_0to10 ? v01 * 10.0f : v01;
        if (slider) slider.SetValueWithoutNotify(v);
    }

    void OnSlider_Changed(float v)
    {
        float v01 = Mathf.Clamp01(use_0to10 ? v * 0.1f : v);

        if (audioManager)
        {
            switch (kind)
            {
                case VolumeKind.Master: audioManager.UI_OnMaster_Changed(v01); break;
                case VolumeKind.BGM   : audioManager.UI_OnBgm_Changed(v01); break;
                case VolumeKind.SFX   : audioManager.UI_OnSfx_Changed(v01); break;
            }
        }
        else
        {
            AudioSettings_Store.SetOne(kind, v01);
        }
    }

    public void Sync_From_Store()
    {
        float v01 = kind switch
        { 
            VolumeKind.Master => AudioSettings_Store.Master,
            VolumeKind.BGM    => AudioSettings_Store.Bgm,
            _                 => AudioSettings_Store.Sfx
        };
        float v = use_0to10 ? v01 * 10.0f : v01;
        if (slider) slider.SetValueWithoutNotify(v);
    }
}
