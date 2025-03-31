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
    public UIManager uiManager;
    [SerializeField] private Image speakerIcon;
    [SerializeField] private List<Sprite> speakers;
    [SerializeField] private Button yes;
    [SerializeField] private Button no;
    [SerializeField] private TextMeshProUGUI Cost;
    private PersistentDataManager persistentDataManager;

    public enum UnlockCost
    {
        Currency=1,
        Herb = 2,
    }

    private UnlockCost currentunlockcost;
    private int currentcost;
    private Spirit currentspirit;

    private void Start()
    {
        persistentDataManager = PersistentDataManager.Instance;
    }
    // Update is called once per frame
    public void Startunlock(Spirit spirit)
    {
        uiManager.ToggleUnlockUI(true);
        if (spirit.spiritID == Spirit.SpiritID.MapSpirit) 
        {
            Cost.text = "(-500<sprite index=0>)";
            currentunlockcost = UnlockCost.Currency;
            currentcost = 500;
            currentspirit = spirit;
        }
        else if(spirit.spiritID == Spirit.SpiritID.HealthSpirit)
        {
            Cost.text = "(-3<sprite index=1>)";
            currentunlockcost = UnlockCost.Herb;
            currentcost = 3;
            currentspirit = spirit;
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
            }
        }
    }

    public void Quit()
    {
        uiManager.ToggleUnlockUI(false);
    }


}
