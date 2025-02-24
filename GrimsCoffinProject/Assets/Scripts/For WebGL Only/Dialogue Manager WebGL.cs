using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManagerWebGL : MonoBehaviour
{
    [Header("Dialogue Data")]
    // This list will be populated by the DialogueLoader.
    public List<DialogueEntry> dialogues;
    [SerializeField] private DialogueLoader dialogueLoader;  // Assign this in the Inspector

    [SerializeField] private string dialogueFileName = "Dialogue.json";
    private string m_Path;

    [Header("UI References")]
    public UIManager uiManager;
    [SerializeField] private Image continuePrompt;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private List<Sprite> continuePromptIcons;
    [SerializeField] private List<Sprite> speakers;

    // The main text UI where we show dialogue.
    private TextMeshProUGUI dialogueText;

    [Header("Typing Settings")]
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private float punctuationDelay = 0.5f;
    [SerializeField] private bool quickSkip = false;
    [SerializeField][Min(1)] private int skipMultiplier = 5;

    // Internal state for typewriting.
    private bool isTyping = false;
    private bool isSkipping = false;
    private Coroutine typingCoroutine;
    private string currentTypingText;

    // Dialogue progression.
    private int currentLine = 1;
    private Spirit currentSpirit;
    public bool canProgressDialogue = false;

    // Input.
    private PlayerControls controls;
    private PlayerInput playerInput;

    private void Awake()
    {
        // Subscribe to the DialogueLoader event.
        if (dialogueLoader != null)
        {
            dialogueLoader.DialogueLoaded += OnDialoguesLoaded;
        }
        else
        {
            Debug.LogWarning("DialogueLoader reference not set in DialogueManager!");
        }

        // Set up input.
        controls = new PlayerControls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks.
        if (dialogueLoader != null)
        {
            dialogueLoader.DialogueLoaded -= OnDialoguesLoaded;
        }
    }

    // Callback when the DialogueLoader finishes loading the dialogues.
    private void OnDialoguesLoaded(List<DialogueEntry> loadedDialogues)
    {
        dialogues = loadedDialogues;
        Debug.Log($"DialogueManager received {dialogues.Count} dialogues.");
    }

    private void Start()
    {
        // Grab the TextMeshProUGUI component from your UIManager's dialogue UI.
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
            // Switch the "Continue" icon based on the current control scheme.
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
            SkipTyping();
        }
        else
        {
            Debug.Log("Progressing Dialogue");
            if (canProgressDialogue && !isSkipping)
            {
                currentLine++;
                ShowDialogueForSpirit(currentSpirit);
            }
        }
    }

    // Called externally to start showing dialogue for a specific Spirit.
    public void ShowDialogueForSpirit(Spirit spirit)
    {
        currentSpirit = spirit;
        int id = (int)spirit.spiritID;
        int state = (int)spirit.spiritState;

        // Find a dialogue line matching SpiritID, SpiritState, and currentLine.
        DialogueEntry dialogue = dialogues.FirstOrDefault(d =>
            d.SpiritID == id &&
            d.SpiritState == state &&
            d.LineID == currentLine);

        if (dialogue != null)
        {
            uiManager.ToggleDialogueUI(true);
            controls.Dialogue.Continue.performed += OnContinue;
            speakerIcon.sprite = speakers[dialogue.SpeakerID];
            StartTypewriter(dialogue.DialogueContent);
        }
        else
        {
            Debug.LogWarning($"No dialogue found for SpiritID {id}, State {state}, LineID {currentLine}.");
            if (spirit.spiritState == 0)
            {
                Destroy(spirit.gameObject.transform.parent.gameObject);
            }
            PersistentDataManager.Instance.UpdateSpiritState(spirit);
            currentLine = 1;
            uiManager.ToggleDialogueUI(false);
            controls.Dialogue.Continue.performed -= OnContinue;
        }
    }

    private void StartTypewriter(string newText)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentTypingText = newText;
        dialogueText.text = string.Empty;
        isTyping = true;
        canProgressDialogue = false;
        typingCoroutine = StartCoroutine(TypeTextRoutine(newText));
    }

    private IEnumerator TypeTextRoutine(string fullText)
    {
        WaitForSeconds normalDelay = new WaitForSeconds(1f / charactersPerSecond);
        WaitForSeconds punctWait = new WaitForSeconds(punctuationDelay);

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

        isTyping = false;
        canProgressDialogue = true;
    }

    private void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogueText.text = currentTypingText;
        isTyping = false;
        canProgressDialogue = true;
    }

    private bool IsPunctuation(char c)
    {
        return (c == '.' || c == ',' || c == '!' ||
                c == '?' || c == ';' || c == ':' || c == '-');
    }
}
