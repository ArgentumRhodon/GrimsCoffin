using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class TypeWriting : MonoBehaviour
{ 
    
    private TMP_Text _textbox;
    private PlayerControls controls;
    private PlayerInput playerInput;

    private int currentVisibleCharacterIndex;
    private Coroutine typewritierCoroutine;

    private WaitForSeconds _simpledelay;
    private WaitForSeconds _interpunctvationDelay;

    [SerializeField] private float characterPerSecond = 20;
    [SerializeField] private float interpuncvationDelay = 0.5f;

    //skipping Function
    public bool CurrentSkipping { get; private set; }
    private WaitForSeconds _skipDelay;
    [SerializeField] private bool quickskip;
    [SerializeField][Min(1)] private int skipSpeedup = 5;

    //Event Function
    private WaitForSeconds _textboxFullEventDelay;
    [SerializeField][Range(0.1f, 0.5f)] private float sendDoneDelay = 0.25f;
    public static event Action CompleteTextRevealed;
    public static Action<char> CharacterRevealed;

    private void Awake()
    {
        _textbox = GetComponent<TMP_Text>();
        controls = new PlayerControls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();

        _simpledelay = new WaitForSeconds(1 / characterPerSecond);
        _interpunctvationDelay = new WaitForSeconds(interpuncvationDelay);

       _skipDelay = new WaitForSeconds(1/(characterPerSecond*skipSpeedup));
        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay); 
    }

    private void Update()
    {
        if (controls.Dialogue.Continue.triggered)
            Skip();
    }

    public void SetText(string text)
    {
        if (typewritierCoroutine != null)
            StopCoroutine(typewritierCoroutine);
        _textbox.text = text;
        _textbox.maxVisibleCharacters = 0;
        currentVisibleCharacterIndex = 0;
        typewritierCoroutine = StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter() 
    {
        TMP_TextInfo textInfo = _textbox.textInfo;
        while (currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;
            if (currentVisibleCharacterIndex==lastCharacterIndex)
            {
                _textbox.maxVisibleCharacters++;
                yield return _textboxFullEventDelay;
                CompleteTextRevealed?.Invoke();
                yield break;
            }

            char character = textInfo.characterInfo[currentVisibleCharacterIndex].character;
            _textbox.maxVisibleCharacters++;
            if(!CurrentSkipping && character == '?' || character==','||character=='.'||character ==';'||character==':'||character=='!'||character=='-')
            {
                yield return interpuncvationDelay;
            }
            else 
            {
                yield return CurrentSkipping ? _skipDelay: _simpledelay; 
            }
            CharacterRevealed?.Invoke(character);
            currentVisibleCharacterIndex++;
        }
    }

    void Skip()
    {
        if (CurrentSkipping)
            return;
        CurrentSkipping = true;
        if (!quickskip)
        {
            StartCoroutine(SkipSpeedupReset());
            return;
        }
        StopCoroutine(typewritierCoroutine);
        _textbox.maxVisibleCharacters=_textbox.textInfo.characterCount;
        CompleteTextRevealed?.Invoke();
    }

    private IEnumerator SkipSpeedupReset() 
    {
        yield return new WaitUntil(() => _textbox.maxVisibleCharacters == _textbox.textInfo.characterCount - 1);
        CurrentSkipping = false;
    }

}
