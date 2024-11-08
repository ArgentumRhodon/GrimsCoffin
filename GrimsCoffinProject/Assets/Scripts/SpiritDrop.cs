using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritDrop : Interactable
{
    [SerializeField] private SpiritCollectUI SpiritUI;
    [SerializeField] private GameObject Spirit;

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
        SpiritUI.ShowSpiritCollectedText();
        Destroy(Spirit);
    }
}
