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
    private void Awake() { if (!slider) slider = GetComponent<Slider>(); }

    private void OnEnable()
    {
        Sync_From_Audio();
        slider.onValueChanged.AddListener(OnSlider_Changed);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSlider_Changed);
    }

    void OnSlider_Changed(float v)
    {
        if (!audioManager) return;

        float v01 = Mathf.Clamp01(use_0to10 ? v * 0.1f : v);

        switch (kind)
        {
            case VolumeKind.Master: audioManager.UI_OnMaster_Changed(v01); break;
            case VolumeKind.BGM: audioManager.UI_OnBgm_Changed(v01); break;
            case VolumeKind.SFX: audioManager.UI_OnSfx_Changed(v01); break;
        }
    }

    public void Sync_From_Audio()
    {
        if (!audioManager || !slider) return;

        float v01 = kind switch
        { 
            VolumeKind.Master => audioManager.GetMaster01(),
            VolumeKind.BGM => audioManager.GetBgm01(),
            _ => audioManager.GetSfx01(),
        };

        float v = use_0to10 ? v01 * 10.0f : v01;
        slider.SetValueWithoutNotify(v);
    }
}
