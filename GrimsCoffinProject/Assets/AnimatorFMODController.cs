using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorFMODController : MonoBehaviour
{
    [SerializeField] public EventReference runFX;
    [SerializeField] protected EventInstance runInstance;
    [SerializeField] public EventReference jumpFX;
    [SerializeField] protected EventInstance jumpInstance;
    [SerializeField] public EventReference dashFX;
    [SerializeField] protected EventInstance dashInstance;
    [SerializeField] public EventReference landFX;
    [SerializeField] protected EventInstance landInstance;
    
    // Start is called before the first frame update
    void Start()
    {
        runInstance = RuntimeManager.CreateInstance(runFX);
        jumpInstance = RuntimeManager.CreateInstance(jumpFX);
        dashInstance = RuntimeManager.CreateInstance(dashFX);
        landInstance = RuntimeManager.CreateInstance(landFX);
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
}
