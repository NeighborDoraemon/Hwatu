using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.iOS;

public class AudioManager : MonoBehaviour
{
    [Header("Channels")]
    [SerializeField] private SFX_Event_Channel sfx_Channel;
    [SerializeField] private BGM_Event_Channel bgm_Channel;

    [Header("SFX Pool")]
    [SerializeField] private AudioSource sfx_Source_Prefab;
    [SerializeField] private int poolSize = 24;

    [Header("BGM")]
    [SerializeField] private AudioSource bgm;

    // 내부 상태
    private readonly Queue<AudioSource> pool = new();
    private readonly Dictionary<Sound_Event, float> last_Played = new();
    private readonly Dictionary<Sound_Event, int> next_Index = new();
    private readonly Dictionary<Sound_Event, int> playing_Count = new();
    

    private void OnEnable()
    {
        if (sfx_Channel) sfx_Channel.OnPlay += Handle_SFX_Play;
    }

    private void OnDisable()
    {
        if (sfx_Channel) sfx_Channel.OnPlay -= Handle_SFX_Play;
    }

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
            pool.Enqueue(Create_Source());
    }

    // ------------------------------------------------------------
    // SFX
    // ------------------------------------------------------------
    AudioSource Create_Source()
    {
        var s = Instantiate(sfx_Source_Prefab, transform);
        s.playOnAwake = false;
        s.gameObject.SetActive(false);
        return s;
    }

    AudioSource Get_Source()
    {
        if (pool.Count == 0) pool.Enqueue(Create_Source());
        var s = pool.Dequeue();
        s.gameObject.SetActive(true);
        return s;
    }

    void Return_Source(AudioSource s)
    {
        s.Stop();
        s.clip = null;
        s.transform.SetParent(transform);
        s.gameObject.SetActive(false);
        pool.Enqueue(s);
    }

    void Handle_SFX_Play(Sound_Event ev, Vector3 pos)
    {
        if (!ev) return;

        // 쿨다운
        if (ev.cooldown > 0.0f && last_Played.TryGetValue(ev, out var t_Last))
        {
            if (Time.unscaledTime - t_Last < ev.cooldown) return;
        }

        // 동시 재생 제한
        if (ev.max_Simultaneous > 0 && playing_Count.TryGetValue(ev, out var cnt) && cnt >= ev.max_Simultaneous)
            return;

        AudioClip clip = null;
        if (ev.play_Order == Play_Order.Sequential)
        {
            if (ev.clips == null || ev.clips.Count == 0) return;
            int idx = 0;
            next_Index.TryGetValue(ev, out idx);
            if (idx >= ev.clips.Count) idx = 0;
            clip = ev.clips[idx];
            next_Index[ev] = (idx + 1) % ev.clips.Count;
        }
        else
        {
            clip = ev.Pick_Clip();
        }
        if (!clip) return;

        var src = Get_Source();
        src.transform.position = pos;
        src.clip = clip;

        // 라우팅/파라미터
        if (ev.mixer_Group) src.outputAudioMixerGroup = ev.mixer_Group;
        src.spatialBlend = ev.spatial_Blend;
        src.rolloffMode = ev.rolloff;
        src.minDistance = ev.min_Distance;
        src.maxDistance = ev.max_Distance;

        float v = ev.volume * UnityEngine.Random.Range(ev.volume_Random.x, ev.volume_Random.y);
        float p = UnityEngine.Random.Range(ev.pitch_Random.x, ev.pitch_Random.y);
        src.volume = Mathf.Clamp01(v);
        src.pitch = p;

        if (!playing_Count.ContainsKey(ev)) playing_Count[ev] = 0;
        playing_Count[ev]++;

        src.Play();
        StartCoroutine(Return_When_Done(ev, src, clip.length, p));
        last_Played[ev] = Time.unscaledTime;
    }

    IEnumerator Return_When_Done(Sound_Event ev, AudioSource s, float len, float pitch)
    {
        float wait = len / Mathf.Max(0.01f, pitch);
        yield return new WaitForSeconds(wait);
        if (playing_Count.ContainsKey(ev)) playing_Count[ev] = Mathf.Max(0, playing_Count[ev] - 1);
        Return_Source(s);
    }

    // ------------------------------------------------------------
    // BGM
    // ------------------------------------------------------------


}
