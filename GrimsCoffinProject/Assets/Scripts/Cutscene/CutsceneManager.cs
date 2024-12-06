using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI indicatorText;
    public TextMeshProUGUI skipText;
    public SceneController sceneController;

    [Header("Sentences and Fade Settings")]
    public string[] sentences;
    public float fadeDuration = 0.5f;
    public string actionDescription = "continue";

    private PlayerControls controls;
    private PlayerInput playerInput;
    private int currentSentenceIndex = 0;
    private bool isFading = false;
    private bool cutsceneActive = false;

    void Awake()
    {
        controls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();
        StartCutscene();
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
        if (cutsceneActive && !isFading && (controls.UI.Click.triggered || controls.UI.Submit.triggered))
        {
            StartCoroutine(AdvanceSentence());
        }

        if (cutsceneActive && controls.UI.SkipCutscene.triggered)
        {
            SkipCutsceneImmediately();
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
        UpdateIndicator();
        UpdateSkipText();
    }

    IEnumerator AdvanceSentence()
    {
        isFading = true;

        // Start both fade-outs at the same time
        Coroutine sentenceFade = StartCoroutine(FadeTextAlpha(dialogueText, 1f, 0f, fadeDuration));
        Coroutine indicatorFade = StartCoroutine(FadeTextAlpha(indicatorText, 1f, 0f, fadeDuration));

        // Wait for both to finish
        yield return sentenceFade;
        yield return indicatorFade;

        currentSentenceIndex++;

        if (currentSentenceIndex < sentences.Length)
        {
            yield return StartCoroutine(ShowSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            // End the cutscene
            sceneController.LoadNextScene();
        }

        isFading = false;
    }

    IEnumerator ShowSentence(string sentence)
    {
        dialogueText.text = sentence;
        SetTextAlpha(dialogueText, 0f);
        SetTextAlpha(indicatorText, 0f); // ensure indicator starts hidden

        // Fade in the sentence first
        yield return StartCoroutine(FadeTextAlpha(dialogueText, 0f, 1f, fadeDuration));

        // After the sentence is fully visible, fade in the indicator text
        yield return StartCoroutine(FadeTextAlpha(indicatorText, 0f, 1f, fadeDuration));
    }

    IEnumerator FadeTextAlpha(TextMeshProUGUI textElement, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTextAlpha(textElement, newAlpha);
            yield return null;
        }
        SetTextAlpha(textElement, endAlpha);
    }

    void SetTextAlpha(TextMeshProUGUI textElement, float alpha)
    {
        Color c = textElement.color;
        c.a = alpha;
        textElement.color = c;
    }

    void SkipCutsceneImmediately()
    {
        StopAllCoroutines();
        sceneController.LoadNextScene();
    }

    void OnControlsChanged(PlayerInput obj)
    {
        UpdateIndicator();
        UpdateSkipText();
    }

    private void UpdateIndicator()
    {
        if (indicatorText == null || playerInput == null) return;

        string currentScheme = playerInput.currentControlScheme;
        if (currentScheme.Contains("Keyboard&Mouse"))
        {
            indicatorText.text = "click to continue";
        }
        else if (currentScheme.Contains("Xbox"))
        {
            indicatorText.text = "press A to continue";
        }
        else if (currentScheme.Contains("PlayStation"))
        {
            indicatorText.text = "press B to continue";
        }
    }

    private void UpdateSkipText()
    {
        if (skipText == null || playerInput == null) return;

        string currentScheme = playerInput.currentControlScheme;
        if (currentScheme.Contains("Keyboard&Mouse"))
        {
            skipText.text = "press esc to skip";
        }
        else
        {
            skipText.text = "press start to skip";
        }
    }

    public void SetActionDescription(string newDescription)
    {
        actionDescription = newDescription;
        UpdateIndicator();
    }
}
