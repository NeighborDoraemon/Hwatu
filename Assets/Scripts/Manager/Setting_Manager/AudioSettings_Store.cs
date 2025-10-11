using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings_Store : MonoBehaviour
{
    public const string KMaster = "AM_Master_Lin";
    public const string KBgm = "AM_Bgm_Lin";
    public const string KSfx = "AM_Sfx_Lin";

    public static event Action<float, float, float> OnChanged;

    public static float Master => PlayerPrefs.GetFloat(KMaster, 0.8f);
    public static float Bgm => PlayerPrefs.GetFloat(KBgm, 0.8f);
    public static float Sfx => PlayerPrefs.GetFloat(KSfx, 0.8f);

    public static void Set(float master01, float bgm01, float sfx01, bool save = true)
    {
        master01 = Mathf.Clamp01(master01);
        bgm01 = Mathf.Clamp01(bgm01);
        sfx01 = Mathf.Clamp01(sfx01);

        if (save)
        {
            PlayerPrefs.SetFloat(KMaster, master01);
            PlayerPrefs.SetFloat(KBgm, bgm01);
            PlayerPrefs.SetFloat(KSfx, sfx01);
            PlayerPrefs.Save();
        }
        OnChanged?.Invoke(master01, bgm01, sfx01);
    }

    public static void SetOne(VolumeKind kind, float v01, bool save = true)
    {
        float m = Master, b = Bgm, s = Sfx;
        v01 = Mathf.Clamp01(v01);
        switch (kind)
        {
            case VolumeKind.Master: m = v01; break;
            case VolumeKind.BGM: b = v01; break;
            case VolumeKind.SFX: s = v01; break;
        }
        Set(m, b, s, save);
    }

    public static void Apply_To_Mixer(AudioMixer mixer, string master_Param, string bgm_Param, string sfx_Param)
    {
        if (!mixer) return;
        mixer.SetFloat(master_Param, Lin_To_Db(Master));
        mixer.SetFloat(bgm_Param, Lin_To_Db(Bgm));
        mixer.SetFloat(sfx_Param, Lin_To_Db(Sfx));
    }

    public static float Lin_To_Db(float x) => (x <= 0.0001f) ? -80.0f : Mathf.Log10(x) * 20.0f;
}
