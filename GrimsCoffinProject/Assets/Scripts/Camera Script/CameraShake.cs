using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    public bool inBossFight;
    [SerializeField] private CinemachineVirtualCamera Vcam;
    [SerializeField] private CinemachineVirtualCamera bossCam;
    [SerializeField] private float shakeTimer;
    [SerializeField] private CinemachineBasicMultiChannelPerlin cameraShake;
    [SerializeField] private CinemachineBasicMultiChannelPerlin bossCameraShake;

    private void Awake()
    {
        Instance = this;
        Vcam = GetComponent<CinemachineVirtualCamera>();
        cameraShake = Vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (bossCam != null)
            bossCameraShake = bossCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ShakeCamera(float amplitude, float frequency, float time) 
    {
        if (inBossFight)
        {
            bossCameraShake.m_AmplitudeGain = amplitude;
            bossCameraShake.m_FrequencyGain = frequency;
        }

        else
        {
            cameraShake.m_AmplitudeGain = amplitude;
            cameraShake.m_FrequencyGain = frequency;
        }

        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer <= 0 && !inBossFight) 
        {
            cameraShake.m_AmplitudeGain = 0;
            cameraShake.m_FrequencyGain = 0;
        }

        else if (shakeTimer <= 0 && inBossFight)
        {
            bossCameraShake.m_AmplitudeGain = 0;
            bossCameraShake.m_FrequencyGain = 0;
        }

        else
        {
            shakeTimer -= Time.deltaTime;
        }
    }
}
