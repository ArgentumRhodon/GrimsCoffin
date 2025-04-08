using Core.AI;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class FMOD_Object_MonsterListener : MonoBehaviour
{
    [SerializeField] public EventReference DamagedSFX;
    [SerializeField] protected EventInstance DamagedInstance;
    [SerializeField] public EventReference DeadSFX;
    [SerializeField] protected EventInstance DeadInstance;
    [SerializeField] public EventReference IdleSFX;
    [SerializeField] protected EventInstance IdleInstance;
    [SerializeField] public EventReference AttackSFX;
    [SerializeField] protected EventInstance AttackInstance;
    [SerializeField] public EventReference FrameMoveSFX;
    [SerializeField] protected EventInstance FrameMoveInstance;
    [SerializeField] public EventReference InstMoveSFX;
    [SerializeField] protected EventInstance InstMoveInstance;

    private Transform object1;
    private Transform object2;
    private float distance;
    private float instanceDistance;

    [Header("Directional Attenuator")]
    [Range(0.1f, 20f)]
    [Tooltip("The farther this panner is to the right, the wider directional sound would be.")]
    [SerializeField] public float attenuation = 1f;
    private float attenuationResult;

    // Start is called before the first frame update
    void Start()
    {
        DamagedInstance = RuntimeManager.CreateInstance(DamagedSFX);
        DeadInstance = RuntimeManager.CreateInstance(DeadSFX);
        IdleInstance = RuntimeManager.CreateInstance(IdleSFX);
        AttackInstance = RuntimeManager.CreateInstance(AttackSFX);
        FrameMoveInstance = RuntimeManager.CreateInstance(FrameMoveSFX);
        InstMoveInstance = RuntimeManager.CreateInstance(InstMoveSFX);
        object1 = this.transform;
        GameObject target2 = GameObject.Find("PlayerForces");
        if (target2 != null)
        {
            object2 = target2.transform;
        }
        IdleInstance.start();
    }

    private void Awake()
    {
        //object2 = target2.transform;
    }

    // Update is called once per frame
    void Update()
    {
        attenuationResult = 2 * attenuation;
        distanceUpdater();
    }

    public void RespondToDamagedEvent()
    {
        DamagedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        DamagedInstance.start();
    }

    public void RespondToDeadEvent()
    {
        DeadInstance.start();
    }

    public void RespondToIdleEvent()
    {
        IdleInstance.start();
    }

    public void RespondToAttackEvent()
    {
        AttackInstance.start();
    }

    public void RespondToFrameMoveEvent()
    {
        FrameMoveInstance.start();
    }

    public void RespondToInstMoveEvent()
    {
        InstMoveInstance.start();
    }

    public void stopAllEvent()
    {
        IdleInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void distanceUpdater()
    {
        if (object1 != null && object2 != null)
        {
            distance = Vector3.Distance(object1.position, object2.position);

            //Debug.Log("Attenuation: " + distance/ attenuationResult);
        }
        DamagedInstance.setParameterByName("LocalDistance", distance);
        DeadInstance.setParameterByName("LocalDistance", distance);
        IdleInstance.setParameterByName("LocalDistance", distance);
        AttackInstance.setParameterByName("LocalDistance", distance);
        FrameMoveInstance.setParameterByName("LocalDistance", distance);
        InstMoveInstance.setParameterByName("LocalDistance", distance);
        float position = object1.position.x - object2.position.x;

        if (position < 0)
        {
            DamagedInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            DeadInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            IdleInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AttackInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            FrameMoveInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            InstMoveInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));

        }
        else
        {
            DamagedInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            DeadInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            IdleInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AttackInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            FrameMoveInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            InstMoveInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
        }
    }


    private void OnDestroy()
    {
        IdleInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
