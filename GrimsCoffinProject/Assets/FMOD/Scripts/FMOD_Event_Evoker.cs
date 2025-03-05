using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FMOD_Event_Evoker : MonoBehaviour
{
    [SerializeField] private UnityEvent oneShotNotifierA;
    [SerializeField] private UnityEvent oneShotNotifierB;
    [SerializeField] private UnityEvent oneShotNotifierC;
    [SerializeField] private UnityEvent oneShotNotifierD;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void evokerA()
    {
        oneShotNotifierA.Invoke();
    }

    void evokerB()
    {
        oneShotNotifierB.Invoke();
    }

    void evokerC()
    {
        oneShotNotifierC.Invoke();
    }

    void evokerD()
    {
        oneShotNotifierD.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
