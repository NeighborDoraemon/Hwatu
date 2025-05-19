using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Manager : MonoBehaviour
{
    public CinemachineVirtualCamera virtual_Camera;
    public CinemachineConfiner confiner;

    [Header("Shake Settings")]
    public float shake_Duration = 0.2f;
    public float shake_Amplitude = 1.0f;
    public float shake_Frequency = 2.0f;

    private CinemachineBasicMultiChannelPerlin noise;
    private Coroutine shake_Routine;

    private void Awake()
    {
        noise = virtual_Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogError("[Camera Manager] Can't found CinemachineBasicMultiChannelPerlin component");
        }
    }

    public void Update_Confiner(Collider2D new_Boundary)
    {
        if (confiner != null)
        {
            confiner.m_BoundingShape2D = new_Boundary;
            confiner.InvalidatePathCache();
        }
    }

    public void Shake_Camera()
    {
        if (noise == null) return;
        if (shake_Routine != null)
        {
            StopCoroutine(shake_Routine);
        }
        shake_Routine = StartCoroutine(Shake_Routine());
    }

    private IEnumerator Shake_Routine()
    {
        float elapsed = 0.0f;

        noise.m_AmplitudeGain = shake_Amplitude;
        noise.m_FrequencyGain = shake_Frequency;

        while (elapsed < shake_Duration)
        {
            elapsed += Time.deltaTime;
            noise.m_AmplitudeGain = Mathf.Lerp(shake_Amplitude, 0.0f, elapsed / shake_Duration);
            yield return null;
        }

        noise.m_AmplitudeGain = 0.0f;
    }
}
