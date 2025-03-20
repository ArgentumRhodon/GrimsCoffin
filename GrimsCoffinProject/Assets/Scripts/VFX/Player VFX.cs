using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] PlayerControllerForces Player;
    [SerializeField] ParticleSystem RunVFX;
    [SerializeField] ParticleSystem JumpVFX;
    [SerializeField] ParticleSystem JumpTrailVFX;
    [SerializeField] ParticleSystem LandVFX;
    [SerializeField] private UnityEvent landTrigger;


    public void Run() 
    {
        if (Player.Grounded())
        {
            RunVFX.Play();
        }
    }
    public void Jump() 
    {
        if (Player.Grounded())
        {
            JumpVFX.Play();
            StartCoroutine("PlayJumpTrailVFX");
        }
    }
    public void Land() 
    {
        LandVFX.Play();
        landTrigger.Invoke();
    }

    IEnumerator PlayJumpTrailVFX()
    {
        JumpTrailVFX.Play();
        yield return new WaitForSeconds(.15f);
        JumpTrailVFX.Stop();
    }
}
