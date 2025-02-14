using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    //Script References
    [SerializeField] public PauseScreenBehavior pauseScript;
    [SerializeField] public RestPointMenu restPointMenu;
    [SerializeField] private Map mapScript;

    //Post-Processing & Effects
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Volume lowHealthVignette;
    [SerializeField] private Volume damageVignette;

    //UI Object References
    [SerializeField] public GameObject gameUI;
    [SerializeField] public GameObject areaText;
    [SerializeField] private GameObject saveIcon;
    [SerializeField] private GameObject abilityUnlockPrefab;

    //Map Specific References
    [SerializeField] private GameObject minimapUI;
    [SerializeField] public GameObject fullMapUI;
    [SerializeField] private List<GameObject> mapRooms;
    [SerializeField] private List<Sprite> mapPromptIcons;
    [SerializeField] private Image mapPrompt;
  
    //Dialogue
    [SerializeField] public GameObject dialogueUI;

    //Player Input
    [SerializeField] public PlayerInput playerInput;

    public bool scytheThrowInMenu;

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
        //Toggle minimap if unlocked by player
        if (PlayerControllerForces.Instance.Data.canViewMap && minimapUI != null)
            minimapUI.SetActive(true);

        if (damageVignette == null || !damageVignette.gameObject.activeInHierarchy)
            return;

        //Turn damage vignette off after being played
        if (damageVignette.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
        {
            damageVignette.GetComponent<Animator>().enabled = false;
            damageVignette.gameObject.SetActive(false);
        }
    }

    public void Pause()
    {
        if (dialogueUI.activeInHierarchy || restPointMenu.gameObject.activeInHierarchy)
            return;

        if (fullMapUI != null)
            if (fullMapUI.activeInHierarchy)
                return;

        pauseScript.Pause();
    }

    //Toggle low health vignette on/off
    public void LowHealthVignette(bool lowHealth)
    {
        if (lowHealthVignette == null)
            return;

        lowHealthVignette.gameObject.GetComponent<Animator>().enabled = lowHealth;
        lowHealthVignette.enabled = lowHealth;
    }

    //Toggle damage vignette on
    public void DamageVignette()
    {
        damageVignette.gameObject.SetActive(true);
        damageVignette.GetComponent<Animator>().enabled = true;
    }

    //Toggle death effect on
    public void HandlePlayerDeath()
    {
        PersistentDataManager.Instance.ToggleFirstSpawn(true);
        deathScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    //Toggle minimap on/off
    public void ToggleMap()
    {
        if (!PlayerControllerForces.Instance.Data.canViewMap || pauseScript.isPaused || restPointMenu.gameObject.activeInHierarchy || fullMapUI == null)
            return;

        bool mapActive = !fullMapUI.activeInHierarchy;

        gameUI.SetActive(!mapActive);

        //bool temp = mapScript.map
        fullMapUI.SetActive(mapActive);

        if (mapActive)
        {
            Time.timeScale = 0;
            ResetMap();
        }
            
        else
        {
            Time.timeScale = 1;
        }
            
    }

    //Toggle dialogue UI on/off
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

    //Show save icon for specified time
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

    //Update Map UI when new room is explored
    public void UpdateMapUI()
    {
        if (mapRooms != null)
        {
            //Get list of bools for if each room is explored or not
            List<bool> roomsExplored = PersistentDataManager.Instance.AreaRoomsLoaded();

            for (int i = 0; i < roomsExplored.Count; i++)
            {
                //If a room is explored, set it active
                if (roomsExplored[i] && mapRooms.Count > 0)
                    mapRooms[i].SetActive(true);
            }
        }
    }

    public void ZoomMap(bool zoomIn)
    {
        mapScript.ZoomMap(zoomIn);
    }

    public void PanMap(Vector2 input, bool drag)
    {
        mapScript.PanMap(input, drag);
    }

    public void ResetMap()
    {
        if (UIManager.Instance.fullMapUI != null)
            mapScript.ResetMap();
    }

    public void ToggleMapKey()
    {
        if (UIManager.Instance.fullMapUI != null)
            mapScript.ToggleMapKey();
    }

    //Show a panel to inform an ability has been unlocked
    public void ShowAbilityUnlock(string abilityName)
    {
        Debug.Log("ABILITY UNLOCK");

        if (abilityUnlockPrefab == null)
            return;

        GameObject popup = Instantiate(abilityUnlockPrefab, gameUI.transform);
        popup.GetComponent<AbilityUnlock>().unlockMessage = abilityName;
    }

    //Show dialogue UI
    public IEnumerator ShowDialogue(float seconds)
    {
        //Animate the UI
        this.GetComponent<DialogueManager>().canProgressDialogue = false;
        dialogueUI.SetActive(true);
        dialogueUI.GetComponent<Animator>().SetBool("ToggleDialogue", true);

        //Disable area text if it's active
        if (areaText != null)
            areaText.SetActive(false);

        gameUI.SetActive(false);
        PlayerControllerForces.Instance.ToggleSleep(true);

        //Disable player control
        PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);
        PlayerControllerForces.Instance.gameObject.GetComponent<PlayerCombat>().ResetCombo();

        //Wait before allowing player to progress through dialogue
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        this.GetComponent<DialogueManager>().canProgressDialogue = true;
        playerInput.SwitchCurrentActionMap("Dialogue");
    }

    //Hides the dialogue UI
    public IEnumerator HideDialogue(float seconds)
    {
        //Animate the UI in reverse
        this.GetComponent<DialogueManager>().canProgressDialogue = false;
        dialogueUI.GetComponent<Animator>().SetBool("ToggleDialogue", false);
        
        //Wait before giving control to the player
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        //Enable game UI and give control to the player
        PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(true);
        gameUI.SetActive(true);
        dialogueUI.SetActive(false);
        PlayerControllerForces.Instance.ToggleSleep(false);
        playerInput.SwitchCurrentActionMap("Player");
        Debug.Log("1111");
    }

    //Cancel out of menus that are active
    public IEnumerator Cancel(float seconds)
    {
        if (pauseScript.controlsScreen.activeInHierarchy)
        {
            PlayerControllerForces.Instance.scytheThrown = true;
            pauseScript.ToggleControls();
        }

        else if (pauseScript.gameObject.activeInHierarchy)
        {
            PlayerControllerForces.Instance.scytheThrown = true;
            scytheThrowInMenu = true;
            Pause();
        }

        else if (restPointMenu.gameObject.activeInHierarchy)
        {
            PlayerControllerForces.Instance.scytheThrown = true;
            scytheThrowInMenu = true;
            restPointMenu.ToggleEnterPrompt();
        }

        else if (fullMapUI.activeInHierarchy)
        {
            PlayerControllerForces.Instance.scytheThrown = true;
            scytheThrowInMenu = true;
            ToggleMap();
        }

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < seconds)
        {
            yield return null;
        }

        PlayerControllerForces.Instance.scytheThrown = false;
        scytheThrowInMenu = false;
    }
}
