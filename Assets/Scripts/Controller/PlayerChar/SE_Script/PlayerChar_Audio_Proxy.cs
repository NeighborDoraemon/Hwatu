using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar_Audio_Proxy : MonoBehaviour
{
    [SerializeField] private SFX_Event_Channel sfx;
    [SerializeField] private BGM_Event_Channel bgm;
    [SerializeField] private Sound_Event footstepSE;
    [SerializeField] private AudioClip bgm_Clip;
    [SerializeField] private SFX_Resolver resolver;

    public void Play(SFX_Tag tag, Vector3? pos = null)
    {
        var se = resolver.Resolve(GetComponent<PlayerCharacter_Controller>(), tag);
        if (se == null) return;
    }

    public void Play_Footstep() => sfx.Raise(footstepSE, transform.position);
}
