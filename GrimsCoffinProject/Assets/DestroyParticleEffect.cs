using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleEffect : MonoBehaviour
{

    private void Start()
    {
        ParticleSystem particles = GetComponent<ParticleSystem>();
        float totalDuration = particles.main.duration;
        Destroy(this.gameObject, totalDuration);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
