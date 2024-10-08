using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private PauseScreenBehavior pauseScript;
    [SerializeField] private Volume postProcessVolume;

    [SerializeField] private bool isPaused = false;

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

    public void Pause()
    {
        pauseScript.Pause();
    }

    public void LowHealthVignette(bool lowHealth)
    {
        postProcessVolume.gameObject.GetComponent<Animator>().enabled = lowHealth;
        postProcessVolume.enabled = lowHealth;
    }
}
