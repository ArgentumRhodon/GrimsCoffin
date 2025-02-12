using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapPrompt : MonoBehaviour
{
    [SerializeField] private List<Sprite> mapPromptIcons;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (UIManager.Instance.playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                this.GetComponent<Image>().sprite = mapPromptIcons[0];
                break;

            case "Playstation":
                this.GetComponent<Image>().sprite = mapPromptIcons[1];
                break;

            case "Xbox":
                this.GetComponent<Image>().sprite = mapPromptIcons[2];
                break;
        }
    }
}
