using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera Vcam;
    [SerializeField] private CinemachineBrain CameraControl;
    [SerializeField] private float deadzone;
    private CinemachineFramingTransposer VCamFramingTransposer;

    private Coroutine xTransitionCoroutine;
    private Coroutine yTransitionCoroutine;

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
            {
                Vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset.x *= -1;
            }
        }*/
    }

    public void LookUp()
    {
        StartScreenYOffset(4.5f, 0.2f);
        //Debug.Log("Up");
    }

    public void LookDown()
    {
        StartScreenYOffset(-4.5f, 0.2f);
        //Debug.Log("Down");
    }

    public void Reset()
    {
        StartScreenYOffset(0.5f, 0.2f);
        //Debug.Log("Reset");
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

    public void StartScreenXOffset(float targetOffsetX, float duration)
    {
        if (xTransitionCoroutine != null)
        {
            StopCoroutine(xTransitionCoroutine);
        }
        xTransitionCoroutine = StartCoroutine(ScreenXOffsetCoroutine(targetOffsetX, duration));
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
        xTransitionCoroutine = null;
    }

    public void StartScreenYOffset(float targetOffsetY, float duration)
    {
        if (yTransitionCoroutine != null)
        {
            StopCoroutine(yTransitionCoroutine);
        }
        yTransitionCoroutine = StartCoroutine(ScreenYOffsetCoroutine(targetOffsetY, duration));
    }

    private IEnumerator ScreenYOffsetCoroutine(float targetScreenY, float duration)
    {
        float initialScreenY = VCamFramingTransposer.m_TrackedObjectOffset.y;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            t = t * t * (3f - 2f * t); 

            VCamFramingTransposer.m_TrackedObjectOffset.y = Mathf.Lerp(initialScreenY, targetScreenY, t);
            yield return null;
        }

        VCamFramingTransposer.m_TrackedObjectOffset.y = targetScreenY;
        yTransitionCoroutine = null;
    }
}