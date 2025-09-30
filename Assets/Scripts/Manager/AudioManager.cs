using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Audio;
//using UnityEngine.InputSystem.iOS;

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

    // 발신자별 활성소스/마지막 트리거 시간
    private readonly Dictionary<Sound_Event, Dictionary<Transform, AudioSource>> active_By_Emitter = new();
    private readonly Dictionary<Sound_Event, Dictionary<Transform, float>> last_Trig_By_Emitter = new();
    

    private void OnEnable()
    {
        if (sfx_Channel)
        {
            sfx_Channel.OnPlay += Handle_SFX_Play;
            sfx_Channel.OnPlay_Attached += Handle_SFX_Play_Attached;
        }
    }

    private void OnDisable()
    {
        if (sfx_Channel)
        {
            sfx_Channel.OnPlay -= Handle_SFX_Play;
            sfx_Channel.OnPlay_Attached -= Handle_SFX_Play_Attached;
        }
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

    void Handle_SFX_Play_Attached(Sound_Event ev, Transform emitter)
    {
        if (!ev || !emitter) return;

        float now = Time.unscaledTime;
        if (!last_Trig_By_Emitter.TryGetValue(ev, out var tMap)) { tMap = new(); last_Trig_By_Emitter[ev] = tMap; }
        if (tMap.TryGetValue(emitter, out var tLast) && now - tLast < ev.retrigger_MinInterval) return;

        if (!active_By_Emitter.TryGetValue(ev, out var map)) { map = new(); active_By_Emitter[ev] = map; }

        map.TryGetValue(emitter, out var active_Src);
        bool has_Active = active_Src != null && active_Src.isPlaying;

        if (ev.max_Simultaneous == 1)
        {
            switch (ev.overlap_Policy)
            {
                case Sound_Event.Overlap_Policy.Ignore_If_Playing:
                    if (has_Active) { tMap[emitter] = now; return; }
                    break;
                case Sound_Event.Overlap_Policy.Restart_On_Same_Emitter:
                    if (active_Src != null)
                    {
                        Setup_Source_Params(active_Src, ev, emitter.position);
                        var clip_R = Select_Clip(ev);
                        if (!clip_R) return;
                        active_Src.clip = clip_R;
                        active_Src.Stop();
                        active_Src.Play();
                        tMap[emitter] = now;
                        return;
                    }
                    break;
                case Sound_Event.Overlap_Policy.Steal_Oldest:
                    if (has_Active)
                    {
                        Return_Source(active_Src);
                        map[emitter] = null;
                        Decrease_Playing(ev);
                    }
                    break;
                case Sound_Event.Overlap_Policy.Allow_Overlap:
                    break;
            }
        }

        var src = Get_Source();
        src.transform.SetParent(emitter);
        src.transform.localPosition = Vector3.zero;

        Setup_Source_Params(src, ev, emitter.position);
        var clip = Select_Clip(ev);
        if (!clip)
        {
            Return_Source(src);
            return;
        }
        src.clip = clip;

        Increase_Playing(ev);
        src.Play();

        map[emitter] = src;
        tMap[emitter] = now;

        StartCoroutine(Return_When_Done_Attached(ev, emitter, src, clip.length, src.pitch));
    }

    void Setup_Source_Params(AudioSource src, Sound_Event ev, Vector3 pos)
    {
        src.transform.position = pos;
        if (ev.mixer_Group) src.outputAudioMixerGroup = ev.mixer_Group;
        src.spatialBlend = ev.spatial_Blend;
        src.rolloffMode = ev.rolloff;
        src.minDistance = ev.min_Distance;
        src.maxDistance = ev.max_Distance;
        src.volume = Mathf.Clamp01(ev.volume * UnityEngine.Random.Range(ev.volume_Random.x, ev.volume_Random.y));
        src.pitch = UnityEngine.Random.Range(ev.pitch_Random.x, ev.pitch_Random.y);
    }

    AudioClip Select_Clip(Sound_Event ev)
    {
        if (ev.play_Order == Play_Order.Sequential)
        {
            int idx = 0;
            next_Index.TryGetValue(ev, out idx);
            if (ev.clips == null || ev.clips.Count == 0) return null;
            if (idx >= ev.clips.Count) idx = 0;
            var c = ev.clips[idx];
            next_Index[ev] = (idx + 1) % ev.clips.Count;
            return c;
        }
        return ev.Pick_Clip();
    }

    void Increase_Playing(Sound_Event ev) { if (!playing_Count.ContainsKey(ev)) playing_Count[ev] = 0; playing_Count[ev]++; }
    void Decrease_Playing(Sound_Event ev) { if (playing_Count.ContainsKey(ev)) playing_Count[ev] = Mathf.Max(0, playing_Count[ev] - 1); }

    IEnumerator Return_When_Done_Attached(Sound_Event ev, Transform emitter, AudioSource s, float len, float pitch)
    {
        yield return new WaitForSecondsRealtime(len / Mathf.Max(0.01f, pitch));
        if (active_By_Emitter.TryGetValue(ev, out var map) && map.TryGetValue(emitter, out var active) && active == s)
            map[emitter] = null;
        Decrease_Playing(ev);
        s.transform.SetParent(transform);
        Return_Source(s);
    }

    // ------------------------------------------------------------
    // BGM
    // ------------------------------------------------------------


}
