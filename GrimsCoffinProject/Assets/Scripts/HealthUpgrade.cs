using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUpgrade : Interactable
{
    [SerializeField] public int collectableID;

    // Start is called before the first frame update
    void Start()
    {
        if (PersistentDataManager.Instance.HealthUpgradesCollected()[collectableID])
            Destroy(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        PersistentDataManager.Instance.CollectHealthUpgrade(collectableID);
        Destroy(this.gameObject);
    }
}
