using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Combined DialogueManager that also handles typewriting.
/// Attach this to a GameObject in your scene (e.g., DialogueManager),
/// and assign the references via the Inspector.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Data")]
    public List<DialogueEntry> dialogues;          // Loaded from JSON
    [SerializeField] private string dialogueFileName = "Dialogue.json";
    private string m_Path;                         // For debugging or advanced usage

    [Header("UI References")]
    public UIManager uiManager;
    [SerializeField] private Image continuePrompt;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private List<Sprite> continuePromptIcons;
    [SerializeField] private List<Sprite> speakers;

    // The main text UI where we show dialogue
    private TextMeshProUGUI dialogueText;

    [Header("Typing Settings")]
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private float punctuationDelay = 0.5f;
    [SerializeField] private bool quickSkip = false;  // you can remove this if you prefer the new 2-press approach only
    [SerializeField][Min(1)] private int skipMultiplier = 5;

    // Internal state for typed text
    private bool isTyping = false;   // Are we in the middle of the typewriter effect?
    private bool isSkipping = false; // Did the user trigger skip?
    private Coroutine typingCoroutine;
    private string currentTypingText; // The text we're currently typing, stored for any reveal logic

    // Dialogue progression
    private int currentLine = 1;     // Tracks which line in the sequence we're on
    private Spirit currentSpirit;    // Which spirit we’re currently talking to
    public bool canProgressDialogue = false; // True once the current line is fully revealed

    // Input
    private PlayerControls controls;
    private PlayerInput playerInput;

    private void Awake()
    {
        // Load the JSON data (or other approach) so dialogues is populated
        m_Path = Application.dataPath; // For debugging
        LoadDialogueData();

        // Set up input
        controls = new PlayerControls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();

        controls.Dialogue.Continue.performed += OnContinue;
    }

    private void Start()
    {
        // If your UIManager has a 'dialogueUI' GameObject with a TextMeshProUGUI child:
        // We'll grab that TextMeshProUGUI so we can type onto it.
        dialogueText = uiManager.dialogueUI.GetComponentInChildren<TextMeshProUGUI>();

        if (!dialogueText)
        {
            Debug.LogWarning("No TextMeshProUGUI found under dialogueUI in UIManager!");
        }
    }

    private void Update()
    {
        if (uiManager.dialogueUI.activeSelf)
        {
            // Example for switching the "Continue" icon based on control scheme
            switch (PlayerControllerForces.Instance.GetComponent<PlayerInput>().currentControlScheme)
            {
                case "Keyboard&Mouse":
                    continuePrompt.sprite = continuePromptIcons[0];
                    break;
                case "Playstation":
                    continuePrompt.sprite = continuePromptIcons[1];
                    break;
                default:
                    continuePrompt.sprite = continuePromptIcons[2];
                    break;
            }
        }
    }

    private void OnContinue(InputAction.CallbackContext ctx)
    {
        if (isTyping)
        {
            Debug.Log("Skipping...");

            SkipTyping(); // First press => skip or reveal
        }
        else
        {
            Debug.Log("Progressing Dialogue");

            // 2) If typing is done (and text fully shown) => we can proceed
            // BUT only if we are not skipping.
            // That ensures we don't jump lines when we just forced it to reveal instantly.
            if (canProgressDialogue && !isSkipping)
            {
                currentLine++;
                ShowDialogueForSpirit(currentSpirit);
            }
        }
    }

    /// <summary>
    /// Loads the dialogue entries from a JSON file (or wherever).
    /// Make sure 'dialogueFileName' exists in your Assets/ or streaming path.
    /// </summary>
    private void LoadDialogueData()
    {
        dialogues = SaveData.ReadFromJSON<DialogueEntry>(dialogueFileName);

        if (dialogues == null || dialogues.Count == 0)
        {
            Debug.LogWarning("No dialogue data loaded or file is empty.");
        }
        else
        {
            Debug.Log($"Loaded {dialogues.Count} dialogue entries.");
        }
    }

    /// <summary>
    /// Called externally to start showing dialogue for a specific Spirit.
    /// This sets the currentSpirit, finds the correct line, shows it, etc.
    /// </summary>
    public void ShowDialogueForSpirit(Spirit spirit)
    {
        currentSpirit = spirit;

        int id = (int)spirit.spiritID;
        int state = (int)spirit.spiritState;

        // Find a dialogue line with matching SpiritID, SpiritState, and currentLine
        DialogueEntry dialogue = dialogues.FirstOrDefault(d =>
            d.SpiritID == id &&
            d.SpiritState == state &&
            d.LineID == currentLine);

        if (dialogue != null)
        {
            // Show the UI
            uiManager.ToggleDialogueUI(true);

            // Update speaker icon
            speakerIcon.sprite = speakers[dialogue.SpeakerID];

            // Start the typed text
            StartTypewriter(dialogue.DialogueContent);
        }
        else
        {
            // No matching line => wrap up
            Debug.LogWarning($"No dialogue found for SpiritID {id}, State {state}, LineID {currentLine}.");

            // Example: if spirit state is 0, destroy the object
            if (spirit.spiritState == 0)
            {
                Destroy(spirit.gameObject.transform.parent.gameObject);
            }

            // Update persistent data, reset line, hide UI
            PersistentDataManager.Instance.UpdateSpiritState(spirit);
            currentLine = 1;
            uiManager.ToggleDialogueUI(false);
        }
    }

    /// <summary>
    /// Begin the typewriter effect for the given text.
    /// </summary>
    private void StartTypewriter(string newText)
    {
        // If a previous line was typing, stop it
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentTypingText = newText;

        // Clear the text box first
        dialogueText.text = string.Empty;

        // Reset flags
        isTyping = true;
        //isSkipping = false;
        canProgressDialogue = false;

        // Start the coroutine
        typingCoroutine = StartCoroutine(TypeTextRoutine(newText));
    }

    /// <summary>
    /// Coroutine that reveals the text character-by-character, 
    /// with optional punctuation delay and skip logic.
    /// </summary>
    private IEnumerator TypeTextRoutine(string fullText)
    {
        WaitForSeconds normalDelay = new WaitForSeconds(1f / charactersPerSecond);
        //WaitForSeconds skipDelay   = new WaitForSeconds(1f / (charactersPerSecond * skipMultiplier));
        WaitForSeconds punctWait   = new WaitForSeconds(punctuationDelay);

        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueText.text += fullText[i];

            if (IsPunctuation(fullText[i]))
            {
                yield return punctWait;
            }
            else
            {
                yield return normalDelay;
            }
        }

        // Finished typing fully
        isTyping = false;
        //isSkipping = false;
        canProgressDialogue = true; // Now the user can press Continue to move on
    }

    /// <summary>
    /// If the user presses Continue while we're typing, 
    /// 1) first press sets skip on (faster speed)
    /// 2) second press (while skipping) reveals everything instantly.
    /// </summary>
    private void SkipTyping()
    {
        // If we are NOT skipping => turn skipping on for faster speed
        //if (!isSkipping)
        //{
        //    isSkipping = true;

        //    // Optionally, if you want a truly "quick skip" on the first press,
        //    // you could do that here (but the requested behavior is a 2-press approach).
        //    if (quickSkip)
        //    {
        //        // *If* you prefer immediate skip on first press,
        //        // uncomment this block:

        //        /*
        //        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        //        dialogueText.text = currentTypingText;
        //        isTyping = false;
        //        isSkipping = false;
        //        canProgressDialogue = true;
        //        */
        //    }

        //    return;
        //}

        // If we ARE already skipping => second press => reveal all text instantly
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentTypingText;
        isTyping = false;
        //isSkipping = false;
        canProgressDialogue = true;
    }

    /// <summary>
    /// Helper method to check punctuation for extra delays.
    /// </summary>
    private bool IsPunctuation(char c)
    {
        return (c == '.' || c == ',' || c == '!' ||
                c == '?' || c == ';' || c == ':' || c == '-');
    }
}
