using Core.AI;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class FMOD_Object_OneShotSoundListener : MonoBehaviour
{
    [SerializeField] public EventReference oneShotFXA;
    [SerializeField] protected EventInstance instanceA;
    [SerializeField] public EventReference oneShotFXB;
    [SerializeField] protected EventInstance instanceB;
    [SerializeField] public EventReference oneShotFXC;
    [SerializeField] protected EventInstance instanceC;
    [SerializeField] public EventReference oneShotFXD;
    [SerializeField] protected EventInstance instanceD;

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
        instanceA = RuntimeManager.CreateInstance(oneShotFXA);
        instanceB = RuntimeManager.CreateInstance(oneShotFXB);
        instanceC = RuntimeManager.CreateInstance(oneShotFXC);
        instanceD = RuntimeManager.CreateInstance(oneShotFXD);
        object1 = this.transform;
        GameObject target2 = GameObject.Find("PlayerForces");
        if (target2 != null)
        {
            object2 = target2.transform;
        }
        instanceC.start();
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

    public void RespondToEventA()
    {
        instanceA.start();
    }

    public void RespondToEventB()
    {
        instanceB.start();
    }

    public void RespondToEventC()
    {
        instanceC.start();
    }

    public void RespondToEventD()
    {
        instanceD.start();
    }

    public void stopAllEvent()
    {
        instanceA.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instanceB.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instanceC.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        instanceD.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    private void distanceUpdater()
    {
        if (object1 != null && object2 != null)
        {
            distance = Vector3.Distance(object1.position, object2.position);

            Debug.Log("Attenuation: " + attenuationResult);
        }
        instanceA.setParameterByName("LocalDistance", distance);
        instanceB.setParameterByName("LocalDistance", distance);
        instanceC.setParameterByName("LocalDistance", distance);
        instanceD.setParameterByName("LocalDistance", distance);
        float position = object1.position.x - object2.position.x;

        if (position < 0)
        {
            instanceA.setParameterByName("LocalDirection", -distance / (attenuationResult));
            instanceB.setParameterByName("LocalDirection", -distance / (attenuationResult));
            instanceC.setParameterByName("LocalDirection", -distance / (attenuationResult));
            instanceD.setParameterByName("LocalDirection", -distance / (attenuationResult));

        }
        else
        {
            instanceA.setParameterByName("LocalDirection", distance / (attenuationResult));
            instanceB.setParameterByName("LocalDirection", distance / (attenuationResult));
            instanceC.setParameterByName("LocalDirection", distance / (attenuationResult));
            instanceD.setParameterByName("LocalDirection", distance / (attenuationResult));
        }
    }


    private void OnDestroy()
    {
        instanceC.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
