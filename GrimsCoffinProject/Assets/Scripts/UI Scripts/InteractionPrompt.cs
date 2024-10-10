using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InteractionPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private Image promptIcon;

    [SerializeField] private List<Sprite> keyboardSprites;
    [SerializeField] private List<Sprite> playstationSprites;
    [SerializeField] private List<Sprite> xboxSprites;

    private string controlScheme;

    private float timer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        controlScheme = PlayerController.Instance.gameObject.GetComponent<PlayerInput>().currentControlScheme;
        HidePrompt();
    }

    // Update is called once per frame
    void Update()
    {
        controlScheme = PlayerController.Instance.gameObject.GetComponent<PlayerInput>().currentControlScheme;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            HidePrompt();
        }
    }

    public void DisplayPrompt(int iconIndex, string text)
    {
        this.gameObject.SetActive(true);
        switch(controlScheme)
        {
            case "Keyboard&Mouse":
                promptIcon.sprite = keyboardSprites[iconIndex];
                break;
            case "Playstation":
                promptIcon.sprite = playstationSprites[iconIndex];
                break;
            case "Xbox":
                promptIcon.sprite = xboxSprites[iconIndex];
                break;
        }
        interactionText.text = text;

        timer = 5.0f;
    }

    public void HidePrompt()
    {
        this.GetComponent<Animator>().Play("Interaction Prompt", -1, 0f);
    }
}
