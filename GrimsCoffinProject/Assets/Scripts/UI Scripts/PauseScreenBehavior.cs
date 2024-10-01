using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseScreenBehavior : MonoBehaviour
{
    [SerializeField] private Button resume;
    [SerializeField] private Button quit;

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
            Time.timeScale = 0.0f;

        else
            Time.timeScale = 1.0f;

        EventSystem.current.SetSelectedGameObject(null);
    }

    private void QuitGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        SceneManager.LoadScene("TitleScreen");
    }
}
