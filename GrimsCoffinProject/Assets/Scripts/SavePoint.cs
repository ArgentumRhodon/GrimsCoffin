using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SavePoint : Interactable
{
    public Vector3 position;
    public int roomIndex;

    [SerializeField] public bool coffinOpen;
    [SerializeField] private bool insideEquilibrium;
    [SerializeField] private float holdTimer = 0.5f;
    [SerializeField] private GameObject fadeToWhite;

    private Animator animator;

    private bool sceneTransition = false;
    public bool isActive = false;

    [SerializeField] private UnityEvent healEvent;
    [SerializeField] private UnityEvent activeEvent;
    [SerializeField] private UnityEvent transEvent;

    // Start is called before the first frame update
    void Start()
    {
        position = gameObject.transform.parent.transform.parent.transform.position;
        animator = this.gameObject.transform.parent.GetComponent<Animator>();

        animator.SetBool("Active", isActive);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerForces.Instance.isHoldingInteract)
        {
            switch (UIManager.Instance.playerInput.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    PlayerControllerForces.Instance.interactionPrompt.keyboardHoldFill.fillAmount = PlayerControllerForces.Instance.holdInteractTimer / holdTimer;
                    break;
                default:
                    PlayerControllerForces.Instance.interactionPrompt.controllerHoldFill.fillAmount = PlayerControllerForces.Instance.holdInteractTimer / holdTimer;
                    break;
            } 

            PlayerControllerForces.Instance.holdInteractTimer += Time.deltaTime;
            
            if (PlayerControllerForces.Instance.holdInteractTimer >= holdTimer)
                EnterEquilibrium();
        }

        else if (!PlayerControllerForces.Instance.isHoldingInteract && !sceneTransition)
        {
            fadeToWhite.SetActive(false);
            PlayerControllerForces.Instance.interactionPrompt.keyboardHoldFill.fillAmount = 0;
            PlayerControllerForces.Instance.interactionPrompt.controllerHoldFill.fillAmount = 0;
        }

        animator.SetBool("Active", isActive);
    }

    public override void PerformInteraction()
    {
        PlayerControllerForces.Instance.isHoldingInteract = true;

        if (Time.timeScale == 0)
            return;

        Heal();

        //FMOD Event to Avoid Loading sound play when already-loaded
        if (!isActive) {
            activeEvent.Invoke();
        }

        isActive = true;
        animator.SetBool("Active", isActive);

        foreach (SavePoint restPoint in PersistentDataManager.Instance.restPoints)
        {
            if (restPoint == this)
            {
                continue;
            }

            else
            {
                restPoint.isActive = false;
            }
                
        }

        if (SceneManager.GetActiveScene().name != "Equilibrium")
        {
            PersistentDataManager.Instance.SaveGame(this);
        }

        //UIManager.Instance.restPointMenu.restPoint = this;
        //UIManager.Instance.restPointMenu.ToggleEnterPrompt();
    }

    public void Heal()
    {
        if (PlayerControllerForces.Instance.currentHP < PlayerControllerForces.Instance.Data.maxHP)
        {
            healEvent.Invoke();
        }
        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.Data.maxHP;
        PlayerControllerForces.Instance.currentSP = PlayerControllerForces.Instance.Data.maxSP;
    }

    public void EnterEquilibrium()
    {
        sceneTransition = true;

        if (insideEquilibrium)
        {
            PersistentDataManager.Instance.ToggleFirstSpawn(true);
            transEvent.Invoke();
            StartCoroutine(TransitionScene(PersistentDataManager.Instance.LastSavedScene));
        }

        else
        {
            transEvent.Invoke();
            StartCoroutine(TransitionScene("Equilibrium"));
        }
    }

    private IEnumerator TransitionScene(string sceneName)
    {
        Time.timeScale = 0;
        fadeToWhite.SetActive(true);

        yield return new WaitForSecondsRealtime(.5f);

        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
