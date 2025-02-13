using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FMODGlobalParameterTester;

public class BreakableWall : MonoBehaviour
{
    [SerializeField]
    private bool isOneWay;

    private float health = 5;

    //FMOD Related Variables
    #region FMODRelated
    private enum wallNameEnum
    {
        Stone,
        Snow,
        Soil,
    }

    
    [SerializeField] public EventReference wallHitSFX;
    [SerializeField] public EventReference wallDestroySFX;
    private EventInstance breakInstance;
    private EventInstance hitInstance;

    [Tooltip("Selection of available ground names")]
    [SerializeField] private wallNameEnum wallTexture;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        textureSetter();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void TakeDamage(float damage = 1)
    {
        //Remove health
        if((health - damage) > 0)
        {
            hitInstance.start();
        }

        health -= damage;
        Debug.Log("The Damage has been dealt");

        //Camera shake based off of damage
        CameraShake.Instance.ShakeCamera(damage / 2.25f, damage / 3.25f, .2f);

        //Enemy death calculation
        if (health <= 0)
        {
            breakInstance.start();
            Destroy(this.gameObject);
        }

    }

    private void textureSetter()
    {
        breakInstance = RuntimeManager.CreateInstance(wallDestroySFX);
        breakInstance.getDescription(out EventDescription breakDescription);
        breakDescription.getParameterDescriptionByIndex(0, out PARAMETER_DESCRIPTION breakParameter);

        hitInstance = RuntimeManager.CreateInstance(wallHitSFX);
        hitInstance.getDescription(out EventDescription hitDescription);
        hitDescription.getParameterDescriptionByIndex(0, out PARAMETER_DESCRIPTION hitParameter);

        breakInstance.setParameterByName(breakParameter.name, (float)wallTexture);
        hitInstance.setParameterByName(hitParameter.name, (float)wallTexture);
    }
}
