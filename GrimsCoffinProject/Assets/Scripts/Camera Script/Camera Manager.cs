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
    
    public CinemachineVirtualCamera VCam
    {
        get { return Vcam; }
    }

    public static CameraManager Instance;

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

    private void Update()
    {
        /*if (PlayerControllerForces.Instance != null)
        {
            if (!PlayerControllerForces.Instance.playerState.IsFacingRight)
            {
                Vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x *= -1;
            }

            else
                Vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x *= -1;
        }*/
    }

    public void LookDown()
    {
        float targetScreenY = 0.25f;
        StartScreenYTransition(targetScreenY, 0.1f);
    }

    public void LookUp()
    {
        float targetScreenY = 0.75f;
        StartScreenYTransition(targetScreenY, 0.1f);
    }

    public void Reset()
    {
        float targetScreenY = 0.5f;
        StartScreenYTransition(targetScreenY, 0.1f);
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

    public void StartScreenXOffset(float targetOffsetX, float duration)
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }
        transitionCoroutine = StartCoroutine(ScreenXOffsetCoroutine(targetOffsetX, duration));
    }

    private IEnumerator ScreenXOffsetCoroutine(float targetScreenX, float duration)
    {
        float initialScreenX = VCamFramingTransposer.m_TrackedObjectOffset.x; 
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = t * t * (3f - 2f * t);

            VCamFramingTransposer.m_TrackedObjectOffset.x = Mathf.Lerp(initialScreenX, targetScreenX, t);
            yield return null;
        }

        VCamFramingTransposer.m_TrackedObjectOffset.x = targetScreenX;
        transitionCoroutine = null;
    }
}
