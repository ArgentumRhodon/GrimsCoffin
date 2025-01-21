using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Spirit : Interactable
{
    private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritID spiritID;
    [SerializeField] public DialogueManager dialogueManager;

    public enum SpiritID
    {
        Spirit1 = 1,
        Spirit2 = 2,
        Spirit3 = 3,
        Spirit4 = 4,
    }

    [SerializeField] public SpiritState spiritState;
    public enum SpiritState
    {
        Uncollected = 0,
        Collected = 1,
        Idle = 2,
    }

    // Start is called before the first frame update
    void Start()
    {
        spiritUI = UIManager.Instance.gameUI.GetComponentInChildren<SpiritCollectUI>();
        dialogueManager = FindObjectOfType<DialogueManager>();

        spiritState = PersistentDataManager.Instance.GetSpiritState(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        dialogueManager.ShowDialogueForSpirit(spiritID, spiritState);

        if (spiritState == SpiritState.Uncollected)
        {
            PersistentDataManager.Instance.UpdateSpiritState(this);

            //spiritUI.ShowSpiritCollectedText();

            //Debug.Log("Spirit Collected: " + spiritID.ToString());

            Destroy(this.gameObject.transform.parent.gameObject);
        }
        else if (spiritState == SpiritState.Collected)
        {
            PersistentDataManager.Instance.UpdateSpiritState(this);
        }
    }
}
