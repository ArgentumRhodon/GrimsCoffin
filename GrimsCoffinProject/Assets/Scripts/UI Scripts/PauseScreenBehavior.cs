using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseScreenBehavior : MonoBehaviour
{
    [SerializeField] private Button resume;
    [SerializeField] private Button quit;

    [SerializeField] private PlayerInput playerInput;

    private bool isPaused;

    // Start is called before the first frame update
    void Start()
    {
        resume.onClick.AddListener(Pause);
        quit.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause()
    {
        isPaused = !isPaused;
        this.gameObject.SetActive(isPaused);

        if (isPaused)
        {
            //Deactivate Player Control if paused
            playerInput.DeactivateInput();

            //Select resume button for controller navigation
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

    private void QuitGame()
    {
        Time.timeScale = 1.0f;
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("TitleScreen");
    }
}
