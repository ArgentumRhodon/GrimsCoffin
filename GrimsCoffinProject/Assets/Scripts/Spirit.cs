using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : Interactable
{
    private SpiritCollectUI spiritUI;
    
    [SerializeField] public SpiritName spiritID;

    public enum SpiritName
    {
        Test = 0,
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
        if (spiritUI != null)
            spiritUI.ShowSpiritCollectedText();
        
        Debug.Log("Spirit Collected: " + spiritID.ToString());
        PlayerPrefs.SetInt(spiritID.ToString(), 1);
        Destroy(this.gameObject.transform.parent.gameObject);
    }
}
