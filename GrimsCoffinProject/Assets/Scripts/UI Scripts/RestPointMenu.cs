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

    [SerializeField] public SavePoint restPoint;
    [SerializeField] private GameObject fadeToWhite;


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
            StartCoroutine(TransitionToEquilibrium());
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

            PlayerAnimationManager.Instance.ChangeAnimationSpeed(0);

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);

            if (UIManager.Instance.areaText != null)
                UIManager.Instance.areaText.SetActive(false);
        }

        else if (this.gameObject.activeInHierarchy && insideEquilibrium)
        {
            Heal();
            outsideEQPanel.SetActive(false);
            insideEQPanel.SetActive(true);

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(leaveEQButton);

            Time.timeScale = 0;

            PlayerAnimationManager.Instance.ChangeAnimationSpeed(0);

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(false);

            if (UIManager.Instance.areaText != null)
                UIManager.Instance.areaText.SetActive(false);
        }

        else
        {
            EventSystem.current.SetSelectedGameObject(null);

            PlayerAnimationManager.Instance.ChangeAnimationSpeed(1);

            PlayerControllerForces.Instance.interactionPrompt.gameObject.SetActive(true);
            Time.timeScale = 1;
        }

        restPoint.coffinOpen = !restPoint.coffinOpen;
    }

    public void Heal()
    {
        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
        PlayerControllerForces.Instance.currentSP = PlayerControllerForces.Instance.Data.maxSP;
    }

    private IEnumerator TransitionToEquilibrium()
    {
        Time.timeScale = 0;
        fadeToWhite.SetActive(true);

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < .8f)
        {
            yield return null;
        }

        Time.timeScale = 1;
        SceneManager.LoadScene("Equilibrium");
    }
}
