using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Audio/Audio_Event_Channel")]
public class Audio_Event_Channel : ScriptableObject
{
    public event Action<Sound_Event, Vector3> OnPlay;

    public void Raise(Sound_Event ev, Vector3 pos)
    {
        OnPlay?.Invoke(ev, pos);
    }
}
