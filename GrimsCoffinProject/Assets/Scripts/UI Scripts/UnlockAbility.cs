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

    // Update is called once per frame
    private void Startunlock(Spirit spirit)
    {
        if (spirit.spiritID == Spirit.SpiritID.MapSpirit) 
        {
            Cost.text = "(-500<sprite index=0>)";
        }
        else if(spirit.spiritID == Spirit.SpiritID.HealthSpirit)
        {
            Cost.text = "(-3<sprite index=1>)";
        }
        /*else if (spirit.spiritID == Spirit.SpiritID.CombatSpirit)
        {

        }*/
    }

}
