using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class PauseScreenBehavior : MonoBehaviour
{
    [SerializeField] private Button resume;
    [SerializeField] private Button quit;
    [SerializeField] private Button controlsBack;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] public GameObject controlsScreen;

    [SerializeField] private GameObject keyboardControls;
    [SerializeField] private GameObject xboxControls;
    [SerializeField] private GameObject playstationControls;

    [SerializeField] private PlayerInput playerInput;

    public bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerInput.currentControlScheme)
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

    public void Pause()
    {
        if (controlsScreen.activeInHierarchy)
        {
            ToggleControls();
            return;
        }

        isPaused = !isPaused;
        UIManager.Instance.gameUI.SetActive(!isPaused);
        this.gameObject.SetActive(isPaused);

        if (isPaused)
        {
            //Deactivate Player Control if paused
            //playerInput.DeactivateInput();

            //Select resume button for controller navigation
            if (UIManager.Instance.areaText != null)
                UIManager.Instance.areaText.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resume.gameObject);
            Time.timeScale = 0.0f;
        }
            

        else
        {
            //Reactivate player control and unpause
            playerInput.ActivateInput();
            Time.timeScale = 1.0f;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
        foreach (Room room in PersistentDataManager.Instance.rooms)
        {
            if (room.gameObject.activeInHierarchy && room.GetComponent<EnemyManager>() != null)
                room.GetComponent<EnemyManager>().DeleteEnemies();
        }
        SceneManager.LoadScene("TitleScreen");
    }

    public void ToggleControls()
    {
        controlsScreen.SetActive(!controlsScreen.activeInHierarchy);
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

        if (controlsScreen.activeInHierarchy)
            EventSystem.current.SetSelectedGameObject(controlsBack.gameObject);

        else
            EventSystem.current.SetSelectedGameObject(resume.gameObject);
    }
}
