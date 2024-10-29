using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject back;

    [SerializeField] private GameObject keyboardControls;
    [SerializeField] private GameObject xboxControls;
    [SerializeField] private GameObject playstationControls;

    [SerializeField] private PlayerInput playerControls;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerControls.currentControlScheme)
        {
            case "Keyboard&Mouse":
                keyboardControls.SetActive(true);
                xboxControls.SetActive(false);
                playstationControls.SetActive(false);
                break;
            case "Xbox":
                keyboardControls.SetActive(false);
                xboxControls.SetActive(true);
                playstationControls.SetActive(false);
                break;
            case "Playstation":
                keyboardControls.SetActive(false);
                xboxControls.SetActive(false);
                playstationControls.SetActive(true);
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleControls()
    {
        controlsScreen.SetActive(!controlsScreen.activeInHierarchy);
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);

        if (controlsScreen.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(back);

        else
            EventSystem.current.SetSelectedGameObject(start);

    }
}
