using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera Vcam;
    [SerializeField] private float shakeTimer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraShake;
    private void Awake()
    {
        Instance = this;
        Vcam = GetComponent<CinemachineVirtualCamera>();
        cameraShake = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float amplitude, float frequency, float time) 
    {
        cameraShake.m_AmplitudeGain = amplitude;
        cameraShake.m_FrequencyGain = frequency;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer <= 0) 
        {
            cameraShake.m_AmplitudeGain = 0;
            cameraShake.m_FrequencyGain = 0;
        }

        else
        {
            shakeTimer -= Time.deltaTime;
        }
    }
}
