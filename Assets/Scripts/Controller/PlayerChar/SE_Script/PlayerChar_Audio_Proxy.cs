using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChar_Audio_Proxy : MonoBehaviour
{
    [SerializeField] private SFX_Event_Channel sfx_Channel;
    [SerializeField] private BGM_Event_Channel bgm_Channel;
    [SerializeField] private SFX_Resolver resolver;
    private PlayerCharacter_Controller player;

    private void Awake()
    {
        if (!player) player = GetComponent<PlayerCharacter_Controller>();
    }

    public void Play_Footstep() => Play_By_Tag(SFX_Tag.Footstep);
    public void Play_Hurt() => Play_By_Tag(SFX_Tag.Hurt);
    public void Play_Attack()
    {
        var se = resolver.Resolve(player, SFX_Tag.Attack);
        if (se != null) sfx_Channel.Raise_Attached(se, transform);
    }
    
    private void Play_By_Tag(SFX_Tag tag)
    {
        if (!sfx_Channel || !resolver || !player) return;
        var se = resolver.Resolve(player, tag);
        if (se != null)
            sfx_Channel.Raise(se, transform.position);
    }
}
