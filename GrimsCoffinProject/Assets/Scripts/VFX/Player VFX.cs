using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour
{
    [SerializeField] PlayerControllerForces Player;
    [SerializeField] ParticleSystem RunVFX;
    [SerializeField] ParticleSystem JumpVFX;
    [SerializeField] ParticleSystem LandVFX;

    public void Run() 
    {
        if (Player.Grounded())
        {
            RunVFX.Play();
        }
    }
    public void Jump() 
    {
        JumpVFX.Play();
    }
    public void Land() 
    {
        LandVFX.Play();
    }
}
