using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum AbilityName
{
    Map,
    Dash,
    ScytheThrow,
    NoAbility
}

public class AbilityUnlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unlockText;
    [SerializeField] private float lifetime;
    [SerializeField] private Image buttonPrompt;

    [SerializeField] List<Sprite> keyboardPrompts;
    [SerializeField] List<Sprite> playstationPrompts;
    [SerializeField] List<Sprite> xboxPrompts;

    public string unlockMessage;
    public AbilityName abilityName;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = lifetime;
        unlockText.text = unlockMessage;
        this.GetComponent<Animator>().keepAnimatorStateOnDisable = true;

        switch (PlayerControllerForces.Instance.GetComponent<PlayerInput>().currentControlScheme)
        {
            case "Keyboard&Mouse":
                buttonPrompt.sprite = keyboardPrompts[(int) abilityName];
                break;
            case "Playstation":
                buttonPrompt.sprite = playstationPrompts[(int) abilityName];
                break;
            default:
                buttonPrompt.sprite = xboxPrompts[(int) abilityName];
                break;
        }

        if (buttonPrompt.sprite == null)
        {
            buttonPrompt.gameObject.SetActive(false);
            buttonPrompt.gameObject.transform.parent.gameObject.SetActive(false);
        }

        else
        {
            buttonPrompt.gameObject.SetActive(true);
            buttonPrompt.gameObject.transform.parent.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        this.GetComponent<Image>().fillAmount = timer / lifetime;

        if (timer <= 0)
            Destroy(gameObject);
    }
}
