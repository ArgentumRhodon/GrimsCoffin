using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] public PauseScreenBehavior pauseScript;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Volume lowHealthVignette;
    [SerializeField] private Volume damageVignette;

    [SerializeField] public GameObject gameUI;

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

    private void Update()
    {
        if (damageVignette.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            damageVignette.GetComponent<Animator>().enabled = false;
            damageVignette.gameObject.SetActive(false);
        }
    }

    public void Pause()
    {
        pauseScript.Pause();
    }

    public void LowHealthVignette(bool lowHealth)
    {
        lowHealthVignette.gameObject.GetComponent<Animator>().enabled = lowHealth;
        lowHealthVignette.enabled = lowHealth;
    }

    public void DamageVignette()
    {
        damageVignette.gameObject.SetActive(true);
        damageVignette.GetComponent<Animator>().enabled = true;
    }

    public void HandlePlayerDeath()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
