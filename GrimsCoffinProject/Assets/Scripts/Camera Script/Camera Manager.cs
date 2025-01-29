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
    private Coroutine transitionCoroutineX;
    private Coroutine transitionCoroutineY;

    private int currentpriority;

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
        StartScreenYTransition(targetScreenY, 0.1f,2);
    }

    public void LookUp()
    {
        float targetScreenY = 0.75f;
        StartScreenYTransition(targetScreenY, 0.1f,2);
    }

    public void Reset()
    {
        //Debug.Log("Camera Reset is running");
        float targetScreenY = 0.5f;
        StartScreenYTransition(targetScreenY, 0.1f,1);
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



    private void StartScreenYTransition(float targetScreenY, float duration,int priority)
    {
        if (transitionCoroutineY != null)
        {
            if (priority > currentpriority)
                StopCoroutine(transitionCoroutineY);
            else return;
        }
        transitionCoroutineY = StartCoroutine(ScreenYTransitionCoroutine(targetScreenY, duration));
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
        transitionCoroutineY = null;
    }

    public void StartScreenXOffset(float targetOffsetX, float duration, int priority)
    {
        if (transitionCoroutineX != null)
        {
            if (priority > currentpriority)
                StopCoroutine(transitionCoroutineX);
            else return;
        }
        transitionCoroutineX = StartCoroutine(ScreenXOffsetCoroutine(targetOffsetX, duration));
        currentpriority = priority;
    }
    private IEnumerator ScreenXOffsetCoroutine(float targetScreenX, float duration)
    {
        Vector3 initialOffset = VCamFramingTransposer.m_TrackedObjectOffset;
        float initialScreenX = VCamFramingTransposer.m_TrackedObjectOffset.x;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = t * t * (3f - 2f * t);
            float newX = Mathf.Lerp(initialScreenX, targetScreenX, t);

            Vector3 newOffset = initialOffset;
            newOffset.x = newX;
            VCamFramingTransposer.m_TrackedObjectOffset = newOffset;

            yield return null;
        }
        VCamFramingTransposer.m_TrackedObjectOffset.x = targetScreenX;
        transitionCoroutineX = null;
    }
}
