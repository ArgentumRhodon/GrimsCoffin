using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnlockAbility : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image speakerIcon;
    [SerializeField] private List<Sprite> speakers;
    [SerializeField] public Button yes;
    [SerializeField] private Button no;
    [SerializeField] private TextMeshProUGUI Cost;
    private int collectablesHeld;

    public enum UnlockCost
    {
        Currency=1,
        Herb = 2,
    }

    public UnlockCost currentunlockcost;
    public float currentcost;
    public Spirit currentspirit;

    private void Start()
    {
        collectablesHeld = PersistentDataManager.Instance.HealthCollectablesHeld;
    }
    // Update is called once per frame
    public void StartUnlock(Spirit spirit)
    {
        UIManager.Instance.ToggleUnlockUI(true);
        int id = (int)spirit.spiritID;
        speakerIcon.sprite = speakers[id-1];
        if (id==1) 
        {
            Cost.text = "(-500<sprite index=0>)";
            currentunlockcost = UnlockCost.Currency;
            currentcost = spirit.upgradeCost;
            currentspirit = spirit;

            if (PersistentDataManager.Instance.EnemyCurrency < currentcost)
            {
                yes.interactable = false;
            }
            else 
            {
                yes.interactable = true;
            }

        }
        else if(id == 4)
        {
            Cost.text = "(-3<sprite index=1>)";
            currentunlockcost = UnlockCost.Herb;
            currentcost = spirit.upgradeCost;
            currentspirit = spirit;

            if (collectablesHeld < currentcost)
            {
                yes.interactable = false;
            }
            else
            {
                yes.interactable = true;
            }
        }
        /*else if (id==5)
        {
        Cost.text = "(-500<sprite index=0>)";
            currentunlockcost = UnlockCost.Currency;
            currentcost = spirit.upgradeCost;
            currentspirit = spirit;

            if (PersistentDataManager.Instance.EnemyCurrency < currentcost)
            {
                yes.interactable = false;
            }
            else 
            {
                yes.interactable = true;
            }
        }*/
    }

    public void PurchaseAbility() 
    {
        if (currentunlockcost == UnlockCost.Currency)
        {
            if (PersistentDataManager.Instance.EnemyCurrency >= currentcost)
            {
                PersistentDataManager.Instance.UpdateSpiritState(currentspirit);
                UIManager.Instance.ToggleUnlockUI(false);
            }
        }
        else if(currentunlockcost == UnlockCost.Herb)
        {
            if (collectablesHeld >= currentcost)
            {
                PersistentDataManager.Instance.UpdateSpiritState(currentspirit);
                UIManager.Instance.ToggleUnlockUI(false);
            }
        }
    }

    public void Quit()
    {
        UIManager.Instance.ToggleUnlockUI(false);
    }


}
