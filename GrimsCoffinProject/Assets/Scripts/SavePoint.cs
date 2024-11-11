using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : Interactable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        PlayerControllerForces.Instance.currentHP = PlayerControllerForces.Instance.maxHP;
        PlayerControllerForces.Instance.respawnPoint = this.transform.position;
    }
}
