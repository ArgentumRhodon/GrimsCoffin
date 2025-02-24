using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    [Header("JSON Settings")]
    [SerializeField] private string dialogueFileName = "Dialogue.json";

    // Holds the loaded dialogue entries.
    public List<DialogueEntry> dialogues;

    // Event to notify when dialogues are loaded.
    public delegate void OnDialogueLoaded(List<DialogueEntry> loadedDialogues);
    public event OnDialogueLoaded DialogueLoaded;

    private void Start()
    {
        StartCoroutine(LoadDialogueData());
    }

    private IEnumerator LoadDialogueData()
    {
        // Construct the file path from StreamingAssets.
        string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, dialogueFileName);
        UnityWebRequest request = UnityWebRequest.Get(filePath);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            // Use the new method to parse the JSON string.
            DialogueEntry[] entries = SaveDataWebGL.ReadFromJSONString<DialogueEntry>(json).ToArray();
            dialogues = new List<DialogueEntry>(entries);
            Debug.Log($"Loaded {dialogues.Count} dialogue entries.");

            // Notify subscribers that the dialogue is loaded.
            DialogueLoaded?.Invoke(dialogues);
        }
        else
        {
            Debug.LogError($"Failed to load dialogue JSON file: {request.error}");
        }
    }
}
