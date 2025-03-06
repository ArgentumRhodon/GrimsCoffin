using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMOD_Object_Animator : MonoBehaviour
{
    [SerializeField] public EventReference runFX;
    [SerializeField] protected EventInstance runInstance;
    [SerializeField] public EventReference jumpFX;
    [SerializeField] protected EventInstance jumpInstance;
    [SerializeField] public EventReference dashFX;
    [SerializeField] protected EventInstance dashInstance;
    [SerializeField] public EventReference landFX;
    [SerializeField] protected EventInstance landInstance;
    [SerializeField] public EventReference chargeFX;
    [SerializeField] protected EventInstance chargeInstance;
    [SerializeField] public EventReference releaseFX;
    [SerializeField] protected EventInstance releaseInstance;
    private bool chargeStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        runInstance = RuntimeManager.CreateInstance(runFX);
        jumpInstance = RuntimeManager.CreateInstance(jumpFX);
        dashInstance = RuntimeManager.CreateInstance(dashFX);
        landInstance = RuntimeManager.CreateInstance(landFX);
        chargeInstance = RuntimeManager.CreateInstance(chargeFX);
        releaseInstance = RuntimeManager.CreateInstance(releaseFX);

    }

    // Update is called once per frame
    public void RespondToEvent()
    {
        fmodLandStart();
    }

    void fmodRunStart()
    {
        runInstance.start();
    }
    void fmodJumpStart()
    {
        jumpInstance.start();
    }
    void fmodDashStart()
    {
        dashInstance.start();
    }
    void fmodLandStart()
    {
        landInstance.start();
    }
    void fmodChargeStart()
    {
        if (chargeStarted == false)
        {
            chargeStarted = true;
            chargeInstance.start();
            Debug.Log("cHARGED");
        }
    }

    void fmodChargeStop()
    {
        chargeStarted = false;
        chargeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    void fmodReleaseStart()
    {
        chargeStarted = false;
        releaseInstance.start();
    }
}




