using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RestPointMenu: MonoBehaviour
{
    [SerializeField] private GameObject healButton;
    [SerializeField] private GameObject leaveEQButton;
    
    [SerializeField] private bool insideEquilibrium;
    [SerializeField] private GameObject outsideEQPanel;
    [SerializeField] private GameObject insideEQPanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnterEquilibrium()
    {
        Time.timeScale = 1;

        if (insideEquilibrium)
        {
            PersistentDataManager.Instance.ToggleFirstSpawn(true);
            SceneManager.LoadScene(PersistentDataManager.Instance.LastSavedScene);
        }

        else
        {
            SceneManager.LoadScene(3);
        }
    }

    public void ToggleEnterPrompt()
    {
        this.gameObject.SetActive(!this.gameObject.activeInHierarchy);

        if (this.gameObject.activeInHierarchy && !insideEquilibrium)
        {
            outsideEQPanel.SetActive(true);
            insideEQPanel.SetActive(false);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(healButton);

            Time.timeScale = 0;

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);

            if (UIManager.Instance.areaText != null)
                UIManager.Instance.areaText.SetActive(false);
        }

        else if (this.gameObject.activeInHierarchy && insideEquilibrium)
        {
            outsideEQPanel.SetActive(false);
            insideEQPanel.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(leaveEQButton);

            Time.timeScale = 0;

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);

            if (UIManager.Instance.areaText != null)
                UIManager.Instance.areaText.SetActive(false);
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(true);
            Time.timeScale = 1;
        }
    }

    public void Heal()
    {
        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
    }
}
