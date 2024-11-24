using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : Interactable
{
    [SerializeField] private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritName spiritID;

    public enum SpiritName
    {
        Test = 0,
    }

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
        spiritUI.ShowSpiritCollectedText();
        Debug.Log(spiritID.ToString());
        PlayerPrefs.SetInt(spiritID.ToString(), 1);
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
