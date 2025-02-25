using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spirit : Interactable
{
    private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritID spiritID;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] public DialogueManager dialogueManager;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject mapIcon;

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
        Idle = 2,
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

        spiritState = PersistentDataManager.Instance.GetSpiritState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControllerForces.Instance.transform.position.x <= this.transform.parent.transform.position.x)
            sprite.flipX = true;
        
        else
            sprite.flipX = false;
    }



    public override void PerformInteraction()
    {
        if (dialogueManager != null) 
            dialogueManager.ShowDialogueForSpirit(this);

        //if (spiritState == SpiritState.Uncollected)
        //{
            //PersistentDataManager.Instance.UpdateSpiritState(this);

            //spiritUI.ShowSpiritCollectedText();

            //Debug.Log("Spirit Collected: " + spiritID.ToString());

            //Destroy(this.gameObject.transform.parent.gameObject);
       // }
        //else if (spiritState == SpiritState.Collected)
        //{
           // PersistentDataManager.Instance.UpdateSpiritState(this);
       // }
    }
}
