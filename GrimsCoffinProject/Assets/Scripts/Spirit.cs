using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : Interactable
{
    private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritName spiritID;

    public enum SpiritName
    {
        Spirit1 = 1,
        Spirit2 = 2,
        Spirit3 = 3,
        Spirit4 = 4,
    }

    // Start is called before the first frame update
    void Start()
    {
        spiritUI = UIManager.Instance.gameUI.GetComponentInChildren<SpiritCollectUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        PlayerPrefs.SetInt((spiritID.ToString()), 1);
       
        spiritUI.ShowSpiritCollectedText();

         //Debug.Log("Spirit Collected: " + spiritID.ToString());
            
         Destroy(this.gameObject.transform.parent.gameObject);       
    }
}
