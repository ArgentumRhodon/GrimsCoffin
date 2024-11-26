using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera Vcam;
    [SerializeField] private CinemachineBrain CameraControl;
    [SerializeField] private float deadzone;
    private CinemachineFramingTransposer VCamFramingTransposer;
    private Coroutine transitionCoroutine;

    public float Deadzone
    {
        get { return deadzone; }
    }

    public static CameraManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        VCamFramingTransposer = Vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (VCamFramingTransposer == null)
        {
            Debug.LogError("CinemachineFramingTransposer component not found on the virtual camera.");
            return;
        }
        Reset();
    }



    public void LookDown()
    {
        float targetScreenY = 0.25f;
        StartScreenYTransition(targetScreenY, 0.15f);
    }

    public void LookUp()
    {
        float targetScreenY = 0.8f;
        StartScreenYTransition(targetScreenY, 0.15f);
    }

    public void Reset()
    {
        float targetScreenY = 0.5f;
        StartScreenYTransition(targetScreenY, 0.15f);
    }

    public void ChangeCamera(CinemachineVirtualCamera Cam) 
    {
        CameraControl.ActiveVirtualCamera.Priority = 9;
        Cam.Priority = 10;
    }
    public void CameraReset()
    {
        CameraControl.ActiveVirtualCamera.Priority = 9;
        Vcam.Priority = 10;
    }



    private void StartScreenYTransition(float targetScreenY, float duration)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(ScreenYTransitionCoroutine(targetScreenY, duration));
    }

    private IEnumerator ScreenYTransitionCoroutine(float targetScreenY, float duration)
    {
        float initialScreenY = VCamFramingTransposer.m_ScreenY;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = t * t * (3f - 2f * t);

            VCamFramingTransposer.m_ScreenY = Mathf.Lerp(initialScreenY, targetScreenY, t);
            yield return null;
        }

        VCamFramingTransposer.m_ScreenY = targetScreenY;
        transitionCoroutine = null;
    }
}
