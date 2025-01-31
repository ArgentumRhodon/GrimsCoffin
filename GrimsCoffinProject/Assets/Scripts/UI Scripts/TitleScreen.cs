using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject newGame;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject back;

    [SerializeField] private GameObject keyboardControls;
    [SerializeField] private GameObject xboxControls;
    [SerializeField] private GameObject playstationControls;

    [SerializeField] private PlayerInput playerControls;

    // Start is called before the first frame update
    void Start()
    {
        PersistentDataManager.Instance.ToggleFirstSpawn(true);

        if (PersistentDataManager.Instance.LastSavedScene == "OnboardingLevel")
            continueButton.GetComponent<Button>().interactable = false;
        else
            continueButton.GetComponent<Button>().interactable = true;
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

    public void NewGame()
    {
        PersistentDataManager.Instance.ResetSaveData();
        PersistentDataManager.Instance.ToggleFirstSpawn(false);
        SceneManager.LoadScene("DenialMinimap");
    }
    public void ContinueGame()
    {
        SceneManager.LoadScene(PersistentDataManager.Instance.LastSavedScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnCancel()
    {
        if (controlsScreen.activeInHierarchy)
        {
            ToggleControls();
        }
    }

    public void ToggleControls()
    {
        controlsScreen.SetActive(!controlsScreen.activeInHierarchy);
        titleScreen.SetActive(!titleScreen.activeInHierarchy);

        if (controlsScreen.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(back);

        else
            EventSystem.current.SetSelectedGameObject(newGame);

    }
}
