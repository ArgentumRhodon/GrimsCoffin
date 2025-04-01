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
    [SerializeField] private Button yes;
    [SerializeField] private Button no;
    [SerializeField] private TextMeshProUGUI Cost;
    private PersistentDataManager persistentDataManager;
    private int collectablesHeld;

    public enum UnlockCost
    {
        Currency=1,
        Herb = 2,
    }

    public UnlockCost currentunlockcost;
    public int currentcost;
    public Spirit currentspirit;

    private void Start()
    {
        persistentDataManager = PersistentDataManager.Instance;
        collectablesHeld = persistentDataManager.HealthCollectablesHeld;
    }
    // Update is called once per frame
    public void Startunlock(Spirit spirit)
    {
        UIManager.Instance.ToggleUnlockUI(true);
        int id = (int)spirit.spiritID;
        if (id==1) 
        {
            Cost.text = "(-500<sprite index=0>)";
            currentunlockcost = UnlockCost.Currency;
            currentcost = 500;
            currentspirit = spirit;

            if (persistentDataManager.EnemyCurrency < currentcost)
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
            currentcost = 3;
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
        /*else if (spirit.spiritID == Spirit.SpiritID.CombatSpirit)
        {

        }*/
    }

    public void PurchaseAbility() 
    {
        if (currentunlockcost == UnlockCost.Currency)
        {
            if (persistentDataManager.EnemyCurrency >= currentcost)
            {
                persistentDataManager.UpdateSpiritState(currentspirit);
                UIManager.Instance.ToggleUnlockUI(false);
            }
        }
        else if(currentunlockcost == UnlockCost.Herb)
        {
            if (collectablesHeld >= currentcost)
            {
                persistentDataManager.UpdateSpiritState(currentspirit);
                UIManager.Instance.ToggleUnlockUI(false);
            }
        }
    }

    public void Quit()
    {
        UIManager.Instance.ToggleUnlockUI(false);
    }


}
