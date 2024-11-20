using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EquilibriumPrompt : MonoBehaviour
{
    [SerializeField] private GameObject yesButton;
    [SerializeField] private bool insideEquilibrium;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnterEquilibrium()
    {
        Time.timeScale = 1;

        if (insideEquilibrium)
            SceneManager.LoadScene(1);

        else
            SceneManager.LoadScene(2);
    }

    public void ToggleEnterPrompt()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);

        if (this.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton);
            Time.timeScale = 0;
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);
            Time.timeScale = 1;
        }
    }
}
