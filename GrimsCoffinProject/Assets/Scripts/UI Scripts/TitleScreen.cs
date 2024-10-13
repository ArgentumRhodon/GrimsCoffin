using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject start;
    [SerializeField] private GameObject back;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
