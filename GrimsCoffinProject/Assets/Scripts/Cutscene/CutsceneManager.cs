using TMPro;
using UnityEngine;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string[] sentences;
    public SceneController sceneController;
    public float fadeDuration = 0.5f;

    private PlayerControls controls;
    private int currentSentenceIndex = 0;
    private bool isFading = false;
    private bool cutsceneActive = false;

    void Awake()
    {
        controls = new PlayerControls();
        StartCutscene();
    }

    void OnEnable()
    {
        controls.UI.Enable();
    }

    void OnDisable()
    {
        controls.UI.Disable();
    }

    void Update()
    {
        if (cutsceneActive && controls.UI.Pause.triggered)
        {
            SkipCutsceneImmediately();
        }
        if (cutsceneActive && !isFading && (controls.UI.Click.triggered || controls.UI.Submit.triggered))
        {
            StartCoroutine(AdvanceSentence());
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
    }

    IEnumerator AdvanceSentence()
    {
        isFading = true;
        yield return StartCoroutine(FadeTextAlpha(1f, 0f, fadeDuration));

        currentSentenceIndex++;

        if (currentSentenceIndex < sentences.Length)
        {
            yield return StartCoroutine(ShowSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            sceneController.LoadNextScene();
        }

        isFading = false;
    }

    IEnumerator ShowSentence(string sentence)
    {
        dialogueText.text = sentence;
        SetTextAlpha(0f);

        yield return StartCoroutine(FadeTextAlpha(0f, 1f, fadeDuration));
    }

    IEnumerator FadeTextAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTextAlpha(newAlpha);
            yield return null;
        }
        SetTextAlpha(endAlpha);
    }

    void SetTextAlpha(float alpha)
    {
        Color c = dialogueText.color;
        c.a = alpha;
        dialogueText.color = c;
    }

    private void SkipCutsceneImmediately()
    {
        StopAllCoroutines();
        sceneController.LoadNextScene();
    }
}