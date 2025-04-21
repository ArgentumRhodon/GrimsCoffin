using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Playables;
using JetBrains.Annotations;

public class AnimationCutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI indicatorText;
    public TextMeshProUGUI skipText;
    public SceneController sceneController;
    public GameObject DialogueUI;
    public Image Portait;

    [Header("Sentences")]
    public string[] sentences;
    public int[] SpeakerID;
    public Sprite[] Speaker;
    public PlayableDirector PrevAnimation;
    public PlayableDirector NextAnimation;
    public PlayableDirector HoldAnimation;

    [Header("Typewriter Settings")]
    [SerializeField] private float charactersPerSecond = 40f;
    [SerializeField] private float punctuationDelay =0.2f;

    private PlayerControls controls;
    [SerializeField] private PlayerInput playerInput;
    private int currentSentenceIndex = 0;
    private bool cutsceneActive = false;
    private bool Skipable = true;
    [SerializeField] private float skipTimer = 0.5f;
    [SerializeField] private Image KeyboardSkipRefill;
    [SerializeField] private Image XboxSkipRefill;
    [SerializeField] private Image PlaystationSkipRefill;
    [SerializeField] private GameObject SkipIndicator;

    // State for typewriter effect
    [SerializeField]private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentSentence;

    [Header("Hold Interaction")]
    [SerializeField] private float holdTimer = 0.5f;
    private float HoldedTimer; 
    private float HoldedSkipTimer;
    private bool NeedToHold = false;
    private bool isHolding;
    private bool PlayerControl = false;
    [SerializeField] private Image KeyboardRefill;
    [SerializeField] private Image ControllerRefill;
    [SerializeField] private GameObject InteractionPrompt;
    [SerializeField] private Animator SavePoint;

    [Header("Prompt Icons and Audio")]
    [SerializeField] private List<Sprite> continuePromptIcons;
    [SerializeField] private List<Sprite> skipPromptIcons;
    [SerializeField] private List<Sprite> HoldPromptIcons;
    [SerializeField] private Image continuePrompt;
    [SerializeField] private Image skipPrompt;
    [SerializeField] private Image HoldPrompt;
    [SerializeField] private EventReference dxTyping;
    [SerializeField] private EventReference dxContinue;
    private EventInstance dxInstance;

    void Awake()
    {
        cutsceneActive = true;
        dxInstance = RuntimeManager.CreateInstance(dxTyping);
        controls = new PlayerControls();
        controls.Enable();
        //playerInput = GetComponent<PlayerInput>();
        //StartCutscene();
        //AdvanceSentence();
    }

    void OnEnable()
    {
        controls.UI.Enable();
        controls.Dialogue.Skip.started += OnSkipStarted;
        controls.Dialogue.Skip.canceled += OnSkipCanceled;
       
        if (playerInput != null)
        {
            playerInput.onControlsChanged += OnControlsChanged;
        }
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }
        controls.Dialogue.Interact.started -= OnInteractStarted;
        controls.Dialogue.Interact.canceled -= OnInteractCanceled;


        controls.UI.Disable();
    }

    void Update()
    {
        Debug.Log(Skipable);
        // Check for player input:
        if (cutsceneActive&&Skipable && isHolding && !NeedToHold)
        {
            HoldedSkipTimer += Time.deltaTime;
            switch (PersistentDataManager.Instance.ControlScheme)
            {
                case "Keyboard&Mouse":
                    KeyboardSkipRefill.fillAmount = HoldedSkipTimer / skipTimer;
                    break;
                case "Playstation":
                    PlaystationSkipRefill.fillAmount = HoldedSkipTimer / skipTimer;
                    break;
                default:
                    XboxSkipRefill.fillAmount = HoldedSkipTimer / skipTimer;
                    break;
            }
                    Debug.Log(HoldedSkipTimer);
            if (HoldedSkipTimer >= skipTimer)
            {
                KeyboardSkipRefill.fillAmount = 0;
                PlaystationSkipRefill.fillAmount = 0;
                XboxSkipRefill.fillAmount = 0;
                SkipCutsceneImmediately();
            }
        }
        // Update prompt icons based on the current control scheme:

        if (PlayerControl && NeedToHold && isHolding&&!Skipable)
        {
            HoldedTimer += Time.deltaTime;
            switch (PersistentDataManager.Instance.ControlScheme)
            {
                case "Keyboard&Mouse":
                    KeyboardRefill.fillAmount = HoldedTimer / holdTimer;
                    break;
                default:
                    ControllerRefill.fillAmount = HoldedTimer / holdTimer;
                    break;
            }
            Debug.Log(HoldedTimer);
            if (HoldedTimer >= holdTimer)
            {
                Interacted();
                // Prevent repeated triggering during the same hold
                NeedToHold = false;
            }
        }

        if (continuePromptIcons != null)
        {
            switch (PersistentDataManager.Instance.ControlScheme)
            {
                case "Keyboard&Mouse":
                    continuePrompt.sprite = continuePromptIcons[0];
                    skipPrompt.sprite = skipPromptIcons[0];
                    if (HoldPrompt != null)
                    {
                        HoldPrompt.sprite = HoldPromptIcons[0];
                    }
                    break;
                case "Playstation":
                    continuePrompt.sprite = continuePromptIcons[1];
                    skipPrompt.sprite = skipPromptIcons[1];
                    if (HoldPrompt != null)
                    {
                        HoldPrompt.sprite = HoldPromptIcons[1];
                    }
                    break;
                default:
                    continuePrompt.sprite = continuePromptIcons[2];
                    skipPrompt.sprite = skipPromptIcons[2];
                    if (HoldPrompt != null)
                    {
                        HoldPrompt.sprite = HoldPromptIcons[2];
                    }
                    break;
            }
        }
    }

    public void StartCutscene()
    {
        DialogueUI.SetActive(true);
        cutsceneActive = true;
        currentSentenceIndex = 0;
        if (sentences.Length > 0)
        {
            Portait.sprite = Speaker[SpeakerID[currentSentenceIndex]];
            RuntimeManager.StudioSystem.setParameterByName("DialogueIndex", SpeakerID[currentSentenceIndex]);
            dxInstance.start();
            RuntimeManager.StudioSystem.setParameterByName("IsSpeaking", 1);
            StartCoroutine(ShowSentence(sentences[currentSentenceIndex]));
        }
        UpdateSkipText();
        // Optionally start the FMOD event: dxInstance.start();
    }

    /// <summary>
    /// Handles the player's continue input.
    /// If the sentence is still typing, this input will skip it (show full text).
    /// Otherwise, it advances to the next sentence.
    /// </summary>
    void OnContinue(InputAction.CallbackContext ctx)
    {
        if (isTyping)
        {
            // First press: stop typing and show the complete sentence immediately.
            SkipTyping();
        }
        else
        {
            // Next press: advance to the next sentence.
            AdvanceSentence();
        }
    }

    void Interacted() 
    {
        Debug.Log("Hold Animation Play");
        NextAnimation.Stop();
        HoldAnimation.Play();
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        isHolding = true;
        Debug.Log("Holded");
        HoldedTimer = 0f;
    }
    private void OnSkipStarted(InputAction.CallbackContext context)
    {
        isHolding = true;
        Debug.Log("Holded");
        // Optionally reset the timer for a fresh hold
        HoldedSkipTimer = 0f;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        isHolding = false;
        Debug.Log("Canceled");
        HoldedTimer = 0f;
        KeyboardRefill.fillAmount = 0;
        ControllerRefill.fillAmount = 0;
    }
    private void OnSkipCanceled(InputAction.CallbackContext context)
    {
        isHolding = false;
        Debug.Log("Canceled");
        HoldedSkipTimer = 0f;
        KeyboardSkipRefill.fillAmount = 0;
        PlaystationSkipRefill.fillAmount = 0;
        XboxSkipRefill.fillAmount = 0;
        }


    /// <summary>
    /// Advances to the next sentence or loads the next scene if there are no more sentences.
    /// </summary>
    void AdvanceSentence()
    {
        dxInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentSentenceIndex++;
        if (currentSentenceIndex < sentences.Length)
        {
            Portait.sprite = Speaker[SpeakerID[currentSentenceIndex]];
            RuntimeManager.StudioSystem.setParameterByName("DialogueIndex", SpeakerID[currentSentenceIndex]);
            dxInstance.start();
            RuntimeManager.StudioSystem.setParameterByName("IsSpeaking", 1);
            StartCoroutine(ShowSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            DialogueUI.SetActive(false);
            //PrevAnimation.time = 0;
            StopAllCoroutines();
            PrevAnimation.Stop();
            NextAnimation.Play();
            Debug.Log("Continue");
            //StopAllCoroutines();
            controls.Dialogue.Continue.performed -= OnContinue;
        }
    }

    /// <summary>
    /// Begins displaying the given sentence using the typewriter effect.
    /// </summary>
    IEnumerator ShowSentence(string sentence)
    {
        controls.Dialogue.Continue.performed += OnContinue;
        currentSentence = sentence;
        dialogueText.text = "";
        typingCoroutine = StartCoroutine(TypeTextRoutine(sentence));
        // Wait until the typewriter effect completes (or is skipped)
        yield return new WaitUntil(() => !isTyping);
    }

    /// <summary>
    /// Reveals text character-by-character. Pauses extra on punctuation.
    /// </summary>
    IEnumerator TypeTextRoutine(string fullText)
    {
        isTyping = true;
        WaitForSeconds normalDelay = new WaitForSeconds(1f / charactersPerSecond);
        WaitForSeconds punctWait = new WaitForSeconds(punctuationDelay);

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            if (IsPunctuation(fullText[i]))
                yield return punctWait;
            else
                yield return normalDelay;
        }
        RuntimeManager.StudioSystem.setParameterByName("IsSpeaking", 0);
        isTyping = false;
    }

    /// <summary>
    /// Immediately stops the typewriter effect and displays the full sentence.
    /// </summary>
    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            RuntimeManager.StudioSystem.setParameterByName("IsSpeaking", 0);
            StopCoroutine(typingCoroutine);
        }
        dialogueText.text = currentSentence;
        isTyping = false;
    }

    /// <summary>
    /// Immediately skips the entire cutscene and loads the next scene.
    /// </summary>
    void SkipCutsceneImmediately()
    {
        StopAllCoroutines();
        sceneController.LoadNextScene();
        controls.Dialogue.Continue.performed -= OnContinue;
    }

    void OnControlsChanged(PlayerInput obj)
    {
        UpdateSkipText();
    }


    private void UpdateSkipText()
    {
        if (skipText == null || playerInput == null)
            return;

        string currentScheme = playerInput.currentControlScheme;
        // Customize the skip text based on the control scheme if needed.
        skipText.text = "press\tto skip";
    }

    /// <summary>
    /// Helper method to determine if a character is punctuation.
    /// </summary>
    private bool IsPunctuation(char c)
    {
        return (c == '.' || c == ',' || c == '!' ||
                c == '?' || c == ';' || c == ':' || c == '-');
    }

    public void GiveBackControl() 
    {
        controls.Dialogue.Skip.started -= OnSkipStarted;
        controls.Dialogue.Skip.canceled -= OnSkipCanceled;
        controls.Dialogue.Interact.started += OnInteractStarted;
        controls.Dialogue.Interact.canceled += OnInteractCanceled;
        Skipable = false;
        SkipIndicator.SetActive(false);
        PlayerControl = true;
        NeedToHold = true;
        InteractionPrompt.SetActive(true);
        SavePoint.Play("ActiveIdle");
        //playerInput.SwitchCurrentActionMap("Player");
    }
}
