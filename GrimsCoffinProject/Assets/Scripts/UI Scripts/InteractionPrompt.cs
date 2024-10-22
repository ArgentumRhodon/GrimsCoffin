using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionPrompt : MonoBehaviour
{
    //Variables for prompt
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private Image promptIcon;

    //Lists of sprites for controls (Each index should match the same interaction so
    [SerializeField] private List<Sprite> keyboardSprites;
    [SerializeField] private List<Sprite> playstationSprites;
    [SerializeField] private List<Sprite> xboxSprites;

    //String to keep track of player's input method
    private string controlScheme;

    //Timer to keep track of how long prompt is visible
    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //Hide prompt and get control scheme
        controlScheme = PlayerControllerForces.Instance.gameObject.GetComponent<PlayerInput>().currentControlScheme;
        HidePrompt();
    }

    // Update is called once per frame
    void Update()
    {
        //Update control scheme in case player changes controller mid game
        controlScheme = PlayerControllerForces.Instance.gameObject.GetComponent<PlayerInput>().currentControlScheme;

        //Reduce the timer 
        timer -= Time.deltaTime;

        //If timer gets below 0, hide prompt
        if (timer <= 0f)
        {
            HidePrompt();
        }
    }

    /// <summary>
    /// Display the prompt above the player with the proper icon and text
    /// </summary>
    /// <param name="iconIndex">Index for the control icon based off of the list of control sprites</param>
    /// <param name="text">Text for the interaction to be displayed to the player</param>
    /// <param name="timeToDisplay">Amount of time to display the prompt for (in seconds)</param>
    public void DisplayPrompt(int iconIndex, string text, float timeToDisplay)
    {
        //Change prompt to the according control scheme being used
        switch(controlScheme)
        {
            case "Keyboard&Mouse":
                promptIcon.sprite = keyboardSprites[iconIndex];
                break;
            case "Playstation":
                promptIcon.sprite = playstationSprites[iconIndex];
                break;
            default:
                promptIcon.sprite = xboxSprites[iconIndex];
                break;
        }

        interactionText.text = text;

        //Set timer 
        timer = timeToDisplay;
    }

    //Reverse Animation to hide the prompt
    public void HidePrompt()
    {
        this.GetComponent<Animator>().Play("Interaction Prompt", -1, 0f);
    }
}
