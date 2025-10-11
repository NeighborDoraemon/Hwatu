using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Channels/SFX")]
public class SFX_Event_Channel : ScriptableObject
{
    public event Action<Sound_Event, Vector3> OnPlay;
    public void Raise(Sound_Event sound, Vector3 position)
    {
        OnPlay?.Invoke(sound, position);
    }

    public event Action<Sound_Event, Transform> OnPlay_Attached;
    public void Raise_Attached(Sound_Event sound, Transform emitter)
    {
        OnPlay_Attached?.Invoke(sound, emitter);
    }
}
