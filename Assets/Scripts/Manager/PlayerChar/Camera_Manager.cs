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

    [Header("Rotation Shake Settings")]
    [Tooltip("회전 흔들림의 최대 각도")]
    public float rotation_Amplitude = 1.0f;
    [Tooltip("회전 흔들림 갱신 빈도(초당 횟수)")]
    public float rotation_Frequency = 25.0f;

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
        float rot_Elapsed = 0.0f;
        float cur_Rot = 0.0f;

        noise.m_AmplitudeGain = shake_Amplitude;
        noise.m_FrequencyGain = shake_Frequency;

        Quaternion og_Rot = virtual_Camera.transform.localRotation;

        while (elapsed < shake_Duration)
        {
            elapsed += Time.deltaTime;
            rot_Elapsed += Time.deltaTime;

            if (rot_Elapsed >= 1.0f / rotation_Frequency)
            {
                rot_Elapsed = 0.0f;
                cur_Rot = Random.Range(-rotation_Amplitude, rotation_Amplitude);
            }

            float damper = 1.0f - (elapsed / shake_Duration);

            noise.m_AmplitudeGain = Mathf.Lerp(shake_Amplitude, 0.0f, elapsed / shake_Duration);

            float rot_Angle = cur_Rot * damper;
            virtual_Camera.transform.localRotation = og_Rot * Quaternion.Euler(0.0f, 0.0f, rot_Angle);

            yield return null;
        }

        noise.m_AmplitudeGain = 0.0f;
        virtual_Camera.transform.localRotation = og_Rot;
    }
}
