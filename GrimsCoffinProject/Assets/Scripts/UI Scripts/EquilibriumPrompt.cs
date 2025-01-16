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

        if (this.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(yesButton);

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
}
