using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossCamera : MonoBehaviour
{
    [SerializeField]
    private CameraManager camMgr;

    [SerializeField]
    private CinemachineVirtualCamera bossVirtualCam;

    [SerializeField]
    private CinemachineVirtualCamera followCam;
    [SerializeField]
    private Camera UICamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            camMgr.ChangeCamera(bossVirtualCam);
            camMgr.Vcam = bossVirtualCam;
            UICamera.orthographicSize = 10f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            camMgr.CameraReset();
            camMgr.Vcam = followCam;
        }
    }
}
