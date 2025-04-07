using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FMOD_Event_Boss_Evoker : MonoBehaviour
{
    [SerializeField] private UnityEvent idleNotifier;
    [SerializeField] private UnityEvent TeleNotifier;
    [SerializeField] private UnityEvent AppearNotifier;
    [SerializeField] private UnityEvent AOEChargeNotifier;
    [SerializeField] private UnityEvent AOESlamNotifier;
    [SerializeField] private UnityEvent AOEBeamNotifier;
    [SerializeField] private UnityEvent LaserNotifier;
    [SerializeField] private UnityEvent BlastNotifier;
    [SerializeField] private UnityEvent DeadNotifier;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void evokerIdle()
    {
        idleNotifier.Invoke();
    }

    void evokerTele()
    {
        TeleNotifier.Invoke();
    }

    void evokerAppear()
    {
        AppearNotifier.Invoke();
    }

    void evokerAOECharge()
    {
        AOEChargeNotifier.Invoke();
    }
    void evokerAOESlam()
    {
        AOESlamNotifier.Invoke();
    }
    void evokerAOEBeam()
    {
        AOEBeamNotifier.Invoke();
    }

    void evokerLaser()
    {
        LaserNotifier.Invoke();
    }
    void evokerBlast()
    {
        BlastNotifier.Invoke();
    }
    void evokerDead()
    {
        DeadNotifier.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
