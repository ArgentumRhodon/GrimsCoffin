using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : Interactable
{
    [SerializeField] private CameraManager cameraManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PerformInteraction()
    {
        cameraManager.LookDown();
        //cameraManager.LookUp();
        //cameraManager.Reset();
    }
}
