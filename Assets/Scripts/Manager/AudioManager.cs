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
    [SerializeField] private AudioClip default_Bgm_Clip;
    [SerializeField] private bool play_On_Awake = true;

    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup default_Sfx_Group;
    [SerializeField] private AudioMixerGroup default_Bgm_Group;

    [Header("Mixer Control")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerSnapshot start_Snapshot;
    [SerializeField] private string master_Param = "MasterVol";
    [SerializeField] private string bgm_Param = "BgmVol";
    [SerializeField] private string sfx_Param = "SfxVol";

    [Header("Default Volumes (0~1)")]
    [SerializeField, Range(0.0f, 1.0f)] private float default_Master = 0.8f;
    [SerializeField, Range(0.0f, 1.0f)] private float default_Bgm = 0.8f;
    [SerializeField, Range(0.0f, 1.0f)] private float default_Sfx = 0.8f;

    // 내부 상태
    private readonly Queue<AudioSource> pool = new();
    private readonly Dictionary<Sound_Event, float> last_Played = new();
    private readonly Dictionary<Sound_Event, int> next_Index = new();
    private readonly Dictionary<Sound_Event, int> playing_Count = new();

    // 발신자별 활성소스/마지막 트리거 시간
    private readonly Dictionary<Sound_Event, Dictionary<Transform, AudioSource>> active_By_Emitter = new();
    private readonly Dictionary<Sound_Event, Dictionary<Transform, float>> last_Trig_By_Emitter = new();

    // PlayerPrefs 키
    const string KMaster = "AM_Master_Lin";
    const string KBgm = "AM_Bgm_Lin";
    const string KSfx = "AM_Sfx_Lin";

    const float kMin_Lin = 0.0001f;

    static float Linear_To_Db(float x) => (x <= 0.0001f) ? -80.0f : Mathf.Log10(x) * 20.0f;

    private void OnEnable()
    {
        if (sfx_Channel)
        {
            sfx_Channel.OnPlay += Handle_SFX_Play;
            sfx_Channel.OnPlay_Attached += Handle_SFX_Play_Attached;
        }
        if (bgm_Channel)
        {
            bgm_Channel.OnPlay += Handle_BGM_Play;
            bgm_Channel.OnStop += Handle_BGM_Stop;
        }
    }

    private void OnDisable()
    {
        if (sfx_Channel)
        {
            sfx_Channel.OnPlay -= Handle_SFX_Play;
            sfx_Channel.OnPlay_Attached -= Handle_SFX_Play_Attached;
        }
        if (bgm_Channel)
        {
            bgm_Channel.OnPlay -= Handle_BGM_Play;
            bgm_Channel.OnStop -= Handle_BGM_Stop;
        }
    }

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
            pool.Enqueue(Create_Source());

        if (!bgm) bgm = gameObject.AddComponent<AudioSource>();
        bgm.playOnAwake = false;
        bgm.loop = true;
        bgm.spatialBlend = 0.0f;

        if (default_Bgm_Group) bgm.outputAudioMixerGroup = default_Bgm_Group;
    }

    private void Start()
    {
        if (play_On_Awake && default_Bgm_Clip)
            Play_BGM(default_Bgm_Clip);

        Ensure_Mixer_Ref();

        AudioSettings_Store.Apply_To_Mixer(mixer, master_Param, bgm_Param, sfx_Param);

        AudioSettings_Store.OnChanged += Apply_From_Store;
    }

    float Read01(string key, float def01)
    {
        var v = PlayerPrefs.GetFloat(key, def01);
        if (v <= kMin_Lin) v = def01;
        return Mathf.Clamp01(v);
    }

    [ContextMenu("AudioPrefs: Reset To Defaults")]
    public void Reset_AudioPrefs()
    {
        PlayerPrefs.DeleteKey("AM_Master_Lin");
        PlayerPrefs.DeleteKey("AM_Bgm_Lin");
        PlayerPrefs.DeleteKey("AM_Sfx_Lin");
        Apply_Volumes01(default_Master, default_Bgm, default_Sfx);
    }

    private void OnDestroy()
    {
        AudioSettings_Store.OnChanged -= Apply_From_Store;
    }

    // ------------------------------------------------------------
    // SFX
    // ------------------------------------------------------------
    AudioSource Create_Source()
    {
        var s = Instantiate(sfx_Source_Prefab, transform);
        s.playOnAwake = false;
        s.gameObject.SetActive(false);

        if (default_Sfx_Group) s.outputAudioMixerGroup = default_Sfx_Group;

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
        else if (default_Sfx_Group) src.outputAudioMixerGroup = default_Sfx_Group;

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
        yield return new WaitForSecondsRealtime(len / Mathf.Max(0.01f, pitch));
        if (playing_Count.ContainsKey(ev))
            playing_Count[ev] = Mathf.Max(0, playing_Count[ev] - 1);
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
        else if (default_Sfx_Group) src.outputAudioMixerGroup = default_Sfx_Group;
            
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

    public void Play_BGM(AudioClip clip = null)
    {
        if (clip != null) bgm.clip = clip;
        if (!bgm.clip) return;

        if (!bgm.isPlaying)
            bgm.Play();
    }

    public void Stop_BGM()
    {
        if (bgm.isPlaying)
            bgm.Stop();
    }

    void Handle_BGM_Play(AudioClip clip, float fade_Seconds, bool loop)
    {
        if (!bgm) return;
        if (default_Bgm_Group) bgm.outputAudioMixerGroup = default_Bgm_Group;

        if (clip && bgm.clip != clip) bgm.clip = clip;
        bgm.loop = loop;

        if (!bgm.isPlaying) bgm.Play();
    }

    void Handle_BGM_Stop(float fade_Seconds)
    {
        if (bgm && bgm.isPlaying) bgm.Stop();
    }

    // ------------------------------------------------------------
    // Mixer Slider
    // ------------------------------------------------------------

    void Ensure_Mixer_Ref()
    {
        if (!mixer)
        {
            if (default_Sfx_Group) mixer = default_Sfx_Group.audioMixer;
            else if (default_Bgm_Group) mixer = default_Bgm_Group.audioMixer;
        }
    }

    public float GetMaster01() => PlayerPrefs.GetFloat(KMaster, default_Master);
    public float GetBgm01() => PlayerPrefs.GetFloat(KBgm, default_Bgm);
    public float GetSfx01() => PlayerPrefs.GetFloat(KSfx, default_Sfx);

    public void Apply_Volumes01(float master01, float bgm01, float sfx01, bool save = true)
    {
        Ensure_Mixer_Ref();
        master01 = Mathf.Clamp01(master01);
        bgm01 = Mathf.Clamp01(bgm01);
        sfx01 = Mathf.Clamp01(sfx01);

        if (mixer)
        {
            mixer.SetFloat(master_Param, Linear_To_Db(master01));
            mixer.SetFloat(bgm_Param, Linear_To_Db(bgm01));
            mixer.SetFloat(sfx_Param, Linear_To_Db(sfx01));
        }
        else
        {
            AudioListener.volume = master01;
            if (bgm) bgm.volume = bgm01;
        }

        if (save)
        {
            PlayerPrefs.SetFloat(KMaster, master01);
            PlayerPrefs.SetFloat(KBgm, bgm01);
            PlayerPrefs.SetFloat(KSfx, sfx01);
            PlayerPrefs.Save();
        }
    }

    private void Apply_From_Store(float m, float b, float s)
    {
        if (!mixer) return;
        mixer.SetFloat(master_Param, AudioSettings_Store.Lin_To_Db(m));
        mixer.SetFloat(bgm_Param, AudioSettings_Store.Lin_To_Db(b));
        mixer.SetFloat(sfx_Param, AudioSettings_Store.Lin_To_Db(s));
    }

    public void UI_OnMaster_Changed(float v01)
        => AudioSettings_Store.Set(v01, AudioSettings_Store.Bgm, AudioSettings_Store.Sfx);
    public void UI_OnBgm_Changed(float v01)
        => AudioSettings_Store.Set(AudioSettings_Store.Master, v01, AudioSettings_Store.Sfx);
    public void UI_OnSfx_Changed(float v01)
        => AudioSettings_Store.Set(AudioSettings_Store.Master, AudioSettings_Store.Bgm, v01);

    public void UI_OnMasterChanged_0_10(float v10) => UI_OnMaster_Changed(v10 * 0.1f);
    public void UI_OnBgmChanged_0_10(float v10) => UI_OnBgm_Changed(v10 * 0.1f);
    public void UI_OnSfxChanged_0_10(float v10) => UI_OnSfx_Changed(v10 * 0.1f);

}
