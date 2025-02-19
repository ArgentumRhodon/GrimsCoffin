using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI indicatorText;
    public TextMeshProUGUI skipText;
    public SceneController sceneController;

    [Header("Sentences")]
    public string[] sentences;

    [Header("Typewriter Settings")]
    [SerializeField] private float charactersPerSecond = 30f;
    [SerializeField] private float punctuationDelay = 0.5f;

    private PlayerControls controls;
    private PlayerInput playerInput;
    private int currentSentenceIndex = 0;
    private bool cutsceneActive = false;

    // State for typewriter effect
    [SerializeField]private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentSentence;

    [Header("Prompt Icons and Audio")]
    [SerializeField] private List<Sprite> continuePromptIcons;
    [SerializeField] private List<Sprite> skipPromptIcons;
    [SerializeField] private Image continuePrompt;
    [SerializeField] private Image skipPrompt;
    [SerializeField] private EventReference dxTyping;
    [SerializeField] private EventReference dxContinue;
    private EventInstance dxInstance;

    void Awake()
    {
        dxInstance = RuntimeManager.CreateInstance(dxTyping);
        controls = new PlayerControls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
        StartCutscene();
        //AdvanceSentence();
    }

    void OnEnable()
    {
        controls.UI.Enable();
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
        controls.UI.Disable();
    }

    void Update()
    {
        // Check for player input:
        if (cutsceneActive)
        {
            if (controls.Dialogue.Skip.triggered)
            {
                SkipCutsceneImmediately();
            }
        }

        // Update prompt icons based on the current control scheme:
        if (continuePromptIcons != null)
        {
            switch (playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    continuePrompt.sprite = continuePromptIcons[0];
                    skipPrompt.sprite = skipPromptIcons[0];
                    break;
                case "Playstation":
                    continuePrompt.sprite = continuePromptIcons[1];
                    skipPrompt.sprite = skipPromptIcons[1];
                    break;
                default:
                    continuePrompt.sprite = continuePromptIcons[2];
                    skipPrompt.sprite = skipPromptIcons[2];
                    break;
            }
        }
    }

    public void StartCutscene()
    {
        cutsceneActive = true;
        currentSentenceIndex = 0;
        if (sentences.Length > 0)
        {
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

    /// <summary>
    /// Advances to the next sentence or loads the next scene if there are no more sentences.
    /// </summary>
    void AdvanceSentence()
    {
        RuntimeManager.PlayOneShot(dxContinue);

        currentSentenceIndex++;
        if (currentSentenceIndex < sentences.Length)
        {
            StartCoroutine(ShowSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            //Debug.Log("1");
            StopAllCoroutines();
            sceneController.LoadNextScene();
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
        isTyping = false;
    }

    /// <summary>
    /// Immediately stops the typewriter effect and displays the full sentence.
    /// </summary>
    void SkipTyping()
    {
        if (typingCoroutine != null)
        {
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
}
