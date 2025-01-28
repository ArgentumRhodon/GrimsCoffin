using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public List<DialogueEntry> dialogues;
    [SerializeField] private string dialogueFileName = "Dialogue.json";
    private string m_Path;
    public UIManager uiManager;
    private int currentline = 1;
    private Spirit.SpiritID currentSpirit;
    private Spirit.SpiritState currentState;

    private PlayerControls controls;
    private PlayerInput playerInput;

    [SerializeField] private List<Sprite> continuePromptIcons;
    [SerializeField] private Image continuePrompt;

    public bool canProgressDialogue = false;

    private void Awake()
    {
        m_Path = Application.dataPath;
        Debug.Log(m_Path);
        LoadDialogueData();
        controls = new PlayerControls();
        controls.Enable();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (uiManager.dialogueUI.activeSelf)
        {
            Debug.Log("active");

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

            if (controls.Dialogue.Continue.triggered && canProgressDialogue)
            {
                Debug.Log("Click");
                currentline++;
                ShowDialogueForSpirit(currentSpirit, currentState);
            }
        }
    }


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

    public void ShowDialogueForSpirit(Spirit.SpiritID spiritID, Spirit.SpiritState spiritState)
    {
        currentSpirit = spiritID;
        currentState = spiritState;
        
        int id = (int)spiritID;
        int state = (int)spiritState;

        // Find the first dialogue matching the given SpiritID, SpiritState, and where LineID == 1
        var dialogue = dialogues.FirstOrDefault(d =>
            d.SpiritID == id &&
            d.SpiritState == state &&
            d.LineID == currentline);

        // If a matching dialogue is found, display its content
        if (dialogue != null)
        {
            UIManager.Instance.ToggleDialogueUI(true);
            TextMeshProUGUI dialogueText = UIManager.Instance.dialogueUI.GetComponentInChildren<TextMeshProUGUI>();
            if (dialogueText != null)
            {
                dialogueText.text = dialogue.DialogueContent;
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in dialogueUI.");
            }
            Debug.Log(dialogue.DialogueContent);
        }
        // If  matching dialogue is not found, quit the dialogue and reset the current line to 1
        else
        {
            Debug.LogWarning($"No dialogue found for SpiritID {id}, State {state} with LineID {currentline}.");
            currentline = 1;
            uiManager.ToggleDialogueUI(false);
        }
    }

}
