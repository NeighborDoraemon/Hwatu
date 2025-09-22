using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum WeaponType { Blade, Blunt, Gun, Light, None }
public enum SFX_Tag { Attack, Skill, Footstep, Hurt }
public enum Play_Order { Random, Sequential }

[CreateAssetMenu(menuName ="Audio/Sound_Event")]
public class Sound_Event : ScriptableObject
{
    [Header("Clips")]
    public List<AudioClip> clips;

    [Header("Playback")]
    [Range(0.0f, 1.0f)] public float volume = 0.5f;
    public Vector2 volume_Random = new(0.95f, 1.05f);
    public Vector2 pitch_Random = new(0.98f, 1.02f);
    [Range(0.0f, 1.0f)] public float spatial_Blend = 0.0f;
    public AudioRolloffMode rolloff = AudioRolloffMode.Linear;
    public float min_Distance = 1.0f, max_Distance = 25.0f;

    [Header("Playback Order")]
    public Play_Order play_Order = Play_Order.Random;

    [Header("Limits")]
    [Tooltip("같은 사운드의 동시 재생 최대 개수(0=무제한)")]
    public int max_Simultaneous = 5;
    [Tooltip("같은 사운드의 최소 재생 간격(초)")]
    public float cooldown = 0.0f;

    [Header("Routing")]
    public AudioMixerGroup mixer_Group;
    
    public AudioClip Pick_Clip()
    {
        if (clips == null || clips.Count == 0) return null;
        if (clips.Count == 1) return clips[0];
        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }
}

[CreateAssetMenu(menuName = "Audio/Player_SFX_Profile")]
public class Player_SFX_Profile : ScriptableObject
{
    public Sound_Event footstep;
    public Sound_Event hurt;
}

[CreateAssetMenu(menuName = "Audio/WeaponType_SFX_Profile")]
public class WeaponType_SFX_Profile : ScriptableObject
{
    public WeaponType type;
    public Sound_Event attack;
    //public Sound_Event skill;
}

[CreateAssetMenu(menuName = "Audio/Channels/SFX")]
public class SFX_Event_Channel : ScriptableObject
{
    public event Action<Sound_Event, Vector3> OnPlay;

    public void Raise(Sound_Event sound, Vector3 position)
    {
        OnPlay?.Invoke(sound, position);
    }
}

[CreateAssetMenu(menuName = "Audio/Channels/BGM")]
public class BGM_Event_Channel : ScriptableObject
{
    public event Action<AudioClip, float, bool> OnPlay;
    public event Action<float> OnStop;

    public void Raise_Play(AudioClip clip, float fade_Seconds = 0.0f, bool loop = true)
        => OnPlay?.Invoke(clip, fade_Seconds, loop);

    public void Raise_Stop(float fade_Seconds = 0.0f)
        => OnStop?.Invoke(fade_Seconds);
}