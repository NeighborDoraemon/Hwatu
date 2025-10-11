using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
