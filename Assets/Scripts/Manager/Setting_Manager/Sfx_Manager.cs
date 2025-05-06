using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sfx_Manager : MonoBehaviour
{
    public static Sfx_Manager Instance;

    [SerializeField] private AudioSource Sfx_Audio;
    [SerializeField] private List<AudioClip> Sfx_Clips = new List<AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Play_Sfx(int index)
    {
        if (index < 0 || index >= Sfx_Clips.Count)
        {
            Debug.LogError("Sfx index out of range: " + index);
            return;
        }
        Sfx_Audio.PlayOneShot(Sfx_Clips[index]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
