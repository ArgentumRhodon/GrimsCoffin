using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public List<DialogueEntry> dialogues;
    [SerializeField] private string dialogueFileName = "Dialogue.json";

    private void Awake()
    {
        LoadDialogueData();
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
        int id = (int)spiritID;
        int state = (int)spiritState;

        // Find the first dialogue matching the given SpiritID, SpiritState, and where LineID == 1
        var dialogue = dialogues.FirstOrDefault(d =>
            d.SpiritID == id &&
            d.SpiritState == state &&
            d.LineID == 1);

        // If a matching dialogue is found, display its content
        if (dialogue != null)
        {
            UIManager.Instance.ToggleDialogueUI(true);
            Debug.Log(dialogue.DialogueContent);
        }
        else
        {
            Debug.LogWarning($"No dialogue found for SpiritID {id}, State {state} with LineID 1.");
        }
    }
}
