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
    [SerializeField] public Button no;
    [SerializeField] private TextMeshProUGUI Cost;

    public enum UnlockCost
    {
        Currency=1,
        Herb = 2,
    }

    public UnlockCost currentunlockcost;
    public float currentcost;
    public Spirit currentspirit;

    // Update is called once per frame
    public void StartUnlock(Spirit spirit)
    {
        UIManager.Instance.ToggleUnlockUI(true);
        int id = (int)spirit.spiritID;
        speakerIcon.sprite = speakers[id-1];
        if (id==1) 
        {
            Cost.text = "(" + spirit.upgradeCost + "<sprite index=0>)";
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
            if (PersistentDataManager.Instance.HealthCollectablesHeld < currentcost)
            {
                yes.interactable = false;
            }
            else
            {
                yes.interactable = true;
            }
        }
        else if (id==5)
        {
        Cost.text = "(" + spirit.upgradeCost + "<sprite index=0>)";
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
    }

    public void PurchaseAbility() 
    {
        PersistentDataManager.Instance.UpdateSpiritState(currentspirit);
        UIManager.Instance.ToggleUnlockUI(false);
        currentspirit.exclamationMark.SetActive(false);
    }

    public void Quit()
    {
        UIManager.Instance.ToggleUnlockUI(false);
    }


}
