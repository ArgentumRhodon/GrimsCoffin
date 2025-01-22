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
    [SerializeField] public EquilibriumPrompt equilibriumPrompt;

    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Volume lowHealthVignette;
    [SerializeField] private Volume damageVignette;

    [SerializeField] public GameObject gameUI;
    [SerializeField] public GameObject areaText;
    [SerializeField] private GameObject saveIcon;

    [SerializeField] private GameObject fullMapUI;
    [SerializeField] private List<GameObject> mapRooms;
    [SerializeField] public GameObject dialogueUI;

    [SerializeField] public PlayerInput playerInput;

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
        if (damageVignette == null)
            return;

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
        if (lowHealthVignette == null)
            return;

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
        PersistentDataManager.Instance.ToggleFirstSpawn(true);
        deathScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ToggleMap()
    {
        bool mapActive = !fullMapUI.activeInHierarchy;

        gameUI.SetActive(!mapActive);
        fullMapUI.SetActive(mapActive);

        if (mapActive)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void ToggleDialogueUI(bool toggle)
    {
        if (toggle)
        {
            StartCoroutine(ShowDialogue(.5f));
        }
        else
        {
            StartCoroutine(HideDialogue(1.3f));
        }
    }

    public IEnumerator ShowSaveIcon(float seconds)
    {
        saveIcon.SetActive(true);

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        saveIcon.SetActive(false);
    }

    public void UpdateMapUI()
    {
        List<bool> roomsExplored = PersistentDataManager.Instance.AreaRoomsLoaded();

        for (int i = 0; i < roomsExplored.Count; i++)
        {
            {
                if (roomsExplored[i])
                    mapRooms[i].SetActive(true);
            }
        }
    }
    public IEnumerator ShowDialogue(float seconds)
    {
        this.GetComponent<DialogueManager>().canProgressDialogue = false;
        dialogueUI.SetActive(true);
        dialogueUI.GetComponent<Animator>().SetBool("ToggleDialogue", true);

        gameUI.SetActive(false);
        Time.timeScale = 0;
        PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);
        PlayerControllerForces.Instance.Sleep(1);
        PlayerControllerForces.Instance.gameObject.GetComponent<PlayerCombat>().ResetCombo();

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        this.GetComponent<DialogueManager>().canProgressDialogue = true;
        playerInput.SwitchCurrentActionMap("Dialogue");
    }

    public IEnumerator HideDialogue(float seconds)
    {
        this.GetComponent<DialogueManager>().canProgressDialogue = false;
        dialogueUI.GetComponent<Animator>().SetBool("ToggleDialogue", false);
        
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(true);
        gameUI.SetActive(true);
        Time.timeScale = 1;
        playerInput.SwitchCurrentActionMap("Player");
    }
}
