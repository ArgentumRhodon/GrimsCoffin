using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera Vcam;
    private CinemachineFramingTransposer VCamFramingTransposer;
    private Coroutine transitionCoroutine;

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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            LookDown();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            LookUp();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reset();
        }
    }

    public void LookDown()
    {
        float targetScreenY = 0.25f;
        StartScreenYTransition(targetScreenY, 0.5f);
    }

    public void LookUp()
    {
        float targetScreenY = 0.8f;
        StartScreenYTransition(targetScreenY, 0.5f);
    }

    public void Reset()
    {
        float targetScreenY = 0.5f;
        StartScreenYTransition(targetScreenY, 0.5f);
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
