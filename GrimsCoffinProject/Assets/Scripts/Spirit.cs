using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static Spirit;

public class Spirit : Interactable
{
    private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritID spiritID;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] public DialogueManager dialogueManager;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject mapIcon;
    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private PlayableDirector Collect;
    [SerializeField] private UnlockAbility UnlockMenu;

    public float upgradeCost;

    public enum SpiritID
    {
        MapSpirit = 1,
        DashSpirit = 2,
        ScytheThrowSpirit = 3,
        HealthSpirit = 4,
    }

    [SerializeField] public SpiritState spiritState;
    public enum SpiritState
    {
        Uncollected = 0,
        Collected = 1,
        Unlocked = 2,
        Idle = 3,
    }

    private void Awake()
    { 
        animator.SetInteger("SpiritID", (int)spiritID);
    }

    // Start is called before the first frame update
    void Start()
    {
        spiritUI = UIManager.Instance.gameUI.GetComponentInChildren<SpiritCollectUI>();
        dialogueManager = FindObjectOfType<DialogueManager>();
        mapIcon.SetActive(true);
        
        if (UIManager.Instance.UnlockUI != null) 
            UnlockMenu = UIManager.Instance.UnlockUI.GetComponent<UnlockAbility>();

        spiritState = PersistentDataManager.Instance.GetSpiritState(this);

        if (spiritState == SpiritState.Collected
            || spiritID == SpiritID.HealthSpirit && PersistentDataManager.Instance.HealthCollectablesHeld >= 3)
        {
            exclamationMark.SetActive(true);
        }

        else
            exclamationMark.SetActive(false);

        switch (spiritID)
        {
            case SpiritID.MapSpirit:
                upgradeCost = 500;
                break;
            case SpiritID.HealthSpirit:
                upgradeCost = 3; 
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerForces.Instance.transform.position.x <= this.transform.parent.transform.position.x)
            sprite.flipX = true;
        
        else
            sprite.flipX = false;
    }

    public void DestroySpirit() 
    {
        Collect.Play();
    }
    public void SelfDestroy() 
    {
        Destroy(this.gameObject.transform.parent.gameObject);
    }



    public override void PerformInteraction()
    {
        if (spiritState != SpiritState.Unlocked)
        {
            if (dialogueManager != null)
            {
                exclamationMark.SetActive(false);
                dialogueManager.ShowDialogueForSpirit(this);
            }
        }
        else if (spiritState == SpiritState.Unlocked)
        {
            if (spiritID == SpiritID.MapSpirit)
            {
                UnlockMenu.StartUnlock(this);
            }
            else if (spiritID == SpiritID.HealthSpirit)
            {
                UnlockMenu.StartUnlock(this);
            }
            /*else if (spiritID == SpiritID.MapSpirit)
            {
                UnlockMenu.Startunlock(this);
            }*/
            else
            {
                PersistentDataManager.Instance.UpdateSpiritState(this);
                PerformInteraction();
            }
        }
    }
}
