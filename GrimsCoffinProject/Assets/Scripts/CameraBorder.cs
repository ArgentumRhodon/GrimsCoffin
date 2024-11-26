using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBorder : MonoBehaviour
{
    [SerializeField] private CameraManager CamManager;
    [SerializeField] private CinemachineVirtualCamera Vcam;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        CamManager.ChangeCamera(Vcam);
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        CamManager.ResetCamera();
    }
}
