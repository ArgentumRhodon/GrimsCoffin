using Core.AI;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class FMOD_Object_BossListener : MonoBehaviour
{
    [SerializeField] public EventReference DamagedSFX;
    [SerializeField] protected EventInstance DamagedInstance;
    [SerializeField] public EventReference DeadSFX;
    [SerializeField] protected EventInstance DeadInstance;
    [SerializeField] public EventReference IdleSFX;
    [SerializeField] protected EventInstance IdleInstance;
    [SerializeField] public EventReference AttackSFX;
    [SerializeField] protected EventInstance AttackInstance;
    [SerializeField] public EventReference TeleSFX;
    [SerializeField] protected EventInstance TeleInstance;
    [SerializeField] public EventReference InstMoveSFX;
    [SerializeField] protected EventInstance AppearInstance;
    [SerializeField] public EventReference AOEChargeSFX;
    [SerializeField] protected EventInstance AOEChargeInstance;
    [SerializeField] public EventReference AOESlamSFX;
    [SerializeField] protected EventInstance AOESlamInstance;
    [SerializeField] public EventReference AOEBeamSFX;
    [SerializeField] protected EventInstance AOEBeamInstance;
    [SerializeField] public EventReference LaserShootSFX;
    [SerializeField] protected EventInstance LaserShootInstance;
    [SerializeField] public EventReference BlastSFX;
    [SerializeField] protected EventInstance BlastInstance;

    private DenialBoss sampleBoss;
    private int bossHalfHealth;

    private Transform object1;
    private Transform object2;
    private float distance;
    private float instanceDistance;
    private int shootIndex;

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
        TeleInstance = RuntimeManager.CreateInstance(TeleSFX);
        AppearInstance = RuntimeManager.CreateInstance(InstMoveSFX);
        AOEChargeInstance = RuntimeManager.CreateInstance(AOEChargeSFX);
        AOESlamInstance = RuntimeManager.CreateInstance(AOESlamSFX);
        AOEBeamInstance = RuntimeManager.CreateInstance(AOEBeamSFX);
        LaserShootInstance = RuntimeManager.CreateInstance(LaserShootSFX);
        BlastInstance = RuntimeManager.CreateInstance(BlastSFX);

        sampleBoss = FindObjectOfType<DenialBoss>();
        bossHalfHealth = (int)sampleBoss.health / 2;

        RuntimeManager.StudioSystem.setParameterByName("DenialLevel", 1);
        RuntimeManager.StudioSystem.setParameterByName("BossLevel", 0);

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
        if (sampleBoss.health < bossHalfHealth) {
            //Debug.Log("Boss Health Triggered");
            RuntimeManager.StudioSystem.setParameterByName("BossLevel", 1);
         }
    }

    public void RespondToDamagedEvent()
    {
        DamagedInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        DamagedInstance.start();
    }

    public void RespondToDeadEvent()
    {
        DeadInstance.start();
        RuntimeManager.StudioSystem.setParameterByName("DenialLevel", 0);
        RuntimeManager.StudioSystem.setParameterByName("BossLevel", 0);
    }

    public void RespondToIdleEvent()
    {
        IdleInstance.start();
    }

    public void RespondToAttackEvent()
    {
        AttackInstance.start();
    }

    public void RespontToTeleEvent()
    {
        TeleInstance.start();
    }

    public void RespondToAppearEvent()
    {
        AppearInstance.start();
    }

    public void RespondToAOEChargeEvent()
    {
        AOEChargeInstance.start();
        shootIndex = 0;
    }

    public void RespondToAOESlamEvent()
    {
        AOESlamInstance.start();
    }

    public void RespondToAOEBeamEvent()
    {
        AOEBeamInstance.start();
        shootIndex++;
        AOEBeamInstance.setParameterByName("ShootIndex", shootIndex);
    }
    public void RespondToLaserShootEvent()
    {
        LaserShootInstance.start();
    }
    public void RespondToBlastEvent()
    {
        BlastInstance.start();
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
        TeleInstance.setParameterByName("LocalDistance", distance);
        AppearInstance.setParameterByName("LocalDistance", distance);
        AOEChargeInstance.setParameterByName("LocalDistance", distance);
        AOESlamInstance.setParameterByName("LocalDistance", distance);
        AOEBeamInstance.setParameterByName("LocalDistance", distance);
        LaserShootInstance.setParameterByName("LocalDistance", distance);
        BlastInstance.setParameterByName("LocalDistance", distance);
        float position = object1.position.x - object2.position.x;

        if (position < 0)
        {
            DamagedInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            DeadInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            IdleInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AttackInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            TeleInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AppearInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AOEChargeInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AOESlamInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            AOEBeamInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            LaserShootInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));
            BlastInstance.setParameterByName("LocalDirection", -distance / (attenuationResult));

        }
        else
        {
            DamagedInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            DeadInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            IdleInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AttackInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            TeleInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AppearInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AOEChargeInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AOESlamInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            AOEBeamInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            LaserShootInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
            BlastInstance.setParameterByName("LocalDirection", distance / (attenuationResult));
        }
    }


    private void OnDestroy()
    {
        IdleInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
